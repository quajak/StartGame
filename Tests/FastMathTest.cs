using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame.Functions;

namespace Tests
{
    [TestClass]
    public class FastMathTest
    {
        [TestMethod]
        public void TestFloor()
        {
            Assert.IsTrue(FastMath.Floor(9.1, 0) == 9);
            Assert.IsTrue(FastMath.Floor(9.8, 0) == 9);
            Assert.IsTrue(FastMath.Floor(9.1, 1) == 9.1);
            Assert.IsTrue(FastMath.Floor(9.8, 1) == 9.8);
            Assert.IsTrue(FastMath.Floor(0.01, 1) == 0);
        }
    }
}
