using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Items
{
    public class Resource : SellableItem
    {
        public Resource(string name, int amount, int cost) : base(name, amount, cost)
        {

        }
    }
}
