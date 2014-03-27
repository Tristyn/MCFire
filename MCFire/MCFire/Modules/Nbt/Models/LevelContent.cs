using System;
using System.IO;
using MCFire.Modules.Nbt.NbtTagAttribute;

namespace MCFire.Modules.Nbt.Models
{
    /// <summary>
    /// Content for level.dat
    /// </summary>
    class LevelContent : NbtContent
    {
        [Nbt("version")]
        public int Version { get; set; }

        [Nbt("initialized")]
        public byte Initialized { get; set; }

        [Nbt("LevelName")]
        public string LevelName { get; set; }

        [Nbt("generatorName")]
        public string GeneratorName { get; set; }

        [Nbt("generatorVersion")]
        public int GeneratorVersion { get; set; }

        [Nbt("generatorOptions")]
        public string GeneratorOptions { get; set; }

        [Nbt("RandomSeed")]
        public long RandomSeed { get; set; }

        [Nbt("MapFeatures")]
        public byte MapFeatures { get; set; }

        [Nbt("LastPlayed")]
        public long LastPlayed { get; set; }

        [Nbt("SizeOnDisk")]
        public long SizeOnDisk { get; set; }

        [Nbt("allowCommands")]
        public byte AllowCommands { get; set; }

        [Nbt("hardcore")]
        public Byte Hardcore { get; set; }

        [Nbt("GameType")]
        public int GameType { get; set; }

        [Nbt("Difficulty")]
        public byte Difficulty { get; set; }

        [Nbt("DifficultyLocked")]
        public byte DifficultyLocked { get; set; }

        [Nbt("Time")]
        public long Time { get; set; }

        [Nbt("DayTime")]
        public long DayTime { get; set; }

        [Nbt("SpawnX")]
        public int SpawnX { get; set; }

        [Nbt("SpawnY")]
        public int SpawnY { get; set; }

        [Nbt("SpawnZ")]
        public int SpawnZ { get; set; }

        [Nbt("raining")]
        public byte Raining { get; set; }

        [Nbt("rainTime")]
        public int RainTime { get; set; }

        [Nbt("thundering")]
        public byte Thundering { get; set; }

        [Nbt("thunderTime")]
        public int ThunderTime { get; set; }
        // TODO:
        //    [Nbt("Player")]
        //    public PlayerContent Player { get; set; }

        [Nbt("GameRules")]
        public GameRulesContent GameRules { get; set; }
    }
}
