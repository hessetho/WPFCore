using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace WPFCore.XAML.DragDrop
{
    /// <summary>
    ///     Diese Klasse stellt den zentralen Manager für Drag-n-Drop Operationen dar.
    /// </summary>
    public static class DragDropManager
    {

        /// <summary>
        ///     Abhängigkeitseigenschaft, welche den DragSourceAdvisor vom Typ <see cref="IDragSourceAdvisor" /> darstellt.
        /// </summary>
        public static readonly DependencyProperty DragSourceAdvisorProperty = DependencyProperty.RegisterAttached(
            "DragSourceAdvisor",
            typeof(IDragSourceAdvisor),
            typeof(DragDropManager),
            new FrameworkPropertyMetadata(OnDragSourceAdvisorChanged));

        /// <summary>
        ///     Abhängigkeitseigenschaft, welche den DropTargetAdvisor vom Typ <see cref="IDropTargetAdvisor" /> darstellt.
        /// </summary>
        public static readonly DependencyProperty DropTargetAdvisorProperty = DependencyProperty.RegisterAttached(
            "DropTargetAdvisor",
            typeof(IDropTargetAdvisor),
            typeof(DragDropManager),
            new FrameworkPropertyMetadata(OnDropTargetAdvisorChanged));

        /// <summary>
        ///     Feld zur Speicherung der Position des Drag-Adorners.
        /// </summary>
        private static Point adornerPosition;

        /// <summary>
        ///     Feld zur Speicherung des Drag-Elements
        /// </summary>
        private static UIElement draggedElt;

        /// <summary>
        ///     Feld zur Speicherung des Startpunkts.
        /// </summary>
        /// <remarks>
        ///     Der Startpunkt dient dazu, festzustellen, ob eine Mausbewegung mit gedrückter
        ///     linker Maustaste als Beginn einer Drag-n-Drop Operation gewertet werden kann.
        /// </remarks>
        private static Point dragStartPoint;

        /// <summary>
        ///     Feld zur Speicherung der Information, dass die linke Maustaste gedrückt ist.
        /// </summary>
        private static bool isMouseDown;

        /// <summary>
        ///     Feld zur Speicherung des Offsets
        /// </summary>
        private static Point offsetPoint;

        /// <summary>
        ///     Overlay-Element zur Darstellung des Adorners.
        /// </summary>
        private static DropPreviewAdorner overlayElt;

        /// <summary>
        ///     Formatbezeichnung für Clipboard-Daten, hier für das Offset der Drag-Bewegung
        /// </summary>
        private const string DragOffsetFormat = "DnD.DragOffset";

        /// <summary>
        ///     Formatbezeichnung für Clipboard-Daten, hier für die Drag-Quelle
        /// </summary>
        private const string DragSourceFormat = "DnD.DragSource";

        /// <summary>
        ///     Gets or sets the current drag source advisor.
        /// </summary>
        /// <value>The current drag source advisor.</value>
        private static IDragSourceAdvisor CurrentDragSourceAdvisor { get; set; }

        /// <summary>
        ///     Gets or sets the current drop target advisor.
        /// </summary>
        /// <value>The current drop target advisor.</value>
        private static IDropTargetAdvisor CurrentDropTargetAdvisor { get; set; }

        /* -------------------------------------------------------------------
         *     Drop Target events 
         * ------------------------------------------------------------------- */
        #region Drop Target Events

        /// <summary>
        ///     Handles the PreviewDrop event of the DropTarget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs" /> instance containing the event data.</param>
        private static void DropTargetPreviewDrop(object sender, DragEventArgs e)
        {
            UpdateEffects(e);

            var dropPoint = e.GetPosition((UIElement) sender);

            // Calculate displacement for (Left, Top)
            var offset = e.GetPosition(overlayElt);
            dropPoint.X = dropPoint.X - offset.X;
            dropPoint.Y = dropPoint.Y - offset.Y;

            RemovePreviewAdorner();
            offsetPoint = new Point(0, 0);

            if (CurrentDropTargetAdvisor.IsValidDataObject(e.Data))
                CurrentDropTargetAdvisor.OnDropCompleted(e.Data, dropPoint);

            e.Handled = true;
        }

        /// <summary>
        ///     Handles the PreviewDragLeave event of the DropTarget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs" /> instance containing the event data.</param>
        private static void DropTargetPreviewDragLeave(object sender, DragEventArgs e)
        {
            UpdateEffects(e);

            RemovePreviewAdorner();

            // Get the current drop target advisor
            CurrentDropTargetAdvisor = GetDropTargetAdvisor((DependencyObject) sender);
            CurrentDropTargetAdvisor.OnLeaveDropZone(e.Data);

            e.Handled = true;
        }

        /// <summary>
        ///     Handles the PreviewDragOver event of the DropTarget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs" /> instance containing the event data.</param>
        private static void DropTargetPreviewDragOver(object sender, DragEventArgs e)
        {
            Debug.Assert(sender is UIElement, "Awaiting sender of type UIElement");
            // Get the current drop target advisor
            CurrentDropTargetAdvisor = GetDropTargetAdvisor((DependencyObject) sender);

            UpdateEffects(e);

            // Update position of the preview Adorner
            adornerPosition = e.GetPosition((UIElement) sender);
            PositionAdorner();

            // Let the target do some stuff -----------------------
            var dropPoint = e.GetPosition((UIElement) sender);

            // Calculate displacement for (Left, Top)
            var offset = e.GetPosition(overlayElt);
            dropPoint.X = dropPoint.X - offset.X;
            dropPoint.Y = dropPoint.Y - offset.Y;

            CurrentDropTargetAdvisor.OnDropQuery(e.Data, dropPoint);

            e.Handled = true;
        }

        /// <summary>
        ///     Handles the PreviewDragEnter event of the DropTarget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs" /> instance containing the event data.</param>
        private static void DropTargetPreviewDragEnter(object sender, DragEventArgs e)
        {
            // Get the current drop target advisor
            CurrentDropTargetAdvisor = GetDropTargetAdvisor(sender as DependencyObject);

            UpdateEffects(e);

            // Setup the preview Adorner
            offsetPoint = new Point();
            if (CurrentDropTargetAdvisor.ApplyMouseOffset && e.Data.GetData(DragOffsetFormat) != null)
                offsetPoint = (Point) e.Data.GetData(DragOffsetFormat);

            CreatePreviewAdorner(sender as UIElement, e.Data);

            CurrentDropTargetAdvisor.OnEnterDropZone(e.Data);
            e.Handled = true;
        }

        /// <summary>
        ///     Updates the mouse pointer effects.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs" /> instance containing the event data.</param>
        private static void UpdateEffects(DragEventArgs e)
        {
            if (CurrentDropTargetAdvisor.IsValidDataObject(e.Data) == false)
                e.Effects = DragDropEffects.None;
            else if ((e.AllowedEffects & DragDropEffects.Move) == 0 &&
                     (e.AllowedEffects & DragDropEffects.Copy) == 0)
                e.Effects = DragDropEffects.None;
            else if ((e.AllowedEffects & DragDropEffects.Move) != 0 &&
                     (e.AllowedEffects & DragDropEffects.Copy) != 0)
            {
                e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0)
                    ? DragDropEffects.Copy
                    : DragDropEffects.Move;
            }
        }
        #endregion Drop Target Events

        /* -------------------------------------------------------------------
         *     Drag Source events 
         * ------------------------------------------------------------------- */
        #region Drag Source events
        /// <summary>
        ///     Handles the PreviewMouseLeftButtonDown event of the DragSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.</param>
        private static void DragSourcePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Make this the new drag source
            CurrentDragSourceAdvisor = GetDragSourceAdvisor(sender as DependencyObject);

            if (CurrentDragSourceAdvisor.IsDraggable(e.Source as UIElement) == false)
                return;

            draggedElt = e.Source as UIElement;
            dragStartPoint = e.GetPosition(CurrentDragSourceAdvisor.GetTopContainer());
            offsetPoint = e.GetPosition(draggedElt);
            isMouseDown = true;
        }

        /// <summary>
        ///     Handles the PreviewMouseMove event of the DragSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs" /> instance containing the event data.</param>
        private static void DragSourcePreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown && IsDragGesture(e.GetPosition(CurrentDragSourceAdvisor.GetTopContainer())))
                DragStarted(sender as UIElement);
        }

        /// <summary>
        ///     Handles the PreviewMouseUp event of the DragSource control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.</param>
        private static void DragSourcePreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;
            Mouse.Capture(null);
        }
        
        /// <summary>
        ///     Starts the dragging operation.
        /// </summary>
        /// <param name="uiElt">The dragged UI element.</param>
        private static void DragStarted(UIElement uiElt)
        {
            isMouseDown = false;
            Mouse.Capture(uiElt);

            // Daten des Drag-Elements holen
            var data = CurrentDragSourceAdvisor.GetDataObject(draggedElt);
            // und diese Daten mit Zusatzinformationen anreichern
            data.SetData(DragOffsetFormat, offsetPoint);
            data.SetData(DragSourceFormat, draggedElt);

            // Die unterstützten DnD-Effekte bei der Drag-Quelle erfragen
            var supportedEffects = CurrentDragSourceAdvisor.SupportedEffects;

            // Perform Drag and Drop operation
            var effects = System.Windows.DragDrop.DoDragDrop(draggedElt, data, supportedEffects);

            // finish DnD operation
            CurrentDragSourceAdvisor.FinishDrag(draggedElt, effects);

            // Clean up
            RemovePreviewAdorner();
            Mouse.Capture(null);
            draggedElt = null;
        }

        /// <summary>
        ///     Determines whether the mouse movement is a drag gesture, based on the specified point.
        /// </summary>
        /// <param name="point">The mouse cursor position.</param>
        /// <returns>
        ///     <c>true</c> if the mouse movement is a drag gesture; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsDragGesture(Point point)
        {
            var horizontalGesture = Math.Abs(point.X - dragStartPoint.X) >
                                    SystemParameters.MinimumHorizontalDragDistance;
            var verticalGesture = Math.Abs(point.Y - dragStartPoint.Y) >
                                  SystemParameters.MinimumVerticalDragDistance;

            return horizontalGesture | verticalGesture;
        }
        #endregion Drag Source events


        #region Dependency Properties Getter/Setters

        /// <summary>
        ///     Sets the drag source advisor.
        /// </summary>
        /// <param name="depObj">The dep obj.</param>
        /// <param name="advisor">The advisor.</param>
        public static void SetDragSourceAdvisor(DependencyObject depObj, IDragSourceAdvisor advisor)
        {
            depObj.SetValue(DragSourceAdvisorProperty, advisor);
        }

        /// <summary>
        ///     Sets the drop target advisor.
        /// </summary>
        /// <param name="depObj">The dep obj.</param>
        /// <param name="advisor">The advisor.</param>
        public static void SetDropTargetAdvisor(DependencyObject depObj, IDropTargetAdvisor advisor)
        {
            depObj.SetValue(DropTargetAdvisorProperty, advisor);
        }

        /// <summary>
        ///     Gets the drag source advisor.
        /// </summary>
        /// <param name="depObj">The dependency object representing a drag source.</param>
        /// <returns>A drag source advisor object.</returns>
        public static IDragSourceAdvisor GetDragSourceAdvisor(DependencyObject depObj)
        {
            return depObj.GetValue(DragSourceAdvisorProperty) as IDragSourceAdvisor;
        }

        /// <summary>
        ///     Gets the drop target advisor.
        /// </summary>
        /// <param name="depObj">The dependency object representing a drop target.</param>
        /// <returns>A drop target advisor object.</returns>
        public static IDropTargetAdvisor GetDropTargetAdvisor(DependencyObject depObj)
        {
            return depObj.GetValue(DropTargetAdvisorProperty) as IDropTargetAdvisor;
        }

        #endregion

        #region Property Change handlers

        /// <summary>
        ///     Called when the drag source advisor changed.
        /// </summary>
        /// <param name="depObj">The dependency object representing the drag source.</param>
        /// <param name="args">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnDragSourceAdvisorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            var sourceElt = (UIElement)depObj;
            if (args.NewValue != null && args.OldValue == null)
            {
                sourceElt.PreviewMouseLeftButtonDown += DragSourcePreviewMouseLeftButtonDown;
                sourceElt.PreviewMouseMove += DragSourcePreviewMouseMove;
                sourceElt.PreviewMouseUp += DragSourcePreviewMouseUp;

                // Set the Drag source UI
                var advisor = (IDragSourceAdvisor)args.NewValue;
                advisor.SourceUI = sourceElt;
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                sourceElt.PreviewMouseLeftButtonDown -= DragSourcePreviewMouseLeftButtonDown;
                sourceElt.PreviewMouseMove -= DragSourcePreviewMouseMove;
                sourceElt.PreviewMouseUp -= DragSourcePreviewMouseUp;
            }
        }

        /// <summary>
        ///     Called when the drop target advisor changed.
        /// </summary>
        /// <param name="depObj">The depenendy object that represents the drop target.</param>
        /// <param name="args">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnDropTargetAdvisorChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            var targetElt = (UIElement)depObj;
            if (args.NewValue != null && args.OldValue == null)
            {
                targetElt.PreviewDragEnter += DropTargetPreviewDragEnter;
                targetElt.PreviewDragOver += DropTargetPreviewDragOver;
                targetElt.PreviewDragLeave += DropTargetPreviewDragLeave;
                targetElt.PreviewDrop += DropTargetPreviewDrop;
                targetElt.AllowDrop = true;

                // Set the Drag source UI
                var advisor = (IDropTargetAdvisor)args.NewValue;
                advisor.TargetUI = targetElt;
            }
            else if (args.NewValue == null && args.OldValue != null)
            {
                targetElt.PreviewDragEnter -= DropTargetPreviewDragEnter;
                targetElt.PreviewDragOver -= DropTargetPreviewDragOver;
                targetElt.PreviewDragLeave -= DropTargetPreviewDragLeave;
                targetElt.PreviewDrop -= DropTargetPreviewDrop;
                targetElt.AllowDrop = false;
            }
        }

        #endregion

       /* -------------------------------------------------------------------
        *       Utility functions
        * ------------------------------------------------------------------- */

        #region PreviewAdorner

        /// <summary>
        ///     Creates the preview adorner.
        /// </summary>
        /// <param name="adornedElt">The adorned element.</param>
        /// <param name="data">The objects data.</param>
        private static void CreatePreviewAdorner(UIElement adornedElt, IDataObject data)
        {
            if (overlayElt != null)
                return;

            var layer = AdornerLayer.GetAdornerLayer(CurrentDropTargetAdvisor.GetTopContainer());
            var feedbackUI = CurrentDropTargetAdvisor.GetVisualFeedback(data);
            overlayElt = new DropPreviewAdorner(feedbackUI, adornedElt);
            PositionAdorner();
            layer.Add(overlayElt);
        }

        /// <summary>
        ///     Positions the adorner.
        /// </summary>
        private static void PositionAdorner()
        {
            overlayElt.Left = adornerPosition.X - offsetPoint.X;
            overlayElt.Top = adornerPosition.Y - offsetPoint.Y;
        }

        /// <summary>
        ///     Removes the preview adorner.
        /// </summary>
        private static void RemovePreviewAdorner()
        {
            if (overlayElt != null)
            {
                AdornerLayer.GetAdornerLayer(CurrentDropTargetAdvisor.GetTopContainer()).Remove(overlayElt);
                overlayElt = null;
            }
        }

        #endregion
    }
}