using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Input;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Client.Modules.EditorTools.BoxSelector;
using MCFire.Client.Services.Clipboard;
using MCFire.Common.Coordinates;
using MCFire.Graphics.Editor;
using MCFire.Graphics.Editor.Tools.BoxSelector;
using MCFire.Graphics.Infrastructure.Extensions;
using MCFire.Graphics.Primitives;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using KeyEventArgs = MCFire.Graphics.Editor.KeyEventArgs;

namespace MCFire.Client.Modules.EditorModules.BoxSelector
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IGameComponent))]
    public class BoxSelectorComponent : GameComponentBase
    {
        TiledBoxVisual _box;
        [CanBeNull]
        BoxSelection _selection;
        [NotNull]
        Texture2D _gridTexture;

        SelectionState _selectionState;
        Ray _faceDragRay;

        [Import] BoxSelectorTool _tool;
        [Import]
        IEventAggregator _aggregator;
        TranslationGizmo _translationGizmo;

        protected override void LoadContent()
        {
            base.LoadContent(Game);
            var gridTexture = Game.LoadContent<Texture2D>("Grid");
            _translationGizmo = new TranslationGizmo(Game);
            if (gridTexture == null)
                throw new SharpDXException();
            _gridTexture = gridTexture;
            _box = new TiledBoxVisual(gridTexture, 1f / 16, .01f);
            _box.LoadContent(Game);

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
            if(!_tool.Selected)
                return;

            BlockPosition pos;
            if (!Tasks.TryGetBlockAtScreenCoord(e.Position, out pos))
                return;

            var selection = _selection;

            switch (_selectionState)
            {
                case SelectionState.NotSet:
                    NewSelection(pos, pos, SelectionState.HalfSet);
                    break;
                case SelectionState.HalfSet:
                case SelectionState.DragMove:
                    if (selection != null)
                        NewSelection(selection.CornerOne, pos, SelectionState.Set);
                    else NewSelection(pos, pos, SelectionState.HalfSet);
                    break;
                case SelectionState.Set:
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
            if (newState == SelectionState.Set)
                // use selection (on the stack) to avoid modified closure.
                _aggregator.Publish(new BoxSelectionUpdatedMessage(selection, box => new BlockSelection(selection, Dimension, World)));
        }

        void DragStart(object sender, KeyEventArgs e)
        {
            // Drag the face of the selection along its axis
            if (_selectionState != SelectionState.Set) return;
            var selection = _selection;
            if (selection == null) return;

            var cuboid = selection.GetCuboid();
            var lesser = new Vector3(selection.Lesser.X, selection.Lesser.Y, selection.Lesser.Z);
            var greaterTemp = cuboid.Position + cuboid.Dimensions;
            var greater = new Vector3(greaterTemp.X, greaterTemp.Y, greaterTemp.Z);
            var pos = Camera.Position;

            // Calculate if the mouse is over the selection
            var selectionBox = new BoundingBox(lesser, greater);
            var ray = Camera.ScreenPointToRay(e.Position);
            Vector3 hit;
            if (!selectionBox.Intersects(ref ray, out hit)) return;
            var roundedHit = hit.Round(); // BoundingBox.Intersects can return a value + epsilon, so round it

            // enumerate faces
            // left
            if (pos.X < roundedHit.X && Math.Abs(hit.X - lesser.X) < .01 && new Plane(lesser, Vector3.Left).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.Left);
            // right
            else if (pos.X > roundedHit.X && Math.Abs(hit.X - greater.X) < .01 && new Plane(greater, Vector3.Right).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.Right);
            // down
            else if (pos.Y < roundedHit.Y && Math.Abs(hit.Y - lesser.Y) < .01 && new Plane(lesser, Vector3.Down).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.Down);
            // up
            else if (pos.Y > roundedHit.Y && Math.Abs(hit.Y - greater.Y) < .01 && new Plane(greater, Vector3.Up).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.Up);
            // forward
            else if (pos.Z < roundedHit.Z && Math.Abs(hit.Z - lesser.Z) < .01 && new Plane(lesser, Vector3.ForwardRH).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.ForwardRH);
            // back
            else if (pos.Z > roundedHit.Z && Math.Abs(hit.Z - greater.Z) < .01 && new Plane(greater, Vector3.BackwardRH).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.BackwardRH);
        }

        void DragMove(object sender, KeyEventArgs e)
        {
            // update face drag
            if (!_tool.Selected) return;

            var selection = _selection;
            if (selection == null) return;

            var faceDragRay = _faceDragRay;
            if (faceDragRay == default(Ray))
                return;

            var pos = Camera.Position;

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

            // Determine if the drag is moving the face away or towards its center
            // We need this because direction information is always positive
            var draggingFromCenter = Vector3.Dot(faceDragRay.Direction, diff) < 0;
            if (draggingFromCenter)
                a = -a;

            var dragPosWorldSpace = faceDragRay.Position + faceDragRay.Direction * a;
            var alignedDragDir = faceDragRay.Direction.GetFace();
            var lesser = selection.Lesser;
            var greater = selection.Greater;

            switch (alignedDragDir)
            {
                case Faces.Left:
                    NewSelection(new BlockPosition((int)dragPosWorldSpace.X, lesser.Y, lesser.Z), greater, SelectionState.DragMove);
                    break;
                case Faces.Right:
                    NewSelection(lesser, new BlockPosition((int)dragPosWorldSpace.X, greater.Y, greater.Z), SelectionState.DragMove);
                    break;
                case Faces.Bottom:
                    NewSelection(new BlockPosition(lesser.X, (int)dragPosWorldSpace.Y, lesser.Z), greater, SelectionState.DragMove);
                    break;
                case Faces.Top:
                    NewSelection(lesser, new BlockPosition(greater.X, (int)dragPosWorldSpace.Y, greater.Z), SelectionState.DragMove);
                    break;
                case Faces.Forward:
                    NewSelection(new BlockPosition(lesser.X, lesser.Y, (int)dragPosWorldSpace.Z), greater, SelectionState.DragMove);
                    break;
                case Faces.Backward:
                    NewSelection(lesser, new BlockPosition(greater.X, greater.Y, (int)dragPosWorldSpace.Z), SelectionState.DragMove);
                    break;
                default:
                    Debug.Fail("Vector3Extensions.GetFace didn't return a valid face");
                    break;
            }
        }

        private void DragEnd(object sender, KeyEventArgs e)
        {
            _faceDragRay = default(Ray);
            var selection = _selection;
            if (selection != null)
                NewSelection(selection.CornerOne, selection.CornerTwo, SelectionState.Set);
        }

        bool TryGetGizmoPosition(out Vector3 position)
        {
            var selection = _selection;
            if (selection == null)
            {
                position = new Vector3();
                return false;
            }

            var cuboid = selection.GetCuboid();
            position = new Vector3(
                cuboid.Left + (cuboid.Length / 2) + .5f,
                cuboid.Bottom + (cuboid.Height / 2) + .5f,
                cuboid.Forward + (cuboid.Width / 2) + .5f);
            return true;
        }

        public override void Update(GameTime time)
        {
            if (!_tool.Selected) return;

            BlockPosition pos;
            if (!Tasks.TryGetBlockAtScreenCoord(Mouse.Position, out pos))
                return;

            // When the drag is half set, 
            switch (_selectionState)
            {
                case SelectionState.NotSet:
                    _selection = new BoxSelection(pos, pos);
                    break;
                case SelectionState.HalfSet:
                    var selection = _selection;
                    if (selection == null) return;

                    NewSelection(selection.CornerOne, pos, SelectionState.HalfSet);
                    break;
                case SelectionState.DragMove:
                case SelectionState.Set:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Draw(GameTime time)
        {
            var selection = _selection;
            if (selection == null) return;

            var cuboid = selection.GetCuboid();

            Vector3 position;
            if ((cuboid.Length > 1 || cuboid.Height > 1 || cuboid.Width > 1)
                && TryGetGizmoPosition(out position))
            {
                _translationGizmo.Position = position;
                var dist = Vector3.Distance(position, Camera.Position);
                _translationGizmo.Scale = Math.Max((dist) / 8, .75f);
                _translationGizmo.Draw(Game);
            }

            _box.Cuboid = cuboid;
            _box.Draw(Game);
        }

        public override void Dispose()
        {
            _box.Dispose();
            _gridTexture.Dispose();
            _translationGizmo.Dispose();
        }

        public override void WpfKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.C || System.Windows.Input.Keyboard.Modifiers != ModifierKeys.Control) return;

            // Ctrl+C
            if (!_tool.Selected) return;
            _aggregator.Publish(new ClipboardCopyMessage(new BlockSelection(_selection, Dimension, World)));
        }

        public override int DrawPriority { get { return 500; } }

        private enum SelectionState
        {
            NotSet,
            HalfSet,
            Set,
            DragMove
        }
    }
}
