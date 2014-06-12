using System;
using System.Diagnostics;
using JetBrains.Annotations;
using MCFire.Graphics.Primitives;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;

namespace MCFire.Graphics.Editor.Tools.BoxSelector
{
    public class TranslationGizmo : IDisposable
    {
        [CanBeNull]
        Mesh<VertexPosition> _arrowMesh;
        FullColorEffect _effect;
        readonly IEditorGame _game;

        public TranslationGizmo([NotNull] IEditorGame game)
        {
            if (game == null) throw new ArgumentNullException("game");
            _game = game;
            Scale = 1;
            LoadContent(game);
        }

        void LoadContent(IEditorGame game)
        {
            var arrowDesc = new TruncatedConeDescription(
                new ConeSection(0, 0),
                new ConeSection(.03f, 0),
                new ConeSection(.03f, 2f / 3),
                new ConeSection(.10f, 2f / 3),
                new ConeSection(0, 1f));

            var arrowMesh = new TruncatedConeBuilder().Build(arrowDesc);
            var arrowBuffer = Buffer.Vertex.New(game.GraphicsDevice,arrowMesh);
            var effect = new FullColorEffect(game.LoadContent<Effect>("FullColor"));
            _effect = effect;
            _arrowMesh = new Mesh<VertexPosition>(arrowBuffer, effect.Effect);
        }

        public void Draw(IEditorGame game)
        {
            var arrowMesh = _arrowMesh;
            if (arrowMesh == null) Debug.Fail("content has't been loaded, yet draw was called.");

            // ReSharper disable once ImpureMethodCallOnReadonlyValueField - It is a pure call
            var matrix = Matrix.Scaling(Scale) * Matrix.Translation(Position)
                * game.Camera.ViewMatrix
                * game.Camera.ProjectionMatrix;

            // x
            _effect.Transform = matrix;
            _effect.Color = new Color4(1, 0, 0, 1);
            arrowMesh.Draw(game.GraphicsDevice);

            // y
            matrix = Matrix.RotationZ(MathUtil.PiOverTwo) * matrix;
            _effect.Transform = matrix;
            _effect.Color = new Color4(0, 1, 0, 1);
            arrowMesh.Draw(game.GraphicsDevice);

            // z
            matrix = Matrix.RotationY(MathUtil.PiOverTwo + MathUtil.Pi) * matrix;
            _effect.Transform = matrix;
            _effect.Color = new Color4(0, 0, 1, 1);
            arrowMesh.Draw(game.GraphicsDevice);
        }

        public void UnloadContent(IEditorGame game)
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_arrowMesh != null)
                _arrowMesh.Dispose();
        }

        public Vector3 Position { get; set; }
        public float Scale { get; set; }
    }

    // TODO: Entity system. More primitive than Unity3D entities. 
    // entities are managed by GameComponents and have extremely little contact with the rest of the game
    // TODO: EntityCluser that has children
    //public class Entity : ILoadContent
    //{
    //    protected Transform Transform = new Transform { Rotation = Quaternion.Identity };

    //    public void LoadContent(EditorGame game)
    //    {

    //    }

    //    public void Update()
    //    {
    //    }

    //    internal virtual void DrawEntity(EditorGame game, Matrix parentMatrix)
    //    {
    //        var wvp = Transform.GetMatrix()*parentMatrix;
    //        Draw(wvp);
    //    }

    //    protected virtual void Draw(Matrix worldViewProjection)
    //    {

    //    }

    //    internal void UnloadContentInternal()
    //    {
    //        //UnloadContent();
    //    }
    //    public virtual void UnloadContent(EditorGame game)
    //    {

    //    }

    //    public virtual void Dispose() { }
    //}

    //public struct Transform
    //{
    //    public Vector3 Position;
    //    public Quaternion Rotation;
    //    public float Scale;

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public Matrix GetMatrix()
    //    {
    //        return Matrix.AffineTransformation(Scale, Rotation, Position);
    //    }
    //}
}
