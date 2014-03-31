using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using MCFire.Modules.Nbt.Attributes;

namespace MCFire.Modules.Nbt.Models
{
    public class EntityContent : NbtContent
    {
        private string _id;
        private List<double> _pos;
        private List<double> _motion;
        private List<float> _rotation;
        private float _fallDistance;
        private short _fire;
        private short _air;
        private bool _onGround;
        private int _dimensions;
        private byte _invulnerable;
        private int _portalCooldown;
        private long _uuidMost;
        private long _uuidLeast;
        private EntityContent _riding;

        /// <summary>
        /// Entity ID. This tag does not exist for the Player entity.
        /// </summary>
        [Nbt("id")]
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                IsDirty();
            }
        }

        /// <summary>
        /// DO NOT SET ITEMS OF THE LIST. REASSIGN THE ENTIRE LIST.
        /// 3 TAG_Doubles describing the current X,Y,Z position of the entity.
        /// </summary>
        [Nbt("Pos")]
        public List<double> Pos
        {
            get { return _pos; }
            set
            {
                _pos = value;
                IsDirty();
            }
        }

        /// <summary>
        /// DO NOT SET ITEMS OF THE LIST. REASSIGN THE ENTIRE LIST.
        /// 3 TAG_Doubles describing the current dX,dY,dZ velocity of the entity in meters per tick.
        /// </summary>
        [Nbt("Motion")]
        public List<double> Motion
        {
            get { return _motion; }
            set
            {
                _motion = value;
                IsDirty();
            }
        }

        /// <summary>
        /// DO NOT SET ITEMS OF THE LIST. REASSIGN THE ENTIRE LIST.
        /// Two TAG_Floats representing rotation in degrees.
        /// The first float is the entity's rotation clockwise around the Y axis (called yaw). Due west is 0. Does not exceed 360 degrees.
        /// The second float is the entity's declination from the horizon (called pitch). Horizontal is 0. Positive values look downward. Does not exceed positive or negative 90 degrees.
        /// </summary>
        [Nbt("Rotation")]
        public List<float> Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                IsDirty();
            }
        }

        /// <summary>
        /// Distance the entity has fallen. Larger values cause more damage when the entity lands.
        /// </summary>
        [Nbt("FallDistance")]
        public float FallDistance
        {
            get { return _fallDistance; }
            set
            {
                _fallDistance = value;
                IsDirty();
            }
        }

        /// <summary>
        /// Number of ticks until the fire is put out. 
        /// Negative values reflect how long the entity can stand in fire before burning. 
        /// Default -1 when not on fire.
        /// </summary>
        [Nbt("Fire")]
        public short Fire
        {
            get { return _fire; }
            set
            {
                _fire = value;
                IsDirty();
            }
        }

        /// <summary>
        /// How much air the entity has, in ticks. Fills to a maximum of 300 in air, 
        /// giving 15 seconds submerged before the entity starts to drown, 
        /// and a total of up to 35 seconds before the entity dies (if it has 20 health). 
        /// Decreases while underwater. If 0 while underwater, the entity loses 1 health per second.
        /// </summary>
        [Nbt("Air")]
        public short Air
        {
            get { return _air; }
            set
            {
                _air = value;
                IsDirty();
            }
        }

        /// <summary>
        /// 1 or 0 (true/false) - true if the entity is touching the ground.
        /// </summary>
        [Nbt("OnGround")]
        public bool OnGround
        {
            get { return _onGround; }
            set
            {
                _onGround = value; 
                IsDirty();
            }
        }

        /// <summary>
        /// Unknown usage; entities are only saved in the region files for the dimension they are in. 
        /// -1 for The Nether, 0 for The Overworld, and 1 for The End.
        /// </summary>
        [Nbt("Dimensions")]
        public int Dimensions
        {
            get { return _dimensions; }
            set
            {
                _dimensions = value;
                IsDirty();
            }
        }

        /// <summary>
        /// 1 or 0 (true/false) - true if the entity should not take damage. 
        /// This applies to living and nonliving entities alike: mobs will not 
        /// take damage from any source (including potion effects) and objects such 
        /// as vehicles and item frames cannot be destroyed unless their supports are removed.
        /// Note that these entities also cannot be moved by fishing rods, attacks, explosions, 
        /// or projectiles.
        /// </summary>
        [Nbt("Invulnerable")]
        public byte Invulnerable
        {
            get { return _invulnerable; }
            set
            {
                _invulnerable = value; 
                IsDirty();
            }
        }

        /// <summary>
        /// The number of ticks before which the entity may be teleported 
        /// back through a portal of any kind. Initially starts at 900 ticks 
        /// (45 seconds) after teleportation and counts down to 0.
        /// </summary>
        [Nbt("PortalCooldown")]
        public int PortalCooldown
        {
            get { return _portalCooldown; }
            set
            {
                _portalCooldown = value; 
                IsDirty();
            }
        }

        /// <summary>
        /// The most significant bits of this entity's Universally Unique IDentifier. 
        /// This is joined with UUIDLeast to form this entity's unique ID.
        /// </summary>
        [Nbt("UUIDMost")]
        public long UuidMost
        {
            get { return _uuidMost; }
            set
            {
                _uuidMost = value;
                IsDirty();
            }
        }

        /// <summary>
        /// The least significant bits of this entity's Universally Unique IDentifier.
        /// </summary>
        [Nbt("UUIDLeast")]
        public long UuidLeast
        {
            get { return _uuidLeast; }
            set
            {
                _uuidLeast = value;
                IsDirty();
            }
        }

        /// <summary>
        /// The data of the entity being ridden. Note that if an entity is being ridden, 
        /// the topmost entity in the stack has the Pos tag, and the coordinates specify 
        /// the location of the bottommost entity. Also note that the bottommost entity 
        /// controls movement, while the topmost entity determines spawning conditions 
        /// when created by a mob spawner.
        /// </summary>
        [Nbt("Riding")]
        public EntityContent Riding
        {
            get { return _riding; }
            set
            {
                _riding = value; 
                IsDirty();
            }
        }
    }
}
