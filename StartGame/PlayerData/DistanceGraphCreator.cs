using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StartGame.GameMap;

using static StartGame.MainGameWindow;

namespace StartGame.PlayerData
{
    internal class DistanceGraphCreator
    {
        private const MovementType walk = MovementType.walk;
        private readonly Player player;
        private readonly int sX;
        private readonly int sY;
        private readonly Map map;
        public double[,] graph;
        private readonly bool allowWater;

        /// <summary>
        /// Create a double map of the distance cost to get to field from a certain point
        /// </summary>
        /// <param name="Map"></param>
        /// <param name="AllowWater"></param>
        public DistanceGraphCreator(Player Player, int SX, int SY, Map Map, bool AllowWater)
        {
            map = Map;
            player = Player;
            sX = SX;
            sY = SY;
            allowWater = AllowWater;
        }

        public DistanceGraphCreator(Player Player, Point S, Map Map, bool AllowWater)
        {
            player = Player;
            map = Map;
            sX = S.X;
            sY = S.Y;
            allowWater = AllowWater;
        }

        private double[,] mapValues;
        private bool[,] free;

        public void CreateGraph()
        {
            lock (this) lock (map)
                {
                    //I think the +1 on size was done so some checks can be avoided. Is most likely a hack
                    graph = new double[map.map.GetUpperBound(0)+ 1, map.map.GetUpperBound(1) + 1];
                    mapValues = new double[map.map.GetUpperBound(0) + 1, map.map.GetUpperBound(1) + 1];
                    free = new bool[map.map.GetUpperBound(0)+ 1, map.map.GetUpperBound(1) + 1];
                    for (int x = 0; x <= mapValues.GetUpperBound(0); x++)
                    {
                        for (int y = 0; y <= mapValues.GetUpperBound(1); y++)
                        {
                            mapValues[x, y] = map.map[x, y].MovementCost;
                            free[x, y] = map.map[x, y].free;
                            graph[x, y] = 100;
                        }
                    }

                    graph[sX, sY] = 0;

                    List<int[]> toCheck = new List<int[]>() { new int[] { sX, sY } };
                    Point start = new Point(sX, sY);
                    MapTile startTile = map.map.Get(start);
                    while (toCheck.Count != 0)
                    {
                        int[] checking = toCheck[0];
                        toCheck.Remove(checking);

                        List<int[]> sorrounding = new List<int[]>();
                        //Top
                        if (checking[1] != 0)
                            sorrounding.Add(new int[] { checking[0], checking[1] - 1 });
                        //Bottom
                        if (checking[1] != mapValues.GetUpperBound(1))
                            sorrounding.Add(new int[] { checking[0], checking[1] + 1 });
                        //Left
                        if (checking[0] != 0)
                            sorrounding.Add(new int[] { checking[0] - 1, checking[1] });
                        //Right
                        if (checking[0] != mapValues.GetUpperBound(0))
                            sorrounding.Add(new int[] { checking[0] + 1, checking[1] });

                        List<int[]> toAdd = new List<int[]>();
                        foreach (int[] field in sorrounding)
                        {
                            if (!allowWater && (map.map[field[0], field[1]].type.type == MapTileTypeEnum.deepWater
                                    || map.map[field[0], field[1]].type.type == MapTileTypeEnum.shallowWater))
                            {
                            }
                            else if (!free[field[0], field[1]])
                            {
                            }
                            else if (graph[field[0], field[1]] > 0)
                            {
                                double newCost;
                                if (player.constantMovementFunction)
                                {
                                    newCost = graph[checking[0], checking[1]] + mapValues[field[0], field[1]];
                                }
                                else
                                {
                                    MapTile[] path = GeneratePath(new Point(field[0], field[1]), start, map);

                                    newCost = graph.Get(checking) + player.CalculateStep(startTile, map.map.Get(checking), map.map.Get(field), path.Length - 1, walk);

                                }
                                if (newCost < graph[field[0], field[1]])
                                {
                                    graph[field[0], field[1]] = newCost;
                                    toAdd.Add(field);
                                }
                            }
                        }
                        toCheck.AddRange(toAdd);
                    }
                }
        }

        public MapTile[] GeneratePath(Point start, Point end, Map map)
        {
            List<MapTile> path = new List<MapTile>();
            //DEBUG
            int counter = 0;

            Point active = start;
            path.Add(map.map.Get(active));

            while (active != end)
            {
                //DEBUG
                if (++counter == 100) throw new Exception("Something went wrong!");

                //Find field with lowest value for graph (lowest movement cost from start)
                List<Point> fields = AIUtility.GetFields(active, this);
                active = fields.Aggregate((best, field) => {
                    if (path.Exists(f => f.position == field)) return best;
                    double b = graph[best.X, best.Y]; //Do not use Get method for optimization
                    double n = graph[field.X, field.Y];
                    if (n == b)
                    {
                        int bestDistance = AIUtility.Distance(best, end);
                        int fieldDistance = AIUtility.Distance(field, end);
                        //if they have the same cost take the one which is closer if they are the same distance take the newer
                        return bestDistance >= fieldDistance ? field : best;
                    }
                    else
                    {
                        if (b > n)
                        {
                            return field;
                        }
                        else
                        {
                            return best;
                        }
                    }
                });

                path.Add(map.map.Get(active));
            }
            return path.ToArray();
        }
    }
}