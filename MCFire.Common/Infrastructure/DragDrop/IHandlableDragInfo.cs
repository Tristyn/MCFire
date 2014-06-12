using GongSolutions.Wpf.DragDrop;

namespace MCFire.Common.Infrastructure.DragDrop
{
    /// <summary>
    /// A <see cref="GongSolutions.Wpf.DragDrop.IDragInfo"/> that has a <see cref="Handled"/> property.
    /// </summary>
    public interface IHandleableDragInfo : IDragInfo
    {
        bool Handled { get; set; }
    }
}
