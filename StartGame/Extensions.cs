using StartGame.Dungeons;
using StartGame.Entities;
using StartGame.Items;
using StartGame.PlayerData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace StartGame
{
    public static class E
    {
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator e)
        {
            while (e.MoveNext())
            {
                yield return (T)e.Current;
            }
        }

        public static bool AlmostEqual(this double a, double b, double diff = 0.1)
        {
            return Math.Abs(a - b) < diff;
        }

        public static bool AlmostEqual(this float a, float b, float diff = 0.1f)
        {
            return Math.Abs(a - b) < diff;
        }

        public static double GetAverage<T>(this T[,] array, int sX, int eX, int sY, int eY, Func<T,double> func)
        {
            double value = 0;
            for (int x = sX; x < eX; x++)
            {
                for (int y = sY; y < eY; y++)
                {
                    value += func.Invoke(array[x, y]);
                }
            }
            value /= (eX - sX) * (eY - sY);
            if (double.IsNaN(value))
                throw new Exception();
            return value;
        }

        public static double GetAverage<T>(this T[] array, int sX, int eX, int sY, int eY, Func<T, double> func, Func<int,int,int> getIndex)
        {
            double value = default;
            for (int x = sX; x < eX; x++)
            {
                for (int y = sY; y < eY; y++)
                {
                    int index = getIndex.Invoke(x, y);
                    value += func.Invoke(array[index]);
                }
            }
            value /= (eX - sX) * (eY - sY);
            return value;
        }

        public static float GetAverage<T>(this T[] array, int sX, int eX, int sY, int eY, Func<T, float> func, Func<int, int, int> getIndex)
        {
            float value = default;
            for (int x = sX; x < eX; x++)
            {
                for (int y = sY; y < eY; y++)
                {
                    int index = getIndex.Invoke(x, y);
                    value += func.Invoke(array[index]);
                }
            }
            value /= (eX - sX) * (eY - sY);
            return value;
        }

        public static string WriteSignigicantFigures(this int number, int figures)
        {
            return Convert.ToDouble(String.Format($"{{0:G{figures}}}", number)).ToString("R0");
        }

        public static bool TryGet<T>(this List<T> values, Predicate<T> check, out T value)
        {
            if (values.Exists(check))
            {
                value = values.Find(check);
                return true;
            }
            value = default;
            return false;
        }
        public static T[] To1D<T>(this T[,] a)
        {
            T[] b = new T[a.Length];
            int k = 0;
            for (int i = 0; i < a.GetUpperBound(0); i++)
            {
                for (int j = 0; j < a.GetUpperBound(0); j++)
                {
                    b[k++] = a[i, j];
                }
            }
            return b;
        }
        public static bool Between(this int a, int low, int high, bool inclusive = true)
        {
            if (inclusive)
            {
                return low <= a && a <= high;
            }
            else
            {
                return low < a && a < high;
            }
        }
        public static string SplitWords(this string a)
        {
            string output = "";
            foreach (char c in a)
            {
                if (char.IsUpper(c) && output.Length != 0)
                {
                    output += " " ;
                }
                output += c;
            }
            return output;
        }
        public static Point Mult(this Point point, int Size)
        {
            return new Point(point.X * Size, point.Y * Size);
        }

        public static Point Mult(this Point point, double Size)
        {
            return new Point((int)(point.X * Size), (int)(point.Y * Size));
        }

        public static double Mag(this Point point)
        {
            return Math.Sqrt(point.X * point.X + point.Y * point.Y);
        }

        public static Point Div(this Point point, int value)
        {
            return new Point(point.X / value, point.Y / value);
        }

        public static Point Sub(this Point point, Point b)
        {
            return new Point(point.X - b.X, point.Y - b.Y);
        }

        public static Point Sub(this Point point, int x, int y)
        {
            return new Point(point.X - x, point.Y - y);
        }

        public static Point Add(this Point point, Point b)
        {
            return new Point(point.X + b.X, point.Y + b.Y);
        }

        public static Point Add(this Point point, int x, int y)
        {
            return new Point(point.X + x, point.Y + y);
        }

        public static Point Remainder(this Point point, int divisor)
        {
            return new Point(point.X % divisor, point.Y % divisor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>(this T[,] map, Point point)
        {
            return map[point.X, point.Y];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>(this T[,] map, int x, int y)
        {
            return map[x, y];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>(this T[,] map, int[] pos)
        {
            return map[pos[0], pos[1]];
        }

        [Conditional("Debug")]
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

        private static readonly Quality[] qualities = new Quality[] { Quality.Broken, Quality.Poor, Quality.Simple, Quality.Common, Quality.Good, Quality.Superior, Quality.Exceptional, Quality.Legendary };

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

        public static Point Cut(this Point p, int lowX, int highX, int lowY, int highY)
        {
            Point r = new Point(p.X, p.Y);
            if (r.X < lowX) r.X = lowX;
            if (r.X > highX) r.X = highX;
            if (r.Y < lowY) r.Y = lowY;
            if (r.Y > highY) r.Y = highY;
            return r;
        }

        public static Point Copy(this Point p)
        {
            return new Point(p.X, p.Y);
        }

        public static double Cut(this double v, double min, double max)
        {
            return v < min ? min : (v > max ? max : v);
        }

        public static float Cut(this float v, float min, float max)
        {
            return v < min ? min : (v > max ? max : v);
        }

        public static int Cut(this int v, int min, int max)
        {
            return v < min ? min : (v > max ? max : v);
        }

        public static int Min(this int v, int min)
        {
            return v > min ? v : min;
        }

        public static Bitmap ResizeImage(this Image image, int width, int height)
        {
            Bitmap destImage = new Bitmap(width, height);
            try
            {
                Rectangle destRect = new Rectangle(0, 0, width, height);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (Graphics graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (ImageAttributes wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }
                return destImage;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return destImage;
            }

        }

        internal static void TraceFunction(string args)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame caller = stackTrace.GetFrame(1);
            Trace.TraceInformation($"{caller.GetMethod().DeclaringType.Name}::{caller.GetMethod().Name}({args})");
        }

        internal static string GetCaller()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame caller = stackTrace.GetFrame(2);
            return caller.GetMethod().DeclaringType.Name+ "::" + caller.GetMethod().Name;
        }
        public static T GetRandom<T>(this List<T> list)
        {
            if (list.Count == 0) throw new ArgumentException();
            return list[World.World.random.Next(list.Count)];
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

        public static EntityPlaceHolder GetEntity(this string line, Room room, List<CustomPlayer> customEntities)
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