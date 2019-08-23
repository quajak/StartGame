using StartGame.Functions;
using StartGame.World.Cities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StartGame.Items.Crops
{
    public abstract class Crop
    {
        static readonly List<Crop> crops = new List<Crop> {
            new Apple(1),  new Grain(1), new Pear(1), new Strawberry(1), new Date(1), new Melone(1), new Banana(1), new Cacoa(1), new Rice(1), new Corn(1), new Tomato(1), new Potato(1)
        };
        public readonly string name;
        public readonly int amount;
        public int time;
        public readonly Range yield;
        public readonly Range waterRequirement;
        public readonly Range temperature;

        /// <summary>
        /// A crop is used to track where a plant can grow and how long it takes to grow
        /// </summary>
        /// <param name="name">Name of crop</param>
        /// <param name="amount">Amount of crop planted</param>
        /// <param name="time">Time in days it takes the crop to finish</param>
        /// <param name="yield">How much is produced per input</param>
        /// <param name="waterRequirement">1->1000 how much water it needs to succefully grow</param>
        /// <param name="temperature">Temperature range in which it can grow</param>
        public Crop(string name, int amount, int time, Range yield, Range waterRequirement, Range temperature)
        {
            this.name = name;
            this.amount = amount;
            this.time = time;
            this.yield = yield;
            this.waterRequirement = waterRequirement;
            this.temperature = temperature;
        }
        public static List<Crop> GetCrops(int temperature, int water)
        {
            return crops.Where(c => c.temperature.Includes(temperature) && c.waterRequirement.Includes(water)).ToList();
        }

        public Resource Yield()
        {
            int price = World.World.Instance.nation.GetPrice(name);
            return new Resource(name, yield.GetRandom() / 2, price); //Divide by two to debuff farms
        }

        public Crop Duplicate(Crop crop, int amount)
        {
            Type type = crop.GetType();
            if (type == typeof(Apple))
            {
                return new Apple(amount);
            }
            else if (type == typeof(Grain))
            {
                return new Grain(amount);
            }
            else if (type == typeof(Pear))
            {
                return new Pear(amount);
            }
            else if (type == typeof(Strawberry))
            {
                return new Strawberry(amount);
            }
            else if (type == typeof(Date))
            {
                return new Date(amount);
            }
            else if (type == typeof(Melone))
            {
                return new Melone(amount);
            }
            else if (type == typeof(Banana))
            {
                return new Banana(amount);
            }
            else if (type == typeof(Cacoa))
            {
                return new Cacoa(amount);
            }
            else if (type == typeof(Rice))
            {
                return new Rice(amount);
            }
            else if (type == typeof(Corn))
            {
                return new Corn(amount);
            }
            else if (type == typeof(Tomato))
            {
                return new Tomato(amount);
            }
            else if (type == typeof(Potato))
            {
                return new Potato(amount);
            }
            throw new NotImplementedException();
        }

        internal Crop Duplicate()
        {
            return Duplicate(this, amount);
        }
    }

    public class Apple : Crop
    {
        public Apple(int amount) : base("Apple", amount, 100, new Range(200, 300), new Range(25 * 12 * 10, 35 * 12 * 10), new Range(-40, 30))
        {

        }
    }

    public class Grain : Crop
    {
        public Grain(int amount) : base("Grain", amount, 10, new Range(100, 1000), new Range(310, 380), new Range(15, 25))
        {

        }
    }

    public class Pear : Crop
    {
        public Pear(int amount) : base("Pear", amount, 140, new Range(200, 300), new Range(25 * 12, 35 * 12), new Range(-30, 20))
        {

        }
    }

    public class Strawberry : Crop
    {
        public Strawberry(int amount) : base("Strawberry", amount, 35, new Range(30, 60), new Range(25 * 5, 35 * 5), new Range(1, 25))
        {
        }
    }

    public class Date : Crop
    {
        public Date(int amount) : base("Date", amount, 4 * 4 * 7, new Range(300, 400), new Range(0, 6), new Range(20, 50))
        {

        }
    }

    public class Melone : Crop
    {
        public Melone(int amount) : base("Melone", amount, 80, new Range(20, 100), new Range(1 * 12 * 10, 1000), new Range(12, 40))
        {

        }
    }

    public class Banana : Crop
    {
        public Banana(int amount) : base("Banana", amount, 5 * 31, new Range(40, 150), new Range(800, 1000), new Range(20, 35))
        {

        }
    }

    public class Cacoa : Crop
    {
        public Cacoa(int amount) : base("Cacoa", amount, 6 * 31, new Range(2, 10), new Range(300, 1000), new Range(20, 40))
        {

        }
    }

    public class Rice : Crop
    {
        public Rice(int amount) : base("Rice", amount, 5 * 31, new Range(100, 400), new Range(600, 1000), new Range(20, 35))
        {

        }
    }

    public class Corn : Crop
    {
        public Corn(int amount) : base("Corn", amount, 80, new Range(120, 200), new Range(400, 800), new Range(15, 33))
        {

        }
    }

    public class Tomato : Crop
    {
        public Tomato(int amount) : base("Tomato", amount, 70, new Range(20, 40), new Range(400, 700), new Range(5, 30))
        {

        }
    }

    public class Potato : Crop
    {
        public Potato(int amount) : base("Potato", amount, 90, new Range(200, 500), new Range(500, 700), new Range(10, 30))
        {

        }
    }
}