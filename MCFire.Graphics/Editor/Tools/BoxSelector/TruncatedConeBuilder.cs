using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Graphics.Primitives;
using SharpDX;

namespace MCFire.Graphics.Editor.Tools.BoxSelector
{
    /// <summary>
    /// Creates truncated cone geometry for use as 3D model.
    /// </summary>
    public class TruncatedConeBuilder
    {
        /// <summary>
        /// Builds a new truncated cone buffer.
        /// The cone is aligned to the x axis.
        /// </summary>
        /// <param name="description">The description used to build the cone</param>
        /// <param name="tesselation">The tesselation, must be greater than 2</param>
        public VertexPosition[] Build(TruncatedConeDescription description, int tesselation = 32)
        {
            var sections = description.Sections;
            if (sections.Length <2) 
                throw new ArgumentException("sections must contain more than 1 element.");
            if (tesselation < 3)
                throw new ArgumentException("tesselation must be greater than 2.");

            // build tesselation angles
            var tessInverse = (float)1 / tesselation;
            var tesselationAngles = new Vector2[tesselation];
            for (var i = 0; i < tesselationAngles.Length; i++)
            {
                var quat = Quaternion.RotationAxis(Vector3.BackwardRH, tessInverse * i*MathUtil.TwoPi);
                var transformed = Vector3.Transform(Vector3.Up, quat);
                tesselationAngles[i] = new Vector2(transformed.X,transformed.Y);//discard x value
            }

            var vertCount = (sections.Length - 1) * tesselation * 6;
            var buffer = new VertexPosition[vertCount];

            // the first time this gets used it increments to 0
            int bufferIndex = -1;
            var prevSection = sections[0];
            for (var i = 1; i < description.Sections.Length; i++)
            {
                var section = description.Sections[i];

                // start at the last item
                var prevAngle = tesselationAngles.Last();
                for (var j = 0; j < tesselation; j++)
                {
                    var angle = tesselationAngles[j];
                    buffer[++bufferIndex] = GetVertex(prevSection, angle);
                    buffer[++bufferIndex] = GetVertex(section, angle);
                    buffer[++bufferIndex] = GetVertex(prevSection, prevAngle);

                    buffer[++bufferIndex] = GetVertex(section, angle);
                    buffer[++bufferIndex] = GetVertex(section, prevAngle);
                    buffer[++bufferIndex] = GetVertex(prevSection, prevAngle);

                    prevAngle = tesselationAngles[j];
                }
                prevSection = sections[i];
            }

            Debug.Assert(bufferIndex+1 == buffer.Length);
            return buffer;
        }

        static VertexPosition GetVertex(ConeSection section, Vector2 tessAngle)
        {
            return new VertexPosition(section.Position, tessAngle.Y * section.Radi, tessAngle.X*section.Radi);
        }
    }

    public class TruncatedConeDescription
    {
        public ConeSection[] Sections { get; private set; }

        public TruncatedConeDescription([NotNull] params ConeSection[] sections)
        {
            Sections = sections;
        }
    }

    public struct ConeSection
    {
        public float Radi { get; private set; }
        public float Position { get; private set; }

        public ConeSection(float radi, float position)
            : this()
        {
            Radi = radi;
            Position = position;
        }
    }
}
