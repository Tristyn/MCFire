using System.Collections.Generic;
using System.Windows;

namespace MCFire.Modules.Infrastructure.Views
{
    /// <summary>
    /// Interaction logic for CommandsControl.xaml
    /// </summary>
    public partial class CommandsView
    {
        public CommandsView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CommandsProperty = DependencyProperty.Register(
            "Commands", typeof (IEnumerable<object>), typeof (CommandsView), new PropertyMetadata(default(IEnumerable<object>)));

        public IEnumerable<object> Commands
        {
            get { return (IEnumerable<object>) GetValue(CommandsProperty); }
            set { SetValue(CommandsProperty, value); }
        }
    }
}
