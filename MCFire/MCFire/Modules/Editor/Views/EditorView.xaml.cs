using System.Windows.Input;

namespace MCFire.Modules.Editor.Views
{
    /// <summary>
    /// Interaction logic for D3DView.xaml
    /// </summary>
    public partial class D3DView
    {
        public D3DView()
        {
            InitializeComponent();
        }

        private void SetSharpDxFocus(object sender, MouseButtonEventArgs e)
        {
            // a hack to let the sharpdx control regain focus when clicked.
            Keyboard.Focus(SharpDx);
        }
    }
}
