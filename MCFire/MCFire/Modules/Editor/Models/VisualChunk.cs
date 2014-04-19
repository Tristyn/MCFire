﻿using System;
using JetBrains.Annotations;
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
        PopulationState _state;
        [CanBeNull]
        Mesh _mesh;
        [CanBeNull]
        VertexLitEffect _vertexLitEffect;
        bool _disposed;
        public readonly Point ChunkPosition;

        public VisualChunk(PopulationState state, [CanBeNull] Buffer<VertexPositionColor> mainBuffer,
            [NotNull] VertexLitEffect vertexLitEffect, Point position)
        {
            if (vertexLitEffect == null) throw new ArgumentNullException("vertexLitEffect");

            _state = state;
            _vertexLitEffect = vertexLitEffect;
            ChunkPosition = position;
            if (mainBuffer != null)
                _mesh = new Mesh
                {
                    VertexBuffer = mainBuffer,
                    VertexInputLayout = VertexInputLayout.FromBuffer(0, mainBuffer),
                    Effect = _vertexLitEffect.Effect
                };
        }

        public void Draw(EditorGame game)
        {
            if (_disposed)
                throw new ObjectDisposedException("VisualChunk");

            if (_vertexLitEffect == null)
                return;

            _vertexLitEffect.TransformMatrix = Matrix.Translation(ChunkPosition.X * 16, 0, ChunkPosition.Y * 16) * game.Camera.ViewMatrix * game.Camera.ProjectionMatrix;
            if (_mesh != null)
                _mesh.Draw(game.GraphicsDevice);
        }

        public void Dispose()
        {
            _vertexLitEffect = null;
            if (_mesh != null) _mesh.Dispose();
            _mesh = null;
            _disposed = true;
        }
    }

    public enum PopulationState
    {
        Populated,
        NotPopulated
    }
}
