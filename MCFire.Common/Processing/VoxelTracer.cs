using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using MCFire.Common.Coordinates;
using Substrate;

namespace MCFire.Common.Processing
{
    /// <summary>
    /// Enumerates through voxel coordinates that it passes through using its origin and direction.
    /// </summary>
    public class VoxelTracer : IEnumerable<BlockPosition>
    {
        public VoxelTracer([NotNull] Vector3 position, [NotNull] Vector3 direction)
            :this(position,direction,500)
        {
        }

        public VoxelTracer([NotNull] Vector3 position, [NotNull] Vector3 direction, int limit)
        {
            Position = position;
            Direction = direction;
            Limit = limit;
        }

        // TODO: add options for setting dimension limits

        public IEnumerator<BlockPosition> GetEnumerator()
        {
            // an implementation of http://www.cse.yorku.ca/~amana/research/grid.pdf
            var origin = Position;
            var direction = Direction;
            // Cube containing origin point.
            var x = Math.Floor(origin.X);
            var y = Math.Floor(origin.Y);
            var z = Math.Floor(origin.Z);

            // direction vector
            var dx = direction.X;
            var dy = direction.Y;
            var dz = direction.Z;

            // Avoids an infinite loop.
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (dx == 0 && dy == 0 && dz == 0)
                dx = 1;
            // ReSharper restore CompareOfFloatsByEqualityOperator

            // Direction to increment x,y,z when stepping.
            var stepX = Sign(direction.X);
            var stepY = Sign(direction.Y);
            var stepZ = Sign(direction.Z);

            // See description above. The initial values depend on the fractional
            // part of the origin.
            var tMaxX = Intbound(origin.X, dx);
            var tMaxY = Intbound(origin.Y, dy);
            var tMaxZ = Intbound(origin.Z, dz);

            // The change in t when taking a step (always positive).
            var tDeltaX = stepX / dx;
            var tDeltaY = stepY / dy;
            var tDeltaZ = stepZ / dz;

            // Rescale from units of 1 cube-edge to units of 'direction' so we can
            // compare with 't'.
            var localLimit = Limit / Math.Sqrt(dx * dx + dy * dy + dz * dz);
            // tMaxX stores the t-value at which we cross a cube boundary along the
            // X axis, and similarly for Y and Z. Therefore, choosing the least tMax
            // chooses the closest cube boundary. Only the first case of the four
            // has been commented in detail.

            while (true)
            {
                if (tMaxX < tMaxY)
                {
                    if (tMaxX < tMaxZ)
                    {
                        if (tMaxX > localLimit)
                            break;
                        // Update which cube we are now in.
                        x += stepX;
                        // Adjust tMaxX to the next X-oriented boundary crossing.
                        tMaxX += tDeltaX;
                        yield return new BlockPosition((int)x, (int)y, (int)z); // TODO: couldnt you have one return right before the end of the loop?
                    }
                    else
                    {
                        if (tMaxZ > localLimit)
                            break;
                        z += stepZ;
                        tMaxZ += tDeltaZ;
                        yield return new BlockPosition((int)x, (int)y, (int)z);
                    }
                }
                else
                {
                    if (tMaxY < tMaxZ)
                    {
                        if (tMaxY > localLimit)
                            break;
                        y += stepY;
                        tMaxY += tDeltaY;
                        yield return new BlockPosition((int)x, (int)y, (int)z);
                    }
                    else
                    {
                        // Identical to the second case, repeated for simplicity in
                        // the conditionals.
                        if (tMaxZ > localLimit)
                            break;
                        z += stepZ;
                        tMaxZ += tDeltaZ;
                        yield return new BlockPosition((int)x, (int)y, (int)z);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        static double Intbound(double s, double ds)
        {
            // Find the smallest positive t such that s+t*ds is an integer.
            if (ds < 0)
                return Intbound(-s, -ds);

            s = WeirdMod(s, 1);
            // problem is now s+t*ds = 1
            return (1 - s) / ds;
        }

        static double WeirdMod(double value, double modulus)
        {
            return (value % modulus + modulus) % modulus;
        }

        static double Sign(double val)
        {
            return val > 0 ? 1 : val < 0 ? -1 : 0;
        }

        [NotNull]
        public Vector3 Position { get; private set; }

        [NotNull]
        public Vector3 Direction { get; private set; }

        /// <summary>
        /// The amount of voxels to traverse before stopping.
        /// </summary>
        public int Limit { get; private set; }
    }
}
