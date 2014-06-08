using System;
using JetBrains.Annotations;

namespace MCFire.Graphics.Modules.Primitives
{
    public static class GeometricPrimitives
    {
        public static readonly Vector3[] LeftQuad =
        {
            new Vector3(0,0,0),
            new Vector3(0,1,0),
            new Vector3(0,0,1),
            new Vector3(0,0,1),
            new Vector3(0,1,0),
            new Vector3(0,1,1) 
        };

        // notch messed up! it isnt a rotation of z+ like the top is. Its a mirror of z+ on the edge they share. It must have been like this forever.
        public static readonly Vector3[] BottomQuad =
        {
            new Vector3(0,0,1), 
            new Vector3(1,0,1),
            new Vector3(0,0,0),
            new Vector3(0,0,0), 
            new Vector3(1,0,1),
            new Vector3(1,0,0)
        };

        public static readonly Vector3[] ForwardQuad =
        {
            new Vector3(1,0,0),
            new Vector3(1,1,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(1,1,0),
            new Vector3(0,1,0) 
        };

        public static readonly Vector3[] RightQuad =
        {
            new Vector3(1,0,1),
            new Vector3(1,1,1),
            new Vector3(1,0,0),
            new Vector3(1,0,0),
            new Vector3(1,1,1),
            new Vector3(1,1,0)
        };

        public static readonly Vector3[] TopQuad =
        {
            new Vector3(0,1,1),
            new Vector3(0,1,0),
            new Vector3(1,1,1),
            new Vector3(1,1,1),
            new Vector3(0,1,0),
            new Vector3(1,1,0)
        };

        public static readonly Vector3[] BackwardQuad =
        {
            new Vector3(0,0,1),
            new Vector3(0,1,1),
            new Vector3(1,0,1),
            new Vector3(1,0,1),
            new Vector3(0,1,1),
            new Vector3(1,1,1)
        };

        /// <summary>
        /// Returns the an array of all quads in the standard face order: Left, Bottom, Forward, Right, Up, Backward
        /// </summary>
        public static readonly Vector3[][] Quads =
        {
            LeftQuad,
            BottomQuad,
            ForwardQuad,
            RightQuad,
            TopQuad,
            BackwardQuad
        };

        /// <summary>
        /// Returns the quad that coresponds to each face.
        /// </summary>
        /// <param name="face">The face of a cube. Faces must have only one flag set, and cant be set to None.</param>
        /// <returns></returns>
        [NotNull]
        public static Vector3[] GetQuad(Faces face)
        {
            switch (face)
            {
                case Faces.Left:
                    return LeftQuad;
                case Faces.Bottom:
                    return BottomQuad;
                case Faces.Forward:
                    return ForwardQuad;
                case Faces.Right:
                    return RightQuad;
                case Faces.Top:
                    return TopQuad;
                case Faces.Backward:
                    return BackwardQuad;

                default:
                    throw new ArgumentOutOfRangeException("face");
            }
        }

        public static readonly Vector2[] QuadUv =
        {
            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,1),
            new Vector2(1,1),
            new Vector2(0,0),
            new Vector2(1,0)
        };
    }
}
