using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Input;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.BoxSelector.Messages;
using MCFire.Modules.Clipboard.Services;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.Infrastructure.Models;
using SharpDX;
using SharpDX.Toolkit;
using Key = System.Windows.Input.Key;
using KeyEventArgs = MCFire.Modules.Editor.Models.KeyEventArgs;
using Texture2D = SharpDX.Toolkit.Graphics.Texture2D;

namespace MCFire.Modules.BoxSelector.Models
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IGameComponent))]
    public class BoxSelectorComponent : GameComponentBase
    {
        BoxVisual _box;
        BoxSelection _selection;
        [NotNull]
        Texture2D _gridTexture;

        SelectionState _selectionState;
        Ray _faceDragRay;


        [Import]
        IEventAggregator _aggregator;

        public override void LoadContent(EditorGame game)
        {
            base.LoadContent(game);
            var gridTexture = game.LoadContent<Texture2D>("Grid");
            if (gridTexture == null)
                throw new SharpDXException();
            _gridTexture = gridTexture;
            _box = new BoxVisual(gridTexture);
            _box.LoadContent(game);

            // TODO: proper state management (Enabled)
            Enabled = true;

            // state
            _selectionState = SelectionState.NotSet;

            // input
            Mouse.Left.ClickEnd += ClickEnd;
            Mouse.Left.DragStart += DragStart;
            Mouse.Left.DragMove += DragMove;
            Mouse.Left.DragEnd += DragEnd;
        }

        void ClickEnd(object sender, KeyEventArgs e)
        {
            if (!Enabled) return;

            BlockPosition pos;
            if (!Tasks.TryGetBlockAtScreenCoord(e.Position, out pos))
                return;

            switch (_selectionState)
            {
                case SelectionState.NotSet:
                    NewSelection(pos, pos, SelectionState.HalfSet);
                    break;
                case SelectionState.HalfSet:
                    NewSelection(_selection.CornerOne, pos, SelectionState.Set);
                    Console.WriteLine(_selection.GetCuboid());
                    break;
                case SelectionState.Set:
                    // TODO: dragging the selection faces along their normal axes
                    // if the click missed the BoxSelection, create a new selection.
                    NewSelection(pos, pos, SelectionState.HalfSet);
                    break;
                default:
                    Debug.Fail("SelectionState out of range");
                    break;
            }
        }

        // TODO: Highlight face that mouse is over when _selectionState == SelectionState.Set
        void NewSelection(BlockPosition pos1, BlockPosition pos2, SelectionState newState)
        {
            var selection = new BoxSelection(pos1, pos2);
            _selection = selection;
            _selectionState = newState;

            // notify only if selection is set.
            if (newState != SelectionState.Set) return;

            // use the selection (on the stack) to avoid modified closure.
            _aggregator.Publish(new BoxSelectionUpdatedMessage(selection, box => new BlockSelection(selection, Dimension, World)));
        }


        void DragStart(object sender, KeyEventArgs e)
        {
            // Drag the face of the selection along its axis
            if (_selectionState != SelectionState.Set) return;
            var selection = _selection;
            var lesser = selection.Lesser;
            var greater = selection.Greater;
            var pos = Camera.Position;

            // Calculate if the mouse is over the selection
            var selectionBox = new BoundingBox(lesser, greater);
            var ray = Camera.ScreenPointToRay(e.Position);
            Vector3 hit;
            if (!selectionBox.Intersects(ref ray, out hit)) return;
            var roundedHit = hit.Round(); // BoundingBox.Intersects can return a value + epsilon, so round it

            // enumerate faces
            // left
            Console.WriteLine("Hit at {0}", roundedHit);
            if (pos.X < roundedHit.X && new Plane(lesser, Vector3.Left).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.Left);
            // right
            else if (pos.X > roundedHit.X && new Plane(greater, Vector3.Right).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.Right);
            // down
            else if (pos.Y < roundedHit.Y && new Plane(lesser, Vector3.Down).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.Down);
            // up
            else if (pos.Y > roundedHit.Y && new Plane(greater, Vector3.Up).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.Up);
            // forward
            else if (pos.Z < roundedHit.Z && new Plane(lesser, Vector3.ForwardRH).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.ForwardRH);
            // back
            else if (pos.Z > roundedHit.Z && new Plane(greater, Vector3.BackwardRH).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.BackwardRH);
        }

        void DragMove(object sender, KeyEventArgs e)
        {
            if (!Enabled) return;

            var faceDragRay = _faceDragRay;
            var pos = Camera.Position;
            // update face drag
            if (faceDragRay == default(Ray))
                return;

            // dragPlane is used to hit-test the unpropjected mouse position
            // we need to calculate the dragPlane every DragMove, as Camera.Position can change
            var dragPlane = faceDragRay.ToPlane(pos);
            var mouseRay = Camera.ScreenPointToRay(e.Position);

            // find where the mouse intersects the drag plane
            Vector3 mouseHit;
            if (!dragPlane.Intersects(ref mouseRay, out mouseHit))
                return;

            // use pythagorean (C^2-B^2=A^2) to calculate distance from faceDragRay
            var diff = mouseHit - faceDragRay.Position;
            var cSquared = diff.LengthSquared();
            var b = faceDragRay.Distance(mouseHit);
            var a = (float)Math.Sqrt(cSquared - b * b);

            var dragPosWorldSpace = faceDragRay.Position + faceDragRay.Direction * a;
            var alignedDragDir = faceDragRay.Direction.AlignToClosestAxis();
            var selection = _selection;
            var lesser = selection.Lesser;
            var greater = selection.Greater;

            // ReSharper disable CompareOfFloatsByEqualityOperator - AlignToClosestAxis() sets a component to +-one
            if (alignedDragDir.X == 1)
                _selection = new BoxSelection(lesser, new BlockPosition((int)dragPosWorldSpace.X, greater.Y, greater.Z));
            else if (alignedDragDir.X == -1)
                _selection = new BoxSelection(new BlockPosition((int)dragPosWorldSpace.X, lesser.Y, lesser.Z), greater);
            else if (alignedDragDir.Y == 1)
                _selection = new BoxSelection(lesser, new BlockPosition(greater.X, (int)dragPosWorldSpace.Y, greater.Z));
            else if (alignedDragDir.Y == -1)
                _selection = new BoxSelection(new BlockPosition(lesser.X, (int)dragPosWorldSpace.Y, lesser.Z), greater);
            else if (alignedDragDir.Z == 1)
                _selection = new BoxSelection(lesser, new BlockPosition(greater.X, greater.Y, (int)dragPosWorldSpace.Z));
            else if (alignedDragDir.Z == -1)
                _selection = new BoxSelection(new BlockPosition(lesser.X, lesser.Y, (int)dragPosWorldSpace.Z), greater);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        private void DragEnd(object sender, KeyEventArgs e)
        {
            _faceDragRay = default(Ray);
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
            _box.Cuboid = _selection.GetCuboid();
            _box.Draw(Game);
        }

        public override void Dispose()
        {
            _box.Dispose();
            _gridTexture.Dispose();
        }

        public override void WpfKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.C || System.Windows.Input.Keyboard.Modifiers != ModifierKeys.Control) return;

            // Ctrl+C
            if (!Enabled) return;
            _aggregator.Publish(new ClipboardCopyEvent(new BlockSelection(_selection, Dimension, World)));
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
