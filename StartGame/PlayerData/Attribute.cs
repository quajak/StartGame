using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.PlayerData
{
    internal class Attribute
    {
        public string name;
        public int rawValue;
        public string unit = "";
        public List<Buff> buffs = new List<Buff>();
        public int? maxValue = null;
        public bool allowOverflow = false;

        public int Value
        {
            get
            {
                int value = rawValue;
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
                            value = (int)((buff.value + 100d) / 100d * value);
                            break;

                        case BuffType.Constant:
                            value += buff.value;
                            break;

                        default:
                            break;
                    }
                }
                if (!allowOverflow && maxValue != null) value = value > maxValue.Value ? maxValue.Value : value;

                return value;
            }
        }

        public Attribute(int rawValue, string name)
        {
            this.rawValue = rawValue;
            this.name = name;
        }

        public Attribute(int rawValue, List<Buff> buffs, string name)
        {
            this.rawValue = rawValue;
            this.buffs = buffs;
            this.name = name;
        }

        public Attribute(int rawValue, int maxValue, bool allowOverflow, string name)
        {
            this.rawValue = rawValue;
            this.maxValue = maxValue;
            this.allowOverflow = allowOverflow;
            this.name = name;
        }

        public Attribute(int rawValue, int maxValue, bool allowOverflow, List<Buff> buffs, string name)
        {
            this.rawValue = rawValue;
            this.maxValue = maxValue;
            this.allowOverflow = allowOverflow;
            this.buffs = buffs;
            this.name = name;
        }

        public Attribute(string name, int rawValue, string unit)
        {
            this.name = name;
            this.rawValue = rawValue;
            this.unit = unit;
        }

        public Attribute(string name, int rawValue, string unit, List<Buff> buffs)
        {
            this.name = name;
            this.rawValue = rawValue;
            this.unit = unit;
            this.buffs = buffs;
        }

        public Attribute(string name, int rawValue, string unit, List<Buff> buffs, int? maxValue, bool allowOverflow)
        {
            this.name = name;
            this.rawValue = rawValue;
            this.unit = unit;
            this.buffs = buffs;
            this.maxValue = maxValue;
            this.allowOverflow = allowOverflow;
        }

        public override string ToString()
        {
            return $"{name}: {Value}{(maxValue != null ? $"/{maxValue.Value}" : "")}{unit}";
        }
    }

    internal enum BuffType
    {
        Absolute = 0,
        Percentage = 1,
        Constant = 2
    }

    internal class Buff
    {
        public readonly BuffType type;
        public readonly int value;
        public double Value => type == BuffType.Percentage ? value / 100d : value;

        public Buff(BuffType type, int value)
        {
            this.type = type;
            this.value = value;
        }

        /// <summary>
        /// To be used to end sentence: This [buff] affects your [stat] by ...
        /// </summary>
        /// <returns>Does not include final .</returns>
        public override string ToString()
        {
            switch (type)
            {
                case BuffType.Absolute:
                    return $"increasing it by {value} points";

                case BuffType.Percentage:
                    return $"increasing it by {value}%";

                case BuffType.Constant:
                    return $"setting it to {value}";

                default:
                    throw new NotImplementedException();
            }
        }
    }
}