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
            var action = new OpenEditorTo(IoC.Get<WorldExplorerService>().Installations.First().Worlds.First(), 0);
            IoC.BuildUp(action);
            action.Execute(new ActionExecutionContext());
        }
#endif
    }
}
