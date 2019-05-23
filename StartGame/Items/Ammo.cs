using StartGame.Entities;
using StartGame.PlayerData;
using System;
using System.Collections.Generic;

namespace StartGame.Items
{
    public enum AmmoType { Rock, Arrow }

    public class FireArrow : Ammo
    {
        public FireArrow(int amount) : base(AmmoType.Arrow, new Buff(BuffType.Percentage, 10), "Fire Arrow", "Will set the enemy on fire", amount, 10)
        {

        }

        public override void OnUse(Player attacked, MainGameWindow main)
        {
            int turns = World.World.random.Next(1, 4);
            attacked.troop.statuses.Add(new FireStatus(turns, 4, main, attacked));

        }
    }

    public class Ammo : Item
    {
        public Dictionary<RangedWeapon,bool> Selected = new Dictionary<RangedWeapon, bool>();
        public readonly AmmoType ammoType;
        public readonly Buff damage;
        public readonly string description;
        public int Amount;
        private readonly int cost;
        readonly List<RangedWeapon> connected = new List<RangedWeapon>();

        public Ammo(AmmoType ammoType, Buff damage, string Name, string Description, int amount, int cost = 1) : base(Name)
        {
            this.ammoType = ammoType;
            this.damage = damage;
            description = Description;
            Amount = amount;
            this.cost = cost;
        }

        public override string ToString()
        {
            return $"{name} x{Amount}";
        }

        public void Select(RangedWeapon weapon)
        {
            Selected[weapon] = true; ;
            connected.Add(weapon);
            weapon.attackDamage.buffs.Add(damage);
        }

        public void Deselect(RangedWeapon weapon)
        {
            if (!Selected[weapon])
                throw new Exception();
            Selected[weapon] = false;
            weapon.attackDamage.buffs.Remove(damage);
        }

        internal static List<Ammo> GetAmmos()
        {
            List<Ammo> ammos = new List<Ammo>() {
                new Ammo(AmmoType.Rock, Buff.Zero, "Rock", "A simple rock", 1),
                new Ammo(AmmoType.Rock, new Buff(BuffType.Absolute, 1), "Big Rock", "Hurts a lot", 1),
                new Ammo(AmmoType.Arrow, Buff.Zero, "Arrow", "A basic arrow", 1, 2),
                new FireArrow(1)
            };
            return ammos;
        }

        public virtual int GetPrice()
        {
            return cost;
        }

        public virtual void OnUse(Player attacked, MainGameWindow main)
        {

        }
    }
}