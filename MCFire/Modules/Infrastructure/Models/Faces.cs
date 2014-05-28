using System;

namespace MCFire.Modules.Infrastructure.Models
{
    /// <summary>
    /// Enumeration of the six faces of a cube.
    /// </summary>
    [Flags]
    public enum Faces
    {
        /// <summary>
        /// No faces.
        /// </summary>
        None = 0,
        /// <summary>
        /// The left-most (-1,0,0) face of a cuboid.
        /// </summary>
        Left = 1,
        /// <summary>
        /// The bottom-most (0,-1,0) face of a cuboid.
        /// </summary>
        Bottom = 1 << 1,
        /// <summary>
        /// The forward-most (0,0,-1) face of a cuboid.
        /// </summary>
        Forward = 1 << 2,
        /// <summary>
        /// The right-most (1,0,0) face of a cuboid.
        /// </summary>
        Right = 1 << 3,
        /// <summary>
        /// The top-most (0,1,0) face of a cuboid.
        /// </summary>
        Top = 1 << 4,
        /// <summary>
        /// The backward-most (0,0,1) face of a cuboid.
        /// </summary>
        Backward = 1 << 5
    }

    /// <summary>
    /// Static methods for Faces
    /// </summary>
    public static class FacesUtils
    {
        /// <summary>
        /// All 6 Faces ordered: Left, Bottom, Forward, Right, Top, Backward
        /// </summary>
        public static readonly Faces[] AllFaces =
        {
            Faces.Left,
            Faces.Bottom, 
            Faces.Forward, 
            Faces.Right, 
            Faces.Top, 
            Faces.Backward
        };

        /// <summary>
        /// The 3 faces that face (get it?) a negative direction, ordererd: Left, Bottom, Forward
        /// </summary>
        public static readonly Faces NegativeFaces = Faces.Left | Faces.Bottom | Faces.Forward;

        /// <summary>
        /// The 3 faces that face (get it?) a positive direction, ordererd: Right, Top, Backward
        /// </summary>
        public static readonly Faces PositiveFaces = Faces.Right | Faces.Top | Faces.Backward;

        /// <summary>
        /// The 4 faces that are on the sides of a cuboid.
        /// </summary>
        public static readonly Faces SideFace = Faces.Left | Faces.Right | Faces.Forward | Faces.Backward;
    }

    public static class FacesExtensions
    {
        /// <summary>
        /// Returns if any faces are positive.
        /// </summary>
        public static bool AnyPositive(this Faces faces)
        {
            return (faces & FacesUtils.PositiveFaces) != 0;
        }

        /// <summary>
        /// Returns if any faces are negative.
        /// </summary>
        public static bool AnyNegative(this Faces faces)
        {
            return (faces & FacesUtils.NegativeFaces) != 0;
        }

        public static bool HasSideFaces(this Faces faces)
        {
            return (faces & FacesUtils.SideFace) != 0;
        }
    }
}