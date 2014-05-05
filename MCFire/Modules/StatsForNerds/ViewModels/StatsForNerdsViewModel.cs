using System.ComponentModel.Composition;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using MCFire.Modules.Editor.Messages;

namespace MCFire.Modules.StatsForNerds.ViewModels
{
    [Export]
    public class StatsForNerdsViewModel : Tool, IHandle<EditorGainedFocusMessage>
    {
        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Bottom; }
        }

        public void Handle(EditorGainedFocusMessage message)
        {
            
        }
    }
}
