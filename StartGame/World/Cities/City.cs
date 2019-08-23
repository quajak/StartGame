using StartGame.Items;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace StartGame.World.Cities
{
    public abstract class City : Settlement
    {
        private bool isPort = false;
        static List<string> Names = new List<string> { "Edagas"}; //Hack as one name has to be loaded
        readonly Nation nation = World.Instance.nation;
        public bool IsPort
        {
            get => isPort; set
            {
                if (value && !isPort)
                {
                    buildings.Add(new Port(this));
                }
                else if(!value)
                {
                    buildings = buildings.Where(b => !(b is Port)).ToList();
                }
                isPort = value;
            }
        }

        public List<Island> access = new List<Island>();
        public List<City> portConnections = new List<City>();
        public List<City> roadConnections = new List<City>();
        public readonly int connections;
        public double[,] costMap = null;



        public int priority;
        public readonly int value;
        public readonly int agriculturalProduction;
        public readonly int mineralProduction;

        public int lastPopulation = 0;
        public int Population;


        public int Wealth => priority * value / 10;

        public int RequiredFood => Population / 100;

        public List<CityBuilding> buildings = new List<CityBuilding>();

        public City(Point position, Bitmap bitmap, int connections, int priority, int value, int agriculturalProduction, int mineralProduction) : base(Names.GetRandom(), ++ID, position, bitmap, 1)
        {
            Names.Remove(name);
            if(Names.Count == 0)
            {
                Names = File.ReadAllLines(@"./Resources/CityNames.txt").ToList();
            }
            Population = priority * 1000 + World.random.Next(1000);
            lastPopulation = Population;
            foreach (var tile in World.Instance.worldMap.Get(position).sorroundingTiles.rawMaptiles)
            {
                if (!access.Contains(tile.island))
                    access.Add(tile.island);
            }

            this.connections = connections;
            this.priority = priority;
            this.value = value;
            this.agriculturalProduction = agriculturalProduction;
            this.mineralProduction = mineralProduction;
            //World rendering
            size = new Point(3, 2);
            alignment = RenderingAlignment.Center;
            //Add base buildings
            buildings.Add(new MayorHouse(this));
            buildings.Add(FoodMarket.GenerateStore(value * 10, this));
            buildings.Add(CaravanMarket.GenerateStore(this, nation));
            //Add producers
            buildings.Add(new Logger(this));
            //Generate shops
            List<Type> possibleStores = new List<Type> { typeof(ConvenienceStore), typeof(JewleryStore), typeof(MagicStore), typeof(SmithStore) };
            possibleStores = possibleStores.Where(p => (int)p.GetMethod("RequiredWealth").Invoke(null, new object[] { }) < Wealth).ToList();
            if(possibleStores.Count > 0)
            {
                for (int i = 0; i < World.random.Next(value / 20) + 1 + priority / 2; i++)
                {
                    Type store = possibleStores.GetRandom();
                    if(store == typeof(ConvenienceStore))
                    {
                        buildings.Add(ConvenienceStore.GenerateStore(value, this));
                    }
                    else if (store == typeof(JewleryStore))
                    {
                        buildings.Add(JewleryStore.GenerateStore(value, this));
                    }
                    else if (store == typeof(MagicStore))
                    {
                        buildings.Add(MagicStore.GenerateStore(value, this));
                    } else if(store == typeof(SmithStore))
                    {
                        buildings.Add(SmithStore.GenerateStore(value, this));
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                }
            }
        }
        /// <summary>
        /// Method called when the nation has been initialised
        /// </summary>
        internal void Initialise()
        {
            foreach (var building in buildings)
            {
                building.Initialse();
            }
        }

        double day = 0;
        internal override void WorldAction()
        {
            day++;
            //Run once a day
            buildings.OrderBy(b => b.Priority).ToList().ForEach(b => b.WorldAction());
            if(day % 7 == 0)
            {
                //Do weekly action
                //population is calculated weekly for nicer curves
                //do an exponential
                int growth = ((int)(Math.Pow(Population, 1.006d)) - Population) / 50;
                Population += growth;
            }
            if(day % 365 == 0)
            {
                //Do yearly action
                //Calculate population change
                //Trace.TraceInformation($"{name} grew by {growth} people");
                Trace.TraceInformation($"{name} Population Change: From {lastPopulation} to {Population} Change is {Population - lastPopulation} or {((double)(Population - lastPopulation)/ Population).ToString("0.00")}");
                lastPopulation = Population;
            }
            day %= 365;
        }

        public override string ToString()
        {
            return name;
        }

        public int DetermineLocalFoodPrice(string s)
        {
            return DetermineLocalFoodPrice(nation.GetPrice(s));
        }

        public int DetermineLocalFoodPrice(SellableItem item)
        {
            return (int)(item.cost / 2 + ((100 - agriculturalProduction) / 100d) * item.cost).Cut(1, 10000);
        }

        public int DetermineLocalFoodPrice(int cost)
        {
            return (int)(cost / 2 + ((100 - agriculturalProduction) / 100d) * cost).Cut(1, 10000);
        }

        public List<T> GetBuildings<T>() where T : CityBuilding
        {
            return buildings.Where(b => b is T).Select(b => b as T).ToList();
        }
        public T GetBuilding<T>() where T : CityBuilding
        {
            return buildings.Where(b => b is T).Select(b => b as T).First();
        }

        internal abstract string GetSizeDescriptor();

        public string Description => $"{name} is a {GetSizeDescriptor().ToLower()} city in the {World.Instance.worldMap.Get(position).type}.";
            }

    public class SmallCity : City
    {
        public SmallCity(Point position, int value, int agriculture, int mineral) : base(position, Resources.SmallCity, 1, 1, value, agriculture, mineral)
        {
        }

        internal override string GetSizeDescriptor()
        {
            return "Small";
        }
    }

    public class MediumCity : City
    {
        public MediumCity(Point position, int value, int agriculture, int mineral) : base(position, Resources.MediumCity, 1, 3, value, agriculture, mineral)
        {
        }
        internal override string GetSizeDescriptor()
        {
            return "Medium";
        }
    }

    public class LargeCity : City
    {
        public LargeCity(Point position, int value, int agriculture, int mineral) : base(position, Resources.BigCity, 1, 6, value, agriculture, mineral)
        {
        }
        internal override string GetSizeDescriptor()
        {
            return "Large";
        }
    }

    public class CapitalCity : City
    {
        public CapitalCity(Point position, int value, int agriculture, int mineral) : base(position, Resources.CapitalCity, 1, 10, value, agriculture, mineral)
        {
        }
        internal override string GetSizeDescriptor()
        {
            return "Capital";
        }
    }
}