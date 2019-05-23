using System.Drawing;

namespace StartGame.World
{
    public enum WorldTileType { Ocean, River, TemperateGrassland, Rainforest, Desert, Tundra, TemperateForest, Savanna, Alpine, SeaIce }

    public class WorldTile
    {
        public Point position;
        public WorldFeature worldFeature;
        public WorldTileType type;
        public double height;
        public double movementCost;
        public Island island;
        public SorroundingTiles<WorldTile> sorroundingTiles;

        public WorldTile(double Height, WorldTileType type, double cost, Point position)
        {
            height = Height;
            this.type = type;
            movementCost = cost;
            this.position = position;
        }

        public string RawValue()
        {
            return $"{position} {height} {type} {worldFeature.id}";
        }
    }
}