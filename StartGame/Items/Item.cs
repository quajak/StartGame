using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace StartGame.Items
{
    public abstract class Item
    {
        public static readonly List<JewelryType> JewelryTypes = new List<JewelryType>()
        {
            new JewelryType("Necklace", 2),
            new JewelryType("Ring", 10),
            new JewelryType("Earring", 4)
        };

        public static JewelryType GetJewelryType(string name)
        {
            return JewelryTypes.First(j => j.name == name);
        }

        public readonly string name;

        public Item(string Name)
        {
            name = Name;
        }

        public override string ToString()
        {
            return name;
        }
    }

    /// <summary>
    /// Coin is a item representation for money
    /// </summary>
    internal class Coin : Item
    {
        public readonly int amount;

        public Coin(int Amount) : base($"{Amount} {(Math.Abs(Amount) > 1 ? "coins" : "coin")}")
        {
            Contract.Assert(Amount != 0);

            amount = Amount;
        }
    }
}