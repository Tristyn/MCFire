using MCFire.Modules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MCFire.Modules.Meshalyzer.Models
{
    /// <summary>
    /// An object that is queried to return the closest ungenerated chunk.
    /// Once a chunk coordinate has been returned, it is considered generated.
    /// If the observer moves too far away from the chunk, it requests to be regenerated.
    /// </summary>
    public class ChunkPrioritizer
    {
        readonly object _lock = new object();
        bool[,] _generatedChunks;
        int _firstNonGenerated;
        int _viewDistance;
        ChunkPosition[] _closestChunks;
        ChunkPosition _previousPosition;

        public ChunkPrioritizer()
        {
            ViewDistance = 10;
        }

        /// <summary>
        /// Returns the closest non-generated chunk from position.
        /// </summary>
        /// <param name="position">The position of the observer in chunk space.</param>
        /// <param name="point">the point in chunk-space of the chunk.</param>
        /// <returns>If not all chunks are generated.</returns>
        public bool GetNextDesiredChunk(ChunkPosition position, out ChunkPosition point)
        {
            lock (_lock)
            {
                Shift(position);
                for (var i = _firstNonGenerated; i < _closestChunks.Length; ++i)
                {
                    var chunkPoint = _closestChunks[i];
                    if (_generatedChunks[chunkPoint.ChunkX, chunkPoint.ChunkZ]) continue;

                    // transform position back to world space, return it
                    point = position + chunkPoint - ViewDistance;
                    // set it to the index ahead, so when this loop is called again, it will start where it left off.
                    _firstNonGenerated = i + 1;
                    // assume it gets generated.
                    _generatedChunks[chunkPoint.ChunkX, chunkPoint.ChunkZ] = true;
                    return true;
                }
                point = new ChunkPosition();
                return false;
            }
        }

        void Shift(ChunkPosition position)
        {
            var difference = _previousPosition - position;
            if (difference == new ChunkPosition(0, 0))
                return;

            var shiftedChunks = new bool[_viewDistance * 2, _viewDistance * 2];

            // shift the array in the opposite way the observer is going.
            var minX = Math.Max(difference.ChunkX, 0);
            var minY = Math.Max(difference.ChunkZ, 0);
            var maxX = _generatedChunks.GetLength(0) + Math.Min(difference.ChunkX, 0);
            var maxY = _generatedChunks.GetLength(1) + Math.Min(difference.ChunkZ, 0);
            for (var index1 = minX; index1 < maxX; ++index1)
            {
                for (var index2 = minY; index2 < maxY; ++index2)
                    shiftedChunks[index1, index2] = _generatedChunks[index1 - difference.ChunkX, index2 - difference.ChunkZ];
            }
            _generatedChunks = shiftedChunks;
            _previousPosition = position;
            _firstNonGenerated = 0;
        }

        void GenerateChunkPoints()
        {
            var chunkPoints = new List<ChunkPosition>(ViewDistance * ViewDistance * 4);
            for (var x = -ViewDistance; x < ViewDistance; ++x)
            {
                for (var y = -ViewDistance; y < ViewDistance; ++y)
                    chunkPoints.Add(new ChunkPosition(x, y));
            }
            _closestChunks = (from point in chunkPoints
                              // order them by distance from the origin
                              orderby point.ChunkX * point.ChunkX + point.ChunkZ * point.ChunkZ
                              // offset them by viewdistance so none are negative
                              select point + ViewDistance).ToArray();

            _generatedChunks = new bool[ViewDistance * 2, ViewDistance * 2];
            // set to 0 because _generated chunks has changed.
            _firstNonGenerated = 0;
        }

        /// <summary>
        /// Distance in chunk space from the observer.
        /// If the observer leaves and reenters a chunks ViewDistance, it will request to be regenerated.
        /// </summary>
        public int ViewDistance
        {
            get
            {
                return _viewDistance;
            }
            set
            {
                lock (_lock)
                {
                    _viewDistance = value;
                    GenerateChunkPoints();
                }
            }
        }
    }
}
