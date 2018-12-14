using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame.Items;
using StartGame.PlayerData;

namespace Tests
{
    [TestClass]
    public class CustomPlayerTest
    {
        [TestMethod]
        public void SaveAndLoadCustomPlayer()
        {
            CustomPlayer customPlayer = new CustomPlayer("test", "enemyScout", Weapon.Fist, 0);
            Assert.IsNotNull(customPlayer);
            Assert.IsTrue(customPlayer.Save(@"./customPlayer.txt"));
            customPlayer = CustomPlayer.Load(@"./customPlayer.txt");
            Assert.IsNotNull(customPlayer);
            Assert.AreEqual("test", customPlayer.Name);
        }

        [TestMethod]
        public void SaveAndLoadCustomPlayerWithData()
        {
            CustomPlayer customPlayer = new CustomPlayer("complex", "enemyScout", Weapon.Fist, 0) { XP = 5 };
            Assert.IsTrue(customPlayer.Save(@"./c.txt"));
            customPlayer = CustomPlayer.Load(@"./c.txt");
            Assert.IsNotNull(customPlayer);
            Assert.AreEqual(5, customPlayer.XP);
        }
    }
}
