using StartGame.Items;
using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StartGame.World.Cities
{
    public abstract class Store : CityBuilding
    {
        public static int RequiredWealth()
        {
            throw new NotImplementedException();
        }

        public int money;
        private readonly City city;

        public Store(List<InventoryItem> items, int money, string name, string desc, City city) : base(++ID, name, desc,
            new List<CityBuildingAction> { new CityBuildingAction("Buy"), new CityBuildingAction("Sell") })
        {
            Items = items;
            this.money = money;
            this.city = city;
        }

        CityView cityView;

        public List<InventoryItem> Items { get; private set; }

        public override void OnAction(CityBuildingAction action, CityView cityView)
        {
            Trace.TraceInformation($"Store::OnAction {action.name}");
            this.cityView = cityView;
            if (action.name == "Buy")
            {
                cityView.actionOptionList.Visible = true;
                cityView.actionOptionList.Items.Clear();
                foreach (var item in Items)
                {
                    cityView.actionOptionList.Items.Add(item);
                }
                cityView.actionOptionList.SelectedIndexChanged += ActionOptionList_SelectedIndexChanged;
            }
            else if (action.name == "Deselect")
            {
                cityView.actionOptionList.Visible = false;
                cityView.button1.Visible = false;
                cityView.button1.MouseClick -= Button1_MouseClickSell;

                cityView.button1.MouseClick -= Buy_MouseClick;
                cityView.button1.MouseClick -= Button1_MouseClickSell;
                cityView.actionOptionList.SelectedIndexChanged -= ActionOptionList_SelectedIndexChanged;
                cityView.actionOptionList.SelectedIndexChanged -= ActionOptionList_SelectedIndexChangedSell;
            }
            else if (action.name == "Sell")
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

        private void ActionOptionList_SelectedIndexChangedSell(object sender, EventArgs e)
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
                if (GetSellPrice(item) <= money)
                {
                    Trace.TraceInformation("Adding S");
                    cityView.button1.Visible = true;
                    cityView.button1.Text = "Sell";
                    cityView.actionOptionLabel.Text = $"{GetSellPrice(item)} Coins";
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
            if (item is null)
            {
                return;
            }
            int selected = cityView.actionOptionList.SelectedIndex;
            cityView.actionOptionList.Items.Clear();
            bool add = false;
            for (int i1 = 0; i1 < Items.Count; i1++)
            {
                InventoryItem i = Items[i1];
                if (i.item == item)
                    i.Amount++;
                else
                    add = true;
            }
            int sellPrice = GetSellPrice(item);
            int price = GetBuyPrice(item);
            switch (item)
            {
                case Jewelry j:
                    cityView.player.Money.RawValue += sellPrice;
                    cityView.player.troop.jewelries.Remove(j);
                    if (add)
                        Items.Add(new InventoryItem(item, 1, price));
                    break;
                case Armour a:
                    cityView.player.Money.RawValue += sellPrice;
                    cityView.player.troop.armours.Remove(a);
                    if (add)
                        Items.Add(new InventoryItem(item, 1, price));
                    break;
                case Weapon w:
                    cityView.player.troop.weapons.Remove(w);
                    throw new NotImplementedException();
                case Food f:
                    f.amount--;
                    if(f.amount == 0)
                        cityView.player.troop.items.Remove(f);
                    cityView.player.Money.RawValue += sellPrice;
                    if (add)
                    {
                        if (Items.TryGet(i => i.item is Food fo && fo.name == f.name, out InventoryItem it))
                        {
                            it.Amount++;
                        }
                        else
                        {
                            Items.Add(new InventoryItem(item, 1, price));
                        }
                    }
                    break;
                case Item i:
                    cityView.player.troop.items.Remove(i);
                    cityView.player.Money.RawValue += sellPrice;
                    if (add)
                        Items.Add(new InventoryItem(item, 1, price));
                    break;
            }
            foreach (var pitem in cityView.player.troop.Items)
            {
                cityView.actionOptionList.Items.Add(pitem);
            }
            if (selected == cityView.player.troop.Items.Count)
            {
                selected--;
            }
            if (cityView.player.troop.Items.Count != 0)
            {
                cityView.actionOptionList.SetSelected(selected, true);
            }
            cityView.playerView.Render();
        }

        public int GetBuyPrice(Item item)
        {
            return GetBuyPrice(item, city);
        }

        public static int GetBuyPrice(Item item, City city)
        {
            switch (item)
            {
                case Jewelry j:
                    return j.Value;
                case Armour a:
                    return a.Value;
                case Weapon w:
                    throw new NotImplementedException();
                case Ammo a:
                    return (a.GetPrice()).Cut(1, 10000);
                case Food f:
                    return (city.DetermineLocalFoodPrice(f)).Cut(1, 10000);
                case Item i:
                    return (Nation.prices.Find(p => p.name == i.name).cost).Cut(1, 10000);
                default:
                    throw new NotImplementedException();
            }
        }

        public int GetSellPrice(Item item)
        {
            switch (item)
            {
                case Jewelry j:
                    return j.Value / 2 + 1;
                case Armour a:
                    return a.Value / 2 + 1;
                case Weapon w:
                    throw new NotImplementedException();
                case Ammo a:
                    return (a.GetPrice() - 3).Cut(1, 100000);
                case Food f:
                    return (city.DetermineLocalFoodPrice(f) - 3).Cut(1, 10000);
                case Item i:
                    return (Nation.prices.Find(p => p.name == i.name).cost - 3).Cut(1, 100000);
                default:
                    throw new NotImplementedException();
            }
        }

        private void ActionOptionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Trace.TraceInformation("Store::ActionOptionList_SelectedIndexChanged");
            int selected = cityView.actionOptionList.SelectedIndex;
            if (selected == -1)
            {
                //Deselect
                Trace.TraceInformation("Removing");
                cityView.actionOptionLabel.Visible = false;
                cityView.button1.Visible = false;
                cityView.button1.MouseClick -= Buy_MouseClick;
                cityView.button2.Visible = false;
            }
            else
            {
                cityView.actionOptionLabel.Visible = true;
                Item item = Items[selected].item;
                cityView.actionOptionLabel.Text = item.Description;
                if (item is Jewelry j)
                    cityView.actionOptionLabel.Text = j.Description;
                else if (item is Armour a)
                    cityView.actionOptionLabel.Text = a.Description;
                else if (item is Weapon w)
                    cityView.actionOptionLabel.Text = w.Description;
                if (Items[selected].Cost <= cityView.player.Money.Value)
                {
                    Trace.TraceInformation("Adding");
                    cityView.button1.Visible = true;
                    cityView.button1.Text = "Buy";
                    cityView.button1.MouseClick -= Buy_MouseClick;
                    cityView.button1.MouseClick += Buy_MouseClick;
                }
                else
                {
                    Trace.TraceInformation("Removing");
                    cityView.button1.Visible = false;
                    cityView.button1.MouseClick -= Buy_MouseClick;
                }
            }
        }

        private void Buy_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Trace.TraceInformation("Store::Buy_MouseClick");
            int selected = cityView.actionOptionList.SelectedIndex;
            if (selected == -1)
                return;
            InventoryItem inventoryItem = Items[selected];
            int c = Items.Count;
            Items = BuyItem(inventoryItem, Items, cityView.player, this);
            cityView.actionOptionList.Items.Clear();
            foreach (var item in Items)
            {
                cityView.actionOptionList.Items.Add(item);
            }
            if (c != Items.Count)
            {
                selected--;
                selected = selected.Cut(0, 100);
            }
            if (Items.Count != 0)
            {
                cityView.actionOptionList.SetSelected(selected, true);
            }
            cityView.playerView.Render();
        }

        public static List<InventoryItem> BuyItem(InventoryItem inventoryItem, List<InventoryItem> items, HumanPlayer player, Store store)
        {
            inventoryItem.Amount--;
            if (inventoryItem.Amount == 0)
            {
                items.Remove(inventoryItem);
            }
            store.money += inventoryItem.Cost;
            player.Money.RawValue -= inventoryItem.Cost;
            switch (inventoryItem.item)
            {
                case Jewelry j:
                    player.troop.jewelries.Add(j);
                    break;
                case Armour a:
                    player.troop.armours.Add(a);
                    break;
                case Weapon w:
                    player.troop.weapons.Add(w);
                    break;
                case Ammo a:
                    if (player.troop.items.Exists(i => i.name == a.name))
                    {
                        (player.troop.items.Find(i => i.name == a.name) as Ammo).Amount++;
                    }
                    else
                    {
                        player.troop.items.Add(a);
                    }
                    break;
                case Resource r:
                    if (player.troop.items.Exists(it => it.name == r.name))
                    {
                        (player.troop.items.Find(it => it.name == r.name) as Resource).amount++;
                    }
                    else
                    {
                        player.troop.items.Add(new Resource(r.name, 1, r.cost));
                    }
                    break;
                case Item i:
                    player.troop.items.Add(i);
                    break;
            }
            return items;
        }

    }

    public class ConvenienceStore : Store
    {

        public static new int RequiredWealth()
        {
            return 5;
        }
        ConvenienceStore(List<InventoryItem> items, int money, City city) : base(items, money, "Convenience Store", "A small store selling basic items", city)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">From 1 to 100 determins the wealth of the shop. 1 - 20 Poor 20 - 60 Medium 60+ Rich</param>
        /// <returns></returns>
        public static ConvenienceStore GenerateStore(int value, City city)
        {
            List<SellableItem> items = new List<SellableItem> {
                new Food("Bread", 1, 4, 3),
                new Food("Carrot", 1, 2, 2),
                new Food("Sausage", 1, 8, 5),
                new Food("Apple", 1, 2, 1),
                new Resource("Grain", 1, 1),
                new Resource("Iron", 1, 10),
                new Resource("Gold", 1,30 ),
                new Resource("Wood", 1, 5),
                new Resource("Stone", 1, 3)
            };
            items = items.Where(i => i.cost < value / 2).ToList();

            List<InventoryItem> sold = new List<InventoryItem>();
            for (int i = 0; i < value / 10; i++)
            {
                SellableItem item = items[World.random.Next(items.Count)];
                item.amount = World.random.Next(1, value / 10 * 2);
                if (item is Food f)
                    item.cost = city.DetermineLocalFoodPrice(item);
                sold.Add(new InventorySellableItem(item));
            }

            return new ConvenienceStore(sold, World.random.Next(value / 2, 4 * value), city);
        }
    }

    public class FoodMarket : Store
    {
        public static new int RequiredWealth()
        {
            return 0;
        }
        FoodMarket(List<InventoryItem> items, int money, City city) : base(items, money, "Food Market", "A small market selling local produce", city)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">From 1 to 100 determins the wealth of the shop. 1 - 20 Poor 20 - 60 Medium 60+ Rich</param>
        /// <returns></returns>
        public static FoodMarket GenerateStore(int value, City city)
        {
            List<InventoryItem> sold = GenerateGoods(value, city);

            return new FoodMarket(sold, World.random.Next(value / 2, 4 * value), city);
        }

        public static List<InventoryItem> GenerateGoods(int value, City city)
        {
            List<SellableItem> items = new List<SellableItem> {
                new Food("Bread", 1, 4, 3),
                new Food("Carrot", 1, 2, 2),
                new Food("Sausage", 1, 8, 5),
                new Food("Apple", 1, 2, 1),
                new Food("Pear", 1, 2, 2),
                new Food("Turnip", 1, 1, 1),
                new Resource("Grain", 1, 1)
            };
            items = items.Where(i => i.cost < value / 2).ToList();

            List<InventoryItem> sold = new List<InventoryItem>();
            for (int i = 0; i < value / 10; i++)
            {
                SellableItem item = items.GetRandom();
                item.amount = World.random.Next(1, value / 10 * 2);
                //Determine price from local prosperity
                item.cost = GetBuyPrice(item, city);
                sold.Add(new InventorySellableItem(item));
            }

            return sold;
        }

        public override void WorldAction()
        {

        }
    }

    public class JewleryStore : Store
    {
        public static new int RequiredWealth()
        {
            return 40;
        }
        JewleryStore(List<InventoryItem> items, int money, City city) : base(items, money, "Jewlery Store", " A store where you can buy jewelry", city)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">From 1 to 100 determins the wealth of the shop. 1 - 20 Poor 20 - 60 Medium 60+ Rich</param>
        /// <returns></returns>
        public static JewleryStore GenerateStore(int value, City city)
        {
            List<InventoryItem> sold = new List<InventoryItem>();
            for (int i = 0; i < value / 10; i++)
            {
                Jewelry item = Jewelry.GenerateJewelry(E.GetQuality(World.random.Next((value / 10).Cut(0, 8))));
                sold.Add(new InventoryItem(item, 1, item.Value));
            }

            return new JewleryStore(sold, World.random.Next(value / 2, 4 * value), city);
        }
    }

    public class SmithStore : Store
    {
        public static new int RequiredWealth()
        {
            return 20;
        }
        SmithStore(List<InventoryItem> items, int money, City city) : base(items, money, "Smith", " A store where you can buy weapons and armour", city)
        {

        }

        static readonly List<(int minWealth, Quality quality)> WealthVsQuality = new List<(int minWealth, Quality quality)> {
            (0, Quality.Broken),
            (5, Quality.Poor),
            (15, Quality.Simple),
            (25, Quality.Common),
            (40, Quality.Good),
            (50, Quality.Superior),
            (70, Quality.Exceptional),
            (90, Quality.Legendary)
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">From 1 to 100 determins the wealth of the shop. 1 - 20 Poor 20 - 60 Medium 60+ Rich</param>
        /// <returns></returns>
        public static SmithStore GenerateStore(int value, City city)
        {
            List<Quality> allowedQualities = WealthVsQuality.Where(d => d.minWealth <= value).Select(d => d.quality).ToList();
            List<InventoryItem> sold = new List<InventoryItem>();
            for (int i = 0; i < value / 40; i++)
            {
                Weapon weapon = Weapon.GetWeaponTypes().GetRandom();
                sold.Add(new InventoryItem(weapon, 1, Weapon.CalculatePrice(weapon)));
                Ammo ammo = Ammo.GetAmmos().Where(a => a.ammoType != AmmoType.Rock).ToList().GetRandom();
                sold.Add(new InventoryItem(ammo, World.random.Next(value / 5 + 1), ammo.GetPrice()));
                Armour armour = ArmourPrefabs.CreateArmour(true, allowedQualities.GetRandom(), World.random.Next(value / 10));
                sold.Add(new InventoryItem(armour, 1, armour.Value));
                Jewelry item = Jewelry.GenerateJewelry(E.GetQuality(World.random.Next((value / 10).Cut(0, 8))));
                sold.Add(new InventoryItem(item, 1, item.Value));
            }

            return new SmithStore(sold, World.random.Next(value / 2, 4 * value), city);
        }
    }

    public class MagicStore : Store
    {
        public static new int RequiredWealth()
        {
            return 60;
        }
        MagicStore(List<InventoryItem> items, int money, City city) : base(items, money, "Magic Store", " A store where you can buy magical items", city)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">From 1 to 100 determins the wealth of the shop. 1 - 20 Poor 20 - 60 Medium 60+ Rich</param>
        /// <returns></returns>
        public static MagicStore GenerateStore(int value, City city)
        {
            List<InventoryItem> sold = new List<InventoryItem>();
            for (int i = 0; i < value / 10; i++)
            {
                Spell spell = World.Instance.Spells.GetRandom();
                sold.Add(new InventoryItem(spell, 1, (int)(spell.buyCost * (World.random.NextDouble() + 1))));
            }

            return new MagicStore(sold, World.random.Next(value / 2, 4 * value), city);
        }
    }

    public class InventoryItem
    {
        public Item item;
        public int Amount;
        public int Cost;

        public InventoryItem(Item item, int amount, int cost)
        {
            this.item = item;
            Amount = amount;
            Cost = cost;
        }

        public override string ToString()
        {
            return $"{item.name} {Amount} {Cost} coins";
        }
    }

    public class InventorySellableItem : InventoryItem
    {
        public new int Amount { get => (item as SellableItem).amount; set => (item as SellableItem).amount = value; }
        public new int Cost { get => (item as SellableItem).cost; set => (item as SellableItem).cost = value; }
        public InventorySellableItem(SellableItem item) : base(item, item.amount, item.cost)
        {

        }
    }
}
