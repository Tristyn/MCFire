using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Modules.Explorer.Messages;
using MCFire.Modules.Infrastructure.Models;
using Substrate;
using Substrate.Core;

namespace MCFire.Modules.Explorer.Models
{
    public class MCFireWorld : WorldBrowserItem
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
        [NotNull]
        readonly IEventAggregator _aggregator = IoC.Get<IEventAggregator>();

        public MCFireWorld(string path)
        {
            _directory = new DirectoryInfo(path);
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
        public void GetChunk(ChunkPositionDimension pos, AccessMode mode, Action<IChunk> chunkFunction)
        {
            // get the resource and its lock
            var chunkLock = GetChunkLock(pos);

            // run the func
            chunkLock.Access(mode, chunkFunction);

            // notify the chunk has changed if the mode is write
            if (mode == AccessMode.ReadWrite)
                _aggregator.Publish(new ChunkModifiedMessage(new ChunkPositionDimensionWorld(pos, this)));
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
            GetChunks(positions, mode, chunks => chunkRefFunc(chunks[0], chunks[1], chunks[2], chunks[3], chunks[4]));
        }

        /// <summary>
        /// Returns the underlying world from Substrate.
        /// </summary>
        [CanBeNull]
        NbtWorld NbtWorld
        {
            get
            {
                if (_nbtWorldCreated) return _nbtWorld;
                try
                {
                    _nbtWorldCreated = true;
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
        public override string Title
        {
            get { return _directory.Name; }
        }
    }

    public delegate void ChunksFunc(List<IChunk> chunks);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="south"></param>
    /// <param name="north"></param>
    /// <param name="west"></param>
    /// <param name="east"></param>
    public delegate void ChunkRefFunc(IChunk chunk, IChunk south, IChunk north, IChunk west, IChunk east);

    /// <summary>
    /// The operations that can be preformed on the resource.
    /// </summary>
    public enum AccessMode
    {// TODO: create, delete?
        /// <summary>
        /// The resource can be read from, but writing is not allowed.
        /// Constitutes multiple readers for one resource.
        /// </summary>
        Read,
        /// <summary>
        /// The resource can be read from and written to.
        /// Constitutes a single writer for one resource.
        /// </summary>
        ReadWrite
    }
}
