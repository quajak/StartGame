using StartGame.Properties;
using System.Drawing;

namespace StartGame.World
{
    public class WorldFeature
    {
        public static int ID = 0;
        public int id;
        public Point position;
        public Bitmap image;
        public readonly int level;

        public WorldFeature(int id, Point position, Bitmap image, int level = 0)
        {
            this.id = id;
            this.position = position;
            this.image = image;
            this.level = level;
        }
    }

    public class Road : WorldFeature
    {
        public Road(Point position) : base(++ID, position, Resources.Road)
        {
            World.Instance.costMap[position.X, position.Y] -= 0.5;
        }
    }
}