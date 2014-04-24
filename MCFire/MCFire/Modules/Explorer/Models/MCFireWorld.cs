using System;
using System.IO;
using JetBrains.Annotations;
using Substrate;

namespace MCFire.Modules.Explorer.Models
{
    public class MCFireWorld : WorldBrowserItem
    {
        [NotNull]
        readonly DirectoryInfo _directory;
        [CanBeNull]
        NbtWorld _nbtWorld;

        public MCFireWorld(string path)
        {
            _directory = new DirectoryInfo(path);
        }

        [CanBeNull]
        public ChunkRef GetChunk(int dimension, int cx, int cy)
        {
            if (NbtWorld == null)
                return null;

            var cm = NbtWorld.GetChunkManager(dimension);
            return cm.GetChunkRef(cx, cy);
        }

        public void NotifyChunkModified(int dimension, int cx, int cy)
        {
            if (ChunkModified != null) ChunkModified(this, new ChunkCoordEventArgs(dimension, cx, cy));
        }

        public event EventHandler<ChunkCoordEventArgs> ChunkModified;

        /// <summary>
        /// Returns the underlying world from Substrate.
        /// </summary>
        [CanBeNull]
        public NbtWorld NbtWorld
        {
            get
            {
                if (_nbtWorld != null) return _nbtWorld;
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
        public override string Title
        {
            get { return _directory.Name; }
        }
    }

    public class ChunkCoordEventArgs : EventArgs
    {
        public int Dimension { get; private set; }
        public int Cx { get; private set; }
        public int Cy { get; private set; }

        public ChunkCoordEventArgs(int dimension, int cx, int cy)
        {
            Dimension = dimension;
            Cx = cx;
            Cy = cy;
        }
    }
}
