using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Common.Coordinates;
using MCFire.Common.Infrastructure.Models.MCFire.Modules.Infrastructure.Models;
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
        // TODO: MCFireWorld is not a MEF component, so the aggregator needs to go.
        //[NotNull]
        //readonly IEventAggregator _aggregator = IoC.Get<IEventAggregator>();
        [CanBeNull]
        ChunkSize? _chunkSize;

        private World(NbtWorld world)
        {
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

        // TODO: comment it up so that plebs know how to dougie
        // TODO: universal undo (Do(), Undo())
        // TODO: access tracking; output in the UI, also for going read-only we have to block new accesses and wait on running ones.
        public void GetChunk(ChunkPositionDimension pos, AccessMode mode, Action<IChunk> chunkFunction)
        {
            // get the resource and its lock
            var chunkLock = GetChunkLock(pos);

            // run the func
            chunkLock.Access(mode, chunkFunction);

            // notify the chunk has changed if the mode is write

            // TODO: another way to notify of change
            //if (mode == AccessMode.ReadWrite)
                //_aggregator.Publish(new ChunkModifiedMessage(new ChunkPositionDimensionWorld(pos, this)));
        }

        public void GetChunks(IEnumerable<ChunkPositionDimension> positions, AccessMode mode, ChunksFunc chunksFunc)
        {
            List<ReaderWriterObjectLock<IChunk>> locks;
            lock (_chunkAccessLock)
            {
                locks = GetChunkLocks(positions);
            }

            try
            {
                // ToList to call Access immediately
                chunksFunc.Invoke(locks.Select(chunkLock => chunkLock.Access(mode)).ToList());
            }
            finally
            {
                locks.ForEach(chunkLock => chunkLock.EndAccess(mode));
            }
        }

        /// <summary>
        /// Returns a chunk and its 4 neighbours
        /// </summary>
        public void GetChunkRef(ChunkPositionDimension pos, AccessMode mode, ChunkRefFunc chunkRefFunc)
        {
            var positions = new[]
            {
                pos,
                new ChunkPositionDimension(pos.ChunkX+1,pos.ChunkZ,pos.Dimension),
                new ChunkPositionDimension(pos.ChunkX-1,pos.ChunkZ,pos.Dimension),
                new ChunkPositionDimension(pos.ChunkX,pos.ChunkZ+1,pos.Dimension),
                new ChunkPositionDimension(pos.ChunkX,pos.ChunkZ-1,pos.Dimension)
            };
            // translate the ChunksFunc to a ChunkRefFunc and call it.
            GetChunks(positions, mode, chunks =>
            {
                var chunksList = chunks.ToList();
                chunkRefFunc(chunksList[0], chunksList[1], chunksList[2], chunksList[3], chunksList[4]);
            });
        }

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

        [CanBeNull]
        public ChunkSize? ChunkSize
        {
            get
            {
                if (_chunkSize != null)
                    return _chunkSize.Value;
                // TODO: this is all sorts of bad, there needs to be a system to determine a chunks size using an NbtWorld only
                var world = NbtWorld;
                if (world == null)
                    return null;

                lock (_chunkAccessLock)
                    foreach (var chunk in world.GetChunkManager().Where(chunk => chunk != null))
                        return _chunkSize = new ChunkSize(chunk);

                // no chunks exist
                // TODO: catastrophic state, also worldwide constants like ChunkSize should be available in NbtWorld
                return null;
            }
        }

        [NotNull]
        public string Title
        {
            get { return _directory.Name; }
        }
    }

    public delegate void ChunksFunc(List<IChunk> chunks);

    public delegate void ChunkRefFunc(IChunk chunk, IChunk south, IChunk north, IChunk west, IChunk east);
}
