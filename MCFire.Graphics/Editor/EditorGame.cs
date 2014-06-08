﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MCFire.Graphics.Modules.Editor.Models
{
    /// <summary>
    /// A SharpDx Game used to render a minecraft world.
    /// </summary>
    public sealed class EditorGame : Game, IDragSource, IDropTarget
    {
        readonly IEnumerable<IGameComponent> _components;
        // rendering
        BasicEffect _basicEffect;

        // TODO: unified content system
        // content
        // TODO: have the ability to switch IEditors, each IEditor would have its own components and basic classes(Camera, Tasks)
        public SpriteFont Font { get; private set; }
        Texture ErrorTexture { get; set; }

        public Level Level { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorGame" /> class.
        /// </summary>
        /// <param name="sharpDxElement">The control used to listen to input events.</param>
        /// <param name="components">The game component services.</param>
        /// <param name="world">The Minecraft world to use as a data source.</param>
        /// <param name="dimension">The dimension of the world.</param>
        public EditorGame([NotNull] SharpDXElement sharpDxElement, [NotNull] IEnumerable<IGameComponent> components, [NotNull] MCFireWorld world,
            int dimension)
        {
            _components = (from component in components
                orderby component.DrawPriority
                select component).ToArray();
            World = world;
            Dimension = dimension;

            ToDisposeContent(new GraphicsDeviceManager(this));
            SharpDxElement = sharpDxElement;
            Content.RootDirectory = @"Modules/Editor/Content";
        }

        #region Components

        [CanBeNull]
        public TComponent GetComponent<TComponent>()
            where TComponent : class
        {
            return (from component in _components
                    where component is TComponent
                    select component).FirstOrDefault() as TComponent;
        }

        #endregion

        protected override void Update(GameTime gameTime)
        {
            Keyboard.Update();
            Mouse.Update();
            Camera.Update(gameTime);
            Tasks.Update(gameTime);

            foreach (var component in _components)
            {
                component.Update(gameTime);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // Calculate matrices
            _basicEffect.World = Matrix.Identity;
            _basicEffect.View = Camera.ViewMatrix;
            _basicEffect.Projection = Camera.ProjectionMatrix;
            // draw components
            foreach (var component in _components)
            {
                component.Draw(gameTime);
            }

            // Draw debug
            SpriteBatch.Begin();
            SpriteBatch.DrawString(Font, String.Format("Controls: WASD to move, QE to control yaw, RF to control pitch"), new Vector2(0, 0), Color.Black);
            SpriteBatch.DrawString(Font, String.Format((string) "Camera: {0}", (object) Camera.Position), new Vector2(0, 15), Color.Black);
            SpriteBatch.DrawString(Font, String.Format((string) "LookAt: {0}", (object) Camera.Direction), new Vector2(0, 30), Color.Black);
            SpriteBatch.End();

            // Handle base.Draw
            base.Draw(gameTime);
        }

        #region Content

        protected override void LoadContent()
        {
            GraphicsDevice = base.GraphicsDevice;
            ErrorTexture = ToDisposeContent(Content.Load<Texture2D>("Error"));

            // rendering
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _basicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
            });

            // input
            Keyboard = ToDispose(new Keyboard(this));
            Mouse = ToDispose(new Mouse(new MouseManager(this)));
            var camera = ToDispose(new Camera(this) { Position = new Vector3(0, 0, -5), Fov = MathUtil.PiOverTwo });
            camera.IdleRotate(new Vector3(0, 82, 4), 43, MathUtil.Pi / 16, .44f);
            Camera = camera;
            Tasks = new Tasks(this);

            foreach (var component in _components)
            {
                component.LoadContent(this);
            }

            // content
            Font = ToDisposeContent(Content.Load<SpriteFont>("Segoe12"));

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            foreach (var component in _components)
                component.Dispose();
        }

        public T LoadContent<T>(string assetName) where T : class,IDisposable
        {
            return ToDisposeContent<T>(Content.Load<T>(assetName));
        }

        public new T ToDisposeContent<T>(T asset) where T : IDisposable
        {
            return base.ToDisposeContent(asset);
        }

        #endregion

        #region Drag Drop

        #region Drag Source

        public void StartDrag(IDragInfo dragInfo)
        {
            var info = new HandleableDragInfo(dragInfo);
            foreach (var component in _components)
            {
                component.StartDrag(info);
                if (info.Handled) return;
            }
        }

        public void Dropped(IDropInfo dropInfo)
        {
            foreach (var component in _components)
            {
                component.Dropped(dropInfo);
                if (!dropInfo.NotHandled) return;
            }
        }

        public void DragCancelled()
        {
            foreach (var component in _components)
            {
                component.DragCancelled();
            }
        }

        #endregion

        #region Drop Target

        public void DragOver(IDropInfo dropInfo)
        {
            foreach (var component in _components)
            {
                component.DragOver(dropInfo);
                if (!dropInfo.NotHandled) return;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            foreach (var component in _components)
            {
                component.Drop(dropInfo);
                if (!dropInfo.NotHandled) return;
            }
        }

        #endregion

        #endregion

        [NotNull]
        public Camera Camera { get; private set; }

        [NotNull]
        public Mouse Mouse { get; private set; }

        [NotNull]
        public Keyboard Keyboard { get; private set; }

        [NotNull]
        public Tasks Tasks { get; private set; }

        public new GraphicsDevice GraphicsDevice { get; private set; }

        [NotNull]
        public SharpDXElement SharpDxElement { get; private set; }

        [NotNull]
        public MCFireWorld World { get; private set; }

        public int Dimension { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        public void WpfKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            foreach (var component in _components)
            {
                component.WpfKeyDown(e);
                if (e.Handled) return;
            }
        }
    }
}