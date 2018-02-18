using System.Collections.Generic;
using System.Drawing;

namespace PlayerCreator
{
    public enum AttackType
    { melee, range, magic };

    public class Troop
    {
        public string name;
        public Weapon activeWeapon;
        public List<Weapon> weapons = new List<Weapon>();
        public Point position;
        public Bitmap image;

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

        public Troop(string Name, int Health, Weapon Weapon, Bitmap Image, int Defense, int Dodge = 10)
        {
            image = Image;
            name = Name;
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
            position = point;
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