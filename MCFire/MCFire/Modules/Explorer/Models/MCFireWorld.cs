using System;
using System.IO;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Models;
using Substrate;

namespace MCFire.Modules.Explorer.Models
{
    public class MCFireWorld : WorldBrowserItem
    {
        readonly DirectoryInfo _directory;
        NbtWorld _nbtWorld;

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
        public MCFireChunk GetChunk(int dimension, int cx, int cy)
        {
            // TODO: SAFE NNULL NAGIVATOR C# 6
            var cm = NbtWorld.GetChunkManager(dimension);
            if (cm == null)
                return null;
            var chunk = cm.GetChunkRef(cx, cy);
            if (chunk == null)
                return null;
            var mcFireChunk = new MCFireChunk(chunk);
            // TODO: OH GOD
            return mcFireChunk;
        }

        public void NotifyChunkModified(int dimension, int cx, int cy)
        {
            // TODO:
            throw new NotImplementedException();
        }

        public override string Title
        {
            get { return _directory.Name; }
        }
    }
}
