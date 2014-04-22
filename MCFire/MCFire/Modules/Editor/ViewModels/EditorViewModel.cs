using System;
using System.ComponentModel.Composition;
using System.Threading;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Messages;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Editor.Views;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Infrastructure.ViewModels;
using Action = System.Action;

namespace MCFire.Modules.Editor.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class EditorViewModel : SharpDxViewModelBase
    {
        [CanBeNull]
        public EditorGame Game;
        [CanBeNull]
        EditorView _view;
        [CanBeNull]
        Action _viewGained;
        [Import]
        IEventAggregator _aggregator;

        ChunkCreationPolicy _policy;
        Meshalyzer.Meshalyzer _meshalyzer;
        Thread _meshingThread;

        public EditorViewModel()
        {
            DisplayName = "Editor";
        }

        public bool TryInitializeTo([NotNull] MCFireWorld world, int dimension)
        {
            if (Game != null)
                return false;

            if (_view != null)
            {
                return TryInitializeToInternal(world, dimension);
            }

            // we dont have the view yet. when we do, this will be called
            _viewGained = () => TryInitializeToInternal(world, dimension);
            return true;
        }

        bool TryInitializeToInternal(MCFireWorld world, int dimension)
        {
            if (Game != null)
                return false;

            DisplayName = "Starting Up - Editor";

            Game = new EditorGame(_view.SharpDx);
            Game.Disposing += (s, e) => Game = null;
            if (Game != null) RunGame(Game, _view.SharpDx);
            World = world;
            _meshalyzer = new Meshalyzer.Meshalyzer(Game, world, dimension);
            Game.Disposing += (s, e) => { if (_meshingThread != null)_meshingThread.Abort(); };
            SetChunkCreationPolicy(ChunkCreationPolicy.Run);

            // _bridge = new EditorBridge(world, dimension, Game);
            _aggregator.Publish(new EditorOpenedMessage(this));
            _aggregator.Publish(new EditorGainedFocusMessage(this));

            DisplayName = String.Format("{0} - Editor", world.Title);
            return true;
        }

        protected override void OnViewLoaded(object view)
        {
            _view = view as EditorView;
            if (_view == null) return;

            if (_viewGained != null) _viewGained();
            _view.GotFocus += delegate
            {
                _aggregator.Publish(new EditorGainedFocusMessage(this));
            };
        }

        protected override void OnDeactivate(bool close)
        {
            if (!close)
                return;
            _aggregator.Publish(new EditorClosingMessage(this));

            // stop the meshing thread gracefully
            SetChunkCreationPolicy(ChunkCreationPolicy.Idle);
            _meshingThread.Join();

            // dispose game
            if (Game != null) Game.Dispose();
            Game = null;
        }

        /// <summary>
        /// Sets the policy for generating chunk meshes.
        /// </summary>
        /// <param name="policy">The policy.</param>
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
        void BeginBuildChunks()
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
