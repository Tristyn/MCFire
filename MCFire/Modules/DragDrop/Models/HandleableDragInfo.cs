using System.Collections;
using System.Windows;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;

namespace MCFire.Modules.DragDrop.Models
{
    class HandleableDragInfo : IHandleableDragInfo
    {
        public HandleableDragInfo([NotNull] IDragInfo info)
        {
            Data = info.Data;
            DragStartPosition = info.DragStartPosition;
            PositionInDraggedItem = info.PositionInDraggedItem;
            Effects = info.Effects;
            MouseButton = info.MouseButton;
            SourceCollection = info.SourceCollection;
            SourceIndex = info.SourceIndex;
            SourceItem = info.SourceItem;
            SourceItems = info.SourceItems;
            VisualSource = info.VisualSource;
            VisualSourceItem = info.VisualSourceItem;
            VisualSourceFlowDirection = info.VisualSourceFlowDirection;
            DataObject = info.DataObject;
        }
        public object Data { get; set; }
        public Point DragStartPosition { get; private set; }
        public Point PositionInDraggedItem { get; private set; }
        public DragDropEffects Effects { get; set; }
        public MouseButton MouseButton { get; private set; }
        public IEnumerable SourceCollection { get; private set; }
        public int SourceIndex { get; private set; }
        public object SourceItem { get; private set; }
        public IEnumerable SourceItems { get; private set; }
        public UIElement VisualSource { get; private set; }
        public UIElement VisualSourceItem { get; private set; }
        public FlowDirection VisualSourceFlowDirection { get; private set; }
        public IDataObject DataObject { get; set; }
        public bool Handled { get; set; }
    }
}
