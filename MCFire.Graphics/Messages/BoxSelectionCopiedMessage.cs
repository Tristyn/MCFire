using JetBrains.Annotations;
using MCFire.Common;

namespace MCFire.Graphics.Messages
{
    public class BoxSelectionCopiedMessage
    {
        public BoxSelectionCopiedMessage([NotNull] BlockSelection selection)
        {
            Selection = selection;
        }

        [NotNull]
        public BlockSelection Selection { get; private set; }
    }
}
