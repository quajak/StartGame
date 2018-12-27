using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Items
{
    public class Resource : SellableItem
    {
        public Resource(int amount, int cost, string name) : base(cost, amount, name)
        {

        }
    }
}
