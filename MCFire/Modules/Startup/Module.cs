using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using MCFire.Modules.Editor.Actions;
using MCFire.Modules.Explorer.Services;
using MCFire.Modules.Metro.ViewModels;
using MCFire.Modules.Startup.ViewModels;
using MCFire.Properties;

namespace MCFire.Modules.Startup
{
    [Export(typeof(IModule))]
    class Module : ModuleBase
    {
        [Import] IOverlayHost _overlayHost;
        public override void Initialize()
        {
            MainWindow.Title = "MCFire - Getchyo snacks!";
        }

        public override void PostInitialize()
        {
            // remove the toolbox.
            var viewMenu = MainMenu.All.FirstOrDefault(item => item.Name == "View");
            if(viewMenu==null)return;
            foreach (var item in viewMenu.Where(item => item.Name == "Toolbox"))
            {
                viewMenu.Children.Remove(item);
                return;
            }
        }

#if FIRSTRUN
        public override async void PostInitialize()
        {
            if (!Settings.Default.FirstRun)
                return;

            await Task.Delay(1000);

            _overlayHost.TrySetOverlay<ConfigureInstallationsViewModel>();
        }
#endif

#if EDITOR
        public override void PostInitialize()
        {
            var install = IoC.Get<WorldExplorerService>().Installations.FirstOrDefault();
            if(install==null)return;
            var world = install.Worlds.First();
            if(world==null) return;
            var action = new OpenEditorTo(world, 0);
            IoC.BuildUp(action);
            action.Execute(new ActionExecutionContext());
        }
#endif
    }
}
