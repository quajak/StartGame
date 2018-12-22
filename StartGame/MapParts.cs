using StartGame.Extra.Loading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;

namespace StartGame
{
    [DebuggerDisplay("{position} Type:{type.type.ToString()}")]
    public class MapTile : ICloneable
    {
        public Point position;
        public MapTileType type;
        private double height;
        public Continent continent;
        public SorroundingTiles<MapTile> neighbours;
        public bool isBeingChecked;
        public double Cost { set; get; }

        public double Height
        {
            get => height; set
            {
                height = value;
                CalculateDerivedStats();
            }
        }

        public double MovementCost;
        public double[] Costs;
        public string[] GoalIDs;
        public string id;
        public double leftValue = -1;
        public bool free;
        public Color color;
        public Color shader;

        private static Random random = new Random();

        public MapTile(Point Position, MapTileType Type, double Height, bool Free = true)
        {
            Cost = 1;
            isBeingChecked = false;
            position = Position;
            id = position.ToString();
            type = Type;
            this.Height = Height;
            free = Free;
            color = type.BaseColor;
        }

        public string RawData()
        {
            NumberFormatInfo nfi = new NumberFormatInfo {
                NumberDecimalSeparator = "."
            };
            return $"{position.X} {position.Y} {(int)type.type} {height.ToString(nfi)} {Cost} {free} {color.ToArgb()} {shader.ToArgb()}";
        }

        public static MapTile Load(string line)
        {
            string[] parts = line.Split(' ');
            Point pos = Loading.GetPoint(parts[0], parts[1]);
            MapTileTypeEnum typeEnum = (MapTileTypeEnum)parts[2].GetInt();
            double height = parts[3].GetDouble();
            double cost = parts[4].GetDouble();
            bool free = parts[5].GetBool();
            Color color = parts[6].GetColor();
            Color shader = parts[7].GetColor();
            return new MapTile(pos, new MapTileType() { type = typeEnum }, height, free, color, shader);
        }

        private void CalculateDerivedStats()
        {
            switch (type.type)
            {
                case MapTileTypeEnum.land:
                    MovementCost = 1;
                    shader = Color.FromArgb((int)(Height * 255), 0, 0, 0);
                    break;

                case MapTileTypeEnum.mountain:
                    shader = Color.FromArgb((int)(Height * 255), 0, 0, 0);
                    MovementCost = 2;
                    break;

                case MapTileTypeEnum.hill:
                    shader = Color.FromArgb((int)(Height * 255), 0, 0, 0);
                    MovementCost = 1.5;
                    break;

                case MapTileTypeEnum.shallowWater:
                    shader = Color.FromArgb((int)(((1 - Height) / 2) * 255), 0, 0, 0);
                    MovementCost = 1.5;
                    break;

                case MapTileTypeEnum.deepWater:
                    shader = Color.FromArgb((int)(((1 - Height) / 2) * 255), 0, 0, 0);
                    MovementCost = 2;
                    break;

                case MapTileTypeEnum.path:
                    shader = Color.FromArgb((int)(Height * 255), 0, 0, 0);
                    MovementCost = 0.5;
                    break;

                case MapTileTypeEnum.wall:
                    shader = Color.FromArgb(random.Next(100, 150), 0, 0, 0);
                    MovementCost = 20;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public MapTile(int x, int y, MapTileType mapTileType, double setHeight, bool free = true) : this(new Point(x, y), mapTileType, setHeight, free)
        {
        }

        public MapTile(Point Position, MapTileType Type, double Height, bool Free, Color color, Color shader) : this(Position, Type, Height, Free)
        {
            this.color = color;
            this.shader = shader;
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
            return new MapTile(position, type, Height) {
                id = id,
                Cost = Cost,
                Costs = Costs,
                continent = continent,
                neighbours = (SorroundingTiles<MapTile>)neighbours.Clone(),
                isBeingChecked = isBeingChecked,
                GoalIDs = GoalIDs,
                color = color,
                shader = shader
            };
        }

        public override string ToString()
        {
            return $"X:{position.X} Y:{position.Y} Type:{type.type.ToString()}";
        }
    }

    public class SorroundingTiles<T> : ICloneable
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
    {
        [Description("Land")]
        land,

        [Description("Mountain")]
        mountain,

        [Description("Hill")]
        hill,

        [Description("Shallow Water")]
        shallowWater,

        [Description("Deep water")]
        deepWater,

        [Description("Path")]
        path,

        [Description("Wall")]
        wall
    }

    public enum FieldType { water, land, mountain, wall }

    [DebuggerDisplay("Type: {type}")]
    public struct MapTileType
    {
        public MapTileTypeEnum type;

        private static Dictionary<string, MapTileTypeEnum> mapTileMapping;

        public static MapTileTypeEnum MapTileTypeEnumFromString(string data)
        {
            if (mapTileMapping is null)
            {
                mapTileMapping = new Dictionary<string, MapTileTypeEnum>();
                foreach (object type in Enum.GetValues(typeof(MapTileTypeEnum)))
                {
                    MapTileTypeEnum rType = (MapTileTypeEnum)type;
                    mapTileMapping.Add(((MapTileTypeEnum)type).Description(), rType);
                }
            }
            return mapTileMapping[data];
        }

        public Color BaseColor
        {
            get
            {
                switch (type)
                {
                    case MapTileTypeEnum.land:
                        return Color.Green;

                    case MapTileTypeEnum.mountain:
                        return Color.DarkGray;

                    case MapTileTypeEnum.hill:
                        return Color.LightGray;

                    case MapTileTypeEnum.shallowWater:
                        return Color.Blue;

                    case MapTileTypeEnum.deepWater:
                        return Color.MediumBlue;

                    case MapTileTypeEnum.path:
                        return Color.SandyBrown;

                    case MapTileTypeEnum.wall:
                        return Color.Gray;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

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

                    case MapTileTypeEnum.wall:
                        return FieldType.wall;

                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }

    public class EdgeArray<T>
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

    public class Continent
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