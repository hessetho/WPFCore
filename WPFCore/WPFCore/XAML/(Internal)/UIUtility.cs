using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WPFCore.XAML
{
    /// <summary>
    /// Stellt Funktionen bereit, welche in einem hierarchischen <see cref="ItemsControl"/> navigieren.
    /// Aus SynesthesiaMLib übernommen und erweitert.
    /// </summary>
    internal static class UIUtility
    {
        /// <summary>
        /// Selects an item in a hierarchial ItemsControl using a set of options
        /// </summary>
        /// <typeparam name="T">The type of the items present in the control and in the options</typeparam>
        /// <param name="control">The ItemsControl to select an item in</param>
        /// <param name="info">The options used for the selection process</param>
        public static void SetSelectedItem<T>(ItemsControl control, SetSelectedInfo<T> info)
        {
            // nimmt das erste Element aus dem Suchpfad raus
            var currentItem = info.Items.First();

            if (control.IsLoaded && control.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                // Compare each item in the container and look for the next item in the chain.
                foreach (var item in control.Items)
                {
                    T convertedItem;

                    // Convert the item if a conversion method exists. Otherwise just cast the item to the desired type.
                    if (info.ConvertMethod != null)
                        convertedItem = info.ConvertMethod(item);
                    else
                        convertedItem = (T)item;

                    // Compare the converted item with the item in the chain
                    if ((info.CompareMethod != null) &&
                        info.CompareMethod(convertedItem, currentItem))
                    {
                        var container = (ItemsControl)control.ItemContainerGenerator.ContainerFromItem(item);

                        // Replace with the remaining items in the chain
                        info.Items = info.Items.Skip(1);

                        // If no items are left in the chain, then we're finished
                        if (!info.Items.Any())
                        {
                            // Select the last item
                            if (info.OnSelected != null)
                                info.OnSelected(container, info);
                        }
                        else
                        {
                            // Request more items and continue the search
                            if (info.OnNeedMoreItems != null)
                            {
                                info.OnNeedMoreItems(container, info);
                                SetSelectedItem<T>(container, info);
                            }
                        }

                        break;
                    }
                }
            }
            else if (control.IsLoaded == false)
            {
                RoutedEventHandler selectWhenLoadedMethod = null;

                selectWhenLoadedMethod = (ds, de) =>
                    {
                        // Stop listening for the loaded event
                        control.Loaded -= selectWhenLoadedMethod;
                        // Search the container for the item chain
                        SetSelectedItem(control, info);
                    };
                control.Loaded += selectWhenLoadedMethod;
            }
            else
            {
                // If the item containers haven't been generated yet, attach an event
                // and wait for the status to change.
                EventHandler selectWhenReadyMethod = null;

                selectWhenReadyMethod = (ds, de) =>
                {
                    if (control.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                    {
                        // Stop listening for status changes on this container
                        control.ItemContainerGenerator.StatusChanged -= selectWhenReadyMethod;

                        // Search the container for the item chain
                        SetSelectedItem(control, info);
                    }
                };

                control.ItemContainerGenerator.StatusChanged += selectWhenReadyMethod;
            }
        }

    }

    public class SetSelectedInfo<T>
    {
        /// <summary>
        /// Gets or sets the chain of items to search for. The last item in the chain will be selected.
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Gets or sets the method used to compare items in the control with items in the chain
        /// </summary>
        public Func<T, T, bool> CompareMethod { get; set; }

        /// <summary>
        /// Gets or sets the method used to convert items in the control to be compare with items in the chain
        /// </summary>
        public Func<object, T> ConvertMethod { get; set; }

        /// <summary>
        /// Gets or sets the method used to select the final item in the chain
        /// </summary>
        public SetSelectedEventHandler<T> OnSelected { get; set; }

        /// <summary>
        /// Gets or sets the method used to request more child items to be generated in the control
        /// </summary>
        public SetSelectedEventHandler<T> OnNeedMoreItems { get; set; }
    }

    public delegate void SetSelectedEventHandler<T>(ItemsControl container, SetSelectedInfo<T> info);
}
