using MCFire.Modules.Explorer.Models;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Substrate;

namespace MCFire.Modules.Editor.Models
{
    public abstract class GameComponentBase : IGameComponent
    {
        private EditorGame _game;
        public virtual void LoadContent() { }

        public virtual void Update(GameTime time) { }

        public virtual void Draw(GameTime time) { }
        public virtual void Dispose() { }

        public virtual EditorGame Game
        {
            protected get { return _game; }
            set
            {
                _game = value;
                World = Game.World;
                SubstrateWorld = Game.SubstrateWorld;
                Dimension = Game.Dimension;
                GraphicsDevice = Game.GraphicsDevice;
                Camera = Game.Camera;
                Keyboard = Game.Keyboard;
                Mouse = Game.Mouse;
                SharpDxElement = Game.SharpDxElement;
            }
        }

        protected virtual SharpDXElement SharpDxElement { get; private set; }

        protected virtual Mouse Mouse { get; private set; }

        protected virtual Keyboard Keyboard { get; private set; }

        protected virtual Camera Camera { get; private set; }

        protected virtual NbtWorld SubstrateWorld { get; private set; }

        protected virtual MCFireWorld World { get; private set; }
        protected virtual int Dimension { get; private set; }
        protected virtual GraphicsDevice GraphicsDevice { get; private set; }


    }
}
