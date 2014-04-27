using System;
using MCFire.Modules.Nbt.Attributes;

namespace MCFire.Modules.Nbt.Models
{
    /// <summary>
    /// Content for level.dat
    /// </summary>
    public class LevelContent : NbtContent
    {
        private int _version;
        private byte _initialized;
        private string _levelName;
        private string _generatorName;
        private int _generatorVersion;
        private string _generatorOptions;
        private long _randomSeed;
        private byte _mapFeatures;
        private int _thunderTime;
        private byte _thundering;
        private int _rainTime;
        private byte _raining;
        private int _spawnZ;
        private int _spawnY;
        private int _spawnX;
        private long _dayTime;
        private long _time;
        private byte _difficultyLocked;
        private byte _difficulty;
        private int _gameType;
        private byte _hardcore;
        private byte _allowCommands;
        private long _sizeOnDisk;
        private long _lastPlayed;
        private GameRulesContent _gameRules;

        [Nbt("version")]
        public int Version
        {
            get { return _version; }
            set
            {
                _version = value;
                IsDirty();
            }
        }

        [Nbt("initialized")]
        public byte Initialized
        {
            get { return _initialized; }
            set
            {
                _initialized = value;
                IsDirty();
            }
        }

        [Nbt("LevelName")]
        public string LevelName
        {
            get { return _levelName; }
            set
            {
                _levelName = value;
                IsDirty();
            }
        }

        [Nbt("generatorName")]
        public string GeneratorName
        {
            get { return _generatorName; }
            set
            {
                _generatorName = value;
                IsDirty();
            }
        }

        [Nbt("generatorVersion")]
        public int GeneratorVersion
        {
            get { return _generatorVersion; }
            set
            {
                _generatorVersion = value;
                IsDirty();
            }
        }

        [Nbt("generatorOptions")]
        public string GeneratorOptions
        {
            get { return _generatorOptions; }
            set
            {
                _generatorOptions = value;
                IsDirty();
            }
        }

        [Nbt("RandomSeed")]
        public long RandomSeed
        {
            get { return _randomSeed; }
            set
            {
                _randomSeed = value;
                IsDirty();
            }
        }

        [Nbt("MapFeatures")]
        public byte MapFeatures
        {
            get { return _mapFeatures; }
            set
            {
                _mapFeatures = value;
                IsDirty();
            }
        }

        [Nbt("LastPlayed")]
        public long LastPlayed
        {
            get { return _lastPlayed; }
            set
            {
                _lastPlayed = value;
                IsDirty();
            }
        }

        [Nbt("SizeOnDisk")]
        public long SizeOnDisk
        {
            get { return _sizeOnDisk; }
            set
            {
                _sizeOnDisk = value;
                IsDirty();
            }
        }

        [Nbt("allowCommands")]
        public byte AllowCommands
        {
            get { return _allowCommands; }
            set
            {
                _allowCommands = value;
                IsDirty();
            }
        }

        [Nbt("hardcore")]
        public Byte Hardcore
        {
            get { return _hardcore; }
            set
            {
                _hardcore = value;
                IsDirty();
            }
        }

        [Nbt("GameType")]
        public int GameType
        {
            get { return _gameType; }
            set
            {
                _gameType = value;
                IsDirty();
            }
        }

        [Nbt("Difficulty")]
        public byte Difficulty
        {
            get { return _difficulty; }
            set
            {
                _difficulty = value;
                IsDirty();
            }
        }

        [Nbt("DifficultyLocked")]
        public byte DifficultyLocked
        {
            get { return _difficultyLocked; }
            set
            {
                _difficultyLocked = value;
                IsDirty();
            }
        }

        [Nbt("Time")]
        public long Time
        {
            get { return _time; }
            set
            {
                _time = value;
                IsDirty();
            }
        }

        [Nbt("DayTime")]
        public long DayTime
        {
            get { return _dayTime; }
            set
            {
                _dayTime = value;
                IsDirty();
            }
        }

        [Nbt("SpawnX")]
        public int SpawnX
        {
            get { return _spawnX; }
            set
            {
                _spawnX = value;
                IsDirty();
            }
        }

        [Nbt("SpawnY")]
        public int SpawnY
        {
            get { return _spawnY; }
            set
            {
                _spawnY = value;
                IsDirty();
            }
        }

        [Nbt("SpawnZ")]
        public int SpawnZ
        {
            get { return _spawnZ; }
            set
            {
                _spawnZ = value;
                IsDirty();
            }
        }

        [Nbt("raining")]
        public byte Raining
        {
            get { return _raining; }
            set
            {
                _raining = value;
                IsDirty();
            }
        }

        [Nbt("rainTime")]
        public int RainTime
        {
            get { return _rainTime; }
            set
            {
                _rainTime = value;
                IsDirty();
            }
        }

        [Nbt("thundering")]
        public byte Thundering
        {
            get { return _thundering; }
            set
            {
                _thundering = value;
                IsDirty();
            }
        }

        [Nbt("thunderTime")]
        public int ThunderTime
        {
            get { return _thunderTime; }
            set
            {
                _thunderTime = value;
                IsDirty();
            }
        }

        // TODO:
        //[Nbt("Player")]
        //public PlayerContent Player { get; set; }

        [Nbt("GameRules")]
        public GameRulesContent GameRules
        {
            get { return _gameRules; }
            private set
            {
                _gameRules = value;
                _gameRules.Dirtied += (s, e) => IsDirty();
            }
        }
    }
}
