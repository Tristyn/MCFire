namespace MCFire.Common
{
    /// <summary>
    /// A copy of IDisposable.
    /// ICleanup gets around the issue of IDisposable references being held by MEF.
    /// </summary>
    public interface ICleanup
    {
        void Dispose();
    }
}