using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Framework;
using MCFire.Client.Services;
using MCFire.Core.Modules;

namespace MCFire.Client.Gui.Modules.Startup
{
    [Export(typeof(IModule))]
    class Module:ModuleBase
    {
        [Import]
        IOverlayService _overlayService;

        // create all startup objects
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once ValueParameterNotUsed
        [ImportMany]
        IEnumerable<ICreateAtStartup> StartupObjects { set { } }

        public override void Initialize()
        {
            MainWindow.Title = "MCFire - Getchyo snacks!";
        }

        public override void PostInitialize()
        {
            // remove the toolbox.
            var viewMenu = MainMenu.All.FirstOrDefault(item => item.Name == "View");
            if (viewMenu != null)
                foreach (var item in viewMenu.Where(item => item.Name == "Toolbox"))
                {
                    viewMenu.Children.Remove(item);
                    break;
                }

#if EDITOR
            var install = IoC.Get<WorldExplorerService>().Installations.FirstOrDefault();
            if (install == null) return;
            var world = install.Worlds.First();
            if (world == null) return;
            var action = new OpenEditorTo(world, 0);
            IoC.BuildUp(action);
            action.Execute(new ActionExecutionContext());
#endif

#if FIRSTRUN
            if (!Settings.Default.FirstRun)
                return;
            await Task.Delay(1000);
            _overlayHost.TrySetOverlay<ConfigureInstallationsViewModel>();
#endif
        }
    }
}
