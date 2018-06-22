using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Items
{
    internal abstract class Item
    {
        public readonly string name;

        public Item(string Name)
        {
            name = Name;
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