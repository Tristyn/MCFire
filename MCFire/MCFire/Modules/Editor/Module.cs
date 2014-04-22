using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Modules.MainMenu.Models;
using MCFire.Modules.Editor.Actions;
using MCFire.Modules.Explorer.Services;

namespace MCFire.Modules.Editor
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Import]
        WorldExplorerService _explorer;

        public override void Initialize()
        {
            MainMenu.All.First(menu => menu.Name == "View")
                .Add(new MenuItem("3D Test", Open3DTest));
        }

        IEnumerable<IResult> Open3DTest()
        {
            yield return new OpenEditorTo(_explorer.Installations.First().Worlds.First(), 0);
        }
    }
}
