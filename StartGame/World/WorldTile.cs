using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.World
{
    public enum WorldTileType { Ocean, River, Grassland, Mountain, Hill, Forest}

    public class WorldTile
    {
        public Point position;
        public WorldFeature worldFeature;
        public WorldTileType type;
        public double height;
        public double movementCost;

        public WorldTile(double Height, WorldTileType type, double cost)
        {
            height = Height;
            this.type = type;
            movementCost = cost;
        }

        public string RawValue()
        {
            return $"{position} {height} {type} {worldFeature.id}";
        }
    }
}
