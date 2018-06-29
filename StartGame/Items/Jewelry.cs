using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Items
{
    internal abstract class Jewelry : Item
    {
        private bool active;
        public Buff agility;
        public Buff endurance;
        public Buff intelligence;
        public Material material;
        public Quality quality;
        public Buff strength;
        public JewelryType type;
        public Buff wisdom;

        private HumanPlayer player;

        public int Value
        {
            get
            {
                double pricePerAttribute = 1.5;
                int cost = 0;
                cost += (int)Math.Pow(strength.Value, pricePerAttribute);
                cost += (int)Math.Pow(agility.Value, pricePerAttribute);
                cost += (int)Math.Pow(endurance.Value, pricePerAttribute);
                cost += (int)Math.Pow(intelligence.Value, pricePerAttribute);
                cost += (int)Math.Pow(wisdom.Value, pricePerAttribute);
                cost += material.value;
                cost = (int)(cost * (100d - (int)quality) / 100d);
                return cost;
            }
        }

        /// <summary>
        /// Is used to set if Jewlerry effect is active, the checking if jewelry can be worn must be done by other code
        /// </summary>
        public bool Active
        {
            get => active; set
            {
                //DEBUG - ensure that player has jewlery in jewlerry list
                if (player != null && !player.troop.jewelries.Exists(j => j == this)) throw new Exception("Can not activate a jewelry on a player who does not have it!");
                active = value;
                if (Player != null)
                {
                    if (active)
                    {
                        type.number++;
                        if (type.number > type.MaxNumber) throw new Exception("The player is wearing too much jewelry of type " + type.name);
                        Player.Strength.buffs.Add(strength);
                        Player.Intelligence.buffs.Add(intelligence);
                        Player.Wisdom.buffs.Add(wisdom);
                        Player.Endurance.buffs.Add(endurance);
                        Player.Agility.buffs.Add(agility);
                    }
                    else
                    {
                        type.number--;
                        if (type.number < 0) throw new Exception("The player is less than 0 jewelry of type " + type.name);
                        Player.Strength.buffs.Remove(strength);
                        Player.Intelligence.buffs.Remove(intelligence);
                        Player.Wisdom.buffs.Remove(wisdom);
                        Player.Endurance.buffs.Remove(endurance);
                        Player.Agility.buffs.Remove(agility);
                    }
                }
            }
        }

        internal HumanPlayer Player
        {
            get => player; set
            {
                if (player is null && value != null)
                {
                    type = value.GetJewelryType(type.name);
                }
                else if (value is null)
                {
                    type = GetJewelryType(type.name);
                }
                player = value;
            }
        }

        protected Jewelry(Buff strength, Buff intelligence, Buff wisdom, Buff agility, Buff endurance, JewelryType type, bool active, string name, Quality quality, Material material) : base(name)
        {
            this.strength = strength;
            this.intelligence = intelligence;
            this.wisdom = wisdom;
            this.agility = agility;
            this.endurance = endurance;
            this.type = type;
            this.Active = active;
            this.quality = quality;
            this.material = material;
        }

        public string Description
        {
            get
            {
                string buffs = "";
                if (strength.value != 0) buffs += $" It affects your strength by {strength.ToString()}.";
                if (intelligence.value != 0) buffs += $" It affects your intelligence by {intelligence.ToString()}.";
                if (wisdom.value != 0) buffs += $" It affects your wisdom by {wisdom.ToString()}.";
                if (agility.value != 0) buffs += $" It affects your agility by {agility.ToString()}.";
                if (endurance.value != 0) buffs += $" It affects your endurance by {endurance.ToString()}.";
                return $"This is a {GetType().Name} made out of {material.name}. {buffs}. It is of {quality} quality and has a value of {Value}.";
            }
        }

        public override string ToString()
        {
            return name;
        }

        public static Jewelry New(Buff strength, Buff intelligence, Buff wisdom, Buff agility, Buff endurance, string name, Quality quality, Material material, JewelryType jewelryType)
        {
            switch (jewelryType.name)
            {
                case "Necklace":
                    return new Necklace(strength, intelligence, wisdom, agility, endurance, name, quality, material);

                case "Ring":
                    return new Ring(strength, intelligence, wisdom, agility, endurance, name, quality, material);

                case "Earring":
                    return new Earring(strength, intelligence, wisdom, agility, endurance, name, quality, material);

                default:
                    throw new NotImplementedException();
            }
        }
    }

    internal class Necklace : Jewelry
    {
        public Necklace(Buff strength, Buff intelligence, Buff wisdom, Buff agility, Buff endurance, string name, Quality quality, Material material)
        : base(strength, intelligence, wisdom, agility, endurance, GetJewelryType("Necklace"), false, name, quality, material)
        {
        }
    }

    internal class Ring : Jewelry
    {
        public Ring(Buff strength, Buff intelligence, Buff wisdom, Buff agility, Buff endurance, string name, Quality quality, Material material)
            : base(strength, intelligence, wisdom, agility, endurance, GetJewelryType("Ring"), false, name, quality, material)
        {
        }
    }

    internal class Earring : Jewelry
    {
        public Earring(Buff strength, Buff intelligence, Buff wisdom, Buff agility, Buff endurance, string name, Quality quality, Material material)
            : base(strength, intelligence, wisdom, agility, endurance, GetJewelryType("Earring"), false, name, quality, material)
        {
        }
    }

    internal class JewelryType
    {
        public string name;
        public int MaxNumber;
        public int number = 0;
        public bool Space => number != MaxNumber;

        public JewelryType(string name, int maxNumber)
        {
            this.name = name;
            MaxNumber = maxNumber;
        }

        public override bool Equals(object obj)
        {
            return obj is JewelryType type &&
                   name == type.name;
        }

        public override string ToString()
        {
            return name;
        }

        public override int GetHashCode()
        {
            var hashCode = -2071717862;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + MaxNumber.GetHashCode();
            return hashCode;
        }
    }
}