using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Common.Coordinates;
using MCFire.Common.MCFire.Modules.Infrastructure.Models;
using Substrate;
using Substrate.Core;

namespace MCFire.Common
{
    public class World
    {
        [NotNull]
        readonly DirectoryInfo _directory;
        [CanBeNull]
        NbtWorld _nbtWorld;
        bool _nbtWorldCreated;
        [NotNull]
        readonly object _chunkAccessLock = new object();
        [NotNull]
        readonly Dictionary<ChunkPositionDimension, ReaderWriterObjectLock<IChunk>> _chunkAccess
            = new Dictionary<ChunkPositionDimension, ReaderWriterObjectLock<IChunk>>();

        [CanBeNull]
        ChunkSize? _chunkSize;

        private World([NotNull] NbtWorld world)
        {
            if (world == null) throw new ArgumentNullException("world");

            _nbtWorld = world;
            _directory = new DirectoryInfo(world.Path);
        }

        public static World Open(string path)
        {
            var world = NbtWorld.Open(path);
            if (world == null)
                return null;

            var mcfire = new World(world);
            return mcfire;
        }

        // TODO: comment it up so that plebs know how to dougie
        // TODO: universal undo (Do(), Undo())
        // TODO: access tracking; output in the UI, also for going read-only we have to block new accesses and wait on running ones.

        /* TODO: Modify substrate to use cached scratch memory to reduce garbage compaction
         * Caching byte[] (4kb and other sizes) is the main priority, but caching tag nodes is good too.
         * byte[] pool: https://stackoverflow.com/questions/15726214/scratch-memory-in-a-managed-environment
         * Careful of memory leaks if using a singleton and with ICopyable<T>.Copy() ...
         */

        // TODO: all out object pooling https://stackoverflow.com/questions/2510975/c-sharp-object-pooling-pattern-implementation

        // TODO: Substrate: deterministic discovery of dimensions instead of guessing with int

        #region Chunks

        public void GetChunk(ChunkPositionDimension pos, AccessMode mode, [NotNull] Action<IChunk> chunkFunction)
        {
            // get the resource and its lock
            var chunkLock = GetChunkLock(pos);

            // run the func
            chunkLock.Access(mode, chunkFunction);

            // notify the chunk has changed if the mode is write
            var chunksEvent = ChunksModified;
            if (mode != AccessMode.ReadWrite || chunksEvent == null)
                return;

            chunksEvent(this, new ChunksModifiedEventArgs(new[] {pos}));
        }

        public void GetChunks([NotNull] IEnumerable<ChunkPositionDimension> positions, AccessMode mode, [NotNull] ChunksFunc chunksFunc)
        {
            List<ReaderWriterObjectLock<IChunk>> locks;
            lock (_chunkAccessLock)
            {
                locks = GetChunkLocks(positions);
            }

            // ToList to call Access immediately
            var chunks = locks.Select(chunkLock => chunkLock.Access(mode)).Where(chunk => chunk != null).ToList();

            chunksFunc(chunks);

            // notify the chunk has changed if the mode is write
            var chunksEvent = ChunksModified;
            if (mode != AccessMode.ReadWrite || chunksEvent == null)
                return;

            chunksEvent(this, new ChunksModifiedEventArgs(positions));
        }

        /// <summary>
        /// Returns a chunk and its 4 neighbours
        /// </summary>
        public void GetChunkRef(ChunkPositionDimension pos, AccessMode mode, [NotNull] ChunkRefFunc chunkRefFunc)
        {
            var positions = new[]
            {
                pos,
                new ChunkPositionDimension(pos.ChunkX + 1, pos.ChunkZ, pos.Dimension),
                new ChunkPositionDimension(pos.ChunkX - 1, pos.ChunkZ, pos.Dimension),
                new ChunkPositionDimension(pos.ChunkX, pos.ChunkZ + 1, pos.Dimension),
                new ChunkPositionDimension(pos.ChunkX, pos.ChunkZ - 1, pos.Dimension)
            };

            // translate the ChunksFunc to a ChunkRefFunc and call it.
            GetChunks(positions, mode, chunks =>
            {
                var chunksList = chunks.ToList();
                chunkRefFunc(chunksList[0], chunksList[1], chunksList[2], chunksList[3], chunksList[4]);
            });

            // notify the chunk has changed if the mode is write
            var chunksEvent = ChunksModified;
            if (mode == AccessMode.ReadWrite || chunksEvent == null)
                return;

            chunksEvent(this, new ChunksModifiedEventArgs(positions));
        }

        /// <summary>
        /// Gets or creates the chunk lock for the specified chunk position.
        /// </summary>
        [NotNull]
        ReaderWriterObjectLock<IChunk> GetChunkLock(ChunkPositionDimension pos)
        {
            lock (_chunkAccessLock)
            {
                ReaderWriterObjectLock<IChunk> chunkLock;
                if (_chunkAccess.TryGetValue(pos, out chunkLock)) return chunkLock;

                // create a new chunkLock
                if (NbtWorld == null) return _chunkAccess[pos] = new ReaderWriterObjectLock<IChunk>(null);
                var chunk = NbtWorld.GetChunkManager(pos.Dimension).GetChunk(pos.ChunkX, pos.ChunkZ);
                return _chunkAccess[pos] = new ReaderWriterObjectLock<IChunk>(chunk);
            }
        }

        /// <summary>
        /// Gets multiple locks at once.
        /// </summary>
        [NotNull]
        List<ReaderWriterObjectLock<IChunk>> GetChunkLocks(IEnumerable<ChunkPositionDimension> positions)
        {
            lock (_chunkAccessLock)
            {
                var chunkLocks = new List<ReaderWriterObjectLock<IChunk>>();
                foreach (var pos in positions)
                {
                    ReaderWriterObjectLock<IChunk> chunkLock;
                    if (_chunkAccess.TryGetValue(pos, out chunkLock))
                    {
                        chunkLocks.Add(chunkLock);
                        continue;
                    }

                    // create a new chunkLock
                    if (_nbtWorld == null)
                    {
                        chunkLocks.Add(_chunkAccess[pos] = new ReaderWriterObjectLock<IChunk>(null));
                        continue;
                    }

                    var chunk = _nbtWorld.GetChunkManager(pos.Dimension).GetChunk(pos.ChunkX, pos.ChunkZ);
                    chunkLocks.Add(_chunkAccess[pos] = new ReaderWriterObjectLock<IChunk>(chunk));
                }
                return chunkLocks;
            }
        }

        public event EventHandler<ChunksModifiedEventArgs> ChunksModified;

        #endregion

        // TODO: method to access an massive amounts of chunks while using little memory. (GetChunks returns a list)

        /// <summary>
        /// Returns the underlying world from Substrate.
        /// </summary>
        [CanBeNull]
        NbtWorld NbtWorld
        {
            get
            {
                if (_nbtWorldCreated) return _nbtWorld;
                _nbtWorldCreated = true;
                try
                {
                    return _nbtWorld = NbtWorld.Open(_directory.FullName);
                }
                catch (DirectoryNotFoundException)
                {
                    return null;
                }
            }
        }

        public GameType GameType
        {
            get
            {
                var world = NbtWorld;
                return world == null ? GameType.SURVIVAL : world.Level.GameType;
            }
        }

        [NotNull]
        public string Title
        {
            get { return _directory.Name; }
        }
    }

    public class ChunksModifiedEventArgs : EventArgs
    {
        public IEnumerable<ChunkPositionDimension> ModifiedChunks { get; private set; }

        public ChunksModifiedEventArgs(IEnumerable<ChunkPositionDimension> modifiedChunks)
        {
            ModifiedChunks = modifiedChunks;
        }
    }

    public delegate void ChunksFunc(List<IChunk> chunks);

    public delegate void ChunkRefFunc(IChunk chunk, IChunk south, IChunk north, IChunk west, IChunk east);
}
