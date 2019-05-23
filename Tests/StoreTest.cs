using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame.Items;
using StartGame.PlayerData;
using StartGame.Properties;
using StartGame.World;
using StartGame.World.Cities;

namespace Tests
{
    [TestClass]
    public class StoreTest
    {
        [TestMethod]
        public void TestGenerateGoods()
        {
            for (int i = 0; i < 100; i++)
            {
                City city = GenerateCity();
                for (int j = 0; j < 100; j++)
                {
                    int value1 = World.random.Next(city.value);
                    System.Collections.Generic.List<InventoryItem> food = FoodMarket.GenerateGoods(value1, city);
                    InventoryItem tooCostly = food.Find(g => g.Cost > 40 || g.Cost <= 0);
                    if (tooCostly != null)
                    {
                        Trace.TraceInformation(tooCostly.ToString());
                    }
                    Assert.IsNull(tooCostly);
                }
            }
        }

        private static City GenerateCity()
        {
            int value = World.random.Next(100);
            int agriculture = World.random.Next(100);
            int mineral = World.random.Next(100);
            City city = new SmallCity(new System.Drawing.Point(1, 1), value, agriculture, mineral);
            return city;
        }

        [TestMethod]
        public void TestBuying()
        {
            City city = GenerateCity();
            while (!city.buildings.Exists(b => b is FoodMarket))
            {
                city = GenerateCity();
            }
            int money = 200;
            HumanPlayer player = new HumanPlayer(PlayerType.localHuman, "Test", null, new Player[] { }, null, money);
            player.troop = new Troop("Test", null, Resources.playerTroop, 0, null, player);
            int playerItems = player.troop.Items.Count;
            Assert.IsTrue(player.Money.Value == money);
            FoodMarket market = city.buildings.Find(b => b is FoodMarket) as FoodMarket;
            Assert.IsNotNull(market);
            market.Items[0] = new InventoryItem(new Food("Apple", 1, 2, 1), 20, 2);
            Assert.IsTrue(market.Items[0].Cost == 2);
            for (int i = 0; i < 10; i++)
            {
                Store.BuyItem(market.Items[0], market.Items, player, market);
                money -= 2;
                Assert.IsTrue(player.troop.Items.Count == playerItems + 1);
                Assert.IsTrue(player.Money.Value == money);
                Assert.IsTrue(player.Money.ToString() == $"Money: {money} Coins");
            }
        }
    }
}
