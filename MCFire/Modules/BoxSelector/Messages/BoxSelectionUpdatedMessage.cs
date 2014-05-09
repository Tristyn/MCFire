using System;
using MCFire.Modules.BoxSelector.Models;

namespace MCFire.Modules.BoxSelector.Messages
{
    public class BoxSelectionUpdatedMessage
    {
        readonly Func<BoxSelection, BlockSelection> BlockSelectionCallback;

        public BoxSelectionUpdatedMessage(BoxSelection selection, Func<BoxSelection, BlockSelection> blockSelectionCallback)
        {
            Selection = selection;
            BlockSelectionCallback = blockSelectionCallback;
        }

        public BlockSelection GetBlocks()
        {
            return BlockSelectionCallback(Selection);
        }

        public BoxSelection Selection { get; private set; }
    }
}
