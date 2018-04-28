using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PlayerCreator;

namespace StartGame
{
    internal class Map
    {
        public readonly int width;
        public readonly int height;

        private double pathLength;

        private double averageTile = 0.5;
        private int hillTile = 0;
        private int flatTile = 0;
        private int waterTile = 0;

        public List<MapTile> goals;

        public MapTile[,] map;

        private List<Continent> continents;

        private Random generic = new Random();

        public List<Troop> troops = new List<Troop>();

        public const int creationTime = 500;

        public Map(int Width, int Height)
        {
            width = 31;
            height = 31;
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
            return $"Average tile: {averageTile}\nHilltile {hillTile}\nFlattile {flatTile}\nWatertile {waterTile}\n{type}\nPathtiles {pathLength}";
        }

        public string RawStats()
        {
            return $"{averageTile} | {hillTile} | {flatTile} | {waterTile}";
        }

        /// <summary>
        /// Initialises the map
        /// </summary>
        /// <param name="PerlinDiff">Determines the magnitude of the change between fields i.e. accuracy. Normally it has a value of 0.1</param>
        /// <param name="Seed">Determines the seed of the rng</param>
        /// <param name="HeightBias">Increases all values by this amount</param>
        public void SetupMap(Tuple<double, double, double> data)
        {
            (double PerlinDiff, double Seed, double HeightBias) = data;
            //Reset lists
            goals = new List<MapTile>();
            continents = new List<Continent>();
            troops = new List<Troop>();

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
                    mHeight = mHeight - HeightBias;
                    mHeight = mHeight < 0 ? 0 : mHeight; //Remove negative values
                    mHeight = mHeight > 1 ? 1 : mHeight; //Remove values larger than 1

                    MapTileTypeEnum mapTileEnum = MapTileTypeEnum.land;

                    if (mHeight < 0.2) mapTileEnum = MapTileTypeEnum.deepWater;
                    else if (mHeight < 0.4) mapTileEnum = MapTileTypeEnum.shallowWater;
                    else if (mHeight < 0.6) mapTileEnum = MapTileTypeEnum.land;
                    else if (mHeight < 0.8) mapTileEnum = MapTileTypeEnum.hill;
                    else if (mHeight <= 1.0) mapTileEnum = MapTileTypeEnum.mountain;

                    map[x, y] = new MapTile(new Point(x, y), new MapTileType
                    {
                        type = mapTileEnum
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
                if (continent.Type == MapTileTypeEnum.land && maxSize < continent.tiles.Count)
                {
                    maxSize = continent.tiles.Count;
                    maxContinent = continent;
                }
            }
            maxContinent.color = Color.Black;

            if (maxContinent.edges is null)
            {
                while (true)
                {
                    //Wait for thread to end
                };
            }
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
            //Skip code to create path when there are to few goals
            if (goals.Count <= 1) while (true)
                {
                    //Wait for thread to end
                };
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

            CalculateCost(ref map);

            //Path creation
            List<MapTile> toDraw = new List<MapTile>(goals);
            int positionsLength = toDraw.Count;
            IEnumerable<IEnumerable<int>> rawPossibilites = GetPermutations(Enumerable.Range(0, positionsLength).ToList(), positionsLength);
            List<List<int>> possibilites = rawPossibilites.ToList().ConvertAll(i => i.ToList());

            //Save shortest path
            double shortestLength = double.MaxValue;
            MapTile[] shortestGoals = new MapTile[positionsLength];
            //Calculate the length of each path option
            foreach (IEnumerable<int> possibility in possibilites)
            {
                //Generate order
                MapTile[] order = new MapTile[positionsLength];
                foreach (int index in possibility)
                {
                    order[index] = toDraw[index];
                }
                //Initialise variables for path finding
                //Copy map
                MapTile[,] localMap = new MapTile[map.GetUpperBound(0) + 1, map.GetUpperBound(1) + 1];

                for (int x = 0; x <= map.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= map.GetUpperBound(1); y++)
                    {
                        localMap[x, y] = map[x, y].Clone() as MapTile;
                    }
                }

                double length = 0;
                List<MapTile> localGoals = order.ToList();
                localGoals.RemoveAt(0);
                //Calculate path length
                FindPath(order.ToList(), ref localMap, ref length, localGoals.ToArray());

                //Check if shortest
                if (length < shortestLength)
                {
                    shortestLength = length;
                    shortestGoals = order;
                }
            }

            //Draw path
            pathLength = 0;
            //This is starting position
            toDraw.Remove(goals[0]);
            FindPath(shortestGoals.ToList(), ref map, ref pathLength, goals.ToArray());

            //Clean up path
            //CleanPath();
            return;
        }

        private void FindPath(List<MapTile> toDraw, ref MapTile[,] map, ref double pathLength, MapTile[] goals)
        {
            //Set goal of path
            foreach (MapTile goal in toDraw)
            {
                //Set starting position
                MapTile position = goals[0];
                pathLength++;
                map[position.position.X, position.position.Y].type.type = MapTileTypeEnum.path;
                //Repeat until you have found the goal
                while (true)
                {
                    //Find lowest distance neighbour
                    double cost = float.MaxValue;
                    MapTile lowest = null;
                    foreach (MapTile neighbour in position.neighbours.rawMaptiles)
                    {
                        //If neighbour is goal
                        if (neighbour.position == goal.position)
                        {
                            lowest = neighbour;
                            break;
                        }
                        //Check if neighbour is cheaper
                        int index = Array.IndexOf(neighbour.GoalIDs, goal.id);
                        if (index == -1)
                            //TODO: Find reason for this to occur
                            continue;
                        if (neighbour.Costs[index] != -1 && (cost > neighbour.Costs[index]))
                        {
                            cost = neighbour.Costs[index];
                            lowest = neighbour;
                        }
                    }
                    //Set lowest neighbour type to path if not path
                    if (map[lowest.position.X, lowest.position.Y].type.type != MapTileTypeEnum.path)
                    {
                        pathLength++;
                        map[lowest.position.X, lowest.position.Y].type.type = MapTileTypeEnum.path;
                        //Change cost of field
                        map[lowest.position.X, lowest.position.Y].Cost = 0.5;
                        map[lowest.position.X, lowest.position.Y].MovementCost = 0.5;
                    }
                    //If goal break while loop
                    if (position.position == goal.position) break;
                    //Else continue with this position
                    position = lowest;
                }
                //Recalculate the costs of tiles
                CalculateCost(ref map);
            }
        }

        private static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        private class Square<T>
        {
            public T top;
            public T bottom;
            public T left;
            public T right;
            public T[] raw;

            public Square(T Top, T Bottom, T Left, T Right)
            {
                top = Top;
                bottom = Bottom;
                left = Left;
                right = Right;
                raw = new T[] { Top, Bottom, Left, Right };
            }
        }

        private void CleanPath()
        {
            int x;
            int y;
            int width = 1;
            int height = 1;
            Square<bool> sides = new Square<bool>(true, true, true, true);
            for (int X = 0; X < map.GetUpperBound(0) - width; X++)
            {
                for (int Y = 0; Y < map.GetUpperBound(1) - height; Y++)
                {
                    width = 1;
                    height = 1;
                    sides.top = true;
                    sides.bottom = true;
                    sides.left = true;
                    sides.right = true;
                    y = Y;
                    x = X;
                    if (map[x, y].type.type != MapTileTypeEnum.path) continue;
                    while (true)
                    {
                        if (y != 0)
                        {
                            //Iterate top, if all true then enlarge
                            for (int xDiff = 0; xDiff < width; xDiff++)
                            {
                                if (map[x + xDiff, y - 1].type.type != MapTileTypeEnum.path || width == map.GetUpperBound(0)) //This should be false if there is path next
                                {
                                    sides.top = false;
                                    break;
                                }
                            }
                            if (sides.top)
                            {
                                height++; //Height is increased as we are looking for the upper limit for top
                                y--;
                                continue;
                            }
                        }
                        if (y + height != map.GetUpperBound(1))
                        {
                            //Iterate bottom, if all true then enlarge
                            for (int xDiff = 0; xDiff < width; xDiff++)
                            {
                                if (map[x + xDiff, y + height].type.type != MapTileTypeEnum.path || width == map.GetUpperBound(0))
                                {
                                    sides.bottom = false;
                                    break;
                                }
                            }
                            if (sides.bottom)
                            {
                                height++;
                                continue;
                            }
                        }
                        if (x != 0)
                        {
                            //Iterate left, if all true then enlarge
                            for (int yDiff = 0; yDiff < height; yDiff++)
                            {
                                if (map[x - 1, y + yDiff].type.type != MapTileTypeEnum.path || height == map.GetUpperBound(1))
                                {
                                    sides.left = false;
                                    break;
                                }
                            }
                            if (sides.left)
                            {
                                width++;
                                x--;
                                continue;
                            }
                        }
                        if (x + width != map.GetUpperBound(0))
                        {
                            //Iterate right, if all true then enlarge
                            for (int yDiff = 0; yDiff < height; yDiff++)
                            {
                                if (map[x + width, y + yDiff].type.type != MapTileTypeEnum.path || height == map.GetUpperBound(1))
                                {
                                    sides.right = false;
                                    break;
                                }
                            }
                            if (sides.right)
                            {
                                width++;
                                continue;
                            }
                        }
                        break;
                    }
                    //Clean it up
                    if (width >= 2 && height >= 2)
                    {
                        MapTileTypeEnum mapTileType;
                        Square<List<int>> paths = new Square<List<int>>(new List<int>(), new List<int>(), new List<int>(), new List<int>());
                        //Find top
                        if (y != 0)
                        {
                            //Iterate top
                            for (int xDiff = 0; xDiff < width; xDiff++)
                            {
                                mapTileType = map[x + xDiff, y - 1].type.type;
                                if (map[x + xDiff, y - 1].type.type == MapTileTypeEnum.path)
                                {
                                    paths.top.Add(xDiff);
                                }
                            }
                        }
                        if (y + height != map.GetUpperBound(1))
                        {
                            //Iterate bottom
                            for (int xDiff = 0; xDiff < width; xDiff++)
                            {
                                mapTileType = map[x + xDiff, y + height].type.type;
                                if (map[x + xDiff, y + height].type.type == MapTileTypeEnum.path)
                                {
                                    paths.bottom.Add(xDiff);
                                }
                            }
                        }
                        if (x != 0)
                        {
                            //Iterate left
                            for (int yDiff = 0; yDiff < height; yDiff++)
                            {
                                mapTileType = map[x - 1, y + yDiff].type.type;
                                if (map[x - 1, y + yDiff].type.type == MapTileTypeEnum.path)
                                {
                                    paths.left.Add(yDiff);
                                }
                            }
                        }
                        if (x + width != map.GetUpperBound(0))
                        {
                            //Iterate right
                            for (int yDiff = 0; yDiff < height; yDiff++)
                            {
                                mapTileType = map[x + width, y + yDiff].type.type;
                                if (map[x + width, y + yDiff].type.type == MapTileTypeEnum.path)
                                {
                                    paths.right.Add(yDiff);
                                }
                            }
                        }
                        //Found all exits
                        //Clean
                        for (int xOffset = 0; xOffset < width; xOffset++)
                        {
                            for (int yOffset = 0; yOffset < height; yOffset++)
                            {
                                map[x + xOffset, y + yOffset].type.type = MapTileTypeEnum.land;
                            }
                        }
                        //Find max top, bottom + max left, right
                        Square<int> max = new Square<int>(0, 0, 0, 0);
                        if (paths.top.Count != 0) max.top = 0;
                        else max.top = Min(paths.right, paths.left);
                        if (paths.right.Count != 0) max.right = width - 1;
                        else max.right = Max(paths.top, paths.bottom);
                        if (paths.bottom.Count != 0) max.bottom = height - 1;
                        else max.bottom = Max(paths.right, paths.left);
                        if (paths.left.Count != 0) max.left = 0;
                        else max.left = Min(paths.top, paths.bottom);
                        //Calculate x offset to draw the top to bottom line
                        int topPos = 0;
                        if (paths.top.Count >= 1 && paths.bottom.Count >= 1) topPos = Min(paths.top, paths.bottom);
                        else if (paths.top.Count >= 1) topPos = paths.top.Min();
                        else if (paths.bottom.Count >= 1) topPos = paths.bottom.Min();
                        else topPos = max.left;

                        //Draw the top to bottom
                        for (int yDiff = 0; yDiff < max.bottom + 1 - max.top; yDiff++)
                        {
                            map[x + topPos, y + yDiff + max.top].type.type = MapTileTypeEnum.path;
                        }
                        //Draw the left to right
                        for (int xDiff = 0; xDiff < max.right + 1 - max.left; xDiff++)
                        {
                            map[x + max.left + xDiff, y + max.bottom].type.type = MapTileTypeEnum.path;
                        }
                    }
                    else
                    {
                        //Nothing changed
                    }
                    //Reset
                    width = 1;
                    height = 1;
                }
            }
        }

        private int Min(List<int> list1, List<int> list2)
        {
            if (list1.Count != 0 && list2.Count != 0) return Math.Min(list1.Min(), list2.Min());
            else if (list1.Count == 0) return list2.Min();
            else if (list2.Count == 0) return list1.Min();
            else throw new Exception("Both sides are empty!");
        }

        private int Max(List<int> list1, List<int> list2)
        {
            if (list1.Count != 0 && list2.Count != 0) return Math.Max(list1.Max(), list2.Max());
            else if (list1.Count == 0) return list2.Max();
            else if (list2.Count == 0) return list1.Max();
            else throw new Exception("Both sides are empty!");
        }

        private void CalculateCost(ref MapTile[,] map)
        {
            //Set cost for all tiles
            foreach (MapTile goal in goals)
            {
                int position = Array.IndexOf(map[goal.position.X, goal.position.Y].GoalIDs, goal.id);
                map[goal.position.X, goal.position.Y].Costs[position] = 0; //This is the cost of the path
                AddNeighbours(goal, goal.id);
            }
        }

        private void AddNeighbours(MapTile pos, string id)
        {
            foreach (MapTile neighbour in pos.neighbours.rawMaptiles)
            {
                //Don't go through water or hills
                if (neighbour.type.type != MapTileTypeEnum.path && neighbour.type.type != MapTileTypeEnum.land) continue;
                //If already has value
                int index = Array.IndexOf(map[neighbour.position.X, neighbour.position.Y].GoalIDs, id);
                if(index == -1)
                {
                    //TODO: How can this be
                    continue;
                }
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

        #region Drawing

        public Image background;
        private Image rawBackground; //No troops

        /// <summary>
        /// Function which generates a Bitmap from the map without any troops or overlay
        /// </summary>
        /// <param name="Width">Width of the bitmap</param>
        /// <param name="Height">Height of the bitmap</param>
        /// <param name="Debug">Will show cost of field from a specific goal</param>
        /// <param name="size">Size of field in pixel</param>
        /// <param name="continentAlpha">Sets alpha of continent alpha, if 0 they are not shown</param>
        /// <param name="showGoal">Which goal will be selected for debug</param>
        /// <param name="colorAlpha">Sets alpha of field color</param>
        /// <param name="showInverseHeight">Determine if height should be inversed for grey overlay color</param>
        /// <returns></returns>
        public Bitmap DrawMapBackground(int Width, int Height, bool Debug = false, int size = 20, int continentAlpha = 100, int showGoal = 1, int colorAlpha = 255, bool showInverseHeight = false)
        {
            averageTile = 0.5;
            hillTile = 0;
            flatTile = 0;
            waterTile = 0;
            Font font = new Font(FontFamily.GenericSansSerif, 8);
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
                            //Adjust alpha of color
                            if (colorAlpha != 255)
                                c = Color.FromArgb(colorAlpha, c.R, c.G, c.B);

                            if (showInverseHeight)
                                b = Color.FromArgb(255 - b.A, 0, 0, 0);

                            d = map[x, y].continent.color;
                            d = Color.FromArgb(continentAlpha, d.R, d.G, d.B);
                        }

                        if (Debug)
                        {
                            if (!(map[x, y].Costs is null) && map[x, y].Costs.Length > showGoal)
                            {
                                string s = map[x, y].Costs[showGoal].ToString();
                                g.FillRectangle(Brushes.Black, x * size, y * size, size, size);
                                g.DrawString(s, font, Brushes.White, new Point(x * size, y * size));
                            }
                        }
                        else
                        {
                            g.FillRectangle(new SolidBrush(c), x * size, y * size, size, size);
                            g.FillRectangle(new SolidBrush(d), x * size, y * size, size, size);
                            g.FillRectangle(new SolidBrush(b), x * size, y * size, size, size);
                        }
                    }
                }
                rawBackground = mapBackground;
            }
            mapBackground = new Bitmap(rawBackground);
            using (Graphics g = Graphics.FromImage(mapBackground))
            {
                //Draw troops
                foreach (Troop troop in troops)
                {
                    g.DrawImage(troop.image, troop.position.X * size, troop.position.Y * size, 20, 20);
                }
            }

            background = mapBackground;
            return mapBackground;
        }

        public Image DrawTroops(int size = 20)
        {
            Image img = new Bitmap(rawBackground);
            using (Graphics g = Graphics.FromImage(img))
            {
                foreach (Troop troop in troops)
                {
                    g.DrawImage(troop.image, troop.position.X * size, troop.position.Y * size, 20, 20);
                }
            }
            background = img;
            return img;
        }

        #endregion Drawing

        /// <summary>
        /// Function which finds spawnpoints for one or more troops
        /// </summary>
        /// <param name="playerNumber">Determines how many co-ordinates should be returned</param>
        /// <param name="spawnType">Deterines how the co-ordinates should relate to each other and the map</param>
        /// <returns></returns>
        public List<Point> DeterminSpawnPoint(int playerNumber, SpawnType spawnType)
        {
            List<Point> toReturn = new List<Point>();
            switch (spawnType)
            {
                case SpawnType.road:

                    if (goals.Count <= 1)
                    {
                        FindRandomLandTile(playerNumber, ref toReturn);
                        break;
                    }
                    //Determin which road spawn point to start
                    List<MapTile> _goals = new List<MapTile>(goals);
                    while (_goals.Count != 0 && playerNumber != 0)
                    {
                        MapTile pathPoint = _goals[generic.Next(_goals.Count)];
                        _goals.Remove(pathPoint);
                        toReturn.Add(pathPoint.position);
                        playerNumber--;
                        if (playerNumber == 0) break;
                        foreach (MapTile possible in pathPoint.neighbours.rawMaptiles.Where(m => m.type.type == MapTileTypeEnum.path))
                        {
                            toReturn.Add(possible.position);
                            playerNumber--;
                            if (playerNumber == 0) break;
                        }
                    }
                    break;

                case SpawnType.random:
                    for (int i = 0; i < playerNumber; i++)
                    {
                        Point p = new Point();
                        while (true)
                        {
                            p = new Point(generic.Next(map.GetUpperBound(0),
                            generic.Next(map.GetUpperBound(1))));
                            //Determine is viable
                            if (!troops.Exists(t => t.position.X == p.X &&
                                 t.position.Y == p.Y) &&
                                 !toReturn.Exists(c => c.X == p.X && c.Y == p.Y))
                                break;
                        }
                        toReturn.Add(p);
                    }
                    break;

                case SpawnType.randomLand:
                    FindRandomLandTile(playerNumber, ref toReturn);
                    break;

                default:
                    break;
            }
            return toReturn;
        }

        private void FindRandomLandTile(int playerNumber, ref List<Point> toReturn)
        {
            for (int i = 0; i < playerNumber; i++)
            {
                Point point;
                while (true)
                {
                    point = new Point(generic.Next(map.GetUpperBound(0)), generic.Next(map.GetUpperBound(1)));
                    if ((map[point.X, point.Y].type.type == MapTileTypeEnum.land
                        || map[point.X, point.Y].type.type == MapTileTypeEnum.path) &&
                        !troops.Exists(t => t.position.X == point.X && t.position.Y == point.Y))
                    {
                        toReturn.Add(point);
                        break;
                    }
                }
            }
        }

        #region Overlay

        public List<OverlayObject> overlayObjects = new List<OverlayObject>();

        public Image DrawOverlay(int Width, int Height, bool keepAll = false)
        {
            Image image = new Bitmap(Width, Height);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(Color.Transparent);
                foreach (var obj in overlayObjects)
                {
                    if (obj is OverlayRectangle)
                    {
                        OverlayRectangle rect = obj as OverlayRectangle;
                        g.DrawRectangle(new Pen(rect.color), rect.x, rect.y, rect.width, rect.height);
                    }
                    else if (obj is OverlayText)
                    {
                        OverlayText txt = obj as OverlayText;
                        g.DrawString(txt.text, SystemFonts.DefaultFont, new SolidBrush(txt.color),
                            new PointF(txt.x, txt.y));
                    }
                }
                if (!keepAll)
                    overlayObjects = overlayObjects.Where(o => !o.once).ToList();
            }

            return image;
        }

        #endregion Overlay
    }

    internal enum SpawnType
    { road, random, randomLand };

    internal class OverlayObject
    {
        public int x;
        public int y;
        public bool once;

        public OverlayObject(int X, int Y, bool Once = true)
        {
            x = X;
            y = Y;
            once = Once;
        }
    }

    internal class OverlayRectangle : OverlayObject
    {
        public int width;
        public int height;
        public Color color;
        public bool filled;

        public OverlayRectangle(int X, int Y, int Width, int Height, Color Color,
            bool Filled, bool Once = true) : base(X, Y, Once)
        {
            width = Width;
            height = Height;
            color = Color;
            filled = Filled;
        }
    }

    internal class OverlayText : OverlayObject
    {
        public Color color;
        public string text;

        public OverlayText(int X, int Y, Color Color, string Text, bool Once = true) : base(X, Y, Once)
        {
            color = Color;
            text = Text;
        }
    }
}