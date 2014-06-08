using System;

namespace MCFire.Client.Gui.Modules.Metro
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        public override void Initialize()
        {
            // TODO: rename Metro module to "Shell"
            Shell.CurrentTheme = new ResourceDictionary
            {
                Source = new Uri("/Gemini;component/Themes/Metro/Theme.xaml", UriKind.Relative)
            };
        }
    }
}
