using StartGame.Properties;
using System.Collections.Generic;
using System.Drawing;

namespace StartGame.World
{
    public abstract class City : WorldFeature
    {
        public Island Located => World.Instance.worldMap.Get(position).island;

        public List<Island> access = new List<Island>();
        public List<City> connected = new List<City>();
        public readonly int connections;
        public int priority;

        public City(Point position, Bitmap bitmap, int connections, int priority) : base(++ID, position, bitmap, 1)
        {
            foreach (var tile in World.Instance.worldMap.Get(position).sorroundingTiles.rawMaptiles)
            {
                if (!access.Contains(tile.island))
                    access.Add(tile.island);
            }

            this.connections = connections;
            this.priority = priority;
        }
    }

    public class SmallCity : City
    {
        public SmallCity(Point position) : base(position, Resources.SmallCity, 1, 1)
        {
        }
    }

    public class MediumCity : City
    {
        public MediumCity(Point position) : base(position, Resources.MediumCity, 1, 3)
        {
        }
    }

    public class LargeCity : City
    {
        public LargeCity(Point position) : base(position, Resources.BigCity, 2, 6)
        {
        }
    }

    public class CapitalCity : City
    {
        public CapitalCity(Point position) : base(position, Resources.CapitalCity, 3, 10)
        {
        }
    }
}