using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MCFire.Common.Coordinates;

namespace MCFire.Common
{
    public struct BoxSelection
    {
        readonly int _left;
        readonly int _right;
        readonly int _bottom;
        readonly int _top;
        readonly int _forward;
        readonly int _backward;

        public BoxSelection(BlockPosition cornerOne, BlockPosition cornerTwo)
            : this()
        {
            _left = Math.Min(cornerOne.X, cornerTwo.X);
            _right = Math.Max(cornerOne.X, cornerTwo.X);
            _bottom = Math.Min(cornerOne.Y, cornerTwo.Y);
            _top = Math.Max(cornerOne.Y, cornerTwo.Y);
            _forward = Math.Min(cornerOne.Z, cornerTwo.Z);
            _backward = Math.Max(cornerOne.Z, cornerTwo.Z);
        }

        /// <summary>
        /// Creates a BoxSelection that fits the volume of a chunk.
        /// </summary>
        /// <param name="position">The position of the chunk volume.</param>
        /// <param name="size">The size of the chunk volume.</param>
        public BoxSelection(ChunkPosition position, ChunkSize size)
            : this()
        {
            var lesser = new BlockPosition(position, new LocalBlockPosition(), size);
            _left = lesser.X;
            _bottom = lesser.Y;
            _forward = lesser.Z;
            var greater = new BlockPosition(position, new LocalBlockPosition(size.X - 1, size.Y - 1, size.Z - 1), size);
            _right = greater.X;
            _top = greater.Y;
            _backward = greater.Z;
        }

        public static bool operator ==(BoxSelection a, BoxSelection b)
        {
            return a._left == b._left
                && a._right == b._right
                && a._bottom == b._bottom
                && a._top == b._top
                && a._forward == b._forward
                && a._backward == b.Backward;
        }

        public static bool operator !=(BoxSelection a, BoxSelection b)
        {
            return a._left != b._left
                || a._right != b._right
                || a._bottom != b._bottom
                || a._top != b._top
                || a._forward != b._forward
                || a._backward != b._backward;
        }

        public bool Equals(BoxSelection other)
        {
            return _left == other._left 
                   && _right == other._right 
                   && _bottom == other._bottom 
                   && _top == other._top 
                   && _forward == other._forward 
                   && _backward == other._backward;
        }

        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BoxSelection && Equals((BoxSelection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _left;
                hashCode = (hashCode*397) ^ _right;
                hashCode = (hashCode*397) ^ _bottom;
                hashCode = (hashCode*397) ^ _top;
                hashCode = (hashCode*397) ^ _forward;
                hashCode = (hashCode*397) ^ _backward;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return String.Format("Lesser: {0}, Greater: {1}", Lesser, Greater);
        }

        /// <summary>
        /// Returns if a and b overlap.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static bool Overlap(BoxSelection a, BoxSelection b)
        {
            // assert that the projections of all axes overlap
            if (a.Left < b.Right)
                return false;
            if (b.Left < a.Right)
                return false;
            if (a.Bottom < b.Top)
                return false;
            if (b.Bottom < a.Top)
                return false;
            if (a.Forward < b.Backward)
                return false;
            if (b.Forward < a.Backward)
                return false;

            return true;
        }

        /// <summary>
        /// Returns a BoxSelection where two other BoxSelections overlap.
        /// </summary>
        /// <param name="a">The first BoxSelection</param>
        /// <param name="b">The second BoxSelection</param>
        public static BoxSelection? Intersect(BoxSelection a, BoxSelection b)
        {
            if (!Overlap(a, b))
                return null;

            var lesser = new BlockPosition(
                Math.Max(a.Left, b.Left),
                Math.Max(a.Bottom, b.Bottom),
                Math.Max(a.Forward, b.Forward));
            var greater = new BlockPosition(
                Math.Min(a.Right, b.Right),
                Math.Min(a.Top, b.Top),
                Math.Min(a.Backward, b.Backward));
            return new BoxSelection(lesser, greater);
        }

        [NotNull]
        public static IEnumerable<BoxSelection> Deintersect(BoxSelection minuend, BoxSelection subtrahend)
        {
            var inclusive = Intersect(minuend, subtrahend);

            // if subtrahend doesn't share space with this
            if (inclusive == null)
            {
                yield return minuend;
                yield break;
            }


        }

        /// <summary>
        /// The lesser corner of the selection. All fields are less than SelectionGreater
        /// </summary>
        public BlockPosition Lesser
        {
            get { return new BlockPosition(_left, _bottom, _forward); }
        }

        /// <summary>
        /// The greater corner of the selection. All fields are greater than SelectionLesser
        /// </summary>
        public BlockPosition Greater
        {
            get { return new BlockPosition(_right, _top, _backward); }
        }

        /// <summary>
        /// The X value of the right-most (1,0,0) face of the box.
        /// </summary>
        public int Right { get { return _right; } }
        /// <summary>
        /// The X value of the left-most (-1,0,0) face of the box.
        /// </summary>
        public int Left { get { return _left; } }
        /// <summary>
        /// The Y value of the top-most (0,1,0) face of the box.
        /// </summary>
        public int Top { get { return _top; } }
        /// <summary>
        /// The Y value of the bottom-most (0,-1,0) face of the box.
        /// </summary>
        public int Bottom { get { return _bottom; } }
        /// <summary>
        /// The Z value of the forward-most (0,0,-1) face of the box.
        /// </summary>
        public int Forward { get { return _forward; } }
        /// <summary>
        /// The Z value of the back-most (0,0,1) face of the box.
        /// </summary>
        public int Backward { get { return _backward; } }
    }
}
