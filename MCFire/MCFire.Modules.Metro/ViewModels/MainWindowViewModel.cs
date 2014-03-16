using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using Gemini.Framework.Services;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Metro.ViewModels
{
    [Export(typeof(IMainWindow))]
    public class MainWindowViewModel : Gemini.Modules.MainWindow.ViewModels.MainWindowViewModel
    {
        private BindableCollection<IWindowCommand> _commands;

        [Import]
        public IWindowCommand Command
        {
            get { return Commands.First(); }
            set
            {
                Commands = new BindableCollection<IWindowCommand>(new[] { value });
            }
        }

        public BindableCollection<IWindowCommand> Commands
        {
            get { return _commands; }
            set
            {
                if (_commands == value) return;
                _commands = value;
                NotifyOfPropertyChange(() => Commands);
            }
        }

        public MainWindowViewModel()
        {
        }
    }
}
