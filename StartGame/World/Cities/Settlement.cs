using StartGame.Properties;
using System.Drawing;

namespace StartGame.World.Cities
{
    public abstract class Settlement : WorldFeature
    {
        public Island Located => World.Instance.worldMap.Get(position).island;
        public string name;
        public Settlement(string name, int id, Point position, Bitmap image, int level = 0) : base(id, position, image, level: level)
        {
            this.name = name;
        }

        internal virtual void WorldAction()
        {

        }

    }
    public class Mine : Settlement
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly int materialValue;
#pragma warning restore IDE0052 // Remove unread private members

        public Mine(Point position, int materialValue) : base("Farm", ++ID, position, Resources.Mine1, 1)
        {
            this.materialValue = materialValue;
        }
    }
}