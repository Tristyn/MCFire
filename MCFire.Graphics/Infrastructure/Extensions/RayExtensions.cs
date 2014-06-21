using SharpDX;

namespace MCFire.Graphics.Infrastructure.Extensions
{
    public static class RayExtensions
    {
        /// <summary>
        /// Calculates the distance from a vector the closest point along a ray.
        /// </summary>
        public static float Distance(this Ray ray, Vector3 point)
        {
            return Vector3.Cross(ray.Direction, point - ray.Position).Length();
        }

        /// <summary>
        /// Returns a plane that the ray traverses, with its normal intersecting the point.
        /// </summary>
        public static Plane ToPlane(this Ray ray, Vector3 point)
        {
            var pointOfIntersection = ray.Position + ray.Direction * Vector3.Dot(ray.Direction, point - ray.Position);
            return new Plane(pointOfIntersection, (point - pointOfIntersection).ToNormal());
        }
    }
}
