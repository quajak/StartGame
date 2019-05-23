using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame.World;
using StartGame.World.Cities;

namespace Tests
{
    [TestClass]
    public class WorldGenerationTest
    {
        [TestMethod]
        public void TestCityGeneration()
        {
            for (int i = 0; i < 100; i++)
            {
                int value = World.random.Next(60, 100);
                Trace.TraceInformation($"City value is {value}");
                City city = new CapitalCity(new System.Drawing.Point(1,1), value, World.random.Next(0,100), World.random.Next(0,100));
                Assert.IsTrue(city.buildings.Where(b => b is Store).Count() > 2);
            }
        }
    }
}
