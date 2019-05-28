using StartGame.Items;
using StartGame.Items.Crops;
using StartGame.MathFunctions;
using StartGame.PlayerData;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace StartGame.World.Cities
{
    public class Farm : Settlement, IProducer
    {
        private int workers = 10;
        List<Resource> items = new List<Resource>();
        int money = 20000;
        List<Crop> growingCrops = new List<Crop>();
        int yearlyProduction = 0;
        private readonly int agriculturalValue;
        bool dead = false;

        public void SortItems()
        {
            items = items.OrderBy(i => i.cost).ToList();
        }

        readonly City city;
        public Farm(Point position, int agriculturalValue) : base("Farm", ++ID, position, Resources.Farm1, 1)
        {
            this.agriculturalValue = agriculturalValue;
            //Find closest city
            city = World.Instance.nation.cities.Select(c => (c, AIUtility.Distance(position, c.position))).AsQueryable().OrderBy<(City, int), int>(c => c.Item2).First().Item1;
        }
        int lastMoney = 0;
        int day = 0;
        int costIncrease = 0;
        internal override void WorldAction()
        {
            if (dead)
                return;
            //Update all crops
            foreach (var crop in growingCrops)
            {
                crop.time--;
                if(crop.time == 0)
                {
                    Resource r = crop.Yield();
                    r.amount = (int)(r.amount *  (1 + (-0.5 + agriculturalValue/100d)));
                    ProduceItem(r.name, r.amount);
                }
            }
            growingCrops = growingCrops.Where(c => c.time > 0).ToList();
            while (growingCrops.Count <= workers / 4)
            {
                (double temperature, double humidity) = World.Instance.GetWeather(position);
                List<Crop> list = Crop.GetCrops((int)World.GetCelsius(temperature), (int)(humidity * 1000));
                if (list.Count == 0)
                    break;
                growingCrops.Add(list.GetRandom().Duplicate());
            }

            if (day % 365 == 0)
            {
                double rainFall = World.Instance.rainfallMap.Get(position);
                Trace.TraceInformation($"{name} on {World.Instance.worldMap.Get(position).island.Name} has produced {yearlyProduction} food this year and has {money} money from {lastMoney}");
                if (lastMoney > money)
                {
                    costIncrease += 1;
                }
                else
                {
                    costIncrease -= 1;
                }
                costIncrease = costIncrease.Min(0);
                yearlyProduction = 0;
                lastMoney = money;
                while (workers < 20 && money >= 6 * workers * 365)
                {
                    //Trace.TraceInformation($"{name} at {position} has gone grown!");
                    workers++;
                }
            }
            else if (day % 7 == 0)
            {
                money -= workers * 2;
            }
            if (money <= 0)
            {
                Trace.TraceInformation($"{name} at {position} has gone bankrupt!");
                dead = true;
                World.Instance.nation.toChange.Add(this);
            }
            day++;
        }

        internal int GetPrice(string name, int amount)
        {
            Resource item = items.Find(i => i.name == name);
            return GetPrice(amount, item);
        }

        public int Buy(string name, int amount)
        {
            Resource item = items.Find(i => i.name == name);
            if (item is null)
                throw new Exception();
            item.amount -= amount;
            if (item.amount < 0) throw new Exception();
            if (item.amount == 0)
                items.Remove(item);
            int cost = GetPrice(amount, item);
            money += cost;
            return cost;
        }

        private static int GetPrice(int amount, Resource item)
        {
            return item.cost * amount;
        }

        internal void AddItem(Resource resource)
        {
            yearlyProduction += resource.amount;
            if (items.TryGet(r => r.name == resource.name, out Resource item))
            {
                item.amount += resource.amount;
            }
            else
            {
                items.Add(resource);
            }
        }

        internal void ProduceItem(string name, int amount)
        {
            int price = Nation.prices.Find(p => p.name == name).cost;
            price = city.DetermineLocalFoodPrice(new Resource(name, 1, price + costIncrease));
            //Trace.TraceInformation($"{name}: Produced {name} x{amount} at {price + costIncrease}");
            AddItem(new Resource(name, amount, price + costIncrease));
        }

        public List<Resource> Items()
        {
            return items;
        }

        public int Money()
        {
            return money;
        }

        public int Workers()
        {
            return workers;
        }
    }
}

namespace StartGame.Items.Crops
{

    
}