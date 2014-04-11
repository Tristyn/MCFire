using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework.Services;
using JetBrains.Annotations;
using MCFire.Modules.Infrastructure.Interfaces;
using MCFire.Modules.Metro.Commands;

namespace MCFire.Modules.Metro.ViewModels
{
    [Export(typeof(IMainWindow))]
    [Export(typeof(IOverlayHost))]
    public class MainWindowViewModel : Gemini.Modules.MainWindow.ViewModels.MainWindowViewModel, IOverlayHost
    {
        BindableCollection<IWindowCommand> _commands;
        IModalOverlay _dialogue;

        public bool TrySetOverlay<TOverlay>() where TOverlay : IModalOverlay
        {
            //if (Dialogue != null) return false;
            return TrySetOverlay(IoC.Get<TOverlay>());
        }

        public bool TrySetOverlay(IModalOverlay overlay)
        {
            if (Dialogue != null)
                return false;

            Dialogue = overlay;
            overlay.CloseOverlay += ResetOverlay;
            return true;
        }

        void ResetOverlay(object sender, EventArgs e)
        {
            ((IModalOverlay)sender).CloseOverlay -= ResetOverlay;
            Dialogue = null;
        }

        /// <summary>
        /// Having two properties for one field instead of an importing constructor is a hack, so that an annoying Mef exception is not thrown.
        /// </summary>
        [ImportMany]
        // ReSharper disable once UnusedMember.Local
        private IEnumerable<IWindowCommand> CommandImports
        {
            set
            {
                Commands = new BindableCollection<IWindowCommand>(value);
                NotifyOfPropertyChange(() => Commands);
            }
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

        /// <summary>
        /// The current dialogue.
        /// </summary>
        [CanBeNull]
        public IModalOverlay Dialogue
        {
            get { return _dialogue; }
            private set
            {
                _dialogue = value;
                NotifyOfPropertyChange(() => Dialogue);
                NotifyOfPropertyChange(() => DialogueVisible);
            }
        }

        public bool? DialogueVisible
        {
            get { return _dialogue != null; }
        }
    }

    public interface IOverlayHost
    {
        IModalOverlay Dialogue { get; }
        bool? DialogueVisible { get; }
        bool TrySetOverlay(IModalOverlay overlay);
        bool TrySetOverlay<TOverlay>() where TOverlay : IModalOverlay;
    }
}
