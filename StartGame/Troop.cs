using StartGame;
using System.Collections.Generic;
using System.Drawing;

namespace PlayerCreator
{
    public enum AttackType
    { melee, range, magic };

    public abstract class Entity
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
                if (Map != null)
                    Map.map[position.X, position.Y].free = true;
                position = value;
                if (Map != null)
                    Map.map[position.X, position.Y].free = false;
            }
        }

        public Map Map
        {
            get => map; set
            {
                map = value;
                if (map != null)
                    map.map[position.X, position.Y].free = false;
            }
        }

        public Entity(string Name, Point Position, Bitmap Image, bool Blocking, Map map)
        {
            name = Name;
            this.Position = Position;
            image = Image;
            blocking = Blocking;
            Map = map;
        }
    }

    public class Building : Entity
    {
        public Building(string Name, Point Position, Bitmap Image, Map map, bool Blocking = true) : base(Name, Position, Image, Blocking, map)
        {
        }
    }

    public class Troop : Entity
    {
        public Weapon activeWeapon;
        public List<Weapon> weapons = new List<Weapon>();

        public int maxHealth;
        public int health;
        public int defense;
        public int baseDodge;
        public int dodge;

        private int weaponIndex = 0;

        public int WeaponIndex
        {
            get { return weaponIndex; }
            set
            {
                weaponIndex = value;
                activeWeapon = weapons[WeaponIndex];
            }
        }

        public Troop(string Name, int Health, Weapon Weapon, Bitmap Image, int Defense, Map map, int Dodge = 10) : base(Name, new Point(0, 0), Image, true, map)
        {
            maxHealth = Health;
            health = Health;
            weapons.Add(Weapon);
            activeWeapon = weapons[WeaponIndex];
            defense = Defense;
            dodge = Dodge;
            baseDodge = Dodge;
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

    public class Weapon
    {
        public int attackDamage;
        public AttackType type;
        public int range;
        public string name;
        public int attacks;
        public int maxAttacks;
        public bool discardeable;
        public int attackCost;

        public Weapon(int AttackDamage, AttackType Type, int Range, string Name, int Attacks, bool Discardeable, int AttackCost = 1)
        {
            attackCost = AttackCost;
            discardeable = Discardeable;
            name = Name;
            range = Range;
            type = Type;
            attackDamage = AttackDamage;
            attacks = Attacks;
            maxAttacks = Attacks;
        }
    }
}