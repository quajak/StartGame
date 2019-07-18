using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame;
using System.Collections.Generic;
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

        class TestType
        {
            public int i;
            public TestType(int I)
            {
                i = I;
            }
        }

        [TestMethod]
        public void TestGetAverage()
        {
            TestType[,] data = new TestType[10, 10];
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    data[x, y] = new TestType(1);
                }
            }
            Assert.IsTrue(data.GetAverage(0, 10, 0, 10, t => t.i) == 1);
            Assert.IsTrue(data.GetAverage(0, 1, 0, 1, t => t.i) == 1);
            Assert.IsTrue(data.GetAverage(0, 10, 0, 1, t => t.i) == 1);
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    data[x, y] = new TestType(x);
                }
            }
            Assert.IsTrue(data.GetAverage(0, 10, 0, 1, t => t.i) == 4.5);

        }

        [TestMethod]
        public void TestGetRandom()
        {
            List<int> i = new List<int> { 1 };
            Assert.IsTrue(i.GetRandom() == 1);
        }

        [TestMethod]
        public void TestTryGet()
        {
            List<int> i = new List<int> { 1, 2, 3 };
            Assert.IsTrue(i.TryGet(I => I == 2, out int value));
            Assert.IsTrue(value == 2);
            Assert.IsFalse(i.TryGet(I => I == 4, out value));
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

        public void TestBetween()
        {
            Assert.IsTrue(10.Between(0, 100));
            Assert.IsTrue(10.Between(10, 10, true));
            Assert.IsFalse(10.Between(10, 20, false));
            Assert.IsFalse(10.Between(0, 10, false));
        }

        public void TestSplitWord()
        {
            Assert.IsTrue("ACar".SplitWords() == "A Car");

        }
    }
}