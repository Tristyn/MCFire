using System;
using JetBrains.Annotations;
using MCFire.Common;
using MCFire.Graphics.Editor.Tools.BoxSelector;

namespace MCFire.Graphics.Primitives
{
    /// <summary>
    /// Represents a axis-aligned immutable 3D cuboid.
    /// </summary>
    public struct Cuboid
    {
        public Cuboid(int x, int y, int z, int length, int height, int width)
            : this()
        {
            Left = x;
            Bottom = y;
            Forward = z;

            Length = length;
            Height = height;
            Width = width;
        }
        // TODO: cuboid components should use floats. theres some subtle breaking changes though.
        public Cuboid(Point3 cornerOne, Point3 cornerTwo)
            : this()
        {
            var left = Math.Min(cornerOne.X, cornerTwo.X);
            Left = left;
            Length = Math.Max(cornerOne.X, cornerTwo.X) - left;


            var bottom = Math.Min(cornerOne.Y, cornerTwo.Y);
            Bottom = bottom;
            Height = Math.Max(cornerOne.Y, cornerTwo.Y) - bottom;

            var forward = Math.Min(cornerOne.Z, cornerTwo.Z);
            Forward = forward;
            Width = Math.Max(cornerOne.Z, cornerTwo.Z) - forward;
        }

        /// <summary>
        /// The X value of the left-most (-1,0,0) face of the box. Coupled with Length.
        /// </summary>
        public int Left { get; private set; }

        /// <summary>
        /// The Y value of the bottom-most (0,-1,0) face of the box. Coupled with Height.
        /// </summary>
        public int Bottom { get; private set; }

        /// <summary>
        /// The Z value of the forward-most (0,0,-1) face of the box. Coupled with Width.
        /// </summary>
        public int Forward { get; private set; }

        /// <summary>
        /// The side length along the X-axis. Coupled with Left.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The side length along the Y-axis. Coupled with Bottom.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// The side length along the Z-axis. Coupled with Width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// The position of the cuboid
        /// </summary>
        public Point3 Position
        {
            get { return new Point3(Left, Bottom, Forward); }
        }

        /// <summary>
        /// Returns Lenght, Height, and Width packed into one Point3
        /// </summary>
        public Point3 Dimensions
        {
            get { return new Point3(Length, Height, Width); }
        }

        /// <summary>
        /// Assigns the component using the coresponding face
        /// </summary>
        /// <param name="cuboidComponent">The face of the box. Can not use a mix of bit flags</param>
        /// <param name="value">The value to assign</param>
        /// <returns>A new cuboid</returns>
        public Cuboid SetComponent(CuboidComponents cuboidComponent, int value)
        {
            switch (cuboidComponent)
            {
                case CuboidComponents.Left:
                    return new Cuboid(new Point3(value, Bottom, Forward), Dimensions);
                case CuboidComponents.Bottom:
                    return new Cuboid(new Point3(Left, value, Forward), Dimensions);
                case CuboidComponents.Forward:
                    return new Cuboid(new Point3(Left, Bottom, value), Dimensions);

                case CuboidComponents.Length:
                    return new Cuboid(Position, new Point3(value, Height, Width));
                case CuboidComponents.Height:
                    return new Cuboid(Position, new Point3(Length, value, Width));
                case CuboidComponents.Width:
                    return new Cuboid(Position, new Point3(Length, Height, value));

                case CuboidComponents.None:
                    return this;
                default:
                    throw new InvalidOperationException("face argument can not use a mix of bit flags. Call SetComponents(Faces,Cuboid) instead");
            }
        }

        /// <summary>
        /// Assigns the components using the coresponding faces
        /// </summary>
        /// <param name="cuboidComponents">The face of the box. Can not use a mix of bit flags</param>
        /// <param name="values">The value to assign</param>
        /// <returns>A new cuboid with a mix of new and original values, based on if each bit-flag of faces was set.</returns>
        public Cuboid SetComponents(CuboidComponents cuboidComponents, Cuboid values)
        {
            return new Cuboid(
                cuboidComponents.Any(CuboidComponents.Left) ? values.Left : Left,
                cuboidComponents.Any(CuboidComponents.Bottom) ? values.Bottom : Bottom,
                cuboidComponents.Any(CuboidComponents.Forward) ? values.Forward : Forward,
                cuboidComponents.Any(CuboidComponents.Length) ? values.Length : Length,
                cuboidComponents.Any(CuboidComponents.Height) ? values.Height : Height,
                cuboidComponents.Any(CuboidComponents.Width) ? values.Width : Width);
        }

        public int GetComponent(CuboidComponents component)
        {
            switch (component)
            {
                case CuboidComponents.Left:
                    return Left;
                case CuboidComponents.Bottom:
                    return Bottom;
                case CuboidComponents.Forward:
                    return Forward;

                case CuboidComponents.Length:
                    return Length;
                case CuboidComponents.Height:
                    return Height;
                case CuboidComponents.Width:
                    return Width;

                default:
                    throw new InvalidOperationException("face argument can not use a mix of bit flags and can not be set to None (0). Call SetComponents(Faces,Cuboid) instead.");
            }
        }

        /// <summary>
        /// Gets the position component. If faces is set to a positive (Right, Top or Backward), the sign of the returned value will be flipped.
        /// Faces cant use a combination of bit-flags or be set to None.
        /// </summary>
        public int GetPositionComponent(Faces faces)
        {
            switch (faces)
            {
                case Faces.Left:
                    return Left;
                case Faces.Bottom:
                    return Bottom;
                case Faces.Forward:
                    return Forward;
                case Faces.Right:
                    return Left + Length;
                case Faces.Top:
                    return Bottom + Height;
                case Faces.Backward:
                    return Forward + Width;

                default:
                    throw new ArgumentOutOfRangeException("faces");
            }
        }

        /// <summary>
        /// Gets the length component. If faces is set to a positive (Right, Top or Backward), the sign of the returned value will be flipped.
        /// Faces cant use a combination of bit-flags or be set to None.
        /// </summary>
        public int GetLengthComponent(Faces faces)
        {
            switch (faces)
            {
                case Faces.Left:
                    return Length;
                case Faces.Bottom:
                    return Height;
                case Faces.Forward:
                    return Width;
                case Faces.Right:
                    return Length;
                case Faces.Top:
                    return Height;
                case Faces.Backward:
                    return Width;

                default:
                    throw new ArgumentOutOfRangeException("faces");
            }
        }

        public override string ToString()
        {
            return String.Format("Position: {0}, Dimensions: {1}", Position, Dimensions);
        }
    }

    public static class CuboidExtensions
    {
        public static Cuboid GetCuboid(this BoxSelection box)
        {
            // lengths of the cuboid are floored at 1
            var left = box.Left;
            var bot = box.Bottom;
            var forward = box.Forward;

            return new Cuboid(left, bot, forward, box.Right - left + 1, box.Top -bot+ 1, box.Backward - forward + 1);
        }

        public static bool Within(this Cuboid cuboid, Point3 position)
        {
            return position.X >= cuboid.Left && position.X < cuboid.Left + cuboid.Length
                && position.Y >= cuboid.Bottom && position.Y < cuboid.Bottom + cuboid.Height
                && position.Z >= cuboid.Forward && position.Z < cuboid.Forward + cuboid.Width;
        }
    }

    /// <summary>
    /// Represents the 6 components of a cuboid.
    /// </summary>
    [Flags]
    public enum CuboidComponents
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
        /// The length from Left along the X-axis.
        /// </summary>
        Length = 1 << 3,
        /// <summary>
        /// The height from Bottom along the Y-axis.
        /// </summary>
        Height = 1 << 4,
        /// <summary>
        /// The width from Forward along the Z-axis.
        /// </summary>
        Width = 1 << 5
    }

    public static class CuboidComponentsExtensions
    {
        const CuboidComponents NegativeFaces = CuboidComponents.Left | CuboidComponents.Bottom | CuboidComponents.Forward;
        const CuboidComponents Lengths = CuboidComponents.Length | CuboidComponents.Height | CuboidComponents.Width;
        const CuboidComponents All = NegativeFaces | Lengths;

        /// <summary>
        /// Returns if any face flags have been set.
        /// </summary>
        public static bool AnyFaces(this CuboidComponents cuboidComponent)
        {
            return (cuboidComponent & NegativeFaces) != 0;
        }

        /// <summary>
        /// Returns if any lengths have been set.
        /// </summary>
        public static bool AnyLengths(this CuboidComponents cuboidComponent)
        {
            return (cuboidComponent & Lengths) != 0;
        }

        /// <summary>
        /// Returns if the face and its comparer share any common faces.
        /// </summary>
        /// <param name="a">The first face value</param>
        /// <param name="b">The second face value</param>
        public static bool Any(this CuboidComponents a, CuboidComponents b)
        {
            return (a & b) != 0;
        }

        /// <summary>
        /// Returns if any face bit-flags have been set.
        /// </summary>
        /// <param name="cuboidComponents"></param>
        /// <returns></returns>
        public static bool Any(this CuboidComponents cuboidComponents)
        {
            return (cuboidComponents & All) != 0;
        }

        /// <summary>
        /// Returns the face that is visually facing the opposite direction. Cant use bit-flags or be set to None.
        /// </summary>
        public static Faces GetBackFace(this Faces face)
        {
            switch (face)
            {
                case Faces.Left:
                    return Faces.Right;
                case Faces.Bottom:
                    return Faces.Top;
                case Faces.Forward:
                    return Faces.Backward;
                case Faces.Right:
                    return Faces.Left;
                case Faces.Top:
                    return Faces.Bottom;
                case Faces.Backward:
                    return Faces.Forward;

                default:
                    throw new ArgumentOutOfRangeException("face");
            }
        }

        /// <summary>
        /// Returns the face that is visually to the left. Cant use bit-flags or be set to None.
        /// </summary>
        public static Faces GetLeftVisualFace(this Faces face)
        {
            switch (face)
            {
                case Faces.Left:
                    return Faces.Forward;
                case Faces.Right:
                    return Faces.Backward;

                case Faces.Forward:
                    return Faces.Right;
                case Faces.Backward:
                    return Faces.Left;

                case Faces.Bottom:
                    return Faces.Right;
                case Faces.Top:
                    return Faces.Left;
                default:
                    throw new ArgumentOutOfRangeException("face");
            }
        }

        /// <summary>
        /// Returns the face that is visually below. Cant use bit-flags or be set to None.
        /// </summary>
        public static Faces GetTopVisualFace(this Faces face)
        {
            switch (face)
            {
                case Faces.Left:
                    return Faces.Top;
                case Faces.Right:
                    return Faces.Top;

                case Faces.Forward:
                    return Faces.Top;
                case Faces.Backward:
                    return Faces.Top;

                case Faces.Bottom:
                    return Faces.Backward;
                case Faces.Top:
                    return Faces.Forward;

                default:
                    throw new ArgumentOutOfRangeException("face");
            }
        }
    }
}
