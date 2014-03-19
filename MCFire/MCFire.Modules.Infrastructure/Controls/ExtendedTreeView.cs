using System.Windows;
using System.Windows.Controls;

namespace MCFire.Modules.Infrastructure.Controls
{
    public class ExtendedTreeView : TreeView
    {
        public ExtendedTreeView()
        {
            SelectedItemChanged += ___ICH;
        }

        void ___ICH(object sender, RoutedPropertyChangedEventArgs<object> e)
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
