using System;
using System.IO;
using JetBrains.Annotations;
using Substrate;

namespace MCFire.Modules.Explorer.Models
{
    public class MCFireWorld : WorldBrowserItem
    {
        [NotNull] readonly DirectoryInfo _directory;
        [CanBeNull] NbtWorld _nbtWorld;

        public MCFireWorld(string path)
        {
            _directory = new DirectoryInfo(path);
        }

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
            // TODO:
            throw new NotImplementedException();
        }

        [NotNull]
        public override string Title
        {
            get { return _directory.Name; }
        }
    }
}
