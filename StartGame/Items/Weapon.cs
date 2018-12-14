using StartGame.Extra.Loading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Items
{
    public enum BaseAttackType
    { melee, range, magic };

    public enum BaseDamageType
    {
        sharp, blunt, magic
    }

    public class Weapon : Item, ICloneable
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

        public object Clone()
        {
            return new Weapon(attackDamage, type, damageType, range, name, attacks, discardeable, attackCost);
        }

        public string RawValue()
        {
            return $"{attackDamage} {(int)type} {(int)damageType} {range} {maxAttacks} {discardeable} {attackCost} {name}";
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

        public static Weapon Fist = new Weapon(2, BaseAttackType.melee, BaseDamageType.blunt, 1, "Fist", 1, false);
    }
}