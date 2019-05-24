using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.MathFunctions
{
    public class Range
    {
        private readonly int lower;
        private readonly int heigher;

        public Range(int lower, int heigher)
        {
            this.lower = lower;
            this.heigher = heigher;
        }

        public bool Includes(int number)
        {
            return lower <= number && number <= heigher;
        }

        internal int GetRandom()
        {
            return World.World.random.Next(lower, heigher + 1);
        }
    }
}
