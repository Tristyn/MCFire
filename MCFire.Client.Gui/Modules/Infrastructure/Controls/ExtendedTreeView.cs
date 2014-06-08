namespace MCFire.Client.Gui.Modules.Infrastructure.Controls
{
    public class ExtendedTreeView : TreeView
    {
        public ExtendedTreeView()
        {
            SelectedItemChanged += SelectedItemHandler;
        }

        void SelectedItemHandler(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (base.SelectedItem != null)
            {
                SetValue(SelectedItemProperty, base.SelectedItem);
            }
        }

        public new object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public new static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(ExtendedTreeView), new UIPropertyMetadata(null));
    }
}
