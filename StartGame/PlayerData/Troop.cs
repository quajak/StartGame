using StartGame;
using StartGame.Items;
using StartGame.Properties;
using System.Collections.Generic;
using System.Drawing;

namespace StartGame.PlayerData
{
    internal class Troop : Entity
    {
        public Weapon activeWeapon;
        public List<Weapon> weapons = new List<Weapon>();
        public List<Armour> armours = new List<Armour>();

        public int maxHealth;
        public int health;
        public int defense;
        public int baseDodge;
        public int dodge;

        private int weaponIndex = 0;

        public List<Status> statuses = new List<Status>();

        public Body body;

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
            body = new Body();
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
}