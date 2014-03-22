using MCFire.Modules.Nbt.NbtTagAttribute;

namespace MCFire.Modules.Nbt.Models
{
    public class GameRulesContent
    {
        [Nbt("commandBlockOutput")]
        public string CommandBlockOutput { get; set; }

        [Nbt("doDaylightCycle")]
        public string DoDaylightCycle { get; set; }

        [Nbt("doFireTick")]
        public string DoFireTick { get; set; }

        [Nbt("doMobLoot")]
        public string DoMobLoot { get; set; }

        [Nbt("doMobSpawning")]
        public string DoMobSpawning { get; set; }

        [Nbt("doTileDrops")]
        public string DoTileDrops { get; set; }

        [Nbt("keepInventory")]
        public string KeepInventory { get; set; }

        [Nbt("mobGriefing")]
        public string MobGriefing { get; set; }

        [Nbt("naturalRegeneration")]
        public string NaturalRegeneration { get; set; }
    }
}
