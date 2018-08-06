using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Items
{
    public abstract class Jewelry : Item
    {
        private bool active;
        public Buff agility;
        public Buff endurance;
        public Buff vitality;
        public Buff intelligence;
        public Buff defense;
        public Buff health;
        public Buff movementDistance;
        public Buff dodge;
        public Buff mana;
        public Buff carryCapacity;
        public Buff weight;
        public Buff wisdom;
        public Buff strength;
        public Material material;
        public Quality quality;
        public JewelryType type;

        private Player player;

        public int Value
        {
            get
            {
                double pricePerAttribute = 1.5;
                int cost = 0;
                cost += (int)Math.Pow(strength.CValue, pricePerAttribute);
                cost += (int)Math.Pow(agility.CValue, pricePerAttribute);
                cost += (int)Math.Pow(endurance.CValue, pricePerAttribute);
                cost += (int)Math.Pow(intelligence.CValue, pricePerAttribute);
                cost += (int)Math.Pow(wisdom.CValue, pricePerAttribute);

                cost += (int)Math.Pow(defense.CValue, pricePerAttribute);
                cost += (int)Math.Pow(health.CValue, pricePerAttribute);
                cost += (int)Math.Pow(movementDistance.CValue, pricePerAttribute);
                cost += (int)Math.Pow(dodge.CValue, pricePerAttribute);
                cost += (int)Math.Pow(mana.CValue, pricePerAttribute);
                cost += (int)Math.Pow(carryCapacity.CValue, pricePerAttribute);
                cost += (int)Math.Pow(weight.CValue, pricePerAttribute);

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
                        Player.strength.buffs.Add(strength);
                        Player.intelligence.buffs.Add(intelligence);
                        Player.wisdom.buffs.Add(wisdom);
                        Player.endurance.buffs.Add(endurance);
                        Player.agility.buffs.Add(agility);
                        Player.vitality.buffs.Add(vitality);

                        Player.troop.defense.buffs.Add(defense);
                        Player.troop.health.buffs.Add(health);
                        Player.movementPoints.buffs.Add(movementDistance);
                        Player.troop.dodge.buffs.Add(dodge);
                        Player.mana.buffs.Add(mana);
                        Player.gearWeight.maxGearWeight.Add(carryCapacity);
                        Player.gearWeight.itemGearWeight.Add(weight);
                    }
                    else
                    {
                        type.number--;
                        if (type.number < 0) throw new Exception("The player is less than 0 jewelry of type " + type.name);
                        Player.strength.buffs.Remove(strength);
                        Player.intelligence.buffs.Remove(intelligence);
                        Player.wisdom.buffs.Remove(wisdom);
                        Player.endurance.buffs.Remove(endurance);
                        Player.agility.buffs.Remove(agility);
                        Player.vitality.buffs.Remove(vitality);

                        Player.troop.defense.buffs.Remove(defense);
                        Player.troop.health.buffs.Remove(health);
                        Player.movementPoints.buffs.Remove(movementDistance);
                        Player.troop.dodge.buffs.Remove(dodge);
                        Player.mana.buffs.Remove(mana);
                        Player.gearWeight.maxGearWeight.Remove(carryCapacity);
                        Player.gearWeight.itemGearWeight.Remove(weight);
                    }
                }
            }
        }

        internal Player Player
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

        protected Jewelry(Buff strength, Buff intelligence, Buff wisdom, Buff agility, Buff endurance, Buff vitality, Buff defense, Buff health, Buff movementDistance,
            Buff dodge, Buff mana, Buff carryCapacity, Buff weight, JewelryType type, bool active, string name, Quality quality, Material material) : base(name)
        {
            this.strength = strength;
            this.intelligence = intelligence;
            this.wisdom = wisdom;
            this.agility = agility;
            this.endurance = endurance;
            this.vitality = vitality;
            this.defense = defense;
            this.health = health;
            this.movementDistance = movementDistance;
            this.dodge = dodge;
            this.mana = mana;
            this.carryCapacity = carryCapacity;
            this.weight = weight;
            this.type = type;
            Active = active;
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
                if (vitality.value != 0) buffs += $" It affects your vitality by {vitality.ToString()}.";

                if (defense.value != 0) buffs += $" It affects your defense by {defense.ToString()}.";
                if (health.value != 0) buffs += $" It affects your health by {health.ToString()}.";
                if (movementDistance.value != 0) buffs += $" It affects your movement distance by {movementDistance.ToString()}.";
                if (dodge.value != 0) buffs += $" It affects your dodge by {dodge.ToString()}.";
                if (mana.value != 0) buffs += $" It affects your mana by {mana.ToString()}.";
                if (carryCapacity.value != 0) buffs += $" It affects your carry capacity by {carryCapacity.ToString()}.";
                if (weight.value != 0) buffs += $" It affects your item weight by {weight.ToString()}.";

                return $"This is a {GetType().Name} made out of {material.name}. {buffs} It is of {quality} quality and has a value of {Value}.";
            }
        }

        public override string ToString()
        {
            return name;
        }

        public static Jewelry New(Buff strength, Buff intelligence, Buff wisdom, Buff agility, Buff endurance, Buff vitality, Buff defense, Buff health, Buff movementDistance, Buff dodge, Buff mana, Buff carryCapacity, Buff weight, string name, Quality quality, Material material, JewelryType jewelryType)
        {
            switch (jewelryType.name)
            {
                case "Necklace":
                    return new Necklace(strength, intelligence, wisdom, agility, endurance, vitality, defense, health,
                        movementDistance, dodge, mana, carryCapacity, weight, name, quality, material);

                case "Ring":
                    return new Ring(strength, intelligence, wisdom, agility, endurance, vitality, defense, health,
                        movementDistance, dodge, mana, carryCapacity, weight, name, quality, material);

                case "Earring":
                    return new Earring(strength, intelligence, wisdom, agility, endurance, vitality, defense, health,
                        movementDistance, dodge, mana, carryCapacity, weight, name, quality, material);

                default:
                    throw new NotImplementedException();
            }
        }

        private static Random random = new Random();

        public static Jewelry GenerateJewelry(Quality quality)
        {
            //Bonus Points determine how many buffs it has and how many points it has together
            Dictionary<Quality, (int, int)> BonusPoints = new Dictionary<Quality, (int, int)>()
            {
                {Quality.Broken, (1, 2) },
                {Quality.Poor, (1, 4) },
                {Quality.Simple, (1, 6) },
                {Quality.Common, (2, 9) },
                {Quality.Good, (2, 12) },
                {Quality.Superior, (3, 15) },
                {Quality.Exceptional, (3, 19) },
                {Quality.Legendary, (4, 25) }
            };
            (int buffNumber, int totalPoints) = BonusPoints[quality];
            string[] names = new string[13] {   "Strength", "Intelligence", "Wisdom", "Agility", "Endurance", "Vitality",
                                                "Defense", "Health", "MovementDistance", "Dodge", "Mana", "CarryCapacity",
                                                "Weight" };
            Buff[] buffs = new Buff[13];

            List<int> positions = Enumerable.Range(0, 5).ToList();

            for (int i = 0; i < buffNumber; i++)
            {
                int position = positions[random.Next(positions.Count)];
                positions.Remove(position);
                int points = buffNumber - i == 1 ? totalPoints : random.Next(totalPoints);
                if (names[i] == "Weight") points *= -1;
                totalPoints -= points;
                //0 is absolute, this is not used in this case
                BuffType buffType = (BuffType)random.Next(1, 3);
                buffs[position] = new Buff(buffType, points * (buffType == BuffType.Percentage ? 5 : 1));
            }
            for (int i = 0; i < buffs.Length; i++)
            {
                buffs[i] = buffs[i] is null ? new Buff(BuffType.Constant, 0) : buffs[i];
            }
            //Buffs are now created now decide material and type
            Material material = Material.Materials.Where(m => m.armourLayers.Exists(l => l == ArmourLayer.jewelry)).OrderBy(x => random.Next()).First();
            JewelryType jewelryType = JewelryTypes.OrderBy(_ => random.Next()).First();

            //Generate name
            //TODO: Improve the name generation
            Buff strongestBuff = buffs.ToList().OrderByDescending(b => b.type == BuffType.Percentage ? b.value / 5 : b.value).First();
            int index = buffs.ToList().IndexOf(strongestBuff);
            string buffName = names[index];
            string name = $"{jewelryType.name} of {buffName}";

            return New(buffs[0], buffs[1], buffs[2], buffs[3], buffs[4], buffs[5], buffs[6], buffs[7], buffs[8],
                buffs[9], buffs[10], buffs[11], buffs[12], name, quality, material, jewelryType);
        }
    }

    internal class Necklace : Jewelry
    {
        public Necklace(Buff strength, Buff intelligence, Buff wisdom, Buff agility, Buff endurance, Buff vitality, Buff defense, Buff health, Buff movementDistance,
            Buff dodge, Buff mana, Buff carryCapacity, Buff weight, string name, Quality quality, Material material)
        : base(strength, intelligence, wisdom, agility, endurance, vitality, defense, health, movementDistance, dodge, mana, carryCapacity, weight,
              GetJewelryType("Necklace"), false, name, quality, material)
        {
        }
    }

    internal class Ring : Jewelry
    {
        public Ring(Buff strength, Buff intelligence, Buff wisdom, Buff agility, Buff endurance, Buff vitality, Buff defense, Buff health, Buff movementDistance,
            Buff dodge, Buff mana, Buff carryCapacity, Buff weight, string name, Quality quality, Material material)
            : base(strength, intelligence, wisdom, agility, endurance, vitality, defense, health, movementDistance, dodge, mana, carryCapacity, weight,
                  GetJewelryType("Ring"), false, name, quality, material)
        {
        }
    }

    internal class Earring : Jewelry
    {
        public Earring(Buff strength, Buff intelligence, Buff wisdom, Buff agility, Buff endurance, Buff vitality, Buff defense, Buff health, Buff movementDistance,
            Buff dodge, Buff mana, Buff carryCapacity, Buff weight, string name, Quality quality, Material material)
            : base(strength, intelligence, wisdom, agility, endurance, vitality, defense, health, movementDistance, dodge, mana, carryCapacity, weight,
                  GetJewelryType("Earring"), false, name, quality, material)
        {
        }
    }

    public class JewelryType
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
            int hashCode = -2071717862;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + MaxNumber.GetHashCode();
            return hashCode;
        }
    }
}