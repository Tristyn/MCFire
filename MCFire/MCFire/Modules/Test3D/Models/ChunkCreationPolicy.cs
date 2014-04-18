namespace MCFire.Modules.Test3D.Models
{
    public enum ChunkCreationPolicy
    {
        /// <summary>
        /// Create no new chunks and occupy no CPU time.
        /// </summary>
        Idle,
        /// <summary>
        /// Create chunks on multiple threads.
        /// </summary>
        Run
    }
}