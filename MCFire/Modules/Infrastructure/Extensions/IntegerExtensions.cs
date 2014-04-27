namespace MCFire.Modules.Infrastructure.Extensions
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
    }
}
