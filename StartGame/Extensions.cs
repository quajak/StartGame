using StartGame.Dungeons;
using StartGame.Entities;
using StartGame.Items;
using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StartGame
{
    internal static class E
    {
        public static Point Mult(this Point point, int Size)
        {
            return new Point(point.X * Size, point.Y * Size);
        }

        public static Point Div(this Point point, int value)
        {
            return new Point(point.X / value, point.Y / value);
        }

        public static Point Sub(this Point point, Point b)
        {
            return new Point(point.X - b.X, point.Y - b.Y);
        }

        public static Point Add(this Point point, Point b)
        {
            return new Point(point.X + b.X, point.Y + b.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>(this T[,] map, Point point)
        {
            return map[point.X, point.Y];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>(this T[,] map, int[] pos)
        {
            return map[pos[0], pos[1]];
        }

        [ConditionalAttribute("Debug")]
        public static void LogInfo(string message)
        {
            Trace.Write(message);
        }

        public static string Description(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    if (System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }
            return "";
        }

        private static Quality[] qualities = new Quality[] { Quality.Broken, Quality.Poor, Quality.Simple, Quality.Common, Quality.Good, Quality.Superior, Quality.Exceptional, Quality.Legendary };

        public static Quality GetQuality(int num)
        {
            return qualities[num];
        }

        public static int GetQualityPos(this Quality quality)
        {
            return qualities.ToList().IndexOf(quality);
        }

        public static string WriteAttribute(string data, string name)
        {
            return $"{data} {name}";
        }

        internal static string WriteAttribute(int i, string v)
        {
            return WriteAttribute(i.ToString(), v);
        }
    }
}

namespace StartGame.Extra.Loading
{
    internal static class Loading
    {
        public static string GetString(this string line)
        {
            return line.Split(' ')[0];
        }

        public static int GetInt(this string line)
        {
            return int.Parse(line.Split(' ')[0]);
        }

        public static double GetDouble(this string line)
        {
            return double.Parse(line.Split(' ')[0]);
        }

        public static bool GetBool(this string line)
        {
            return bool.Parse(line.Split(' ')[0]);
        }

        public static Color GetColor(this string line)
        {
            return Color.FromArgb(line.GetInt());
        }

        public static Point GetPoint(this string line)
        {
            string[] s = line.Split(' ');
            return GetPoint(s[0], s[1]);
        }

        public static Point GetPoint(string line1, string line2)
        {
            int x = int.Parse(line1);
            int y = int.Parse(line2);
            return new Point(x, y);
        }

        public static List<string> GetStringList(this string line)
        {
            List<string> all = line.Trim().Split(' ').ToList();
            all = all.Take(all.Count - 1).ToList();
            all = all.Where(a => a.Trim().Length != 0).ToList();
            return all;
        }

        public static EntityPlaceHolder GetEntity(this string line, Room room, List<CustomPlayer>  customEntities)
        {
            string[] words = line.Split(' ');
            switch (words[0])
            {
                case "Door":
                    string name = words[1];
                    Point point = GetPoint(words[2], words[3]);
                    //bool blocking = GetBool(words[4]); Not needed for door
                    int id = words[5].GetInt();
                    bool unlinked = words[6].GetBool();
                    string roomName = "";
                    int doorId = 0;
                    if (!unlinked)
                    {
                        roomName = words[7];
                        doorId = words[8].GetInt();
                    }
                    return new DoorPlaceHolder((roomName, doorId), room.name, point, id);

                case "Troop":
                    name = words[5];
                    point = GetPoint(words[2], words[3]);
                    string type = words[1];
                    CustomPlayer player = (CustomPlayer)customEntities.First(c => c.Name == type).Clone();
                    player.troop.Name = name;
                    player.troop.Position = point;
                    player.Name = name;
                    return new PlayerPlaceHolder(player);
                default:
                    throw new NotImplementedException(line);
            }
        }
    }
}