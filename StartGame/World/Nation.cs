using StartGame.AI;
using StartGame.Items;
using StartGame.PlayerData;
using StartGame.World.Cities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.World
{
    public class Nation
    {
        public static List<SellableItem> prices = new List<SellableItem> {
                new Food("Bread", 1, 5, 3),
                new Food("Carrot", 1, 3, 2),
                new Food("Sausage", 1, 9, 5),
                new Food("Apple", 1, 3, 1),
                new Food("Pear", 1, 4, 2),
                new Food("Turnip", 1, 3, 1),
                new Food("Strawberry", 1, 4, 1),
                new Food("Date", 1, 4, 3),
                new Food("Melone", 1, 4, 1),
                new Food("Corn", 1, 3, 1),
                new Food("Rice", 1, 3, 1),
                new Food("Cacoa", 1, 12, 6),
                new Food("Bannana", 1, 4, 2),
                new Food("Tomato", 1, 3, 1),
                new Food("Potato", 1, 3, 1),
                new Resource("Grain", 1, 2),
                new Resource("Iron", 1, 10),
                new Resource("Gold", 1,30 ),
                new Resource("Wood", 1, 5),
                new Resource("Stone", 1, 3),
                new Food("Bear Meat", 1, 20, 2),
                new Resource("Ice Core", 1, 50)
        };

        public int GetPrice(string name)
        {
            return prices.Find(p => p.name == name).cost;
        }

        public List<NationIslandInfo> islands = new List<NationIslandInfo>();
        public List<City> cities = new List<City>();
        public List<Farm> farms = new List<Farm>();
        public List<Mine> mines = new List<Mine>();
        public List<Settlement> toChange = new List<Settlement>();

        public City Captial => cities.Find(c => c is CapitalCity);

        public void Intialise(List<City> existingCities)
        {
            cities.AddRange(existingCities);
            UpdateIslandData();
            DeterminePortConnections();
        }

        public void UpdateIslandData()
        {
            for (int i = 0; i < World.Instance.islands.Count; i++)
            {
                Island island = World.Instance.islands[i];
                islands.Add(new NationIslandInfo(island, cities.Where(c => c.Located == island).ToList()));
            }
        }

        /// <summary>
        /// Determins which cities are connected with which others by being poarts on same water body
        /// </summary>
        public void DeterminePortConnections()
        {
            foreach (var ocean in World.Instance.Oceans)
            {
                List<City> ports = cities.Where(c => c.access.Contains(ocean)).ToList();
                foreach (var port in ports)
                {
                    port.portConnections.AddRange(ports.Where(c => c != port));
                }
            }
        }

        /// <summary>
        /// Determines which islands do not have ports at each connected ocean
        /// </summary>
        /// <returns>List of islands where ports have to be built</returns>
        public List<Island> DetermineNeededPorts()
        {
            return islands.Where(i => i.Settled && i.ConnectedCities.Count == 0).Select(i => i.island).ToList();
        }

        /// <summary>
        /// Generates roads between cities where it makes sense
        /// </summary>
        public void GenerateRoads()
        {
            int roads = 0;
            foreach (var island in islands.Where(i => i.Settled))
            {
                Trace.TraceInformation($"Generating roads for island {island.island.Name} \t Cities: {island.cities.Count}");
                List<City> order = island.cities.OrderByDescending(c => c.priority).ToList();
                foreach (var city in island.cities.OrderBy(c => AIUtility.Distance(order[0].position, c.position)))
                {
                    List<City> allowedCities = order.Where(c => c != city && AIUtility.Distance(c.position, city.position) < Math.Max(c.Wealth, 15)).ToList();
                    for (int i = 0; i < Math.Min(city.connections, allowedCities.Count()); i++)
                    {
                        City toCity = allowedCities[i];
                        if (city.roadConnections.Contains(toCity) || toCity.roadConnections.Contains(city))
                            continue;
                        Trace.TraceInformation($"Generated road between {city.position} and {toCity.position}");
                        city.roadConnections.Add(toCity);
                        toCity.roadConnections.Add(city);
                        roads++;
                        toCity.costMap = AStar.GenerateCostMap(World.Instance.costMap, ref toCity.position, AIUtility.Distance(toCity.position, city.position) * 10);
                        Point[] route = AStar.FindPath(toCity.position, city.position, toCity.costMap);
                        foreach (var point in route)
                        {
                            if (!World.Instance.features.Exists(f => f is Road && f.position == point))
                            {
                                World.Instance.features.Add(new Road(point));
                            }
                        }
                    }
                }
            }
            Trace.TraceInformation($"Generated {roads} roads!");
        }

        double actionsPassed = 0;
        internal void WorldAction(double actions)
        {
            actionsPassed += actions;
            if (actionsPassed >= 24)
            {
                actionsPassed -= 24;
                farms.ForEach(f => f.WorldAction());
                cities.ForEach(c => c.WorldAction());
                foreach (var settlement in toChange)
                {
                    switch (settlement)
                    {
                        case City c:
                            if (cities.Contains(c))
                            {
                                cities.Remove(c);
                                cities.ForEach(ci => ci.portConnections.Remove(c));
                                cities.ForEach(ci => ci.roadConnections.Remove(c));
                                World.Instance.features.Remove(c);
                            }
                            else
                                cities.Add(c);
                            break;
                        case Farm f:
                            if (farms.Contains(f))
                            {
                                farms.Remove(f);
                                World.Instance.features.Remove(f);
                            }
                            else
                                farms.Add(f);
                            break;
                        case Mine m:
                            if (mines.Contains(m))
                            {
                                mines.Remove(m);
                                World.Instance.features.Remove(m);
                            }
                            else
                                mines.Add(m);
                            break;
                        default:
                            break;
                    }
                }
                toChange.Clear();
            }
        }
    }

    public struct NationIslandInfo
    {
        public bool Settled => cities.Count != 0;
        public List<City> cities;
        public List<City> ports;
        public List<Island> ConnectedIslands => ports.SelectMany(p => p.portConnections.Select(c => c.Located)).Distinct().ToList();
        public List<City> ConnectedCities => ports.SelectMany(p => p.portConnections).Distinct().ToList();
        public Island island;

        public NationIslandInfo(Island island, List<City> cities)
        {
            this.island = island;
            this.cities = new List<City>();
            this.cities.AddRange(cities);
            ports = new List<City>();
            foreach (var city in cities)
            {
                if (World.Instance.worldMap.Get(city.position).sorroundingTiles.rawMaptiles.Any(t => !World.IsLand(t.type)))
                {
                    ports.Add(city);
                    city.IsPort = true;
                }
            }
        }
    }
}
