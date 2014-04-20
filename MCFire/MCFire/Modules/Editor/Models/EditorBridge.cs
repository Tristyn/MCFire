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
        readonly Meshalyzer.Meshalyzer _meshalyzer;
        Thread _meshingThread;

        public EditorBridge(MCFireWorld world, int dimension, EditorGame game)
        {
            // TODO: move meshalyzer to inside EditorGame because it isnt needed by editor bridge
            // TODO: perhaps let the bridge create the game, EditorViewModel doesn't need the game.
            World = world;
            _meshalyzer = new Meshalyzer.Meshalyzer(game, world, dimension);
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
            /* TODO: leverage multiple cores. some considerations: 
             * a chunk will be generated multiple times unless a thread 
             * can claim that point and it wont be picked again. 
             * mabey have a more sophisticated chunk selection system.
             */
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
    }
}
