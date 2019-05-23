using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using StartGame.GameMap;


namespace StartGame.PlayerData
{
    internal static class AIUtility
    {
        public static IEnumerable<Point> GetCircle(int radius, Point center, Map map, bool withCenter)
        {
            for (int x = 0; x < radius * 2 + 1; x++)
            {
                for (int y = 0; y < radius * 2 + 1; y++)
                {
                    //Check in bounds
                    Point point = new Point(x + center.X - radius, y + center.Y - radius);
                    if ((point.X < 0 || point.X >= map.map.GetUpperBound(0) - 1) || (point.Y < 0 || point.Y >= map.map.GetUpperBound(1) - 1)) continue;

                    if (!withCenter && point == center) continue;

                    int dis = Distance(point, center);
                    if (dis <= radius)
                    {
                        //Damage fields
                        yield return point;
                    }
                }
            }
        }

        public static int Distance(MapTile A, MapTile B)
        {
            Contract.Assert(A != null);
            Contract.Assert(B != null);

            return Distance(A.position, B.position);
        }

        public static int Distance(Point A, Point B)
        {
            return Math.Abs(A.X - B.X) + Math.Abs(A.Y - B.Y);
        }

        public static Point FindClosestField(DistanceGraphCreator distanceGraph, Point goal, double actionPoints, Map map, FieldOptimiser chooser)
        {
            int closestDistance = int.MaxValue;
            List<(Point point, double points, double height)> closestPoints = new List<(Point point, double points, double height)>();
            for (int x = 0; x <= distanceGraph.graph.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= distanceGraph.graph.GetUpperBound(1); y++)
                {
                    int playerDis = Distance(goal, new Point(x, y));
                    if (distanceGraph.graph[x, y] <= actionPoints
                        && map.map[x, y].free)
                    {
                        if (playerDis < closestDistance)
                        {
                            closestDistance = playerDis;
                            closestPoints.Clear();
                            closestPoints.Add((new Point(x, y), distanceGraph.graph[x, y], map.map[x, y].Height));
                        }
                        else if (playerDis == closestDistance)
                        {
                            closestPoints.Add((new Point(x, y), distanceGraph.graph[x, y], map.map[x, y].Height));
                        }
                    }
                }
            }
            if (closestPoints.Count == 0)
            {
                string neighbourDescriber = "";
                foreach (var tile in map.map[goal.X, goal.Y].neighbours.rawMaptiles)
                {
                    neighbourDescriber += tile.ToString() + " ";
                }
                throw new Exception($"Did not find any close field! Action points: {actionPoints} Point: {goal} Neighbours: {neighbourDescriber}");
            }
            return chooser.Invoke(closestPoints);
        }

        public static List<Point> GetFields(int[] point, DistanceGraphCreator distanceGraph)
        {
            Contract.Assert(point.Length == 2);

            return GetFields(new Point(point[0], point[1]), distanceGraph);
        }

        public static List<Point> GetFields(Point point, DistanceGraphCreator distanceGraph)
        {
            List<Point> points = new List<Point>();
            if (point.Y != 0)
                points.Add(new Point(point.X, point.Y - 1));
            if (point.Y != distanceGraph.graph.GetUpperBound(1))
                points.Add(new Point(point.X, point.Y + 1));
            if (point.X != 0)
                points.Add(new Point(point.X - 1, point.Y));
            if (point.X != distanceGraph.graph.GetUpperBound(0))
                points.Add(new Point(point.X + 1, point.Y));
            return points;
        }
    }
}