using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame.World;

namespace Tests
{
    [TestClass]
    public class AtmosphereTests
    {
        [TestMethod]
        public void SingleIteration()
        {
            World world = World.Instance;
            world.ProgressTime(new TimeSpan(0, (int)(60 * World.atmosphereTimeStep), 0), true);
        }
    }
}
