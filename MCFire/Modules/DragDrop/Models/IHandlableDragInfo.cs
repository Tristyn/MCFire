using GongSolutions.Wpf.DragDrop;

namespace MCFire.Modules.DragDrop.Models
{
    /// <summary>
    /// A <see cref="IDragInfo"/> that has a <see cref="Handled"/> property.
    /// </summary>
    public interface IHandleableDragInfo : IDragInfo
    {
        bool Handled { get; set; }
    }
}
