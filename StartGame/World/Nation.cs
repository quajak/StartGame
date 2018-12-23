using StartGame.AI;
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
        public List<NationIslandInfo> islands = new List<NationIslandInfo>();
        public List<City> cities = new List<City>();
        public City Captial => cities.Find(c => c is CapitalCity);

        public void Intialise(List<City> existingCities)
        {
            cities.AddRange(existingCities);
            UpdateIslandData();
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
        /// Determins which cities are connected with which others due to roads or being poarts on same water body
        /// </summary>
        public void DetermineConnections()
        {
            foreach (var ocean in World.Instance.Oceans)
            {
                List<City> ports = cities.Where(c => c.access.Contains(ocean)).ToList();
                foreach (var port in ports)
                {
                    port.connected.AddRange(ports.Where(c => c != port));
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

        public void GenerateRoads()
        {
            int roads = 0;
            foreach (var island in islands.Where(i => i.Settled))
            {
                List<City> order = island.cities.OrderByDescending(c => c.priority).ToList();
                foreach (var city in island.cities)
                {
                    for (int i = 0; i < Math.Min(city.connections, island.cities.Count); i++)
                    {
                        Trace.TraceInformation($"Generated road between {city.position} and {order[i].position}");
                        roads++;
                        Point[] route = AStar.FindOptimalRoute(World.Instance.costMap, city.position, order[i].position);
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
    }

    public struct NationIslandInfo
    {
        public bool Settled => cities.Count != 0;
        public List<City> cities;
        public List<City> ports;
        public List<Island> ConnectedIslands => ports.SelectMany(p => p.connected.Select(c => c.Located)).Distinct().ToList();
        public List<City> ConnectedCities => ports.SelectMany(p => p.connected).Distinct().ToList();
        public Island island;

        public NationIslandInfo(Island island, List<City> cities)
        {
            this.island = island;
            this.cities = new List<City> ();
            this.cities.AddRange(cities);
            ports = new List<City>();
            foreach (var city in cities)
            {
                if(World.Instance.worldMap.Get(city.position).sorroundingTiles.rawMaptiles.Any(t => !World.IsLand(t.type)))
                {
                    ports.Add(city);
                }
            }
        }
    }
}
