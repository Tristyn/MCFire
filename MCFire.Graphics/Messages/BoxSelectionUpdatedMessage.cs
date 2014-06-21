using System;
using MCFire.Common;

namespace MCFire.Graphics.Messages
{
   public class BoxSelectionUpdatedMessage
    {
        readonly Func<BoxSelection, IBlockSelection> _blockSelectionCallback;

        public BoxSelectionUpdatedMessage(BoxSelection selection, Func<BoxSelection, IBlockSelection> blockSelectionCallback)
        {
            Selection = selection;
            _blockSelectionCallback = blockSelectionCallback;
        }

        public IBlockSelection GetBlocks()
        {
            return _blockSelectionCallback(Selection);
        }

        public BoxSelection Selection { get; private set; }
    }
}
