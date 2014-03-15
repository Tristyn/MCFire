using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MCFire.Modules.Infrastructure
{
    public interface IFile : IFolderItem
    {
        #region Methods

        [NotNull]
        Task Open();

        [NotNull]
        Task ReplaceWith([NotNull] IFormat format);

        #endregion

        #region Properties

        string Extension { get; }
        IFolder Parent { get; }

        #endregion
    }
}