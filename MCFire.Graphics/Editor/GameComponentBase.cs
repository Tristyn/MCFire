using System;
using GongSolutions.Wpf.DragDrop;
using MCFire.Common;
using MCFire.Common.Infrastructure.DragDrop;
using MCFire.Graphics.Modules.Editor.Models;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Graphics.Editor
{
    public abstract class GameComponentBase : IGameComponent
    {
        protected IEditorGame Game { get; private set; }
        // TODO: add an Update that runs off of the game loop (TPL mabey?) that gets used for blocking/long running calls (eg: voxel tracing in BoxSelectorComponent and the meshalyzer)
        // TODO: perhaps EditorGame, IGameComponents and IEditorTool shouldn't have access to MCFireWorld and code that relies on this gets moved to MCFire.Core 
        public virtual void Update(GameTime time) { }
        public virtual void Draw(GameTime time) { }

        /// <inheritDoc/>
        public virtual int DrawPriority { get { return 100; } }
        public virtual void Dispose() { }

        public void LoadContent(IEditorGame game)
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

            LoadContent();
        }

        protected virtual void LoadContent(){}

        public virtual void UnloadContent() { }

        #region Drag Drop
        #region DragSource

        public virtual void StartDrag(IHandleableDragInfo dragInfo)
        {
        }

        public virtual void Dropped(IDropInfo dropInfo)
        {
        }

        public virtual void DragCancelled()
        {
        }

        #endregion

        #region DropTarget

        public virtual void DragOver(IDropInfo dropInfo)
        {
        }

        public virtual void Drop(IDropInfo dropInfo)
        {
        }

        #endregion
        #endregion

        public virtual void WpfKeyDown(System.Windows.Input.KeyEventArgs e)
        {
        }

        protected virtual SharpDXElement SharpDxElement { get; private set; }

        protected virtual Mouse Mouse { get; private set; }

        protected virtual Keyboard Keyboard { get; private set; }

        protected virtual Camera Camera { get; private set; }

        protected virtual Tasks Tasks { get; private set; }

        protected virtual World World { get; private set; }
        protected virtual int Dimension { get; private set; }
        protected virtual GraphicsDevice GraphicsDevice { get; private set; }

        
    }
}
