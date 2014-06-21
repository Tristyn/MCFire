using System.Diagnostics;

namespace MCFire.Common.Infrastructure.Extensions
{
    public static class IntegerExtensions
    {
        /// <summary>
        /// Modulus operation on the integer.
        /// The % operator is really a remainder operation, not a modulus operation.
        /// </summary>
        /// <param name="x">The number to modulate</param>
        /// <param name="m">The modulululu</param>
        /// <returns>The modulated int</returns>
        public static int Mod(this int x, int m)
        {
            var r = x % m;
            return r < 0 ? r + m : r;
        }

        /// <summary>
        /// Clamps the integer inclusively within floor and ceiling.
        /// </summary>
        public static int Clamp(this int x, int floor, int ceil)
        {
            Debug.Assert(floor<=ceil);
            return x < floor ? floor : x > ceil ? ceil : x;
        }
    }
}
