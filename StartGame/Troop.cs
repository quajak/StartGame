using StartGame;
using StartGame.Properties;
using System.Collections.Generic;
using System.Drawing;

namespace PlayerCreator
{
    public enum AttackType
    { melee, range, magic };

    internal abstract class Entity
    {
        public readonly string name;
        private Point position;
        public readonly Bitmap image;
        public readonly bool blocking;
        private Map map;

        public Point Position
        {
            get => position; set
            {
                if (Map != null && blocking)
                    Map.map[position.X, position.Y].free = true;
                position = value;
                if (Map != null && blocking)
                    Map.map[position.X, position.Y].free = false;

                //Update the position of the render entity
                if (Map != null)
                {
                    lock (Map.RenderController)
                    {
                        EntityRenderObject entity = Map.EntityRenderObjects.Find(o => o.Name == name);
                        if (entity != null)
                        {
                            //If an objects position is changed two or more times between two animations, the object will do the last animation
                            if (entity.toRender != entity.position)
                            {
                                entity.SetPosition(entity.toRender, PositionSetType.absolute);
                            }

                            entity.SetPosition(value, PositionSetType.goal);
                            //TODO: Generate a path not just set goal
                            entity.Animation = new LinearPointAnimation(entity.position, entity.toRender);
                            entity.movementAnimationPoints = (entity.Animation as LinearPointAnimation).Animate().GetEnumerator();
                        }
                    }
                }
            }
        }

        public Map Map
        {
            get => map; set
            {
                map = value;
                if (map != null && blocking)
                    map.map[position.X, position.Y].free = false;
            }
        }

        public Entity(string Name, Point Position, Bitmap Image, bool Blocking, Map map)
        {
            name = Name;
            Map = map;
            blocking = Blocking;
            this.Position = Position;
            image = Image;
        }
    }

    internal class Building : Entity
    {
        public Building(string Name, Point Position, Bitmap Image, Map map, bool Blocking = true) : base(Name, Position, Image, Blocking, map)
        {
        }
    }

    internal class Troop : Entity
    {
        public Weapon activeWeapon;
        public List<Weapon> weapons = new List<Weapon>();

        public int maxHealth;
        public int health;
        public int defense;
        public int baseDodge;
        public int dodge;

        private int weaponIndex = 0;

        public List<Status> statuses = new List<Status>();

        public int WeaponIndex
        {
            get { return weaponIndex; }
            set
            {
                weaponIndex = value;
                activeWeapon = weapons[WeaponIndex];
            }
        }

        public Troop(string Name, int Health, Weapon Weapon, Bitmap Image, int Defense, Map map, int Dodge = 10
            , Dictionary<DamageType, double> Vurneabilities = null) : base(Name, new Point(0, 0), Image, true, map)
        {
            vurneabilites = Vurneabilities ?? new Dictionary<DamageType, double>();
            maxHealth = Health;
            health = Health;
            weapons.Add(Weapon);
            activeWeapon = weapons[WeaponIndex];
            defense = Defense;
            dodge = Dodge;
            baseDodge = Dodge;
        }

        private Dictionary<DamageType, double> vurneabilites;

        public double GetVurneability(DamageType damageType)
        {
            if (vurneabilites.TryGetValue(damageType, out double vurn))
            {
                return vurn;
            }
            else
            {
                return 1;
            }
        }

        public void Spawn(Point point)
        {
            Position = point;
        }

        public void Die()
        {
            Map.map[Position.X, Position.Y].free = true;
        }
    }

    internal class Weapon : Item
    {
        public int attackDamage;
        public AttackType type;
        public int range;
        public int attacks;
        public int maxAttacks;
        public bool discardeable;
        public int attackCost;

        public Weapon(int AttackDamage, AttackType Type, int Range, string Name, int Attacks, bool Discardeable, int AttackCost = 1) : base(Name)
        {
            attackCost = AttackCost;
            discardeable = Discardeable;
            range = Range;
            type = Type;
            attackDamage = AttackDamage;
            attacks = Attacks;
            maxAttacks = Attacks;
        }
    }
}