using System;
using MCFire.Common.Coordinates;
using MCFire.Graphics.Primitives;
using SharpDX;

namespace MCFire.Graphics.Infrastructure.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 ToNormal(this Vector3 vector)
        {
            vector.Normalize();
            return vector;
        }


        /// <summary>
        /// Aligns the unit vector to the nearest axis
        /// </summary>
        public static Vector3 AlignToClosestAxis(this Vector3 vector)
        {
            var abs = Absolute(vector);

            // determine the greatest component of abs, then check if the same component of vector is positive
            if (abs.X > abs.Y)
            {
                if (abs.X > abs.Z)
                    return vector.X > 0 ? Vector3.Right : Vector3.Left;
                return vector.Z > 0 ? Vector3.BackwardRH : Vector3.ForwardRH;
            }

            if (abs.Y > abs.Z)
                return vector.Y > 0 ? Vector3.Up : Vector3.Down;
            return vector.Z > 0 ? Vector3.BackwardRH : Vector3.ForwardRH;
        }

        /// <summary>
        /// Returns a face based on the direction of a unit vector.
        /// </summary>
        public static Faces GetFace(this Vector3 vector)
        {
            var abs = Absolute(vector);

            // determine the greatest component of abs, then check if the same component of vector is positive
            if (abs.X > abs.Y)
            {
                if (abs.X > abs.Z)
                    return vector.X > 0 ? Faces.Right : Faces.Left;
                return vector.Z > 0 ? Faces.Backward : Faces.Forward;
            }

            if (abs.Y > abs.Z)
                return vector.Y > 0 ? Faces.Top :Faces.Bottom;
            return vector.Z > 0 ? Faces.Backward : Faces.Forward;
            
        }

        public static Vector3 Absolute(this Vector3 vector)
        {
            return new Vector3(Math.Abs(vector.X), Math.Abs(vector.Y), Math.Abs(vector.Z));
        }

        /// <summary>
        /// Rounds all components of the vector to the nearest whole number
        /// </summary>
        /// <returns></returns>
        public static Vector3 Round(this Vector3 vector)
        {
            return new Vector3((float)Math.Round(vector.X), (float)Math.Round(vector.Y), (float)Math.Round(vector.Z));
        }

        public static Vector3 AsVector3(this BlockPosition pos)
        {
            return new Vector3(pos.X,pos.Y,pos.Z);
        }
    }
}
