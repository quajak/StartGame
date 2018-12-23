using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame.World;

namespace Tests
{
    [TestClass]
    public class WordMapTest
    {
        [TestMethod]
        public void GenerateMap()
        {
            Assert.IsNotNull(World.Instance);

            var movemementCosts = World.Instance.MovementCost();
        }
    }
}