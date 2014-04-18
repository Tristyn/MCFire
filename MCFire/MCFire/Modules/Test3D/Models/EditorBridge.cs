using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDX;
using Substrate;
using Substrate.Core;

namespace MCFire.Modules.Test3D.Models
{
    /// <summary>
    /// Provides the connection between an editor and MC Fire's data and services.
    /// </summary>
    public class EditorBridge
    {
        readonly D3DTestGame _game;
        readonly Dictionary<Point, EditorChunk> _chunkDict;
        ChunkCreationPolicy _policy;
        IChunkManager _manager;

        public EditorBridge(NbtWorld world, int dimension, D3DTestGame game)
        {
            _game = game;
            World = world;
            _manager = world.GetChunkManager(dimension);
            _chunkDict = new Dictionary<Point, EditorChunk>();
            Chunks = new List<EditorChunk>();
            _game.ChunkSource = _chunkDict;
        }

        public async void SetChunkCreationPolicy(ChunkCreationPolicy policy)
        {
            if (policy == _policy)
                return;

            _policy = policy;

            if (policy == ChunkCreationPolicy.Run)
                await BuildChunks();
        }

        private async Task BuildChunks()
        {
            if (_policy != ChunkCreationPolicy.Run)
                return;

            var task = Task.Factory.StartNew(() =>
            {
                
            });
        }

        public NbtWorld World { get; private set; }
        public List<EditorChunk> Chunks { get; private set; }
    }
}
