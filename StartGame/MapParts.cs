using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace StartGame
{
    [DebuggerDisplay("X:{position.X} Y:{position.Y} Type:{type.type.ToString()}")]
    internal class MapTile : ICloneable
    {
        public Point position;
        public MapTileType type;
        public double height;
        public Continent continent;
        public SorroundingTiles<MapTile> neighbours;
        public bool isBeingChecked;
        public double Cost { set; get; }
        public double MovementCost;
        public double[] Costs;
        public string[] GoalIDs;
        public string id;
        public double leftValue = -1;
        public bool free = true;

        public MapTile(Point Position, MapTileType Type, double Height)
        {
            Cost = 1;
            isBeingChecked = false;
            position = Position;
            id = position.ToString();
            type = Type;
            height = Height;
            switch (Type.type)
            {
                case MapTileTypeEnum.land:
                    MovementCost = 1;
                    break;

                case MapTileTypeEnum.mountain:
                    MovementCost = 2;
                    break;

                case MapTileTypeEnum.hill:
                    MovementCost = 1.5;
                    break;

                case MapTileTypeEnum.shallowWater:
                    MovementCost = 1.5;
                    break;

                case MapTileTypeEnum.deepWater:
                    MovementCost = 2;
                    break;

                case MapTileTypeEnum.path:
                    MovementCost = 0.5;
                    break;

                default:
                    break;
            }
        }

        public void InitialiseDistances(int length)
        {
            Costs = new double[length];
            GoalIDs = new string[length];
        }

        public void GetNeighbours(Map map)
        {
            neighbours = new SorroundingTiles<MapTile>(position, map.map);
        }

        public object Clone()
        {
            return new MapTile(position, type, height)
            {
                id = id,
                Cost = Cost,
                Costs = Costs,
                continent = continent,
                neighbours = (SorroundingTiles<MapTile>)neighbours.Clone(),
                isBeingChecked = isBeingChecked,
                GoalIDs = GoalIDs
            };
        }

        public override string ToString()
        {
            return $"X:{position.X} Y:{position.Y} Type:{type.type.ToString()}";
        }
    }

    internal class SorroundingTiles<T> : ICloneable
    {
        public T top;
        public T left;
        public T bottom;
        public T right;
        public T[] rawMaptiles;

        public SorroundingTiles(Point pos, T[,] map)
        {
            if (default(T) != null) throw new Exception("This class can only be used with nullable types!");
            List<T> rawData = new List<T>();
            if (pos.Y != 0)
            {
                top = map[pos.X, pos.Y - 1];
                rawData.Add(top);
            }
            else top = default;
            if (pos.X != 0)
            {
                left = map[pos.X - 1, pos.Y];
                rawData.Add(left);
            }
            else left = default;
            if (pos.X != map.GetUpperBound(0))
            {
                right = map[pos.X + 1, pos.Y];
                rawData.Add(right);
            }
            else right = default;
            if (pos.Y != map.GetUpperBound(1))
            {
                bottom = map[pos.X, pos.Y + 1];
                rawData.Add(bottom);
            }
            else bottom = default;
            rawMaptiles = rawData.ToArray();
        }

        public SorroundingTiles(T Top, T Bottom, T Left, T Right)
        {
            top = Top;
            bottom = Bottom;
            left = Left;
            right = Right;
            rawMaptiles = new T[] { top, bottom, left, right };
        }

        public object Clone()
        {
            return new SorroundingTiles<T>(top, bottom, left, right);
        }

        public List<MapTile> GetSameType(MapTileTypeEnum type)
        {
            if ((top == null || top is MapTile) && (bottom == null || bottom is MapTile) && (right == null || right is MapTile) && (left == null || left is MapTile))
            {
                MapTile _top = top as MapTile;
                MapTile _bottom = bottom as MapTile;
                MapTile _right = right as MapTile;
                MapTile _left = left as MapTile;
                List<MapTile> sameType = new List<MapTile>();

                if (top != null && _top.type.type == type && _top.continent == null && !_top.isBeingChecked) sameType.Add(_top);
                if (bottom != null && _bottom.type.type == type && _bottom.continent == null && !_bottom.isBeingChecked) sameType.Add(_bottom);
                if (right != null && _right.type.type == type && _right.continent == null && !_right.isBeingChecked) sameType.Add(_right);
                if (left != null && _left.type.type == type && _left.continent == null && !_left.isBeingChecked) sameType.Add(_left);

                return sameType;
            }
            throw new NotImplementedException();
        }
    }

    public enum MapTileTypeEnum
    { land, mountain, hill, shallowWater, deepWater, path }

    public enum FieldType { water, land, mountain }

    [DebuggerDisplay("Type: {type}")]
    public struct MapTileType
    {
        public MapTileTypeEnum type;

        public FieldType FType
        {
            get
            {
                switch (type)
                {
                    case MapTileTypeEnum.land:
                        return FieldType.land;

                    case MapTileTypeEnum.mountain:
                        return FieldType.mountain;

                    case MapTileTypeEnum.hill:
                        return FieldType.mountain;

                    case MapTileTypeEnum.shallowWater:
                        return FieldType.water;

                    case MapTileTypeEnum.deepWater:
                        return FieldType.water;

                    case MapTileTypeEnum.path:
                        return FieldType.land;

                    default:
                        throw new Exception();
                }
            }
        }
    }

    internal class EdgeArray<T>
    {
        public List<T> top;
        public List<T> bottom;
        public List<T> right;
        public List<T> left;

        public EdgeArray(T[] Top, T[] Bottom, T[] Left, T[] Right)
        {
            top = new List<T>(Top);
            bottom = new List<T>(Bottom);
            right = new List<T>(Right);
            left = new List<T>(Left);
        }
    }

    internal class Continent
    {
        public List<MapTile> tiles;
        public MapTileTypeEnum Type { get; private set; }
        private string id;
        public EdgeArray<MapTile> edges;
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
            edges = new EdgeArray<MapTile>(top.ToArray(), bottom.ToArray(), left.ToArray(), right.ToArray());

            return map;
        }
    }
}