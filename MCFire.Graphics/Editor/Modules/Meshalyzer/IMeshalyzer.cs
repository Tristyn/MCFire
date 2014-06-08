using JetBrains.Annotations;

namespace MCFire.Graphics.Modules.Meshalyzer.Models
{
    public interface IMeshalyzer : ILoadContent
    {
        /// <summary>
        /// Meshalyzes a part of the world.
        /// </summary>
        /// <returns>A drawable part of the world. Null denotes that no more work is to be done.</returns>
        [CanBeNull]
        IChunkMesh MeshalyzeNext([NotNull] EditorGame game);
    }
}
