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
        [CanBeNull] EditorGame _game;
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
            if (_game != null)
                return false;

            if (_view != null)
            {
                return TryInitializeToInternal(world, dimension);
            }

            // we dont have the view yet. when we do, this will be called
            _viewGained = () => TryInitializeToInternal(world, dimension);
            return true;
        }

        bool TryInitializeToInternal([NotNull] MCFireWorld world, int dimension)
        {
            if (world == null) throw new ArgumentNullException("world");

            if (_game != null)
                return false;
            var view = _view;
            if (view == null) 
                return false;
            var substrateWorld = world.NbtWorld;
            if (substrateWorld == null)
                return false;

            DisplayName = "Starting Up - Editor";

            _game = new EditorGame(view.SharpDx, IoC.GetAll<IGameComponent>(), world, substrateWorld,dimension);
            _game.Disposing += (s, e) => _game = null;
            if (_game != null) RunGame(_game, view.SharpDx);
            _meshalyzer = new Meshalyzer.Meshalyzer(_game, world, dimension);
            SetChunkCreationPolicy(ChunkCreationPolicy.Run);

            // _bridge = new EditorBridge(world, dimension, _game);
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

            // stop the meshing thread gracefully
            SetChunkCreationPolicy(ChunkCreationPolicy.Idle);
            _meshingThread.Join();
            _meshalyzer = null;

            _aggregator.Publish(new EditorClosingMessage(this));
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
    }
}
