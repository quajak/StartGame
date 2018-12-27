using StartGame.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StartGame.World.Cities
{
    public abstract class Store : CityBuilding
    {
        public int money;
        private List<InventoryItem> items;

        public Store(List<InventoryItem> items, int money, string name, string desc) : base(++ID, name, desc,
            new List<CityBuildingAction> { new CityBuildingAction("Buy"), new CityBuildingAction("Sell") })
        {
            this.items = items;
        }

        CityView cityView;
        public override void OnAction(CityBuildingAction action, CityView cityView)
        {
            Trace.TraceInformation($"Store::OnAction {action.name}");
            this.cityView = cityView;
            if(action.name == "Buy")
            {
                cityView.actionOptionList.Visible = true;
                cityView.actionOptionList.Items.Clear();
                foreach (var item in items)
                {
                    cityView.actionOptionList.Items.Add(item);
                }
                cityView.actionOptionList.SelectedIndexChanged += ActionOptionList_SelectedIndexChanged;
            } else if(action.name == "Deselect")
            {
                cityView.actionOptionList.Visible = false;
                cityView.button1.Visible = false;
                cityView.button1.MouseClick -= Button1_MouseClickSell;

                cityView.button1.MouseClick -= Button1_MouseClick;
                cityView.button1.MouseClick -= Button1_MouseClickSell;
                cityView.actionOptionList.SelectedIndexChanged -= ActionOptionList_SelectedIndexChanged;
                cityView.actionOptionList.SelectedIndexChanged -= ActionOptionList_SelectedIndexChangedSell;
            } else if(action.name == "Sell")
            {
                cityView.actionOptionList.Visible = true;
                cityView.actionOptionList.Items.Clear();
                foreach (var item in cityView.player.troop.Items)
                {
                    cityView.actionOptionList.Items.Add(item);
                }
                cityView.actionOptionList.SelectedIndexChanged += ActionOptionList_SelectedIndexChangedSell;
            }
        }

        private void ActionOptionList_SelectedIndexChangedSell(object sender, System.EventArgs e)
        {
            Trace.TraceInformation("Store::ActionOptionList_SelectedIndexChangedSell");
            int selected = cityView.actionOptionList.SelectedIndex;
            if (selected == -1)
            {
                Trace.TraceInformation("Removing S");
                //Deselect
                cityView.actionOptionLabel.Visible = false;
                cityView.button1.Visible = false;
                cityView.button1.MouseClick -= Button1_MouseClickSell;
                cityView.button2.Visible = false;
            }
            else
            { 
                cityView.actionOptionLabel.Visible = true;
                Item item = cityView.actionOptionList.SelectedItem as Item;
                cityView.actionOptionLabel.Text = item.Description;
                if (GetPrice(item) <= money)
                {
                    Trace.TraceInformation("Adding S");
                    cityView.button1.Visible = true;
                    cityView.button1.Text = "Sell";
                    cityView.button1.MouseClick -= Button1_MouseClickSell;
                    cityView.button1.MouseClick += Button1_MouseClickSell;
                }
                else
                {
                Trace.TraceInformation("Removing S");
                    cityView.button1.Visible = false;
                    cityView.button1.MouseClick -= Button1_MouseClickSell;
                }
            }
        }

        private void Button1_MouseClickSell(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Trace.TraceInformation("Store::Button1_MouseClickSell");

            Item item = cityView.actionOptionList.SelectedItem as Item;
            if(item is null)
            {
                return;
            }

            cityView.actionOptionList.Items.Clear();
            foreach (var pitem in cityView.player.troop.Items)
            {
                cityView.actionOptionList.Items.Add(pitem);
            }
            bool add = false;
            for (int i1 = 0; i1 < items.Count; i1++)
            {
                InventoryItem i = items[i1];
                if (i.item == item)
                    i.amount++;
                else
                    add = true;
            }
            switch (item)
            {
                case Jewelry j:
                    cityView.player.money.rawValue += j.Value;
                    cityView.player.troop.jewelries.Remove(j);
                    if(add)
                        items.Add(new InventoryItem(item, 1, j.Value));
                    break;
                case Armour a:
            cityView.player.money.rawValue += a.Value;
                    cityView.player.troop.armours.Remove(a);
                    if (add)
                        items.Add(new InventoryItem(item, 1, a.Value));
                    break;
                case Weapon w:
                    cityView.player.troop.weapons.Remove(w);
                    throw new NotImplementedException();
                case Item i:
                    int price = Nation.prices.Find(p => p.name == i.name).cost;
                    cityView.player.troop.items.Remove(i);
                    cityView.player.money.rawValue += price;
                    if (add)
                        items.Add(new InventoryItem(item, 1, price));
                    break;
            }
        }

        public int GetPrice(Item item)
        {
            Trace.TraceInformation("Store::GetPrice");

            switch (item)
            {
                case Jewelry j:
                    return j.Value;
                case Armour a:
                    return a.Value;
                case Weapon w:
                    throw new NotImplementedException();
                case Item i:
                    return Nation.prices.Find(p => p.name == i.name).cost;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ActionOptionList_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Trace.TraceInformation("Store::ActionOptionList_SelectedIndexChanged");
            int selected = cityView.actionOptionList.SelectedIndex;
            if(selected == -1)
            {
                //Deselect
                Trace.TraceInformation("Removing");
                cityView.actionOptionLabel.Visible = false;
                cityView.button1.Visible = false;
                cityView.button1.MouseClick -= Button1_MouseClick;
                cityView.button2.Visible = false;
            }
            else
            {
                cityView.actionOptionLabel.Visible = true;
                Item item = items[selected].item;
                cityView.actionOptionLabel.Text = item.Description;
                if(item is Jewelry j)
                    cityView.actionOptionLabel.Text = j.Description;
                if(items[selected].cost <= cityView.player.money.Value)
                {
                    Trace.TraceInformation("Adding");
                    cityView.button1.Visible = true;
                    cityView.button1.Text = "Buy";
                    cityView.button1.MouseClick -= Button1_MouseClick;
                    cityView.button1.MouseClick += Button1_MouseClick;
                    money += items[selected].cost;
                    cityView.player.money.rawValue -= money;
                }
                else
                {
                Trace.TraceInformation("Removing");
                    cityView.button1.Visible = false;
                    cityView.button1.MouseClick -= Button1_MouseClick;
                }
            }
        }

        private void Button1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Trace.TraceInformation("Store::Button1_MouseClick");
            int selected = cityView.actionOptionList.SelectedIndex;
            if (selected == -1)
                return;
            InventoryItem inventoryItem = items[selected];
            inventoryItem.amount--;
            if (inventoryItem.amount == 0)
            {
                items.Remove(inventoryItem);
                selected--;
                selected = selected.Cut(0, 100);
            }
            cityView.actionOptionList.Items.Clear();
            foreach (var item in items)
            {
                cityView.actionOptionList.Items.Add(item);
            }
            if(items.Count != 0)
                cityView.actionOptionList.SetSelected(selected, true);
            cityView.player.money.rawValue -= inventoryItem.cost;
            switch (inventoryItem.item)
            {
                case Jewelry j:
                    cityView.player.troop.jewelries.Add(j);
                    break;
                case Armour a:
                    cityView.player.troop.armours.Add(a);
                    break;
                case Weapon w:
                    cityView.player.troop.weapons.Add(w);
                    break;
                case Item i:
                    cityView.player.troop.items.Add(i);
                    break;
            }
            cityView.playerView.Render();
        }
    }

    public class ConvenienceStore : Store
    {
        ConvenienceStore(List<InventoryItem> items, int money) : base(items, money, "Convenience Store", "A small store selling basic items")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">From 1 to 100 determins the wealth of the shop. 1 - 20 Poor 20 - 60 Medium 60+ Rich</param>
        /// <returns></returns>
        public static ConvenienceStore GenerateStore(int value)
        {
            List<SellableItem> items = new List<SellableItem> {
                new Food("Bread", 1, 4),
                new Food("Carrot", 1, 2),
                new Food("Sausage", 1, 8),
                new Food("Apple", 1, 2),
                new Resource(1, 1, "Grain"),
                new Resource(1, 10, "Iron"),
                new Resource(1, 30 ,"Gold"),
                new Resource(1, 5, "Wood"),
                new Resource(1, 3, "Stone")
            };
            items = items.Where(i => i.cost < value / 2).ToList();

            List<InventoryItem> sold = new List<InventoryItem>();
            for (int i = 0; i < value / 10; i++)
            {
                SellableItem item = items[World.random.Next(items.Count)];
                item.amount = World.random.Next(1, value / 10 * 2);
                sold.Add(new InventorySellableItem(item));
            }

            return new ConvenienceStore(sold, World.random.Next(value / 2, 4 * value));
        }
    }

    public class JewleryStore : Store
    {
        JewleryStore(List<InventoryItem> items, int money) : base(items, money, "Jewlery Store", " A store where you can buy jewelry")
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">From 1 to 100 determins the wealth of the shop. 1 - 20 Poor 20 - 60 Medium 60+ Rich</param>
        /// <returns></returns>
        public static JewleryStore GenerateStore(int value)
        {
            List<InventoryItem> sold = new List<InventoryItem>();
            for (int i = 0; i < value / 10; i++)
            {
                Jewelry item = Jewelry.GenerateJewelry(E.GetQuality(World.random.Next((value / 10).Cut(0, 8))));
                sold.Add(new InventoryItem(item, 1, item.Value));
            }

            return new JewleryStore(sold, World.random.Next(value / 2, 4 * value));
        }
    }

    public class InventoryItem
    {
        public Item item;
        public int amount;
        public int cost;

        public InventoryItem(Item item, int amount, int cost)
        {
            this.item = item;
            this.amount = amount;
            this.cost = cost;
        }

        public override string ToString()
        {
            return $"{item.name} {amount} {cost} coins";
        }
    }

    public class InventorySellableItem : InventoryItem
    {
        public new int amount { get => (item as SellableItem).amount; set => (item as SellableItem).amount = value; }
        public new int cost { get => (item as SellableItem).cost; set => (item as SellableItem).cost = value; }
        public InventorySellableItem(SellableItem item) : base(item, item.amount, item.cost)
        {

        }
    }
}
