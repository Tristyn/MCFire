using System;
using JetBrains.Annotations;
using MCFire.Modules.Infrastructure.Models;
using MCFire.Modules.Meshalyzer.Models;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// Used by the EditorGame, contains all information required to render and interact with chunks.
    /// Does not contain block data or anything like that.
    /// NOTE: Effects will not be disposed when disposing.
    /// </summary>
    public class VisualChunk : IChunkMesh
    {
        [CanBeNull]
        Mesh<VertexPositionColor> _mesh;
        [CanBeNull]
        VertexLitEffect _vertexLitEffect;
        bool _disposed;

        public VisualChunk(ChunkPosition position, [NotNull] VertexLitEffect vertexLitEffect, [CanBeNull] Buffer<VertexPositionColor> mainBuffer = null)
        {
            if (vertexLitEffect == null) throw new ArgumentNullException("vertexLitEffect");

            _vertexLitEffect = vertexLitEffect;
            Position = position;
            if (mainBuffer != null)
                _mesh = new Mesh<VertexPositionColor>(mainBuffer, _vertexLitEffect.Effect);
        }

        public void Draw(EditorGame game)
        {
            if (_disposed)
                throw new ObjectDisposedException("VisualChunk");

            if (_vertexLitEffect == null)
                return;

            _vertexLitEffect.TransformMatrix = Matrix.Translation(Position.ChunkX * 16, 0, Position.ChunkZ * 16) * game.Camera.ViewMatrix * game.Camera.ProjectionMatrix;
            if (_mesh != null)
                _mesh.Draw(game.GraphicsDevice);
        }

        public void Dispose()
        {
            _vertexLitEffect = null;
            if (_mesh != null) _mesh.Dispose();
            _disposed = true;
        }

        public ChunkPosition Position { get; private set; }
    }
}
