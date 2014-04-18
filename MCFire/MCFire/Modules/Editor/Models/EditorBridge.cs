using System.Collections.Generic;
using System.Threading;
using MCFire.Modules.Explorer.Models;
using SharpDX;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// Provides the connection between an editor and MC Fire's data and services.
    /// </summary>
    public class EditorBridge
    {
        private readonly int _dimension;
        readonly EditorGame _game;
        readonly Dictionary<Point, MCFireChunk> _chunkDict;
        ChunkCreationPolicy _policy;
        Meshalyzer _meshalyzer;

        public EditorBridge(MCFireWorld world, int dimension, EditorGame game)
        {
            _dimension = dimension;
            _game = game;
            World = world;
            _chunkDict = new Dictionary<Point, MCFireChunk>();
            Chunks = new List<MCFireChunk>();
            _game.ChunkSource = _chunkDict;
            _meshalyzer = new Meshalyzer(game);
            SetChunkCreationPolicy(ChunkCreationPolicy.Run);
        }

        public void SetChunkCreationPolicy(ChunkCreationPolicy policy)
        {
            if (policy == _policy)
                return;

            _policy = policy;

            if (policy == ChunkCreationPolicy.Run)
                BeginBuildChunks();
        }

        /// <summary>
        /// Creates chunks on a background thread until policy changes.
        /// </summary>
        private void BeginBuildChunks()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var meshingThread = new Thread(() =>
            {
                while (_policy == ChunkCreationPolicy.Run)
                {
                    Point chunkPoint;
                    if (!_game.GetNextDesiredChunk(out chunkPoint))
                    {
                        Thread.Sleep(5000); // wait because there is no work to do
                        continue;
                    }

                    var chunk = World.GetChunk(_dimension, chunkPoint.X, chunkPoint.Y);
                    if (chunk == null) return;
                    _meshalyzer.Meshalyze(chunk);
                }
            });
            Thread.CurrentThread.IsBackground = true;
            meshingThread.Start();
        }

        public MCFireWorld World { get; private set; }
        public List<MCFireChunk> Chunks { get; private set; }
    }
}
