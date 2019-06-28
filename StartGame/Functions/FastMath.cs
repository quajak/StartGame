﻿using System;
//Taken from https://stackoverflow.com/questions/40460850/significant-drop-in-performance-of-math-round-on-x64-platform
namespace StartGame.Functions
{
    public static class FastMath
    {
        private static readonly double[] RoundLookup = CreateRoundLookup();

        private static double[] CreateRoundLookup()
        {
            double[] result = new double[15];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Math.Pow(10, i);
            }

            return result;
        }

        public static double Round(double value)
        {
            return Math.Floor(value + 0.5);
        }

        public static double Round(double value, int decimalPlaces)
        {
            double adjustment = RoundLookup[decimalPlaces];
            return Math.Floor(value * adjustment + 0.5) / adjustment;
        }
    }
}