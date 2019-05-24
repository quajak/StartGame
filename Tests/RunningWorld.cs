using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame.World;

namespace Tests
{
    [TestClass]
    public class RunningWorld
    {
        [TestMethod]
        public void RunWorld()
        {
            int hours = 365 * 24;
            while (hours > 0)
            {
                World.Instance.ProgressTime(new TimeSpan(0, 2, 0, 0));
                hours -= 2;
            }
            Assert.IsNotNull(World.Instance); //Random check, only real check right now is if any exception is thrown
        }

        [TestMethod]
        public void RunWorldLong()
        {
            int hours = 365 * 24 * 10;
            while (hours > 0)
            {
                World.Instance.ProgressTime(new TimeSpan(0, 2, 0, 0));
                hours -= 2;
            }
            Assert.IsNotNull(World.Instance); //Random check, only real check right now is if any exception is thrown
        }
    }
}
