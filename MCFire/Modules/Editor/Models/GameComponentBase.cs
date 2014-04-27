using System;
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

        /// <inheritDoc/>
        public virtual int DrawPriority { get { return 100; } }
        public virtual void Dispose() { }

        public virtual EditorGame Game
        {
            protected get { return _game; }
            set
            {
                if (_game != null)
                    throw new InvalidOperationException("GameComponent.Game can not be set twice.");

                _game = value;
                World = value.World;
                SubstrateWorld = value.SubstrateWorld;
                Dimension = value.Dimension;
                GraphicsDevice = value.GraphicsDevice;
                Camera = value.Camera;
                Keyboard = value.Keyboard;
                Mouse = value.Mouse;
                Tasks = value.Tasks;
                SharpDxElement = value.SharpDxElement;
            }
        }

        protected virtual SharpDXElement SharpDxElement { get; private set; }

        protected virtual Mouse Mouse { get; private set; }

        protected virtual Keyboard Keyboard { get; private set; }

        protected virtual Camera Camera { get; private set; }

        protected virtual Tasks Tasks { get; private set; }

        protected virtual NbtWorld SubstrateWorld { get; private set; }

        protected virtual MCFireWorld World { get; private set; }
        protected virtual int Dimension { get; private set; }
        protected virtual GraphicsDevice GraphicsDevice { get; private set; }


    }
}
