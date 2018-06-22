using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Items
{
    public enum BaseAttackType
    { melee, range, magic };

    internal enum BaseDamageType
    {
        sharp, blunt, magic
    }

    internal class Weapon : Item
    {
        public int attackDamage;
        public BaseAttackType type;
        public readonly BaseDamageType damageType;
        public int range;
        public int attacks;
        public int maxAttacks;
        public bool discardeable;
        public int attackCost;

        public Weapon(int AttackDamage, BaseAttackType Type, BaseDamageType damageType, int Range, string Name, int Attacks, bool Discardeable, int AttackCost = 1) : base(Name)
        {
            attackCost = AttackCost;
            discardeable = Discardeable;
            range = Range;
            type = Type;
            this.damageType = damageType;
            attackDamage = AttackDamage;
            attacks = Attacks;
            maxAttacks = Attacks;
        }
    }
}