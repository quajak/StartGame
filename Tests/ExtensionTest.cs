using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame;
using System.Drawing;

namespace Tests
{
    [TestClass]
    public class ExtensionTest
    {
        [TestMethod]
        public void TestCopy()
        {
            Point x = new Point(10, 100);
            Point copy = x.Copy();
            Assert.AreEqual(x, copy);
            //Changing the copy does not affect the original
            copy.X = 20;
            Assert.IsTrue(x.X == 10);
        }

        [TestMethod]
        public void TestCut()
        {
            Point p = new Point(10, 20);
            Assert.AreEqual(p, p.Cut(0, 100, 0, 100));
            //Check that p is not changed
            Point _p = p.Copy();
            _p.Cut(20, 100, 0, 100);
            Assert.IsTrue(p.X == 10);
            //Check that the bounds are correctly handled
            Assert.IsTrue(p.Cut(20, 100, 0, 100).X == 20);
            Assert.IsTrue(p.Cut(0, 100, 30, 100).Y == 30);
            Assert.IsTrue(p.Cut(0, 5, 0, 100).X == 5);
            Assert.IsTrue(p.Cut(0, 100, 0, 15).Y == 15);
        }
    }
}