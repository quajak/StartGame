using StartGame.AI;
using StartGame.Items;
using StartGame.PlayerData;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.World.Cities
{

    // Caravans currently only buy items locally and sell them in other cities
    public class CaravanRoute
    {
        public bool Active { get; private set;} = false;

        readonly CaravanMarket caravanMarket;
        public readonly City start;
        public readonly City end;
        readonly List<string> itemTypes;
        private int money;
        public List<InventoryItem> items;
        readonly int days;

        public CaravanRoute(City start, City end, CaravanMarket caravanMarket, List<string> itemTypes, int money, List<InventoryItem> items = null)
        {
            this.start = start;
            this.end = end;
            this.itemTypes = itemTypes;
            this.money = money;
            this.items = items;
            days = AIUtility.Distance(start.position, end.position) / 10;
            BuyItems();
            this.caravanMarket = caravanMarket;
        }
        int DetermineTravelCost()
        {
            return days * 10;
        }

        void BuyItems()
        {
            if (items != null) return;
            items = new List<InventoryItem>();
            int moneyPerItem = money / itemTypes.Count;
            FoodMarket foodMarket = start.GetBuilding<FoodMarket>();
            List<InventoryItem> localItems = foodMarket.Items;
            foreach (string item in itemTypes)
            {
                int currentItemMoney = moneyPerItem;
                int amount = 0;
                int healAmount = 0;
                int sellablePrice = end.DetermineLocalFoodPrice(item);
                List<InventoryItem> enumerable = new List<InventoryItem>(localItems.Where(i => i.item.name == item));
                foreach (var i in enumerable)
                {
                    if (i.Cost < sellablePrice)
                    {
                        int addedAmount = Math.Min(i.Amount, moneyPerItem / i.Cost);
                        if (addedAmount == 0)
                            continue;
                        amount += addedAmount;
                        money -= i.Cost * addedAmount;
                        currentItemMoney -= i.Cost * addedAmount;
                        healAmount = (i.item as Food).healAmount;
                        if (i.Amount == addedAmount)
                        {
                            foodMarket.Items.Remove(i);
                        }
                        else
                            i.Amount -= addedAmount;
                        foodMarket.money += i.Cost * addedAmount;
                        if (currentItemMoney <= 0)
                            break;
                    }
                }
                items.Add(new InventoryItem(new Food(item, 1, sellablePrice, healAmount), amount, sellablePrice));
            }
        }

        public int PredictProfit()
        {
            int profit = 200;
            foreach (var item in items)
            {
                int sell = end.DetermineLocalFoodPrice(item.item.name);
                int buy = start.DetermineLocalFoodPrice(item.item.name);
                profit += item.Amount * (sell - buy);
            }
            return profit - DetermineTravelCost();
        }

        public int GetGuardPayment()
        {
            return GetValueOfItems() / 8;
        }

        public override string ToString()
        {
            return $"{start.name} -> {end.name}";
        }

        public string Description
        {
            get
            {
                string text = $"This caravan goes from {start.name} to {end.name} in {days} days. It will pay you {GetGuardPayment()} to act as a guard and carries approximately {GetValueOfItems()} worth of goods.";
                text += "It will carry ";
                foreach (string item in itemTypes)
                {
                    text += $" {item}";
                }
                text += ".";
                return text;
            }
        }

        private int GetValueOfItems()
        {
            return items.Select(i => i.Cost * i.Amount).Sum();
        }

        internal void End()
        {
            int profit = 0;
            foreach (InventoryItem item in items)
            {
                profit += end.GetBuilding<FoodMarket>().Sell(item);
            }
            caravanMarket.wealth += profit;
            Trace.TraceInformation($"Caravan Route - {ToString()} - Made {profit} profit");
            caravanMarket.routes.Remove(this);
            caravanMarket.NewRoute();
        }

        public void Start()
        {
            Trace.TraceInformation($"Caravan Market {caravanMarket.city.name} - Start route {ToString()}");
            Active = true;
            new Caravan(this);
        }
    }
    public class CaravanMarket : CityBuilding
    {
        public readonly City city;
        private readonly Nation nation;
        CityView cityV;
        public readonly List<CaravanRoute> routes = new List<CaravanRoute>();
        List<City> possibleCities;
        public int wealth;
        readonly List<InventoryItem> items;
        public CaravanMarket(City city, Nation nation) : base(ID++, "Caravan Market", "", new List<CityBuildingAction> {
            new CityBuildingAction("Join caravan")
        })
        {
            this.city = city;
            this.nation = nation;
            wealth = city.Wealth * 10;
            //Determine products
            items = city.buildings.Where(b => b is FoodMarket).Select(f => ((FoodMarket)f).Items)
                .Aggregate((l, n) => { l.AddRange(n); return l; }).ToList();
            //Determine routes later when all cities have been initialised
        }

        public override void Initialse()
        {
            possibleCities = new List<City>(city.roadConnections);
            DetermineRoutes();
            if (city.roadConnections.Count != 0)
                description = $"A place where all traders and caravans meet. The city is connected to" +
                $"{city.roadConnections.Select(c => c.name).Aggregate((a, b) => a += ", " + b)}.";
            else
                description = $"A place where all traders and caravans meet.";

        }

        private void DetermineRoutes()
        {
            for (int i = 0; i < Math.Min(wealth / 20, possibleCities.Count); i++)
            {
                var c = possibleCities[i];
                DetermineRoute(c);
            }
        }

        public void NewRoute()
        {
            for (int i = 0; i < 100; i++)
            {
                City to = possibleCities.GetRandom();
                if (DetermineRoute(to))
                    break;
            }
        }

        private bool DetermineRoute(City c)
        {
            List<string> profitableItems = new List<string>();
            foreach (var item in items)
            {
                int sellPrice = c.DetermineLocalFoodPrice(nation.GetPrice(item.item.name));
                if (sellPrice > item.Cost && !profitableItems.Contains(item.item.name))
                {
                    profitableItems.Add(item.item.name);
                }
            }
            if (profitableItems.Count > 0)
            {
                CaravanRoute possibleRoute = new CaravanRoute(city, c, this, profitableItems, Math.Min(wealth / 10, 1000));
                if (possibleRoute.PredictProfit() > 0)
                {
                    routes.Add(possibleRoute);
                    return true;
                }
            }
            return false;
        }

        public override void OnAction(CityBuildingAction action, CityView cityView)
        {
            cityV = cityView;
            Trace.TraceInformation($"Store::OnAction {action.name}");
            if (action.name == "Deselect")
            {
                cityV.actionOptionLabel.Visible = false;
                cityView.actionOptionList.Visible = false;
                cityView.actionOptionList.SelectedIndexChanged -= ActionOptionList_SelectedIndexChanged;
            }
            else if (action.name == "Join caravan")
            {
                cityView.actionOptionList.Visible = true;
                cityV.actionOptionList.Items.Clear();
                cityV.actionOptionList.Visible = true;
                cityV.actionOptionLabel.Visible = false;
                foreach (var c in routes)
                {
                    cityV.actionOptionList.Items.Add(c);
                }
                cityView.actionOptionList.SelectedIndexChanged += ActionOptionList_SelectedIndexChanged;
            }
        }

        private void ActionOptionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            CaravanRoute caravanRoute = cityV.actionOptionList.SelectedItem as CaravanRoute;
            if(caravanRoute is null)
            {
                cityV.actionOptionLabel.Visible = false;
                cityV.button1.Visible = false;
                cityV.button1.Click -= Button1_Click;
                return;
            }
            cityV.actionOptionLabel.Visible = true;
            cityV.button1.Visible = true;
            cityV.actionOptionLabel.Text = caravanRoute.Description;
            cityV.button1.Text = "Join Caravan";
            cityV.button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var player = cityV.worldView.player;
            player.caravan = cityV.actionOptionList.SelectedItem as CaravanRoute;
            if (player.caravan is null) return;
            player.troop.Image = Resources.Caravan;
            player.spectatorMode = true;
            var route = AStar.FindOptimalRoute(World.Instance.MovementCost(), player.WorldPosition, player.caravan.end.position);
            cityV.worldView.ShowPlayerRoute(route);
            player.toMove = route.Skip(1).ToList();

            cityV.Close();
        }

        public static CaravanMarket GenerateStore(City city, Nation nation)
        {
            return new CaravanMarket(city, nation);
        }
        public static int RequiredWealth()
        {
            return 10;
        }

        public override void WorldAction()
        {
            if (city.roadConnections.Count != 0)
                description = $"A place where all traders and caravans meet. It has {wealth} coins. The city is connected to" +
                $"{city.roadConnections.Select(c => c.name).Aggregate((a, b) => a += ", " + b)}.";
            else
                description = $"A place where all traders and caravans meet. It has {wealth} coins.";
            int active = routes.Where(r => r.Active).Count();
            if(active * 2 < routes.Count)
            {
                //Start a new caravan
                routes.Where(r => !r.Active).ToList().GetRandom().Start();
            }
        }
    }
}
