using System;
using System.Collections.Generic;
using System.Drawing;

namespace StartGame
{
    internal class Map
    {
        private int width;
        private int height;

        private double averageTile = 0.5;
        private int hillTile = 0;
        private int flatTile = 0;
        private int waterTile = 0;

        private List<MapTile> goals;

        public MapTile[,] map;

        private List<Continent> continents;

        public Map(int Width, int Height)
        {
            width = Width;
            height = Height;
        }

        public string Stats()
        {
            string type = "";
            if ((double)hillTile / (flatTile + hillTile + waterTile) > 0.2)
            {
                type = "Hilly";
            }
            else if ((double)waterTile / (flatTile + hillTile + waterTile) > 0.5)
            {
                type = "Swamp";
            }
            else type = "Flat";
            return $"Average tile: {averageTile}\nHilltile {hillTile}\nFlattile {flatTile}\nWatertile {waterTile}\n{type}";
        }

        public string RawStats()
        {
            return $"{averageTile} | {hillTile} | {flatTile} | {waterTile}";
        }

        public void SetupMap(double PerlinDiff, double Seed, double HeightBias, double Zoom)
        {
            map = new MapTile[width, height];

            PerlinNoise p = new PerlinNoise();

            //Generate tiles
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //Map generation
                    //Calculate perlin
                    double mHeight = p.perlin(x * PerlinDiff, y * PerlinDiff, Seed);

                    //Modify perlin value for more extremes
                    mHeight = mHeight * Zoom - HeightBias;
                    mHeight = mHeight < 0 ? 0 : mHeight; //Remove negative values
                    mHeight = mHeight > 1 ? 1 : mHeight; //Remove values larger than 1

                    Color c = Color.Black;

                    MapTileTypeEnum mapTileEnum = MapTileTypeEnum.land;

                    if (mHeight < 0.2) mapTileEnum = MapTileTypeEnum.deepWater;
                    else if (mHeight < 0.4) mapTileEnum = MapTileTypeEnum.shallowWater;
                    else if (mHeight < 0.6) mapTileEnum = MapTileTypeEnum.land;
                    else if (mHeight < 0.8) mapTileEnum = MapTileTypeEnum.hill;
                    else if (mHeight <= 1.0) mapTileEnum = MapTileTypeEnum.mountain;

                    map[x, y] = new MapTile(new Point(x, y), new MapTileType
                    {
                        type = mapTileEnum,
                        color = c
                    }, mHeight);
                }
            }

            //Add neighbours as they are now initialised
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    map[x, y].GetNeighbours(this);
                }
            }

            //Generate continents
            int counter = 0;
            Random colorGenerator = new Random();
            continents = new List<Continent>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y].continent == null)
                    {
                        Continent continent = new Continent();
                        map = continent.NewContinent(this, new Point(x, y), counter++.ToString(), colorGenerator).map;
                        continents.Add(continent);
                    }
                }
            }

            //Find largest land continent for road TODO: Do this for all continents of certain size
            int maxSize = 0;
            Continent maxContinent = new Continent();
            foreach (Continent continent in continents)
            {
                if (maxSize < continent.tiles.Count)
                {
                    maxSize = continent.tiles.Count;
                    maxContinent = continent;
                }
            }
            maxContinent.color = Color.Black;
            //TODO: For edges in continents find all the different zones, not only side
            //TODO: Create a road between each of the zones
            goals = new List<MapTile>();
            while (true)
            {
                if (maxContinent.edges.top.Count != 0)
                {
                    MapTile toAdd = maxContinent.edges.top[colorGenerator.Next(maxContinent.edges.top.Count)];
                    if (!goals.Contains(toAdd))
                    {
                        goals.Add(toAdd);
                        break;
                    }
                }
                else break;
            }
            while (true)
            {
                if (maxContinent.edges.bottom.Count != 0)
                {
                    MapTile toAdd = maxContinent.edges.bottom[colorGenerator.Next(maxContinent.edges.bottom.Count)];
                    if (!goals.Contains(toAdd))
                    {
                        goals.Add(toAdd);
                        break;
                    }
                }
                else break;
            }
            while (true)
            {
                if (maxContinent.edges.left.Count != 0)
                {
                    MapTile toAdd = maxContinent.edges.left[colorGenerator.Next(maxContinent.edges.left.Count)];
                    if (!goals.Contains(toAdd))
                    {
                        goals.Add(toAdd);
                        break;
                    }
                }
                else break;
            }
            while (true)
            {
                if (maxContinent.edges.right.Count != 0)
                {
                    MapTile toAdd = maxContinent.edges.right[colorGenerator.Next(maxContinent.edges.right.Count)];
                    if (!goals.Contains(toAdd))
                    {
                        goals.Add(toAdd);
                        break;
                    }
                }
                else break;
            }
            for (int x = 0; x <= map.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= map.GetUpperBound(1); y++)
                {
                    map[x, y].InitialiseDistances(goals.Count);
                    map[x, y].GoalIDs = goals.ConvertAll(g => g.id).ToArray();
                    for (int i = 0; i < goals.Count; i++)
                    {
                        map[x, y].Costs[i] = -1;
                    }
                }
            }
            CalculateCost();

            List<MapTile> toDraw = new List<MapTile>(goals);
            toDraw.Remove(goals[0]);
            foreach (MapTile goal in toDraw)
            {
                MapTile position = goals[0];
                while (true)
                {
                    //Find lowest distance neighbour
                    double cost = float.MaxValue;
                    MapTile lowest = null;
                    foreach (MapTile neighbour in position.neighbours.rawMaptiles)
                    {
                        if (neighbour.position == goal.position)
                        {
                            lowest = neighbour;
                            break;
                        }
                        //TODO: Rather than try and catch use contains key
                        int index = Array.IndexOf(neighbour.GoalIDs, goal.id);
                        if (neighbour.Costs[index] != -1 && (cost > neighbour.Costs[index]))
                        {
                            cost = neighbour.Costs[index];
                            lowest = neighbour;
                        }
                    }
                    //Set neighbour type to path
                    map[lowest.position.X, lowest.position.Y].type.type = MapTileTypeEnum.path;
                    //Change cost of field
                    map[lowest.position.X, lowest.position.Y].Cost = 0.5;
                    if (position.position == goal.position) break;
                    position = lowest;
                }
                //Recalculate the costs of tiles
                CalculateCost();
            }
        }

        private void CalculateCost()
        {
            //Set cost for all tiles
            foreach (MapTile goal in goals)
            {
                int position = Array.IndexOf(map[goal.position.X, goal.position.Y].GoalIDs, goal.id);
                map[goal.position.X, goal.position.Y].Costs[position] = 0;
                AddNeighbours(goal, goal.id);
            }
        }

        private void AddNeighbours(MapTile pos, string id)
        {
            foreach (MapTile neighbour in pos.neighbours.rawMaptiles)
            {
                if (neighbour.type.type != pos.type.type) continue;
                //If already has value
                int index = Array.IndexOf(map[neighbour.position.X, neighbour.position.Y].GoalIDs, id);
                double disCost = map[neighbour.position.X, neighbour.position.Y].Costs[index];
                if (disCost != -1)
                {
                    if (disCost > pos.Costs[index] + neighbour.Cost)
                    {
                        map[neighbour.position.X, neighbour.position.Y].Costs[index] = pos.Costs[index] + neighbour.Cost;
                        AddNeighbours(neighbour, id);
                    }
                }
                else
                {
                    //Else give value
                    map[neighbour.position.X, neighbour.position.Y].Costs[index] = pos.Costs[index] + neighbour.Cost;
                    AddNeighbours(neighbour, id);
                }
            }
        }

        public Bitmap DrawMapBackground(int Width, int Height, bool Debug = false, int size = 20, int continentAlpha = 100, int showGoal = 1)
        {
            averageTile = 0.5;
            hillTile = 0;
            flatTile = 0;
            waterTile = 0;
            Font font = new Font(FontFamily.GenericSansSerif, 6);
            Bitmap mapBackground = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(mapBackground))
            {
                g.Clear(Color.Transparent);
                for (int x = 0; x < Math.Sqrt(map.Length); x++)
                {
                    for (int y = 0; y < Math.Sqrt(map.Length); y++)
                    {
                        //Average tile
                        averageTile = (averageTile + map[x, y].height) / 2;

                        Color b = Color.Black;
                        Color c = Color.Black;
                        Color d = Color.Black;
                        if (Debug)
                        {
                            c = Color.FromArgb((int)(map[x, y].height * 255), 0, 0, 0);
                        }
                        else
                        {
                            //The five different types of land are cut of evenly - due to the fact that perlin has gaussian distribution
                            //there will be a higher amount of flat land
                            if (map[x, y].height < 0.2)
                            {
                                //Deep water
                                b = Color.FromArgb((int)((1 - map[x, y].height) * 255), 0, 0, 0);
                                c = Color.MediumBlue;
                                waterTile += 2;
                            }
                            else if (map[x, y].height < 0.4)
                            {
                                //Shallow water
                                b = Color.FromArgb((int)((1 - map[x, y].height) * 255), 0, 0, 0);
                                c = Color.Blue;
                                waterTile++;
                            }
                            else if (map[x, y].height < 0.6)
                            {
                                //Flat land
                                b = Color.FromArgb((int)(map[x, y].height * 255), 0, 0, 0);
                                c = Color.Green;
                                flatTile++;
                            }
                            else if (map[x, y].height < 0.8)
                            {
                                //Hills
                                b = Color.FromArgb((int)(map[x, y].height * 255), 0, 0, 0);
                                c = Color.LightGray;
                                hillTile++;
                            }
                            else if (map[x, y].height <= 1)
                            {
                                //Mountains
                                b = Color.FromArgb((int)(map[x, y].height * 255), 0, 0, 0);
                                c = Color.DarkGray;
                                hillTile += 2;
                            }
                            if (map[x, y].type.type == MapTileTypeEnum.path)
                            {
                                c = Color.SandyBrown;
                            }
                            d = map[x, y].continent.color;
                            d = Color.FromArgb(continentAlpha, d.R, d.G, d.B);
                        }
                        g.FillRectangle(new SolidBrush(c), x * size, y * size, size, size);
                        if (!Debug) g.FillRectangle(new SolidBrush(b), x * size, y * size, size, size);
                        if (!Debug) g.FillRectangle(new SolidBrush(d), x * size, y * size, size, size);

                        if (Debug)
                        {
                            try
                            {
                                string s = map[x, y].Costs[showGoal].ToString();
                                g.FillRectangle(Brushes.Black, x * size, y * size, size, size);
                                g.DrawString(s, font, Brushes.White, new Point(x * size, y * size));
                            }
                            catch (System.Collections.Generic.KeyNotFoundException)
                            {
                            }
                        }
                    }
                }
            }
            return mapBackground;
        }
    }
}