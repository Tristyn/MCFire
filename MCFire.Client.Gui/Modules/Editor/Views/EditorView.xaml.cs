namespace MCFire.Graphics.Modules.Editor.Views
{
    /// <summary>
    /// Interaction logic for EditorView.xaml
    /// </summary>
    public partial class EditorView
    {
        public EditorView()
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
