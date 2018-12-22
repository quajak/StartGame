using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.AI
{
    public class AStar
    {
        public static Point[] FindOptimalRoute(double[,] cost, Point start, Point end)
        {
            double[,] fields = new double[cost.GetUpperBound(0) + 1, cost.GetUpperBound(1) + 1];
            for (int x = 0; x <= fields.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= fields.GetUpperBound(1); y++)
                {
                    fields[x, y] = 10000;
                }
            }
            List<Point> toCheck = new List<Point> { start };
            fields[start.X, start.Y] = 0;
            while (toCheck.Count != 0)
            {
                Point checking = toCheck[0];
                toCheck.RemoveAt(0);
                //left
                if(checking.X != 0)
                {
                    Point left = checking.Add(new Point(-1, 0));
                    if (fields.Get(left) > fields.Get(checking) + cost.Get(left)){
                        fields[checking.X - 1, checking.Y] = fields.Get(checking) + cost[checking.X - 1, checking.Y];
                        toCheck.Add(left);
                    }
                }
                //right
                if(checking.X != cost.GetUpperBound(0))
                {
                    Point right = checking.Add(new Point(1, 0));
                    if (fields.Get(right) > fields.Get(checking) + cost.Get(right))
                    {
                        fields[checking.X + 1, checking.Y] = fields.Get(checking) + cost[checking.X + 1, checking.Y];
                        toCheck.Add(right);
                    }
                }
                //top
                if(checking.Y != 0)
                {
                    Point top = checking.Add(new Point(0, -1));
                    if (fields.Get(top) > fields.Get(checking) + cost.Get(top))
                    {
                        fields[checking.X, checking.Y - 1] = fields.Get(checking) + cost[checking.X, checking.Y - 1];
                        toCheck.Add(top);
                    }
                }
                //bottom
                if (checking.Y != cost.GetUpperBound(1))
                {
                    Point bottom = checking.Add(new Point(0, 1));
                    if (fields.Get(bottom) > fields.Get(checking) + cost.Get(bottom))
                    {
                        fields[checking.X, checking.Y + 1] = fields.Get(checking) + cost[checking.X, checking.Y + 1];
                        toCheck.Add(bottom);
                    }
                }
            }

            //now we backtrace from end to start
            List<Point> path = new List<Point> { end };
            Point active = end;
            while (active != start)
            {
                Point min = new Point(-1, -1);
                double minV = 1000000000000;
                Point tryP;
                double tryV;
                if(active.X != 0)
                {
                    tryP = active.Add(-1, 0);
                    tryV = fields.Get(tryP);
                    if (tryV < minV)
                    {
                        minV = tryV;
                        min = tryP;
                    }
                }
                if (active.X != fields.GetUpperBound(0))
                {
                    tryP = active.Add(1, 0);
                    tryV = fields.Get(tryP);
                    if (tryV<minV)
                    {
                        minV = tryV;
                        min = tryP;
                    }
                }
                if(active.Y != 0)
                {
                    tryP = active.Add(0, -1);
                    tryV = fields.Get(tryP);
                    if (tryV < minV)
                    {
                        minV = tryV;
                        min = tryP;
                    }
                }
                if(active.Y != fields.GetUpperBound(1))
                {
                    tryP = active.Add(0, 1);
                    tryV = fields.Get(tryP);
                    if (tryV < minV)
                    {
                        minV = tryV;
                        min = tryP;
                    }
                }
                path.Add(min);
                active = min;
            }
            path.Reverse();
            return path.ToArray();
        }
    }
}
