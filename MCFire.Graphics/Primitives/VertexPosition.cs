using System;

namespace MCFire.Graphics.Modules.Primitives
{
    /// <summary>
    /// Describes a custom vertex format structure that contains position information.
    /// </summary>
    public struct VertexPosition : IEquatable<VertexPosition>
    {
        /// <summary>
        /// Defines structure byte size.
        /// </summary>
        public static readonly int Size = 12;
        /// <summary>
        /// XYZ position.
        /// </summary>
        [VertexElement("SV_Position")]
        public Vector3 Position;

        /// <summary>
        /// Initializes a new <see cref="T:SharpDX.Toolkit.Graphics.VertexPositionColor"/> instance.
        /// </summary>
        /// <param name="position">The position of this vertex.</param><param name="color">The color of this vertex.</param>
        public VertexPosition(Vector3 position)
            :this()
        {
            Position = position;
        }

        public VertexPosition(float x, float y, float z):this()
        {
            Position=new Vector3(x,y,z);
        }

        public static bool operator ==(VertexPosition left, VertexPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VertexPosition left, VertexPosition right)
        {
            return !left.Equals(right);
        }

        public bool Equals(VertexPosition other)
        {
            return Position.Equals(other.Position);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) || !(obj is VertexPosition))
                return false;
            return Equals((VertexPosition)obj);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format((string) "Position: {0}", (object) Position);
        }
    }
}
