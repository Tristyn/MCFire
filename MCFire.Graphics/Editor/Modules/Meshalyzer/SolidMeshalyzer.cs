namespace MCFire.Graphics.Modules.Meshalyzer.Models
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
