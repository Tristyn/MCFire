using System;
using JetBrains.Annotations;
using MCFire.Graphics.Editor.Modules.Meshalyzer;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Graphics.Editor
{
    /// <summary>
    /// Used by the EditorGame, contains all information required to render and interact with chunks.
    /// Does not contain block data or anything like that.
    /// NOTE: Effects will not be disposed when disposing.
    /// </summary>
    public class ChunkMesh : IChunkMesh
    {
        // TODO: this class has changed from a renderable wrapper of a minecraft chunk, to a renderable wrapper of an arbitrary block volume.
        [CanBeNull]
        Mesh<VertexPositionColorTexture> _mesh;
        [CanBeNull]
        VertexLitEffect _vertexLitEffect;
        bool _disposed;
        private Matrix _world;

        public ChunkMesh(Vector3 modelOrigin, [NotNull] VertexLitEffect vertexLitEffect, [CanBeNull] Buffer<VertexPositionColorTexture> mainBuffer = null)
        {
            if (vertexLitEffect == null) throw new ArgumentNullException("vertexLitEffect");

            _vertexLitEffect = vertexLitEffect;
            ModelOrigin = modelOrigin;
            _world = Matrix.Identity;
            if (mainBuffer != null)
                _mesh = new Mesh<VertexPositionColorTexture>(mainBuffer, _vertexLitEffect.Effect);
        }

        public void Draw(IEditorGame game)
        {
            if (_disposed)
                throw new ObjectDisposedException("VisualChunk");

            if (_vertexLitEffect == null)
                return;
            _vertexLitEffect.TransformMatrix = Matrix.Translation(ModelOrigin) * _world * game.Camera.ViewMatrix * game.Camera.ProjectionMatrix;
            if (_mesh != null)
                _mesh.Draw(game.GraphicsDevice);
        }

        public void Dispose()
        {
            _vertexLitEffect = null;
            if (_mesh != null) _mesh.Dispose();
            _disposed = true;
        }

        public Vector3 ModelOrigin { get; private set; }

        public Matrix World
        {
            private get { return _world; }
            set { _world = value; }
        }
    }
}
