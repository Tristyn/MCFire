using System;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// Used by the EditorGame, contains all information required to render and interact with chunks.
    /// Does not contain block data or anything like that.
    /// NOTE: Effects will not be disposed when disposing.
    /// </summary>
    public class VisualChunk : IDisposable
    {
        VertexLitEffect _vertexLitEffect;
        public readonly Point ChunkPosition;
        Mesh _mesh;
        bool _disposed;

        public VisualChunk(Buffer<VertexPositionColor> vertexBuffer, VertexLitEffect vertexLitEffect, Point position)
        {
            _vertexLitEffect = vertexLitEffect;
            ChunkPosition = position;
            _mesh = new Mesh
            {
                VertexBuffer = vertexBuffer,
                VertexInputLayout = VertexInputLayout.FromBuffer(0, vertexBuffer),
                Effect = _vertexLitEffect.Effect
            };
        }

        public void Draw(EditorGame game)
        {
            if (_disposed)
                throw new ObjectDisposedException("VisualChunk");

            _vertexLitEffect.TransformMatrix = Matrix.Translation(ChunkPosition.X * 16, 0, ChunkPosition.Y * 16) * game.Camera.ViewMatrix * game.Camera.ProjectionMatrix;
            _mesh.Draw(game.GraphicsDevice);
        }

        public void Dispose()
        {
            _vertexLitEffect = null;
            _mesh.Dispose();
            _mesh = null;
            _disposed = true;
        }
    }
}
