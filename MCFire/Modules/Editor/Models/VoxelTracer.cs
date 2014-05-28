using System;
using System.Collections;
using System.Collections.Generic;
using MCFire.Modules.Infrastructure.Models;
using SharpDX;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// Enumerates through voxel coordinates that it passes through using its origin and direction.
    /// </summary>
    public class VoxelTracer : IEnumerable<Point3>
    {
        public VoxelTracer(Ray ray)
        {
            Limit = 500;
            Ray = ray;
        }

        // TODO: add options for setting dimension limits

        public IEnumerator<Point3> GetEnumerator()
        {
            // an implementation of http://www.cse.yorku.ca/~amana/research/grid.pdf
            var ray = Ray;
            var origin = ray.Position;
            var direction = ray.Direction;
            // Cube containing origin point.
            var x = (float)Math.Floor(origin.X);
            var y = (float)Math.Floor(origin.Y);
            var z = (float)Math.Floor(origin.Z);

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
                        yield return new Point3((int)x, (int)y, (int)z); // TODO: couldnt you have one return right before the end of the loop?
                    }
                    else
                    {
                        if (tMaxZ > localLimit)
                            break;
                        z += stepZ;
                        tMaxZ += tDeltaZ;
                        yield return new Point3((int)x, (int)y, (int)z);
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
                        yield return new Point3((int)x, (int)y, (int)z);
                    }
                    else
                    {
                        // Identical to the second case, repeated for simplicity in
                        // the conditionals.
                        if (tMaxZ > localLimit)
                            break;
                        z += stepZ;
                        tMaxZ += tDeltaZ;
                        yield return new Point3((int)x, (int)y, (int)z);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        static float Intbound(float s, float ds)
        {
            // Find the smallest positive t such that s+t*ds is an integer.
            if (ds < 0)
                return Intbound(-s, -ds);

            s = WeirdMod(s, 1);
            // problem is now s+t*ds = 1
            return (1 - s) / ds;
        }

        static float WeirdMod(float value, float modulus)
        {
            return (value % modulus + modulus) % modulus;
        }

        static float Sign(float val)
        {
            return val > 0 ? 1 : val < 0 ? -1 : 0;
        }

        public Ray Ray { get; private set; }

        /// <summary>
        /// The amount of voxels to traverse before stopping.
        /// </summary>
        public int Limit { get; set; }
    }
}
