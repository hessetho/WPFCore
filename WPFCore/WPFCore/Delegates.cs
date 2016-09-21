using System.Windows.Controls;

namespace WPFCore
{
    /// <summary>
    /// Describes a callback used to show a content control in modal dialog
    /// </summary>
    /// <param name="contentControl">The content control.</param>
    /// <param name="title">The title.</param>
    /// <returns></returns>
    public delegate bool OpenDialogDelegate(ContentControl contentControl, string title);

    /// <summary>
    /// Describes a callback used to show a content control in non-modal dialog
    /// </summary>
    /// <param name="contentControl">The content control.</param>
    /// <param name="title">The title.</param>
    public delegate void OpenWindowDelegate(ContentControl contentControl, string title);
}
