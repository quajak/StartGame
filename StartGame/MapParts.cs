using System;
using System.Collections.Generic;
using System.Drawing;

namespace StartGame
{
    internal class MapTile
    {
        public Point position;
        public MapTileType type;
        public double height;
        public Continent continent;
        public SorroundingTiles neighbours;
        public bool isBeingChecked;
        public double Cost { set; get; }
        public double[] Costs;
        public string[] GoalIDs;
        public string id;

        public MapTile(Point Position, MapTileType Type, double Height)
        {
            Cost = 1;
            isBeingChecked = false;
            position = Position;
            id = position.ToString();
            type = Type;
            height = Height;
        }

        public void InitialiseDistances(int length)
        {
            Costs = new double[length];
            GoalIDs = new string[length];
        }

        public void GetNeighbours(Map map)
        {
            neighbours = new SorroundingTiles(position, map);
        }
    }

    internal class SorroundingTiles
    {
        public MapTile top;
        public MapTile left;
        public MapTile bottom;
        public MapTile right;
        public MapTile[] rawMaptiles;

        public SorroundingTiles(Point pos, Map map)
        {
            List<MapTile> rawData = new List<MapTile>();
            if (pos.Y != 0)
            {
                top = map.map[pos.X, pos.Y - 1];
                rawData.Add(top);
            }
            else top = null;
            if (pos.X != 0)
            {
                left = map.map[pos.X - 1, pos.Y];
                rawData.Add(left);
            }
            else left = null;
            if (pos.X != map.map.GetUpperBound(0))
            {
                right = map.map[pos.X + 1, pos.Y];
                rawData.Add(right);
            }
            else right = null;
            if (pos.Y != map.map.GetUpperBound(1))
            {
                bottom = map.map[pos.X, pos.Y + 1];
                rawData.Add(bottom);
            }
            else bottom = null;
            rawMaptiles = rawData.ToArray();
        }

        public List<MapTile> GetSameType(MapTileTypeEnum type)
        {
            List<MapTile> sameType = new List<MapTile>();

            if (top != null && top.type.type == type && top.continent == null && !top.isBeingChecked) sameType.Add(top);
            if (bottom != null && bottom.type.type == type && bottom.continent == null && !bottom.isBeingChecked) sameType.Add(bottom);
            if (right != null && right.type.type == type && right.continent == null && !right.isBeingChecked) sameType.Add(right);
            if (left != null && left.type.type == type && left.continent == null && !left.isBeingChecked) sameType.Add(left);

            return sameType;
        }
    }

    internal enum MapTileTypeEnum
    { land, mountain, hill, shallowWater, deepWater, path }

    internal struct MapTileType
    {
        public MapTileTypeEnum type;
        public Color color;
    }

    internal class EdgeArray
    {
        public List<MapTile> top;
        public List<MapTile> bottom;
        public List<MapTile> right;
        public List<MapTile> left;

        public EdgeArray(MapTile[] Top, MapTile[] Bottom, MapTile[] Left, MapTile[] Right)
        {
            top = new List<MapTile>(Top);
            bottom = new List<MapTile>(Bottom);
            right = new List<MapTile>(Right);
            left = new List<MapTile>(Left);
        }
    }

    internal class Continent
    {
        public List<MapTile> tiles;
        public MapTileTypeEnum Type { get; private set; }
        private string id;
        public EdgeArray edges;
        public Color color;

        //Internal
        private List<MapTile> toCheck;

        public Map NewContinent(Map map, Point point, string ID, Random rng)
        {
            color = Color.FromArgb(100, rng.Next(255), rng.Next(255), rng.Next(255));
            id = ID;

            MapTile activeTile = map.map[point.X, point.Y];
            Type = activeTile.type.type;
            map.map[point.X, point.Y].isBeingChecked = true;

            toCheck = new List<MapTile>();
            tiles = new List<MapTile>();
            //Find tiles
            while (true)
            {
                //Add new tiles to continent
                List<MapTile> neighbours = activeTile.neighbours.GetSameType(Type);
                if (neighbours.Count != 0)
                {
                    foreach (MapTile tile in neighbours)
                    {
                        map.map[tile.position.X, tile.position.Y].isBeingChecked = true;
                    }
                    toCheck.AddRange(neighbours);
                }
                tiles.Add(map.map[activeTile.position.X, activeTile.position.Y]);
                map.map[activeTile.position.X, activeTile.position.Y].continent = this;
                //Choose new tile
                if (toCheck.Count != 0)
                {
                    activeTile = toCheck[0];
                    toCheck.RemoveAt(0);
                }
                //End
                else
                {
                    break;
                }
            }

            //Find edges
            List<MapTile> top = new List<MapTile>();
            List<MapTile> bottom = new List<MapTile>();
            List<MapTile> left = new List<MapTile>();
            List<MapTile> right = new List<MapTile>();
            foreach (MapTile tile in tiles)
            {
                if (tile.position.X == 0) left.Add(tile);
                if (tile.position.X == map.map.GetUpperBound(0)) right.Add(tile);
                if (tile.position.Y == 0) top.Add(tile);
                if (tile.position.Y == map.map.GetUpperBound(1)) bottom.Add(tile);
            }
            edges = new EdgeArray(top.ToArray(), bottom.ToArray(), left.ToArray(), right.ToArray());

            return map;
        }
    }
}