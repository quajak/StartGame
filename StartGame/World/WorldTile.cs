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

        public double forest;

        public WorldTile(double Height, WorldTileType type, double cost, Point position, double rainFall)
        {
            height = Height;
            this.type = type;
            movementCost = cost;
            this.position = position;
            switch (type)
            {
                case WorldTileType.Ocean:
                    forest = 0;
                    break;
                case WorldTileType.River:
                    forest = 0;
                    break;
                case WorldTileType.TemperateGrassland:
                    forest = 10 * (1 + rainFall);
                    break;
                case WorldTileType.Rainforest:
                    forest = 100;
                    break;
                case WorldTileType.Desert:
                    forest = 0;
                    break;
                case WorldTileType.Tundra:
                    forest = 40 * (1 + rainFall);
                    break;
                case WorldTileType.TemperateForest:
                    forest = 60 * (1 + rainFall / 2);
                    break;
                case WorldTileType.Savanna:
                    forest = 10 * (1 + rainFall);
                    break;
                case WorldTileType.Alpine:
                    forest = 20 * ( 1 + rainFall);
                    break;
                case WorldTileType.SeaIce:
                    forest = 0;
                    break;
                default:
                    throw new System.NotImplementedException();
            }
            forest.Cut(0, 100);
        }

        public string RawValue()
        {
            return $"{position} {height} {type} {worldFeature.id}";
        }
    }
}