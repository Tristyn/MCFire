using System.Collections.Generic;
using System.Threading;
using MCFire.Modules.Explorer.Models;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// Provides the connection between an editor and MC Fire's data and services.
    /// </summary>
    public class EditorBridge
    {
        ChunkCreationPolicy _policy;
        readonly Meshalyzer _meshalyzer;
        Thread _meshingThread;

        public EditorBridge(MCFireWorld world, int dimension, EditorGame game)
        {
            World = world;
            Chunks = new List<MCFireChunk>();
            _meshalyzer = new Meshalyzer(game, world, dimension);
            game.Disposing += (s, e) => { if (_meshingThread != null)_meshingThread.Abort(); };
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
            if (_meshingThread != null && _meshingThread.IsAlive)
                return;

            _meshingThread = new Thread(() =>
            {
                while (_policy == ChunkCreationPolicy.Run)
                {
                    if (!_meshalyzer.MeshalyzeNext())
                        Thread.Sleep(5000);
                }
            });
            Thread.CurrentThread.IsBackground = true;
            _meshingThread.Start();
        }

        public MCFireWorld World { get; private set; }
        public List<MCFireChunk> Chunks { get; private set; }
    }
}
