using StartGame.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StartGame.PlayerData
{
    public class Attribute
    {
        public string name;
        private int rawValue;

        public virtual int GetRawValue()
        {
            return RawValue;
        }

        public string unit = "";
        public List<Buff> buffs = new List<Buff>();
        public int? rawMaxValue = null;

        public virtual int? MaxValue()
        {
            return !rawMaxValue.HasValue ? rawMaxValue : ApplyBonus(rawMaxValue.Value, false);
        }

        public bool allowOverflow = false;

        public virtual int Value
        {
            get
            {
                int value = GetRawValue();
                value = ApplyBonus(value);
                return value;
            }
        }

        public int RawValue
        {
            get => rawValue; set
            {
                rawValue = value;
                if(!allowOverflow && MaxValue() != null)
                {
                    int diff = ApplyBonus(rawValue, false) - (int)MaxValue();
                    if(diff > 0)
                    {
                        rawValue -= diff;
                    }

                }
            }
        }

        internal double ApplyBonus(double value, bool checkMax = true)
        {
            //Sort buffs
            buffs = buffs.OrderByDescending(b => (int)b.type).ToList();
            foreach (var buff in buffs)
            {
                switch (buff.type)
                {
                    case BuffType.Absolute:
                        value = buff.value;
                        break;

                    case BuffType.Percentage:
                        //Value will be to closest .5
                        value = Math.Round(((buff.value + 100d) / 100d * value) / 0.5) * 0.5;
                        break;

                    case BuffType.Constant:
                        value += buff.value;
                        break;

                    default:
                        break;
                }
            }
            if (!checkMax)
            {
                int? _MaxValue = MaxValue();
                if (!allowOverflow && _MaxValue != null) value = value > _MaxValue.Value ? _MaxValue.Value : value;
            }
            return value;
        }

        internal int ApplyBonus(int value, bool checkMax = true, List<Buff> _buffs = null)
        {
            if (_buffs is null) _buffs = buffs;
            //Sort buffs
            _buffs = _buffs.OrderByDescending(b => (int)b.type).ToList();
            foreach (var buff in _buffs)
            {
                switch (buff.type)
                {
                    case BuffType.Absolute:
                        value = buff.value;
                        break;

                    case BuffType.Percentage:
                        value = (int)((buff.value + 100d) / 100d * value);
                        break;

                    case BuffType.Constant:
                        value += buff.value;
                        break;

                    default:
                        break;
                }
            }
            if (checkMax)
            {
                int? _MaxValue = MaxValue();
                if (!allowOverflow && _MaxValue != null) value = value > _MaxValue.Value ? _MaxValue.Value : value;
            }
            return value;
        }

        #region ctor

        public Attribute(int rawValue, string name)
        {
            RawValue = rawValue;
            this.name = name;
        }

        public Attribute(int rawValue, List<Buff> buffs, string name)
        {
            RawValue = rawValue;
            this.buffs = buffs;
            this.name = name;
        }

        public Attribute(int rawValue, int maxValue, bool allowOverflow, string name)
        {
            RawValue = rawValue;
            rawMaxValue = maxValue;
            this.allowOverflow = allowOverflow;
            this.name = name;
        }

        public Attribute(int rawValue, int maxValue, bool allowOverflow, List<Buff> buffs, string name)
        {
            RawValue = rawValue;
            rawMaxValue = maxValue;
            this.allowOverflow = allowOverflow;
            this.buffs = buffs;
            this.name = name;
        }

        public Attribute(int rawValue, string name, string unit)
        {
            RawValue = rawValue;
            this.name = name;
            this.unit = unit;
        }

        public Attribute(int rawValue, string name, string unit, List<Buff> buffs)
        {
            RawValue = rawValue;
            this.name = name;
            this.unit = unit;
            this.buffs = buffs;
        }

        public Attribute(int rawValue, string name, string unit, List<Buff> buffs, int? maxValue, bool allowOverflow)
        {
            RawValue = rawValue;
            this.name = name;
            this.unit = unit;
            this.buffs = buffs;
            rawMaxValue = maxValue;
            this.allowOverflow = allowOverflow;
        }

        #endregion ctor

        public override string ToString()
        {
            return $"{name}: {Value}{(MaxValue() != null ? $"/{MaxValue().Value}" : "")} {unit}";
        }
    }

    public abstract class DerivedAttribute : Attribute
    {
        internal Player player;

        new public virtual int GetRawValue()
        {
            throw new NotImplementedException();
        }

        public DerivedAttribute(Player player, string name, int rawValue) : base(rawValue, name)
        {
            this.player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public DerivedAttribute(Player player, string name, int rawValue, int rawMaxValue, bool allowOverflow) : base(rawValue, rawMaxValue, allowOverflow, name)
        {
            this.player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class Health : DerivedAttribute
    {
        public Health(Player player, int bonusMaxValue) : base(player, "Health", 0, bonusMaxValue, false)
        {
            RawValue = MaxValue().Value;
        }

        public override int GetRawValue()
        {
            return RawValue; //Todo fix this
        }

        public override int Value => GetRawValue();

        public override int? MaxValue()
        {
            return player is null ? (int?)null : ApplyBonus(player.vitality.Value * 2 + rawMaxValue ?? 0, false);
        }

        public override string ToString()
        {
            return $"{name}: {Value}{(MaxValue() != null ? $"/{MaxValue().Value}" : "")}{unit}";
        }
    }

    public class Defense : DerivedAttribute
    {
        public Defense(Player player, int bonusDefense) : base(player, "Defense", bonusDefense)
        {
        }

        public override int GetRawValue()
        {
            return RawValue + player.endurance.Value / 5;
        }

        public override int Value => ApplyBonus(GetRawValue());

        public override int? MaxValue()
        {
            return null;
        }

        public override string ToString()
        {
            return $"{name}: {Value}{(MaxValue() != null ? $"/{MaxValue().Value}" : "")}{unit}";
        }
    }

    public class Dodge : DerivedAttribute
    {
        public Dodge(Player player, int bonusDodge) : base(player, "Dodge", bonusDodge)
        {
        }

        public override int GetRawValue()
        {
            return player.agility.Value * 2 + RawValue;
        }

        public override int Value => ApplyBonus(GetRawValue());

        public override string ToString()
        {
            return $"{name}: {Value}{(MaxValue() != null ? $"/{MaxValue().Value}" : "")}{unit}";
        }
    }

    public class Mana : DerivedAttribute
    {
        public Mana(Player player, int baseMaxMana = 10) : base(player, "Mana", 0, baseMaxMana, true)
        {
        }

        public override int? MaxValue()
        {
            return player is null ? (int?)null : ApplyBonus(player.wisdom.Value * 2 + rawMaxValue.Value, false);
        }

        public override int GetRawValue()
        {
            return RawValue;
        }

        public override int Value => GetRawValue();

        public override string ToString()
        {
            return $"{name}: {Value}{(MaxValue() != null ? $"/{MaxValue().Value}" : "")}{unit}";
        }
    }

    public class ActionPoint : DerivedAttribute
    {
        new public double RawValue;

        public ActionPoint(Player player, int baseActionPoints) : base(player, "Action Points", 0, baseActionPoints, true)
        {
        }

        new public double? MaxValue()
        {
            return ApplyBonus(player.endurance.Value / 10 + rawMaxValue.Value, false);
        }

        new public double Value => GetRawValue();

        new public double GetRawValue()
        {
            return RawValue;
        }

        public override string ToString()
        {
            return $"{name}: {Value}{(MaxValue() != null ? $"/{MaxValue().Value}" : "")}{unit}";
        }
    }

    public class GearWeight : DerivedAttribute
    {
        public List<Buff> maxGearWeight = new List<Buff>();
        public List<Buff> itemGearWeight = new List<Buff>();
        private OverweightStatus overweightStatus;
        private int lastMaxGearWeight;

        public GearWeight(Player player, int baseMaxGearWeight) : base(player, "Gear Weight", 0, baseMaxGearWeight, true)
        {
            lastMaxGearWeight = MaxValue().Value;
        }

        public override int GetRawValue()
        {
            if (player.troop == null) return 0;
            return ApplyBonus(player.troop.armours.Sum(a => a.active ? a.weight : 0), false, itemGearWeight);
        }

        public override int Value => GetRawValue();

        public override int? MaxValue()
        {
            if (player is null)
                return (int?)null;
            int max = ApplyBonus(player.strength.Value * 1000 + rawMaxValue.Value, false, maxGearWeight);
            if (max != lastMaxGearWeight)
            {
                int value = Value;
                if (value <= lastMaxGearWeight && value > max)
                {
                    //Player becomes overweight
                    overweightStatus = new OverweightStatus(player);
                }
                else if (value <= max && value > lastMaxGearWeight)
                {
                    //Lose overweight status
                    overweightStatus.RemoveEffect();
                    overweightStatus = null;
                }
            }
            lastMaxGearWeight = max;
            return max;
        }

        public override string ToString()
        {
            return $"{name}: {Value}{(MaxValue() != null ? $"/{MaxValue().Value}" : "")}{unit}";
        }
    }

    /// <summary>
    /// Movement Distance is used to calculate how many fields a unit can move. It is calculate from action points and extra moves a unit has.
    /// To keep track of this, Reset must be called at the beginning of each turn and whenever a unit moves.
    /// </summary>
    public class MovementPoints : DerivedAttribute
    {
        public new double RawValue;
        public double maxExtraMovement = 0;

        /// <summary>
        /// Movement Distance
        /// </summary>
        /// <param name="player">Player for which this applies</param>
        /// <param name="rawValue">Extra movements a unit has in a turn</param>
        public MovementPoints(Player player, double rawValue) : base(player, "Movement Distance", 0)
        {
            RawValue = rawValue;
            maxExtraMovement = rawValue;
        }

        public new double Value => GetRawValue() + player.actionPoints.Value;

        public override int? MaxValue()
        {
            return null;
        }

        public new double GetRawValue()
        {
            return RawValue;
        }

        public void Reset()
        {
            RawValue = ApplyBonus(maxExtraMovement);
        }

        public void MoveUnit(double cost)
        {
            double diff = cost - RawValue;
            if (diff == 0)
            {
                cost = 0;
                RawValue = 0;
            }
            else if (diff > 0)
            {
                cost -= RawValue;
                RawValue = 0;
            }
            else
            {
                RawValue -= cost;
                cost = 0;
            }
            player.actionPoints.RawValue -= cost;
            if (RawValue < 0) throw new Exception("Value can not be negative!");
        }

        public override string ToString()
        {
            return $"{name}: {Value}{(MaxValue() != null ? $"/{MaxValue().Value}" : "")}{unit}";
        }
    }

    public enum BuffType
    {
        Absolute = 0,
        Percentage = 1,
        Constant = 2
    }

    public class Buff
    {
        public readonly BuffType type;
        public int value;
        public double Value => type == BuffType.Percentage ? value / 100d : value;

        /// <summary>
        /// Value which is comparable between types. As percentage scaled by 1/5 otherwise raw value
        /// </summary>
        public int CValue => type == BuffType.Percentage ? value / 5 : value;

        public string Name { get; }
        public static Buff Zero => new Buff(BuffType.Absolute, 0);

        public Buff(BuffType type, int value, string name = "")
        {
            this.type = type;
            this.value = value;
            Name = name;
        }

        /// <summary>
        /// To be used to end sentence: This [buff] affects your [stat] by ...
        /// </summary>
        /// <returns>Does not include final .</returns>
        public override string ToString()
        {
            switch (type)
            {
                case BuffType.Constant:
                    return $"increasing it by {value} points";

                case BuffType.Percentage:
                    return $"increasing it by {value}%";

                case BuffType.Absolute:
                    return $"setting it to {value}";

                default:
                    throw new NotImplementedException();
            }
        }
    }
}