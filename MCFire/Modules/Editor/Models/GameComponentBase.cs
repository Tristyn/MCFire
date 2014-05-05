using System;
using MCFire.Modules.Explorer.Models;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.Editor.Models
{
    public abstract class GameComponentBase : IGameComponent
    {
        protected EditorGame Game { get; private set; }
        public virtual void Update(GameTime time) { }
        public virtual void Draw(GameTime time) { }

        /// <inheritDoc/>
        public virtual int DrawPriority { get { return 100; } }
        public virtual void Dispose() { }

        public virtual void LoadContent(EditorGame game)
        {
            if (Game != null)
                throw new InvalidOperationException("GameComponent.Game can not be set twice.");

            Game = game;
            World = game.World;
            Dimension = game.Dimension;
            GraphicsDevice = game.GraphicsDevice;
            Camera = game.Camera;
            Keyboard = game.Keyboard;
            Mouse = game.Mouse;
            Tasks = game.Tasks;
            SharpDxElement = game.SharpDxElement;
        }

        public virtual void UnloadContent(EditorGame game) { }

        protected virtual SharpDXElement SharpDxElement { get; private set; }

        protected virtual Mouse Mouse { get; private set; }

        protected virtual Keyboard Keyboard { get; private set; }

        protected virtual Camera Camera { get; private set; }

        protected virtual Tasks Tasks { get; private set; }

        protected virtual MCFireWorld World { get; private set; }
        protected virtual int Dimension { get; private set; }
        protected virtual GraphicsDevice GraphicsDevice { get; private set; }


    }
}
