﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Modules.Explorer.Models;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Substrate;
using Substrate.Core;
using Substrate.Data;
using Vector3 = SharpDX.Vector3;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// Simple MiniCube application using SharpDX.Toolkit.
    /// The purpose of this application is to show a rotating cube using <see cref="BasicEffect"/>.
    /// </summary>
    public sealed class EditorGame : Game
    {
        readonly IEnumerable<IGameComponent> _components;
        // rendering
        BasicEffect _basicEffect;
#if DEBUG
        DebugCube _debugCube;
#endif

        // TODO: unified content system
        // content
        public SpriteFont Font { get; private set; }
        Texture ErrorTexture { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorGame" /> class.
        /// </summary>
        /// <param name="sharpDxElement">The control used to listen to input events.</param>
        /// <param name="components">The game component services.</param>
        /// <param name="world">The Minecraft world to use as a data source.</param>
        /// <param name="substrateWorld"></param>
        /// <param name="dimension">The dimension of the world.</param>
        public EditorGame([NotNull] SharpDXElement sharpDxElement, [CanBeNull] IEnumerable<IGameComponent> components, [NotNull] MCFireWorld world,
            [NotNull] NbtWorld substrateWorld, int dimension)
        {
            _components = (from component in components
                           orderby component.DrawPriority
                           select component).ToArray();
            World = world;
            Dimension = dimension;
            SubstrateWorld = substrateWorld;
            ChunkManager = substrateWorld.GetChunkManager(dimension);
            BlockManager = substrateWorld.GetBlockManager(dimension);
            DataManager = substrateWorld.GetDataManager();
            Level = substrateWorld.Level;

            ToDispose(new GraphicsDeviceManager(this));
            SharpDxElement = sharpDxElement;
            Content.RootDirectory = @"Modules/Editor/Content";
        }

        public Level Level { get; set; }

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

#if DEBUG
            // Draw the cube
            _debugCube.Draw(this);
#endif

            // Draw debug
            SpriteBatch.Begin();
            SpriteBatch.DrawString(Font, String.Format("Controls: WASD to move, QE to control yaw, RF to control pitch"), new Vector2(0, 0), Color.Black);
            SpriteBatch.DrawString(Font, String.Format("Camera: {0}", Camera.Position), new Vector2(0, 15), Color.Black);
            SpriteBatch.DrawString(Font, String.Format("LookAt: {0}", Camera.Direction), new Vector2(0, 30), Color.Black);
            SpriteBatch.End();

            // Reset
            GraphicsDevice.Flush();

            // Handle base.Draw
            base.Draw(gameTime);
        }

        [NotNull]
        public Camera Camera { get; private set; }

        [NotNull]
        public Mouse Mouse { get; private set; }

        [NotNull]
        public Keyboard Keyboard { get; private set; }

        [NotNull]
        public Tasks Tasks { get; private set; }

        [NotNull]
        public SharpDXElement SharpDxElement { get; private set; }

        [NotNull]
        public MCFireWorld World { get; private set; }

        [NotNull]
        public NbtWorld SubstrateWorld { get; private set; }
        public IChunkManager ChunkManager { get; private set; }
        public IBlockManager BlockManager { get; set; }
        public DataManager DataManager { get; set; }

        #region Content

        protected override void LoadContent()
        {
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
                component.Game = this;
                component.LoadContent();
            }

            // content
            Font = ToDisposeContent(Content.Load<SpriteFont>("Segoe12"));
#if DEBUG
            _debugCube = ToDisposeContent(new DebugCube(this));
#endif

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            foreach (var component in _components)
            {
                component.Dispose();
            }
        }

        public T LoadContent<T>(string assetName) where T : class,IDisposable
        {
            return ToDisposeContent(Content.Load<T>(assetName));
        }

        public new T ToDisposeContent<T>(T asset) where T : IDisposable
        {
            return base.ToDisposeContent(asset);
        }

        #endregion

        public int Dimension { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
    }
}