using System;
using System.ComponentModel.Composition;
using System.Windows;
using Gemini.Framework;

namespace MCFire.Client.Gui.Modules.MainWindow
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
