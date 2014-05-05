using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Explorer.Messages;
using MCFire.Modules.Infrastructure.Models;
using SharpDX.Toolkit;

namespace MCFire.Modules.Meshalyzer.Models
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IGameComponent))]
    public class MeshalyzerComponent : GameComponentBase, IHandle<ChunkModifiedMessage>
    {
        Thread _meshingThread;
        ChunkCreationPolicy _policy;
        bool _disposed;

        readonly ChunkPrioritizer _prioritizer = new ChunkPrioritizer();
        readonly ConcurrentQueue<IChunkMesh> _chunksToIntegrate = new ConcurrentQueue<IChunkMesh>();
        readonly Dictionary<ChunkPosition, IChunkMesh> _chunks = new Dictionary<ChunkPosition, IChunkMesh>();

        [Import]
        IEventAggregator Aggregator { set { value.Subscribe(this); } }
        [ImportMany, UsedImplicitly]
        IEnumerable<IMeshalyzer> _meshalyzers;
        IMeshalyzer _currentMeshalyzer;

        // TODO: use new meshalying system (BlockMeshalyzer)
        // this comment is so old that it should be in IMeshalyzer

        public override void LoadContent(EditorGame game)
        {
            base.LoadContent(game);
            // TODO: settings
            var mesher = _meshalyzers.LastOrDefault();
            if (mesher == null) return;
            SwitchMeshalyzer(mesher);
        }

        public override void UnloadContent(EditorGame game)
        {
            base.UnloadContent(game);
            if (_currentMeshalyzer != null)
                _currentMeshalyzer.UnloadContent(game);
        }

        void StopMeshalyzer()
        {
            _policy = ChunkCreationPolicy.Idle;
            if (_meshingThread != null)
                _meshingThread.Join();
            if (_currentMeshalyzer != null)
                _currentMeshalyzer.UnloadContent(Game);
        }

        void SwitchMeshalyzer(IMeshalyzer newMeshalyzer)
        {
            // load content
            StopMeshalyzer();
            newMeshalyzer.LoadContent(Game);
            _currentMeshalyzer = newMeshalyzer;

            // start meshing
            if (_meshingThread != null && _meshingThread.IsAlive)
                return;
            _policy = ChunkCreationPolicy.Run;
            /* TODO: leverage multiple cores. some considerations: 
             * a chunk will be generated multiple times unless a thread 
             * can claim that point and it wont be picked again. 
             * mabey have a more sophisticated chunk selection system.
             */
            _meshingThread = new Thread(MeshLoop);
            _meshingThread.Start();
        }

        public override void Draw(GameTime time)
        {
            if (_disposed)
                throw new ObjectDisposedException("MeshalyzerComponent");

            IntegrateNewChunks();

            GraphicsDevice.SetBlendState(GraphicsDevice.BlendStates.Opaque);

            foreach (var chunk in _chunks.Values)
            {
                chunk.Draw(Game);
            }
        }

        public void Handle(ChunkModifiedMessage message)
        {
            var pos = message.Position;
            if (pos.World != World ||
                pos.Dimension != Dimension) return;
            _prioritizer.ChunkNeedsRegen(message.Position);
        }

        void IntegrateNewChunks()
        {
            while (true)
            {
                IChunkMesh chunk;
                if (!_chunksToIntegrate.TryDequeue(out chunk)) break;

                // Add it to the dictionary using its position as a key.
                _chunks[chunk.Position] = chunk;
            }
        }

        /// <summary>
        /// Creates chunks on a background thread until policy changes.
        /// </summary>
        void MeshLoop()
        {
            Thread.CurrentThread.IsBackground = true;
            while (_policy == ChunkCreationPolicy.Run)
            {
                var drawable = _currentMeshalyzer.MeshalyzeNext(Game);
                if (drawable == null)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                _chunksToIntegrate.Enqueue(drawable);
            }
        }

        public override void Dispose()
        {
            if (_disposed)
                return;

            StopMeshalyzer();
            foreach (var meshalyzer in _meshalyzers)
            {
                meshalyzer.Dispose();
            }

            IChunkMesh chunk;
            while (_chunksToIntegrate.TryDequeue(out chunk))
            {
                chunk.Dispose();
            }

            foreach (var keyValue in _chunks)
            {
                keyValue.Value.Dispose();
            }

            _disposed = true;
        }

        public override int DrawPriority { get { return 20; } }
    }

    public interface IChunkMesh : IDrawable
    {
        ChunkPosition Position { get; }
    }
}
