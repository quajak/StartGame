using StartGame.MathFunctions;
using StartGame.Mission;
using StartGame.PlayerData;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace StartGame.World
{
    public class World
    {
        public const int WORLD_SIZE = 200;
        public const int WORLD_DIFFICULTY = 5;
        private static World world;
        public DateTime time = new DateTime(2000, 1, 1, 0, 0, 0, 0);
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

        public List<Spell> Spells => spells;

        private List<Spell> spells;
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
        private static Random random = new Random();

        public List<WorldPlayer> actors = new List<WorldPlayer>();
        public List<WorldFeature> features = new List<WorldFeature>();
        public List<Island> islands = new List<Island>();
        public List<Island> Oceans => islands.Where(i => !i.land).ToList();
        public Nation nation;

        private World(double a = 0.30, double b = 0.14, double c = 0.31, double Seed = 0, int octaves = 4, double persistance = 0.26)
        {
            Trace.TraceInformation($"World::ctor");
            world = this;
            spells = new List<Spell>()
            {
                new HealingSpell(5, 500),
                new EarthQuakeSpell(5, 5, 4500),
                new LightningBoltSpell(15, 4000),
                new DebuffSpell(2, 1, 5, 0, 800),
                new TeleportSpell(2, 0, 9600),
                new FireBall(8, 3, 4, 0, 2300)
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

            //Generate continents
            int continentNumber = 3;
            Point[] continentPositions = new Point[continentNumber];
            for (int i = 0; i < continentNumber; i++)
            {
                continentPositions[i] = new Point(random.Next(WORLD_SIZE), random.Next(WORLD_SIZE));
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
                    double agriculturalValue = rainfall * -2 * (rainfall - 1) * Math.Pow(Math.E, rainfall); //y = -2x(x-1)*e^x
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
                        type = WorldTileType.Ocean;
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

                    mineralMap[x, y] = minerals.Cut(0,1);
                    agriculturalMap[x, y] = agriculturalValue.Cut(0,1);
                    valueMap[x, y] = value.Cut(0,1);

                    worldMap[x, y] = new WorldTile(mHeight, type, cost, new Point(x, y));
                    costMap[x, y] = cost;
                }
            }
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

            //Generate nation
            nation = new Nation();

            //Generate cities
            //City number
            //capital: 1
            //large: 5
            //medium: 10
            //small: a lot - generate close to areas of high resources
            int[] numbers = new[] { 1, 5, 10 };
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
                do
                {
                    p = new Point(random.Next(WORLD_SIZE), random.Next(WORLD_SIZE));
                } while (TileTypeInfo(worldMap[p.X, p.Y].type).cityDensity < random.NextDouble());
                if (type == 0)
                {
                    city = new CapitalCity(p);
                }
                else if (type == 1)
                {
                    city = new LargeCity(p);
                }
                else
                {
                    city = new MediumCity(p);
                }
                Trace.TraceInformation($"Generated {city.GetType().Name} at {p} of type {worldMap.Get(p).type} and {worldMap.Get(p).height}");
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
            int cityNum = random.Next(10, 30);
            while (cityNum > 0)
            {
                int xS = random.Next(WORLD_SIZE / 10);
                int yS = random.Next(WORLD_SIZE / 10);
                if(regionalValue[xS, yS] > random.NextDouble() + 0.2)
                {
                    int i = random.Next(10);
                    int j = random.Next(10);
                    Point position1 = new Point(xS * 10 + i, yS * 10 + j);
                    if (IsLand(worldMap.Get(position1).type))
                    {
                        City city = new SmallCity(position1);
                        features.Add(city);
                        nation.cities.Add(city);
                        Trace.TraceInformation($"Added small city at {position1} where average value is {regionalValue[xS, yS]}");
                        cityNum--;
                    }
                }
            }

            //Add ports and other connections
            nation.Intialise(new List<City>());
            nation.DetermineConnections();
            List<Island> toBuild = nation.DetermineNeededPorts();
            foreach (var island in toBuild)
            {
                Point p = island.border[random.Next(island.border.Count)].position;
                City port = new SmallCity(p) {
                    priority = 4
                };
                features.Add(port);
                nation.cities.Add(port);
                Trace.TraceInformation($"Port added at {p}");
            }
            nation.UpdateIslandData();

            //Generate roads
            nation.GenerateRoads();

            //Generate missions
            for (int i = 0; i < WORLD_SIZE / 4; i++)
            {
                Point position = new Point(random.Next(0, WORLD_SIZE), random.Next(0, WORLD_SIZE));
                Mission.Mission mission = campaign.DecideMission(missionsCompleted + 1);
                actors.Add(new MissionWorldPlayer(mission, missionsCompleted + 1, position));
            }
        }

        public void DesignateIsland(bool one = false)
        {
            List<WorldTile> toCheck = worldMap.Cast<WorldTile>().Where(t => t.island is null).ToList();
            while (toCheck.Count != 0)
            {
                WorldTile checking = toCheck[0];
                toCheck.RemoveAt(0);
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
            Trace.TraceInformation($"Generated {islands.Count} islands!");
            foreach (var island in islands)
            {
                Trace.TraceInformation($"Island Size: {island.tiles.Count} Land: {island.land}");
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
        }

        public Spell GainSpell<T>() where T : Spell
        {
            Spell spell = spells.Where(s => s is T).FirstOrDefault();
            spells.Remove(spell);
            return spell;
        }

        public Spell GainSpell(string name)
        {
            Spell spell = spells.Where(s => s.name == name).FirstOrDefault();
            spells.Remove(spell);
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

        public void ProgressTime(TimeSpan timePassed)
        {
            time += timePassed;
            double actions = Math.Round(timePassed.TotalHours * 2, MidpointRounding.AwayFromZero) / 2;
            actors.ForEach(a => a.WorldAction(actions));
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
            return !(type == WorldTileType.Ocean || type == WorldTileType.River);
        }
    }
}