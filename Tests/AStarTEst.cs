using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame.AI;
using System.Drawing;

namespace Tests
{
    [TestClass]
    public class AStarTest
    {
        [TestMethod]
        public void FullAStarTest()
        {
            double[,] map = new double[,] { { 0.5, 0.3, 0.1 }, { 0.5, 0.1, 0.1 }, { 0.5, 0.3, 0.5 } };
            Point[] expectedPath = new Point[] { new Point(0, 2), new Point(1, 2), new Point(1, 1), new Point(2, 1), new Point(2, 0) };
            Point[] path = AStar.FindOptimalRoute(map, new Point(0, 2), new Point(2, 0));
            bool equal = true;
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] != expectedPath[i])
                {
                    equal = false;
                    break;
                }
            }
            Assert.IsTrue(equal);
        }
    }
}