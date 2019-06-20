using StartGame.Functions;
using StartGame.Mission;
using StartGame.PlayerData;
using StartGame.Properties;
using StartGame.World.Cities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace StartGame.World
{
    public partial class World
    {
        public const int WORLD_SIZE = 200;
        public const int WORLD_DIFFICULTY = 5;
        private static World world;
        public DateTime time = new DateTime(1000, 1, 1, 0, 0, 0, 0);
        public Campaign campaign; //Get a better system to generate mission
        public int missionsCompleted = 0;

        public static World Instance
        {
            get
            {
                if (world is null)
                    new World();
                return world;
            }
            set => world = value;
        }

        public List<Spell> Spells { get; }

        public List<Tree> trees = Tree.GenerateTrees();

        public WorldTile[,] worldMap;
        public double[,] costMap;
        public double[,] heightMap;
        public double[,] rawheightMap;
        public double[,] rainfallMap;
        public double[,] temperatureMap;
        public double[,] mineralMap;
        public double[,] valueMap;
        public double[,] agriculturalMap;
        public static Random random = new Random();

        public List<WorldPlayer> actors = new List<WorldPlayer>();
        public List<WorldFeature> features = new List<WorldFeature>();
        public List<Island> islands = new List<Island>();
        public List<Island> Oceans => islands.Where(i => !i.land).ToList();
        public Nation nation;

        private World(double a = 0.30, double b = 0.14, double c = 0.31, double Seed = 0, int octaves = 4, double persistance = 0.26)
        {
            Trace.TraceInformation($"World::ctor");
            DateTime time = DateTime.Now;
            world = this;
            Spells = new List<Spell>()
            {
                new HealingSpell(5, 500),
                new EarthQuakeSpell(5, 5, 4500),
                new LightningBoltSpell(15, 4000),
                new LightningBoltSpell(40, 7000),
                new DebuffSpell(2, 1, 5, 0, 800),
                new DebuffSpell(5, 3, 5, 0, 2400),
                new TeleportSpell(2, 0, 9600),
                new FireBall(8, 3, 4, 0, 2300),
                new FireBall(2, 2, 5, 0, 700),
                new IceSpikeSpell(4, 3000),
                new IceSpikeSpell(10, 12000),
            };

            //Generate the terrain
            //Setup variables
            campaign = new Campaign(null, 0, WORLD_DIFFICULTY);
            worldMap = new WorldTile[WORLD_SIZE, WORLD_SIZE];
            costMap = new double[WORLD_SIZE, WORLD_SIZE];
            heightMap = new double[WORLD_SIZE, WORLD_SIZE];
            rawheightMap = new double[WORLD_SIZE, WORLD_SIZE];
            rainfallMap = new double[WORLD_SIZE, WORLD_SIZE];
            temperatureMap = new double[WORLD_SIZE, WORLD_SIZE];
            mineralMap = new double[WORLD_SIZE, WORLD_SIZE];
            valueMap = new double[WORLD_SIZE, WORLD_SIZE];
            agriculturalMap = new double[WORLD_SIZE, WORLD_SIZE];

            PerlinNoise perlin = new PerlinNoise();
            Seed = random.NextDouble() * 100;
            double perlinDiff = 0.045;
            double s2 = random.Next(1000);

            //Generate continents in the center of the world
            int continentNumber = 3;
            Point[] continentPositions = new Point[continentNumber];
            for (int i = 0; i < continentNumber; i++)
            {
                continentPositions[i] = new Point(random.Next(50, WORLD_SIZE - 50), random.Next(50, WORLD_SIZE - 50));
            }

            //Generate each tile
            for (int x = 0; x < WORLD_SIZE; x++)
            {
                for (int y = 0; y < WORLD_SIZE; y++)
                {
                    //Height
                    double mHeight = perlin.OctavePerlin(s2 + x * perlinDiff, y * perlinDiff, Seed, octaves, persistance);
                    rawheightMap[x, y] = mHeight;
                    //scale to closest continent
                    double distance = continentPositions.Select(co => AIUtility.Distance(co, new Point(x, y))).Min();
                    mHeight = (mHeight + a) - b * Math.Pow(distance, c);
                    mHeight = mHeight.Cut(0, 1);
                    double rainfall = perlin.Perlin(s2 + (x + 200) * (perlinDiff / 1.5), s2 + (y + 200) * (perlinDiff / 2), Seed);
                    double temperature = Math.Sin((y - 10d) / 60d) - mHeight / 3;
                    temperature = temperature.Cut(-1, 1);

                    double minerals = Math.Pow(perlin.OctavePerlin(s2 + x * (perlinDiff * 0.4), y * (perlinDiff * 0.4), Seed, 4, 0.4), 3) * 2;
                    double agriculturalValue = rainfall * -4 * (rainfall - 1) * Math.Pow(Math.E, rainfall); //y = -2x(x-1)*e^x
                    agriculturalValue *= -3 * mHeight * (mHeight - 1) * Math.Pow(Math.E, -1 * mHeight); // y = -6x(x-1)*e^-x

                    heightMap[x, y] = mHeight;
                    rainfallMap[x, y] = rainfall;
                    temperatureMap[x, y] = temperature;

                    WorldTileType type = WorldTileType.TemperateGrassland;
                    double cost = 1;
                    double rtemp = temperature * 35;
                    double rrain = rainfall * 1000;
                    if (mHeight < 0.42)
                    {
                        if(temperature < 0)
                        {
                            type = WorldTileType.SeaIce;
                        }
                        else
                        {
                            type = WorldTileType.Ocean;
                        }
                        cost = 20;
                        minerals = 0;
                        agriculturalValue = 0;
                    }
                    else if (mHeight > 0.7)
                    {
                        type = WorldTileType.Alpine;
                        cost = 4;
                        minerals *= 2;
                        agriculturalValue *= 0.5;
                    }
                    else if (rtemp < 10)
                    {
                        type = WorldTileType.Tundra;
                        cost = 2;
                        minerals *= 1.5;
                        agriculturalValue *= 0.5;
                    }
                    else if (rrain > 680 && rtemp > 23)
                    {
                        type = WorldTileType.Rainforest;
                        cost = 4;
                    }
                    else if (rrain < 350 && rtemp > 25)
                    {
                        type = WorldTileType.Desert;
                        cost = 3;
                        agriculturalValue *= 0.5;
                    }
                    else if (rtemp > 26)
                    {
                        type = WorldTileType.Savanna;
                        cost = 2;
                        agriculturalValue *= 0.75;
                    }
                    else if (rrain > 500)
                    {
                        type = WorldTileType.TemperateForest;
                        cost = 1;
                    }
                    double value = Math.Max(minerals, agriculturalValue);

                    mineralMap[x, y] = minerals.Cut(0, 1);
                    agriculturalMap[x, y] = agriculturalValue.Cut(0, 1);
                    valueMap[x, y] = value.Cut(0, 1);

                    cost += (int)(mHeight * 10) / 10d;

                    worldMap[x, y] = new WorldTile(mHeight, type, cost, new Point(x, y), rainfall);
                    costMap[x, y] = cost;
                }
            }

            //Initialise atmosphere
            InitialiseAtmosphere();

            //Calculate connections
            for (int x = 0; x < WORLD_SIZE; x++)
            {
                for (int y = 0; y < WORLD_SIZE; y++)
                {
                    worldMap[x, y].sorroundingTiles = new SorroundingTiles<WorldTile>(new Point(x, y), worldMap);
                }
            }
            DesignateIsland(false);
            islands.ForEach(i => i.DetermineOceans());

            //Make the world nicer by generating features which make the landscape nicer
            Dictionary<WorldTileType, List<Bitmap>> natureFeatures = new Dictionary<WorldTileType, List<Bitmap>> {
                {
                    WorldTileType.TemperateForest, new List<Bitmap>{Resources.Forest1,Resources.Forest2,Resources.Forest3,Resources.Forest4 }
                },
                {
                    WorldTileType.Ocean, new List<Bitmap>{Resources.Ocean1,Resources.Ocean2,Resources.Ocean3}
                },
                {
                    WorldTileType.Desert, new List<Bitmap>{Resources.Desert1,Resources.Desert2,Resources.Desert3}
                },
                {
                    WorldTileType.TemperateGrassland, new List<Bitmap>{Resources.Grassland1,Resources.Grassland2,Resources.Grassland3}
                },
                {
                    WorldTileType.Rainforest, new List<Bitmap>{Resources.Rainforest1,Resources.Rainforest2,Resources.Rainforest3}
                },
                {
                    WorldTileType.Savanna, new List<Bitmap>{Resources.Savanna1, Resources.Savanna2, Resources.Savanna3, Resources.Savanna4}
                }
            };

            for (int i = 0; i < WORLD_SIZE*10; i++)
            {
                Point p = new Point(random.Next(WORLD_SIZE), random.Next(WORLD_SIZE));
                var type = worldMap[p.X, p.Y].type;
                if (natureFeatures.ContainsKey(type) && !features.Exists(f => f.Blocks(p)))
                {
                    features.Add(new WorldFeature(++WorldFeature.ID, p, natureFeatures[type].GetRandom()));
                }
            }

            //Generate nation
            nation = new Nation();

            //Generate cities
            //City number
            //capital: 1
            //large: 5
            //medium: 10
            //small: a lot - generate close to areas of high resources
            int[] numbers = new[] { 1, 4, 8 };
            //int[] numbers = new[] { 1, 2, 4 };
            int cityNum = random.Next(10, 15); //Small city number - TODO: Better way to solve overpopulation of cities
            //int cityNum = random.Next(2, 5); //Small city number - TODO: Better way to solve overpopulation of cities

            int total = numbers.Sum();
            for (int i = 0; i < total; i++)
            {
                int type;
                do
                {
                    type = random.Next(numbers.Length);
                } while (numbers[type] == 0);
                numbers[type]--;
                City city;
                Point p;
                bool v;
                do
                {
                    p = new Point(random.Next(WORLD_SIZE), random.Next(WORLD_SIZE));
                    v = (nation.cities.Count != 0 && (nation.cities.Select(c2 => AIUtility.Distance(c2.position, p)).Min() < 20));
                } while (TileTypeInfo(worldMap[p.X, p.Y].type).cityDensity < random.NextDouble() || v);
                int value = (int)(valueMap[p.X, p.Y] * 100);
                if (type == 0)
                {
                    city = new CapitalCity(p, value + 50, (int)(100 * agriculturalMap.Get(p)),(int)(100 * mineralMap.Get(p)));
                }
                else if (type == 1)
                {
                    city = new LargeCity(p, value + 30, (int)(100 * agriculturalMap.Get(p)),(int)(100 * mineralMap.Get(p)));
                }
                else
                {
                    city = new MediumCity(p, value + 20, (int)(100 * agriculturalMap.Get(p)),(int)(100 * mineralMap.Get(p)));
                }
                Trace.TraceInformation($"Generated {city.GetType().Name.Substring(0, 8)} \t Position: {p} \t Height: {worldMap.Get(p).height.ToString("0.00")}" +
                    $" \t Value: {city.value} \t Wealth: {city.Wealth} \t Type: {worldMap.Get(p).type} \t Population: {city.Population}");
                features.Add(city);
                nation.cities.Add(city);

            }

            //Generate small cities in valuable areas
            //Make the map into many small 10x10 chunks and find average value
            double[,] regionalValue = new double[WORLD_SIZE / 10, WORLD_SIZE / 10];
            for (int x = 0; x < WORLD_SIZE / 10; x++)
            {
                for (int y = 0; y < WORLD_SIZE / 10; y++)
                {
                    double average = 0;
                    for (int i = 0; i < 10; i++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            average += valueMap[x * 10 + i, y * 10 + j];
                        }
                    }
                    average /= 100;
                    regionalValue[x, y] = average;
                }
            }
            while (cityNum > 0)
            {
                int xS = random.Next(WORLD_SIZE / 10);
                int yS = random.Next(WORLD_SIZE / 10);
                if (regionalValue[xS, yS] > random.NextDouble() + 0.2)
                {
                    int i = random.Next(10);
                    int j = random.Next(10);
                    Point position1 = new Point(xS * 10 + i, yS * 10 + j);
                    if (IsLand(worldMap.Get(position1).type) && nation.cities.Select(city => AIUtility.Distance(city.position, position1)).Min() > 10)
                    {
                        City city = new SmallCity(position1, (int)(100 * regionalValue[xS, yS]), (int)(100 * agriculturalMap.Get(position1)), (int)(100 * mineralMap.Get(position1)));
                        features.Add(city);
                        nation.cities.Add(city);
                        Trace.TraceInformation($"Generated SmallCity \t Position: {position1} \t Value: {city.value} \t Wealth: {city.Wealth}");
                        cityNum--;
                    }
                }
            }

            nation.Intialise(new List<City>());


            int farms = nation.cities.Count + 20;
            while (farms > 0)
            {
                int xS = random.Next(WORLD_SIZE / 10);
                int yS = random.Next(WORLD_SIZE / 10);
                int _x = xS * 10 + random.Next(10);
                int _y = yS * 10 + random.Next(10);
                if(nation.islands.Exists(isl => isl.island == worldMap.Get(_x, _y).island))
                {
                    //Generate farms on agricultural hotspots
                    Point p = new Point(_x, _y);
                    if (agriculturalMap[_x, _y] > 0.4f && !world.features.Exists(f => f.Blocks(p)))
                    {
                        Farm farm = new Farm(p, (int)(agriculturalMap[_x, _y] * 100));
                        features.Add(farm);
                        nation.farms.Add(farm);
                        farms--;
                    }

                    //Generate mine on mineral hotspots
                    p = new Point(_x, _y);
                    if (mineralMap[_x, _y] > 0.7f && !world.features.Exists(f => f.Blocks(p)))
                    {
                        Mine mine = new Mine(p, (int)(mineralMap[_x, _y] * 100));
                        features.Add(mine);
                        nation.mines.Add(mine);
                    }
                }
            }



            //Add ports and other connections
            List<Island> toBuild = nation.DetermineNeededPorts();
            foreach (var island in toBuild)
            {
                Point p = island.border[random.Next(island.border.Count)].position;
                int tries = 10;
                while (nation.cities.Select(city => AIUtility.Distance(city.position, p)).Min() < 5 && tries > 0)
                {
                    tries--;
                    p = island.border[random.Next(island.border.Count)].position;
                }
                if (tries == 0)
                    continue;
                City port = new SmallCity(p, (int)(100 * valueMap[p.X, p.Y]) + 20, (int)(100 * agriculturalMap.Get(p)),(int)(100 * mineralMap.Get(p))) {
                    priority = 4,
                };
                features.Add(port);
                port.IsPort = true;
                nation.cities.Add(port);
                Trace.TraceInformation($"Port added at {p}");
            }
            nation.UpdateIslandData();
            nation.DeterminePortConnections();
            //Generate roads
            nation.GenerateRoads();

            //Add labels to each city
            nation.cities.ForEach(ci => features.Add(new WorldMapText(ci.name, ci.position)));

            //Generate missions
            for (int i = 0; i < WORLD_SIZE / 4; i++)
            {
                Point position = new Point(random.Next(0, WORLD_SIZE), random.Next(0, WORLD_SIZE));
                while (!IsLand(worldMap[position.X, position.Y].type))
                {
                    position = new Point(random.Next(0, WORLD_SIZE), random.Next(0, WORLD_SIZE));
                }
                //TODO: Find a better way to describe wealth
                Mission.Mission mission = campaign.DecideMission(worldMap[position.X, position.Y].type, (int)(valueMap[position.X, position.Y] * 100));
                actors.Add(new MissionWorldPlayer(mission, missionsCompleted + 1, position));
            }

            Trace.TraceInformation($"Created world in {(DateTime.Now - time).TotalMilliseconds}");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Temperature in °C Humidity 0 -> 1</returns>
        internal (double temperature, double humidity) GetWeather(Point point)
        {
            return (temperatureMap[point.X, point.Y], rainfallMap[point.X, point.Y]);
        }

        /// <summary>
        /// Scan the world to find islands
        /// </summary>
        /// <param name="one"></param>
        public void DesignateIsland(bool one = false)
        {
            List<WorldTile> toCheck = worldMap.Cast<WorldTile>().Where(t => t.island is null).ToList();
            for (int i = 0; i < toCheck.Count; i++)
            {
                WorldTile checking = toCheck[i];
                List<WorldTile> sameCheck = new List<WorldTile> { checking };
                while (sameCheck.Count != 0)
                {
                    checking = sameCheck[0];
                    sameCheck.Remove(checking);
                    Island toAssign = null;
                    bool type = IsLand(worldMap.Get(checking.position).type);
                    if (checking.position.X != 0)
                    {
                        WorldTile left = worldMap.Get(checking.position.Sub(1, 0));
                        if (IsLand(left.type) == type)
                            if (left.island != null)
                                toAssign = left.island;
                            else if (!sameCheck.Contains(left))
                                sameCheck.Add(left);
                    }
                    if (checking.position.Y != 0)
                    {
                        WorldTile top = worldMap.Get(checking.position.Sub(0, 1));
                        if (IsLand(top.type) == type)
                            if (top.island != null)
                                toAssign = top.island;
                            else if (!sameCheck.Contains(top))
                                sameCheck.Add(top);
                    }
                    if (checking.position.Y != WORLD_SIZE - 1)
                    {
                        WorldTile bottom = worldMap.Get(checking.position.Sub(0, -1));
                        if (IsLand(bottom.type) == type)
                            if (bottom.island != null)
                                toAssign = bottom.island;
                            else if (!sameCheck.Contains(bottom))
                                sameCheck.Add(bottom);
                    }
                    if (checking.position.X != WORLD_SIZE - 1)
                    {
                        WorldTile right = worldMap.Get(checking.position.Sub(-1, 0));
                        if (IsLand(right.type) == type)
                            if (right.island != null)
                                toAssign = right.island;
                            else if (!sameCheck.Contains(right))
                                sameCheck.Add(right);
                    }
                    if (checking.island is null)
                    {
                        if (toAssign is null)
                        {
                            toAssign = new Island(type);
                            islands.Add(toAssign);
                        }

                        checking.island = toAssign;
                        toAssign.tiles.Add(checking);
                    }
                }
                if (one)
                    break;
            }
            Trace.TraceInformation($"Found {islands.Count} islands!");
            foreach (var island in islands)
            {
                Trace.TraceInformation($"Island: {island.Name} - Size: {island.tiles.Count} Land: {island.land}");
            }
        }

        public void InitialisePlayer(HumanPlayer player)
        {
            foreach (var actor in actors)
            {
                if (actor is MissionWorldPlayer missionPlayer)
                {
                    player.possibleActions.Add(new StartMission(missionPlayer.mission, missionPlayer.difficulty, missionPlayer.WorldPosition));
                }
            }
            foreach (var city in nation.cities)
            {
                player.possibleActions.Add(new InteractCity(city));
            }
        }

        public Spell GainSpell<T>() where T : Spell
        {
            Spell spell = Spells.Where(s => s is T).FirstOrDefault();
            Spells.Remove(spell);
            return spell;
        }

        public Spell GainSpell(string name)
        {
            Spell spell = Spells.Where(s => s.name == name).FirstOrDefault();
            Spells.Remove(spell);
            return spell;
        }

        public double[,] MovementCost()
        {
            double[,] values = new double[WORLD_SIZE, WORLD_SIZE];
            for (int x = 0; x <= worldMap.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= worldMap.GetUpperBound(1); y++)
                {
                    values[x, y] = worldMap[x, y].movementCost;
                }
            }
            return values;
        }

        public void ProgressTime(TimeSpan timePassed, bool debug = false)
        {
            TimeSpan counter = new TimeSpan(0);
            int timeHours = (int)timePassed.TotalHours;
            for (int i = 0; i < timeHours / atmosphereTimeStep; i++)
            {
                CalculateAtmosphereTimeStep(debug);
                //CalculateAtmosphereTimeStep(debug);
                counter -= new TimeSpan(2, 0, 0);
                nation.WorldAction(1);
                actors.ForEach(a => a.WorldAction(1));
            }
        }

        public void Move(WorldPlayer actor, Point position)
        {
            Trace.TraceInformation($"{actor.Name} moved to {position} at {time.ToString("MM/dd/yyyy H:mm")}");
            actor.WorldPosition = position;
            //How does the renderer know that something has happened?
        }

        public void GenerateNewMission()
        {
            Point position = new Point(random.Next(0, WORLD_SIZE), random.Next(0, WORLD_SIZE));
            Mission.Mission mission = campaign.DecideMission(missionsCompleted + 1);
            actors.Add(new MissionWorldPlayer(mission, missionsCompleted + 1, position));
        }

        public static void NewWorld(double a = 0.40, double b = 0.14, double c = 0.31, int d = 4, double e = 0.26)
        {
            world = new World(a, b, c, 0, d, e);
        }

        private static (double cityDensity, double villageDensity) TileTypeInfo(WorldTileType type)
        {
            switch (type)
            {
                case WorldTileType.Ocean:
                    return (0, 0);

                case WorldTileType.River:
                    return (0, 0);

                case WorldTileType.SeaIce:
                    return (0, 0);

                case WorldTileType.TemperateGrassland:
                    return (0.01, 0.05);

                case WorldTileType.Rainforest:
                    return (0.005, 0.02);

                case WorldTileType.Desert:
                    return (0.005, 0.01);

                case WorldTileType.Tundra:
                    return (0.008, 0.01);

                case WorldTileType.TemperateForest:
                    return (0.02, 0.07);

                case WorldTileType.Savanna:
                    return (0.01, 0.04);

                case WorldTileType.Alpine:
                    return (0.003, 0.03);

                default:
                    throw new NotImplementedException();
            }
        }

        public static bool IsLand(WorldTileType type)
        {
            return !(type == WorldTileType.Ocean || type == WorldTileType.River || type == WorldTileType.SeaIce);
        }

        //TODO: Maybe I should change the temperature range to include negatives
        /// <summary>
        /// Converts temp from 0->1 to 0 -> 35 
        /// </summary>
        /// <param name="temperature"></param>
        /// <returns></returns>
        public static int GetCelsius(double temperature)
        {
            return (int)(35 * temperature);
        }
    }
}