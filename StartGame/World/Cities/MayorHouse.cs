using StartGame.Items;
using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StartGame.World.Cities
{
    /// <summary>
    /// The mayor is used to simulate the population of a city
    /// </summary>
    public class MayorHouse : CityBuilding
    {
        List<Farm> farms = new List<Farm>();
        int money;
        public MayorHouse(City city) : base(++ID, "Mayor House", "This is the mayor speaking.", new List<CityBuildingAction> { })
        {
            allowMultiple = false;
            Priority = 1;
            this.city = city;
            description = $"The agricultural value of the town is {city.agriculturalProduction} and the mineral value is {city.mineralProduction}. The city requires {FoodRequirement()} food per week.";
            money = city.Population;
        }

        int FoodRequirement()
        {
            return city.RequiredFood;
        }

        int day = 0;
        private readonly City city;

        bool dead = false;
        public override void WorldAction()
        {
            if (dead)
                return;
            day++;
            description = $"The agricultural value of the town is {city.agriculturalProduction} and the mineral value is {city.mineralProduction}. The city requires {FoodRequirement()} food per week and is getting food from {farms.Count}. It has {money} coins.";
            if (day % 7 == 0)
            {
                //Update this in case farms get added or removed
                Island island = World.Instance.worldMap.Get(city.position).island;
                farms = World.Instance.nation.farms.Where(f => World.Instance.worldMap.Get(f.position).island == island).OrderBy(f => AIUtility.Distance(city.position, f.position)).ToList();
                int foodRequirement = FoodRequirement();
                int initialFoodRequirement = foodRequirement;
                FoodMarket market = city.buildings.Find(f => f is FoodMarket) as FoodMarket;
                while (foodRequirement != 0)
                {
                    if (market.Items.Count == 0 || !market.Items.Exists(i => market.GetBuyPrice(i.item) <= money))
                    {
                        //People are going hungry
                        Trace.TraceInformation($"In {city.name} {foodRequirement} died from hunger - Wealth {money} - Market Items: {market.Items.Sum(i => i.Amount)}" +
                            $" Market Money: {market.money}");
                        checked
                        {
                            city.Population -= foodRequirement;
                        }
                        break;
                    }
                    List<InventoryItem> foodList = market.Items.OrderBy(r => r.Cost).ToList();
                    foreach (var food in foodList)
                    {
                        int amount = Math.Min(food.Amount, foodRequirement);
                        int individualPrice = market.GetBuyPrice(food.item);
                        amount = Math.Min(money / individualPrice, amount);
                        int price = individualPrice * amount;
                        if (money >= price)
                        {
                            foodRequirement -= amount;
                            money -= price;
                            market.money += price;
                            food.Amount -= amount;
                            if (food.Amount == 0)
                                market.Items.Remove(food);
                        }
                        if (foodRequirement == 0)
                            break;
                    }
                }
            }

            if (day % 365 == 0)
            {
                money += city.Population * 2;
                Trace.TraceInformation($"City: {city.name} Wealth: {money}");
                if (city.Population < city.lastPopulation || (city.buildings.Find(f => f is FoodMarket) as FoodMarket).Items.Count == 0)
                {
                    //Find close tile and build new farm
                    WorldTile tile = null;
                    double maxAgri = 0;
                    for (int x = 0; x < 11; x++)
                    {
                        for (int y = 0; y < 11; y++)
                        {
                            WorldTile worldTile = World.Instance.worldMap[(city.position.X - 5 + x).Cut(0, World.WORLD_SIZE - 1), (city.position.Y - 5 + y).Cut(0, World.WORLD_SIZE - 1)];
                            double agriculture = World.Instance.agriculturalMap[(city.position.X - 5 + x).Cut(0, World.WORLD_SIZE - 1), (city.position.Y - 5 + y).Cut(0, World.WORLD_SIZE - 1)];
                            if (maxAgri < agriculture)
                            {
                                tile = worldTile;
                                maxAgri = agriculture;
                            }
                        }
                    }
                    if (tile != null && maxAgri > 0)
                    {
                        Trace.TraceInformation($"{city.name} created a new farm at {tile.position}");
                        World.Instance.nation.toChange.Add(new Farm(tile.position, (int)(maxAgri * 100)));
                    }
                }
            }
            if (city.Population < 100)
            {
                Trace.TraceInformation($"The city {city.name} is dead!");
                dead = true;
                //The city is dead
                World.Instance.nation.toChange.Add(city);
            }
            //Trace.TraceInformation($"{city.name} on {World.Instance.worldMap.Get(city.position).island.Name}: {initialFoodRequirement}  - Hunger deaths: {foodRequirement}");
        }
    }
}
