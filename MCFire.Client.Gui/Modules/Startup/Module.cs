using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Gemini.Framework;
using MCFire.Client.Primitives.Installations;
using MCFire.Common.Components;
using NUnrar.Archive;
using NUnrar.Common;
#if EDITOR
using MCFire.Client.Services;
using Caliburn.Micro;
using MCFire.Client.Gui.Modules.Editor.Actions;
#endif
#if FIRSTRUN || !DEBUG
using MCFire.Client.Services;
using MCFire.Client.Gui.Modules.Startup.ViewModels;
using MCFire.Client.Gui.Properties;
using System.Threading.Tasks;
#endif

namespace MCFire.Client.Gui.Modules.Startup
{
    [Export(typeof(IModule))]
    class Module : ModuleBase
    {
#if FIRSTRUN||!DEBUG
        [Import]
        IOverlayService _overlayService;
#endif

#if EDITOR
        [Import]
        IWorldExplorerService _explorerService;
#endif

        // create all startup objects
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once ValueParameterNotUsed
        [ImportMany]
        IEnumerable<ICreateAtStartup> StartupObjects { set { } }
        // TODO: import IEnumerable<string[]> startup messages. pick one at random
        public override void Initialize()
        {
            MainWindow.Title = "MCFire - Getchyo snacks!";
        }

#if FIRSTRUN || !DEBUG
        public async override void PostInitialize()
#else
        public override void PostInitialize()
#endif
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
            try
            {
                var rarUri = Path.Combine(MCFireDirectories.Install, "Resources", "SampleWorld.rar");
                var rarDir = new Uri(rarUri).LocalPath;
                var extractDir = Path.Combine(MCFireDirectories.MCFireAppdata, "SampleWorld");

                try
                {
                    Directory.Delete(extractDir, true);
                }
                catch (DirectoryNotFoundException) { }
                RarArchive.WriteToDirectory(rarDir, MCFireDirectories.MCFireAppdata, ExtractOptions.ExtractFullPath);
                var sampleInstall = Installation.New(extractDir);
                if (sampleInstall != null)
                    _explorerService.TryAddInstallation(sampleInstall);
                if (sampleInstall == null) return;
                var world = sampleInstall.Worlds.First();
                if (world == null) return;
                var action = new OpenEditorTo(world, 0);
                IoC.BuildUp(action);
                action.Execute(new ActionExecutionContext());
            }
            // ReSharper disable once EmptyGeneralCatchClause DIRTY HACK
            catch
            {
                Debug.Assert(false);
            }
#endif

#if FIRSTRUN || !DEBUG
            if (!Settings.Default.FirstRun)
                return;
            await Task.Delay(1000);
            _overlayService.TrySetOverlay<ConfigureInstallationsViewModel>();
#endif
        }
    }
}
