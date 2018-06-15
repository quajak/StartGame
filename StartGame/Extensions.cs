using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StartGame
{
    internal static class Extensions
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
    }
}