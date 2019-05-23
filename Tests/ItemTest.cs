using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame.Items;

namespace Tests
{
    [TestClass]
    public class ItemTest
    {
        [TestMethod]
        public void ArmourGenerationTest()
        {
            for (int i = 0; i < 1000; i++)
            {
                Armour a = ArmourPrefabs.CreateArmour(true);
                Assert.IsNotNull(a);
                Assert.IsFalse(a.active);
                Assert.IsTrue(a.Value > 0);
                Assert.IsTrue(a.bluntDefense >= 0);
                Assert.IsTrue(a.sharpDefense >= 0);
                Assert.IsTrue(a.magicDefense >= 0);
                Assert.IsTrue(a.durability > 0);
                Assert.IsTrue(a.affected.Count > 0);
                Assert.IsTrue(a.maxDurability >= a.durability);
                Assert.IsTrue(a.weight > 0);
                Assert.IsNotNull(a.Description);
            }
        }
    }
}
