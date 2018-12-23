using StartGame.Entities;
using StartGame.Items;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StartGame.PlayerData
{
    public class Troop : Entity
    {
        public Weapon activeWeapon;
        public List<Weapon> weapons = new List<Weapon>();
        public List<Armour> armours = new List<Armour>();
        public List<Jewelry> jewelries = new List<Jewelry>();

        /// <summary>
        /// List of all items player has
        /// </summary>
        public List<Item> Items => armours.Concat<Item>(jewelries).ToList();

        public Health health;
        public Defense defense;
        public Dodge dodge;

        private int weaponIndex = 0;

        public List<Status> statuses = new List<Status>();

        public Body body;

        public int WeaponIndex
        {
            get => weaponIndex;
            set
            {
                weaponIndex = value;
                activeWeapon = weapons[WeaponIndex];
            }
        }

        public Troop(string Name, Weapon Weapon, Bitmap Image, int Defense, Map map, Player player,
            int Dodge = 10, Dictionary<DamageType, double> Vurneabilities = null)
            : base(Name, new Point(0, 0), Image, true, map)
        {
            this.player = player;
            vurneabilites = Vurneabilities ?? new Dictionary<DamageType, double>();
            health = new Health(player, 0);
            defense = new Defense(player, Defense);
            weapons.Add(Weapon);
            activeWeapon = weapons[WeaponIndex];
            dodge = new Dodge(player, Dodge);
            body = new Body();
        }

        public readonly Player player;
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

        public override string RawValue()
        {
            //type name x y blocking (all from base) [name of player]
            return base.RawValue() + " " + player.Name;
        }
    }
}