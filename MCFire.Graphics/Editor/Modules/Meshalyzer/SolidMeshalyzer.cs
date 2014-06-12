using System.Collections.Generic;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Graphics.Editor.Modules.Meshalyzer
{
    public class SolidMeshalyzer : BlockMeshalyzer
    {
        public override void Meshalyze(int x, int y, int z, ICollection<VertexPositionColorTexture> buffer)
        {
            var block = GetBlock(x, y, z);

            //switch (block.ID)
            //{
            //    case BlockType.STONE:
                    
            //}


        }

        public void Stone(int x,int y, int z)
        {
            
        }
    }
}
