using MCFire.Common.MCFire.Modules.Infrastructure.Models;

namespace MCFire.Common
{
    /// <summary>
    /// A generic view of a selection of partitioned chunks.
    /// </summary>
    public interface IBlockSelection
    {
        void GetChunks(AccessMode mode, PartionedChunksFunc chunksAction);
        BoxSelection Selection { get; }
        int Dimension { get; }
        World World { get; }
    }
}