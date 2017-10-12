using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace StartGame
{
    internal class Map
    {
        private int width;
        private int height;

        private double pathLength;

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
            return $"Average tile: {averageTile}\nHilltile {hillTile}\nFlattile {flatTile}\nWatertile {waterTile}\n{type}\nPathtiles {pathLength}";
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
                        localMap[x, y] = (MapTile)map[x, y].Clone();
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
            CleanPath();
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
                                if (map[x, y].Costs.Length > showGoal)
                                {
                                    string s = map[x, y].Costs[showGoal].ToString();
                                    g.FillRectangle(Brushes.Black, x * size, y * size, size, size);
                                    g.DrawString(s, font, Brushes.White, new Point(x * size, y * size));
                                }
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