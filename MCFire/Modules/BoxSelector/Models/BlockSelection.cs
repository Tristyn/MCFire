using System;
using MCFire.Modules.Infrastructure.Models;

namespace MCFire.Modules.BoxSelector.Models
{
    /// <summary>
    /// Specifies a box-shaped selection in a minecraft world.
    /// A selection is immutable and can be used to enumerate through the collective data of multiple chunks.
    /// </summary>
    public class BlockSelection
    {
        readonly BlockPosition _lesser;
        readonly BlockPosition _greater;
        //readonly int _dimension;
        //readonly MCFireWorld _world;

        ///// <summary>
        ///// Creates a new BoxSelection with the specified dimensions and world.
        ///// </summary>
        ///// <param name="cornerOne">One corner of the box</param>
        ///// <param name="cornerTwo">The other corner of the box</param>
        ///// <param name="dimension">The dimension of the selection</param>
        ///// <param name="world">The world containing the selection</param>
        //public BoxSelection(BlockPosition cornerOne, BlockPosition cornerTwo, int dimension, [NotNull] MCFireWorld world)
        //{
        //    if (world == null) throw new ArgumentNullException("world");

        //    _lesser = new BlockPosition(
        //        Math.Min(cornerOne.X, cornerTwo.X),
        //        Math.Min(cornerOne.Y, cornerTwo.Y),
        //        Math.Min(cornerOne.Z, cornerTwo.Z));
        //    _greater = new BlockPosition(
        //        Math.Max(cornerOne.X, cornerTwo.X),
        //        Math.Max(cornerOne.Y, cornerTwo.Y),
        //        Math.Max(cornerOne.Z, cornerTwo.Z));
        //    _dimension = dimension;
        //    _world = world;
        //}

        /// <summary>
        /// Creates a new BoxSelection with the specified dimensions and world.
        /// </summary>
        /// <param name="cornerOne">One corner of the box</param>
        /// <param name="cornerTwo">The other corner of the box</param>
        /// <param name="dimension">The dimension of the selection</param>
        /// <param name="world">The world containing the selection</param>
        public BlockSelection(BlockPosition cornerOne, BlockPosition cornerTwo)
        {
            _lesser = new BlockPosition(
                Math.Min(cornerOne.X, cornerTwo.X),
                Math.Min(cornerOne.Y, cornerTwo.Y),
                Math.Min(cornerOne.Z, cornerTwo.Z));
            _greater = new BlockPosition(
                Math.Max(cornerOne.X, cornerTwo.X),
                Math.Max(cornerOne.Y, cornerTwo.Y),
                Math.Max(cornerOne.Z, cornerTwo.Z));
        }

        /// <summary>
        /// The lesser corner of the selection. All fields are less than SelectionGreater
        /// </summary>
        public BlockPosition Lesser
        {
            get { return _lesser; }
        }

        /// <summary>
        /// The greater corner of the selection. All fields are greater than SelectionGreater
        /// </summary>
        public BlockPosition Greater
        {
            get { return _greater; }
        }

        /// <summary>
        /// Length of the selection on the X axis
        /// </summary>
        public int XDim { get { return Greater.X - Lesser.X + 1; } } // + 1 because it is technically a collection length
        /// <summary>
        /// Height of the selection on the Y axis
        /// </summary>
        public int YDim { get { return Greater.Y - Lesser.Y + 1; } }
        /// <summary>
        /// Width of the selection on the Z axis
        /// </summary>
        public int ZDim { get { return Greater.Z - Lesser.Z + 1; } }
        //public int Dimension { get { return _dimension; } }

        //public void GetChunks(AccessMode mode, ChunksFunc chunksAction)
        //{
        //    var lesserChunk = (ChunkPosition)Lesser;
        //    var greaterChunk = (ChunkPosition)Greater;

        //    // create an enumerator from every chunk that is within the selection
        //   var chunkPositions = from x in Enumerable.Range(lesserChunk.ChunkX, greaterChunk.ChunkX)
        //                        from z in Enumerable.Range(lesserChunk.ChunkZ, greaterChunk.ChunkZ)
        //                        select new ChunkPositionDimension(x, z, Dimension);

        //    _world.GetChunks(chunkPositions, mode, chunksAction);
        //}

        // TODO: edit chunks inside selection
        // TODO: GetChunks method that can access infinite chunks at once, enumerates them and assumes that they wont be accessed after being enumerated over
    }
}