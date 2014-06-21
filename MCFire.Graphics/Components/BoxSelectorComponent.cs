using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows.Input;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Common;
using MCFire.Common.Coordinates;
using MCFire.Graphics.Editor.Tools.BoxSelector;
using MCFire.Graphics.Infrastructure.Extensions;
using MCFire.Graphics.Messages;
using MCFire.Graphics.Primitives;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Key = System.Windows.Input.Key;
using KeyEventArgs = MCFire.Graphics.Editor.KeyEventArgs;

namespace MCFire.Graphics.Components
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IGameComponent))]
    class BoxSelectorComponent : ToolComponentBase
    {
        TiledBoxVisual _box;
        BlockPosition _cornerOne;
        BlockPosition _cornerTwo;
        [NotNull]
        Texture2D _gridTexture;
        TranslationGizmo _translationGizmo;

        private SelectionState _selectionState;
        Ray _faceDragRay;

        [Import]
        IEventAggregator _aggregator;

        protected override void LoadContent()
        {
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

        void ClickEnd([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            if (!Selected)
                return;

            BlockPosition pos;
            if (!Camera.TryGetBlockAtMousePosition(out pos))
                return;

            switch (SelectionState)
            {
                case SelectionState.NotSet:
                    NewSelection(pos, pos, SelectionState.HalfSet);
                    break;
                case SelectionState.HalfSet:
                case SelectionState.DragMove:
                    NewSelection(_cornerOne, pos, SelectionState.Set);
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
            _cornerOne = pos1;
            _cornerTwo = pos2;
            var selection = new BoxSelection(pos1,pos2);
            _selectionState = newState;

            // notify only if selection is set.
            if (newState != SelectionState.Set)
                return;

            // use variables on the stack to avoid modified closure.
            var dim = Dimension;
            var world = World;
            _aggregator.Publish(new BoxSelectionUpdatedMessage(selection, box => new BlockSelection(selection, dim, world)));
        }

        void DragStart([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            // Drag the face of the selection along its axis
            if (SelectionState != SelectionState.Set) return;
            var selection = Selection;

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
            if (pos.X < roundedHit.X && Math.Abs(hit.X - lesser.X) < .01 &&
                new Plane(lesser, Vector3.Left).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.Left);
            // right
            else if (pos.X > roundedHit.X && Math.Abs(hit.X - greater.X) < .01 &&
                     new Plane(greater, Vector3.Right).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.Right);
            // down
            else if (pos.Y < roundedHit.Y && Math.Abs(hit.Y - lesser.Y) < .01 &&
                     new Plane(lesser, Vector3.Down).Intersects(ref roundedHit) == PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.Down);
            // up
            else if (pos.Y > roundedHit.Y && Math.Abs(hit.Y - greater.Y) < .01 &&
                     new Plane(greater, Vector3.Up).Intersects(ref roundedHit) ==
                     PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.Up);
            // forward
            else if (pos.Z < roundedHit.Z && Math.Abs(hit.Z - lesser.Z) < .01 &&
                     new Plane(lesser, Vector3.ForwardRH).Intersects(ref roundedHit) ==
                     PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.ForwardRH);
            // back
            else if (pos.Z > roundedHit.Z && Math.Abs(hit.Z - greater.Z) < .01 &&
                     new Plane(greater, Vector3.BackwardRH).Intersects(ref roundedHit) ==
                     PlaneIntersectionType.Intersecting)
                _faceDragRay = new Ray(roundedHit, Vector3.BackwardRH);
        }

        void DragMove([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            // update face drag
            if (!Selected) return;

            var selection = Selection;

            var faceDragRay = _faceDragRay;
            if (faceDragRay == default(Ray))
                return;

            var pos = Camera.Position;

            // dragPlane is used to hit-test the unprojected mouse position
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
                    NewSelection(new BlockPosition((int)dragPosWorldSpace.X, lesser.Y, lesser.Z), greater,
                        SelectionState.DragMove);
                    break;
                case Faces.Right:
                    NewSelection(lesser, new BlockPosition((int)dragPosWorldSpace.X, greater.Y, greater.Z),
                        SelectionState.DragMove);
                    break;
                case Faces.Bottom:
                    NewSelection(new BlockPosition(lesser.X, (int)dragPosWorldSpace.Y, lesser.Z), greater,
                        SelectionState.DragMove);
                    break;
                case Faces.Top:
                    NewSelection(lesser, new BlockPosition(greater.X, (int)dragPosWorldSpace.Y, greater.Z),
                        SelectionState.DragMove);
                    break;
                case Faces.Forward:
                    NewSelection(new BlockPosition(lesser.X, lesser.Y, (int)dragPosWorldSpace.Z), greater,
                        SelectionState.DragMove);
                    break;
                case Faces.Backward:
                    NewSelection(lesser, new BlockPosition(greater.X, greater.Y, (int)dragPosWorldSpace.Z),
                        SelectionState.DragMove);
                    break;
                default:
                    Debug.Fail("Vector3Extensions.GetFace didn't return a valid face");
                    break;
            }
        }

        void DragEnd([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            _faceDragRay = default(Ray);
            NewSelection(_cornerOne, _cornerTwo, SelectionState.Set);
        }

        bool TryGetGizmoPosition(out Vector3 position)
        {
            var selection = Selection;

            var cuboid = selection.GetCuboid();
            position = new Vector3(
                cuboid.Left + (cuboid.Length / 2) + .5f,
                cuboid.Bottom + (cuboid.Height / 2) + .5f,
                cuboid.Forward + (cuboid.Width / 2) + .5f);
            return true;
        }

        public override void Update( GameTime time)
        {
            if (!Selected) return;

            BlockPosition pos;
            if (!Camera.TryGetBlockAtMousePosition(out pos))
                return;

            switch (SelectionState)
            {
                case SelectionState.NotSet:
                    NewSelection(pos,pos, SelectionState.NotSet);
                    break;
                case SelectionState.HalfSet:
                    NewSelection(_cornerOne, pos, SelectionState.HalfSet);
                    break;
                case SelectionState.DragMove:
                case SelectionState.Set:
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        public override void Draw( GameTime time)
        {
            var cuboid = Selection.GetCuboid();

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

        public override void WpfKeyDown( System.Windows.Input.KeyEventArgs e)
        {
            // Ctrl+C
            if (e.Key != Key.C || System.Windows.Input.Keyboard.Modifiers != ModifierKeys.Control) return;
            if (!Selected) return;

            _aggregator.Publish(new BoxSelectionCopiedMessage(new BlockSelection(Selection, Dimension, World)));
        }

        public override int DrawPriority { get { return 500; } }

        [Import(typeof(BoxSelectorTool))]
        protected override IEditorTool Tool { get; set; }

        public BoxSelection Selection
        {
            get { return new BoxSelection(_cornerOne, _cornerTwo); }
        }

        public SelectionState SelectionState
        {
            get { return _selectionState; }
        }
    }

    public enum SelectionState
    {
        NotSet,
        HalfSet,
        Set,
        DragMove
    }
}
