using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.BoxSelector.Messages;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Infrastructure.Models;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;

namespace MCFire.Modules.BoxSelector.Models
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IGameComponent))]
    public class BoxSelectorComponent : GameComponentBase
    {
        BoxSelection _selection;
        DebugCube _cube;
        [NotNull]
        Mesh<VertexPositionTexture> _topQuad;
        [NotNull]
        Mesh<VertexPositionTexture> _botQuad;
        [NotNull]
        Mesh<VertexPositionTexture> _rightQuad;
        [NotNull]
        Mesh<VertexPositionTexture> _leftQuad;
        [NotNull]
        Mesh<VertexPositionTexture> _forwardQuad;
        [NotNull]
        Mesh<VertexPositionTexture> _backQuad;
        [NotNull]
        BoxSelectEffect _effect;
        [NotNull]
        Texture2D _gridTexture;
        SelectionState _selectionState;

        [Import]
        IEventAggregator _aggregator;

        public override void LoadContent(EditorGame game)
        {
            // TODO: proper state management (Enabled)
            Enabled = true;

            base.LoadContent(game);
            _effect = new BoxSelectEffect(game.LoadContent<Effect>("BoxSelect"))
            {
                Sampler = GraphicsDevice.SamplerStates.PointWrap
            };
            _gridTexture = game.LoadContent<Texture2D>("Grid");
            _topQuad = new Mesh<VertexPositionTexture>(Buffer.Vertex.New(game.GraphicsDevice, GetQuadUv(GeometricPrimitives.UpQuad)), _effect.Effect);
            _botQuad = new Mesh<VertexPositionTexture>(Buffer.Vertex.New(game.GraphicsDevice, GetQuadUv(GeometricPrimitives.DownQuad)), _effect.Effect);
            _rightQuad = new Mesh<VertexPositionTexture>(Buffer.Vertex.New(game.GraphicsDevice, GetQuadUv(GeometricPrimitives.RightQuad)), _effect.Effect);
            _leftQuad = new Mesh<VertexPositionTexture>(Buffer.Vertex.New(game.GraphicsDevice, GetQuadUv(GeometricPrimitives.LeftQuad)), _effect.Effect);
            _forwardQuad = new Mesh<VertexPositionTexture>(Buffer.Vertex.New(game.GraphicsDevice, GetQuadUv(GeometricPrimitives.ForwardQuad)), _effect.Effect);
            _backQuad = new Mesh<VertexPositionTexture>(Buffer.Vertex.New(game.GraphicsDevice, GetQuadUv(GeometricPrimitives.BackwardQuad)), _effect.Effect);


            // state
            _selectionState = SelectionState.NotSet;

            // input
            Mouse.Left.Click += Click;
            Mouse.Left.DragStart += DragStart;
            _cube = new DebugCube(game);
        }

        static VertexPositionTexture[] GetQuadUv(IList<Vector3> quad)
        {
            Debug.Assert(quad.Count == GeometricPrimitives.QuadUv.Length);

            var mesh = new VertexPositionTexture[quad.Count];
            for (int i = 0; i < quad.Count; i++)
            {
                mesh[i] = new VertexPositionTexture(quad[i], GeometricPrimitives.QuadUv[i]);
            }
            return mesh;
        }

        private void Click(object sender, KeyEventArgs e)
        {
            if (!Enabled) return;

            BlockPosition pos;
            if (!Tasks.TryGetBlockAtScreenCoord(e.Position, out pos))
                return;
            _cube.Position = pos;

            switch (_selectionState)
            {
                case SelectionState.NotSet:
                    _selection = new BoxSelection(pos, pos);
                    _selectionState = SelectionState.HalfSet;
                    break;
                case SelectionState.HalfSet:
                    _selection = new BoxSelection(_selection.CornerOne, pos);
                    _selectionState = SelectionState.Set;
                    break;
                case SelectionState.Set:
                    // TODO: dragging the selection faces along their normal axes
                    // if the click missed the BoxSelection, create a new selection.
                    _selection = new BoxSelection(pos, pos);
                    _selectionState = SelectionState.HalfSet;
                    // capture variable to avoid modified closure.
                    var selection = _selection;
                    _aggregator.Publish(new BoxSelectionUpdatedMessage(_selection, box => new BlockSelection(selection, Dimension, World)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DragStart(object sender, KeyEventArgs e)
        {
            if (!Enabled) return;


        }

        public override void Update(GameTime time)
        {
            if (!Enabled) return;

            BlockPosition pos;
            if (!Tasks.TryGetBlockAtScreenCoord(Mouse.Position, out pos))
                return;

            switch (_selectionState)
            {
                case SelectionState.NotSet:
                    _selection = new BoxSelection(pos, pos);
                    break;
                case SelectionState.HalfSet:
                    _selection = new BoxSelection(_selection.CornerOne, pos);
                    break;
                case SelectionState.Set:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Draw(GameTime time)
        {
            _cube.Draw(Game);
            //_cube.Position = _selection.CornerOne;

            GraphicsDevice.SetBlendState(GraphicsDevice.BlendStates.AlphaBlend);
            var viewProj = Camera.ViewMatrix * Camera.ProjectionMatrix;
            _effect.Main = _gridTexture;
            // TODO: refactor 6 Quad cube into seperate class
            // TODO: texture allignment (up is good)
            // up
            _effect.TransformMatrix = Matrix.Scaling(_selection.XLength, 0, _selection.ZLength) *
                                      Matrix.Translation(_selection.Left, _selection.Top + 1, _selection.Forward) *
                                      viewProj;
            _effect.MainTransform = new Vector4(_selection.XLength, _selection.ZLength, _selection.Left, _selection.Forward) / 16;
            _topQuad.Draw(GraphicsDevice);

            // down
            _effect.TransformMatrix = Matrix.Scaling(_selection.XLength, 0, _selection.ZLength) *
                                      Matrix.Translation(_selection.Left, _selection.Bottom, _selection.Forward) *
                                      viewProj;
            _effect.MainTransform = new Vector4(_selection.ZLength, _selection.XLength, _selection.Left, _selection.Forward) / 16;
            _botQuad.Draw(GraphicsDevice);

            // right
            _effect.TransformMatrix = Matrix.Scaling(0, _selection.YLength, _selection.ZLength) *
                                      Matrix.Translation(_selection.Right + 1, _selection.Bottom, _selection.Forward) *
                                      viewProj;
            _effect.MainTransform = new Vector4(_selection.ZLength, _selection.YLength, _selection.Forward, _selection.Bottom) / 16;
            _rightQuad.Draw(GraphicsDevice);

            // left
            _effect.TransformMatrix = Matrix.Scaling(0, _selection.YLength, _selection.ZLength) *
                                      Matrix.Translation(_selection.Left, _selection.Bottom, _selection.Forward) *
                                      viewProj;
            _effect.MainTransform = new Vector4(_selection.ZLength, _selection.YLength, _selection.Forward, _selection.Bottom) / 16;
            _leftQuad.Draw(GraphicsDevice);

            // back
            _effect.TransformMatrix = Matrix.Scaling(_selection.XLength, _selection.YLength, 0) *
                                      Matrix.Translation(_selection.Left, _selection.Bottom, _selection.Backward + 1) *
                                      viewProj;
            _effect.MainTransform = new Vector4(_selection.XLength, _selection.YLength, _selection.Left, _selection.Bottom) / 16;
            _backQuad.Draw(GraphicsDevice);
            // forward
            _effect.TransformMatrix = Matrix.Scaling(_selection.XLength, _selection.YLength, 0) *
                                      Matrix.Translation(_selection.Left, _selection.Bottom, _selection.Forward) *
                                      viewProj;
            _effect.MainTransform = new Vector4(_selection.XLength, _selection.YLength, _selection.Left, _selection.Bottom) / 16;
            _forwardQuad.Draw(GraphicsDevice);
        }

        public override void Dispose()
        {
            _topQuad.Dispose();
            _effect.Dispose();
        }

        public override int DrawPriority { get { return 500; } }

        /// <summary>
        /// If this component is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        private enum SelectionState
        {
            NotSet,
            HalfSet,
            Set
        }
    }
}
