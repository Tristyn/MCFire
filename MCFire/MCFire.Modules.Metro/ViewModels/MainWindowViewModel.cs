using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using MCFire.Modules.Metro.Commands;

namespace MCFire.Modules.Metro.ViewModels
{
    [Export(typeof(IMainWindow))]
    public class MainWindowViewModel : Gemini.Modules.MainWindow.ViewModels.MainWindowViewModel
    {
        private BindableCollection<IWindowCommand> _commands;

        /// <summary>
        /// Having two properties for one field instead of an importing constructor is a hack, so that an annoying Mef exception is not thrown.
        /// </summary>
        [ImportMany]
        // ReSharper disable once UnusedMember.Local
        private IEnumerable<IWindowCommand> CommandImports
        {
            set { Commands = new BindableCollection<IWindowCommand>(value); }
        }

        public BindableCollection<IWindowCommand> Commands
        {
            get { return _commands; }
            private set
            {
                if (_commands == value) return;
                _commands = value;
                NotifyOfPropertyChange(() => Commands);
            }
        }
    }
}
