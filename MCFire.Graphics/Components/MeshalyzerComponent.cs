using System.ComponentModel.Composition;
using SharpDX.Toolkit;

namespace MCFire.Graphics.Components
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof (IGameComponent))]
    public class MeshalyzerComponent : GameComponentBase
    {
        [Import] WorldRenderer _world;

        // TODO: use new meshalyzing system (BlockMeshalyzer)

        protected override void LoadContent()
        {
            _world.LoadContent(Game);
        }

        public override void Dispose()
        {
            _world.Dispose();
        }

        public override void Draw(GameTime time)
        {
            _world.Draw(time);
        }
    }
}
