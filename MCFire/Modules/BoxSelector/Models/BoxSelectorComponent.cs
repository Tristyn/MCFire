using System;
using System.ComponentModel.Composition;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Infrastructure.Models;
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
        Mesh<VertexPositionTexture> _quadMesh;
        [NotNull]
        BoxSelectEffect _effect;
        [NotNull]
        Texture2D _gridTexture;
        SelectionState _selectionState;

        public override void LoadContent(EditorGame game)
        {
            // TODO: proper state management (Enabled)
            Enabled = true;

            base.LoadContent(game);
            var vertices = Buffer.Vertex.New(
                game.GraphicsDevice,
                GeometricPrimitives.QuadVertexPositionTexture);
            _effect = new BoxSelectEffect(game.LoadContent<Effect>("BoxSelect"))
            {
                Sampler = GraphicsDevice.SamplerStates.PointWrap
            };
            _gridTexture = game.LoadContent<Texture2D>("Grid");
            _quadMesh = new Mesh<VertexPositionTexture>(vertices, _effect.Effect, true);

            // state
            _selectionState = SelectionState.NotSet;

            // input
            Mouse.Left.Click += Click;
            Mouse.Left.DragStart += DragStart;
            _cube = new DebugCube(game);
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

            //GraphicsDevice.SetBlendState(GraphicsDevice.BlendStates.AlphaBlend);
            //var viewProj = Camera.ViewMatrix * Camera.ProjectionMatrix;
            //_effect.Main = _gridTexture;

            //// TODO: _effect.MainShift
            //// up
            //_effect.TransformMatrix = GeometricPrimitives.UpMatrix * 
            //                          Matrix.Scaling(_selection.XLength, _selection.YLength, _selection.ZLength) * 
            //                          Matrix.Translation(_selection.Left, _selection.Top, _selection.Backward) * 
            //                          viewProj;
            //_effect.MainShift = new Vector2(-_selection.Left.Mod(16), -_selection.Backward.Mod(16));
            //_effect.MainScale = new Vector2(((float)_selection.Right - _selection.Left + 1), ((float)_selection.Forward - _selection.Backward + 1));
            //_quadMesh.Draw(GraphicsDevice);

            //// down
            //_effect.TransformMatrix = GeometricPrimitives.DownMatrix * 
            //                          Matrix.Scaling(_selection.XLength, 0, _selection.ZLength) * 
            //                          Matrix.Translation(_selection.Left, _selection.Bottom, _selection.Backward) * 
            //                          viewProj;
            //_quadMesh.Draw(GraphicsDevice);

            //// right
            //_effect.TransformMatrix = GeometricPrimitives.RightMatrix*
            //                          Matrix.Scaling(_selection.XLength, _selection.YLength, _selection.ZLength)*
            //                          Matrix.Translation(_selection.Right, _selection.Bottom, _selection.Backward)*
            //                          viewProj;
            ////_effect.MainShift=new Vector2(_selection.Forward,);
            //_effect.MainScale = new Vector2(_selection.Top - _selection.Bottom + 1, _selection.Forward - _selection.Backward + 1);
            //_quadMesh.Draw(GraphicsDevice);
            
            //// left
            //_effect.TransformMatrix = GeometricPrimitives.LeftMatrix *
            //                          Matrix.Scaling(_selection.XLength, _selection.YLength, _selection.ZLength) *
            //                          Matrix.Translation(_selection.Left, _selection.Bottom, _selection.Backward) *
            //                          viewProj;
            //_quadMesh.Draw(GraphicsDevice);
        }

        public override void Dispose()
        {
            _quadMesh.Dispose();
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
