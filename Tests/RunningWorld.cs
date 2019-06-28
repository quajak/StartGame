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
            
            World.Instance.ProgressTime(new TimeSpan(365, 0, 0, 0));
            Assert.IsNotNull(World.Instance); //Random check, only real check right now is if any exception is thrown
        }

        [TestMethod]
        public void RunWorldLong()
        {
            World.Instance.ProgressTime(new TimeSpan(365 * 10, 0, 0, 0));
            Assert.IsNotNull(World.Instance); //Random check, only real check right now is if any exception is thrown
        }
    }
}
