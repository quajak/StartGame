using StartGame.Extra.Loading;
using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StartGame.Items
{
    public enum BaseAttackType
    { melee, range, magic };

    public enum BaseDamageType
    {
        sharp, blunt, magic
    }

    public class RangedWeapon : Weapon
    {
        public readonly AmmoType AmmoType;
        readonly List<Ammo> ammo = new List<Ammo>();
        /// <summary>
        /// This is readonly use AddAmmo to add a new ammo.
        /// </summary>
        
        public List<Ammo> Ammo => ammo;

        public override int Attacks()
        {
            return GetSelectedAmmo()?.Amount ?? 0;
        }

        public override int MaxAttacks()
        {
            return ammo.Sum(a => a.Amount);
        }

        public RangedWeapon(int AttackDamage, BaseDamageType damageType, int Range, string Name, int Attacks, bool Discardeable, AmmoType ammoType, int AttackCost = 1)
            : base(AttackDamage, BaseAttackType.range, damageType, Range, Name, Attacks, Discardeable, AttackCost)
        {
            AmmoType = ammoType;
            ammo.Add(new Ammo(ammoType, new Buff(BuffType.Absolute, 0), ammoType.ToString(), "", Attacks));
        }

        public void AddAmo(Ammo newAmmo)
        {
            ammo.Add(newAmmo);
            if(ammo.Count == 1)
            {
                ammo[0].Select(this);
            }
        }

        public override void UseWeapon(Player player, MainGameWindow main)
        {
            Ammo ammo1 = GetSelectedAmmo();
            ammo1.Amount--;
            if (ammo1.Amount == 0)
            {
                ammo1.Selected.Keys.ToList().ForEach(w => {
                    ammo1.Deselect(w);
                    w.ammo.Remove(ammo1);
                });
                if (ammo.Count != 0)
                {
                    ammo[0].Select(this);
                }
            }
            ammo1.OnUse(player, main);
        }

        private Ammo GetSelectedAmmo()
        {
            return ammo.Find(a =>a.Selected.ContainsKey(this) && a.Selected[this]);
        }

        internal void DeselectCurrent()
        {
            ammo.Find(a => a.Selected.ContainsKey(this) && a.Selected[this])?.Deselect(this);
        }
    }

    public class Weapon : Item, ICloneable
    {
        public PlayerData.Attribute attackDamage;
        public BaseAttackType type;
        public readonly BaseDamageType damageType;
        public int range;
        private int attacks;
        public virtual int Attacks()
        {
            return attacks;
        }

        public virtual int MaxAttacks()
        {
            return maxAttacks;
        }

        readonly int maxAttacks;
        public bool discardeable;
        public int attackCost;

        public Weapon(int AttackDamage, BaseAttackType Type, BaseDamageType damageType, int Range, string Name, int Attacks, bool Discardeable, int AttackCost = 1) : base(Name)
        {
            attackCost = AttackCost;
            discardeable = Discardeable;
            range = Range;
            type = Type;
            this.damageType = damageType;
            attackDamage = new PlayerData.Attribute(AttackDamage, "Attack Damage");
            attacks = Attacks;
            maxAttacks = Attacks;
        }

        public object Clone()
        {
            return new Weapon(attackDamage.RawValue, type, damageType, range, name, Attacks(), discardeable, attackCost);
        }

        public string RawValue()
        {
            return $"{attackDamage.RawValue} {(int)type} {(int)damageType} {range} {MaxAttacks()} {discardeable} {attackCost} {name}";
        }

        public static Weapon Load(string line)
        {
            string[] words = line.Split(' ');
            int attackDamage = words[0].GetInt();
            BaseAttackType type = (BaseAttackType)words[1].GetInt();
            BaseDamageType damageType = (BaseDamageType)words[2].GetInt();
            int range = words[3].GetInt();
            int attacks = words[4].GetInt();
            bool discardeable = words[5].GetBool();
            int cost = words[6].GetInt();
            string name = words[7];
            return new Weapon(attackDamage, type, damageType, range, name, attacks, discardeable, cost);
        }

        internal void SetAttacks(int v)
        {
            attacks = v;
        }

        public static Weapon Fist = new Weapon(2, BaseAttackType.melee, BaseDamageType.blunt, 1, "Fist", 1, false);


        public static List<Weapon> GetWeaponTypes()
        {
            return new List<Weapon>
            {
                new Weapon(6, BaseAttackType.melee, BaseDamageType.blunt, 2, "Stick", 2, true),
                new Weapon(8, BaseAttackType.melee,BaseDamageType.blunt, 1, "Large rock", 1, true),
                new RangedWeapon(4, BaseDamageType.blunt, 5, "Rock", 1, true, AmmoType.Rock),
                new Weapon(11, BaseAttackType.melee,BaseDamageType.sharp,  1, "Dagger", 1, true),
                new Weapon(12, BaseAttackType.melee,BaseDamageType.sharp, 2, "Axe", 1, true, 2),
                new Weapon(9, BaseAttackType.melee,BaseDamageType.sharp, 2, "Sword", 2, true),
                new Weapon(8, BaseAttackType.melee, BaseDamageType.sharp, 4, "Spear", 2, true),
                new Weapon(12, BaseAttackType.melee,BaseDamageType.sharp, 2, "Long sword", 2, true),
                new Weapon(7, BaseAttackType.melee, BaseDamageType.sharp, 1, "Short Sword", 4, true),
                new Weapon(15, BaseAttackType.magic,BaseDamageType.magic, 7, "Firewand", 1, true, 2),
                new RangedWeapon(11, BaseDamageType.sharp, 11, "Bow", 8, true, AmmoType.Arrow, 2),
                new Weapon(15, BaseAttackType.range,BaseDamageType.sharp, 20, "Long bow", 5, true, 3),
                new Weapon(20, BaseAttackType.range,BaseDamageType.sharp, 32, "Crossbow", 5, true, 5),
                new RangedWeapon(4, BaseDamageType.blunt, 8, "Slingshot", 7, true, AmmoType.Rock)
            };
        }
        /// <summary>
        /// Returns weapons which were improved from the base stats
        /// </summary>
        /// <param name="improveBy">Maximum number the damage can be increased</param>
        /// <returns></returns>
        public static List<Weapon> GetWeaponTypes(int improveBy)
        {
            List<Weapon> list = GetWeaponTypes();
            list.ForEach(w => w.attackDamage.RawValue += World.World.random.Next(improveBy));
            return list;
        }
        /// <summary>
        /// This function is used to handle the internal weapon stats such as attacks left
        /// </summary>
        public virtual void UseWeapon(Player player, MainGameWindow main)
        {
            attacks--;
        }

        internal static int CalculatePrice(Weapon weapon)
        {
            int price = 60;
            price *= weapon.attackDamage.RawValue;
            price /= weapon.attackCost; // This seems to be a litle too much
            price *= weapon.MaxAttacks() / 2 + 1;
            price *= (int)Math.Log(weapon.range + 2, 4);
            return price;
        }
    }
}