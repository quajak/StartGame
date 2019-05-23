using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame.GameMap;
using StartGame.Items;
using StartGame.PlayerData;
using StartGame.Properties;

namespace Tests
{
    [TestClass]
    public class HumanPlayerTest
    {
        [TestMethod]
        public void TestCreation()
        {
            HumanPlayer player = new HumanPlayer(PlayerType.localHuman, "Test", null, new Player[] { }, null, 200);
            Assert.IsNotNull(player.Money);
            Assert.IsNotNull(player.actionPoints);
            Assert.IsNotNull(player.agility);
            Assert.IsNotNull(player.endurance);
            Assert.IsNotNull(player.gearWeight);
            Assert.IsNotNull(player.intelligence);
            Assert.IsNotNull(player.wisdom);
            Assert.IsNotNull(player.mana);
            Assert.IsNotNull(player.strength);
            Assert.IsNotNull(player.trees);
            Assert.IsTrue(player.Money.Value > 0);
            Assert.IsTrue(player.mana.Value > 0);
            Assert.IsTrue(player.Dangerous);
            Assert.IsFalse(player.Hidden);
        }

        [TestMethod]
        public void TestPlayerAttributeChanges()
        {
            HumanPlayer player = new HumanPlayer(PlayerType.localHuman, "Test", null, new Player[] { }, null, 200);
            Troop playerTroop = new Troop("Test", new Weapon(5, BaseAttackType.melee, BaseDamageType.blunt, 1, "Punch", 2, false), Resources.playerTroop, 0, null, player)
            {
                armours = new List<Armour>
                        {
                        new Armour("Woolen Tunic", 50, new List<BodyParts>{BodyParts.LeftUpperArm,BodyParts.RightUpperArm,BodyParts.Torso}, Material.Materials.First(m => m.name == "Wool"),Quality.Common, ArmourLayer.clothing),
                        new Armour("Old Pants", 40, new List<BodyParts> { BodyParts.UpperLegs, BodyParts.LeftLowerLeg, BodyParts.RightLowerLeg, BodyParts.LeftShin, BodyParts.RightShin }, Material.Materials.First(m => m.name == "Cloth"), Quality.Poor, ArmourLayer.clothing),
                        new Armour("Wooden Shoes", 32, new List<BodyParts> { BodyParts.LeftFoot, BodyParts.RightFoot }, Material.Materials.First(m => m.name == "Wood"), Quality.Poor, ArmourLayer.light)
                    }
            };
            playerTroop.weapons.Add(new Weapon(50, BaseAttackType.magic, BaseDamageType.magic, 40, "GOD", 10, true));
            player.troop = playerTroop;
            Assert.IsTrue(player.troop.health.Value > 0 && player.troop.health.Value <= player.troop.health.MaxValue());
            player.troop.health.RawValue += 1000;
            Assert.IsTrue(player.troop.health.Value > 0 && player.troop.health.Value <= player.troop.health.MaxValue() && player.troop.health.RawValue <= player.troop.health.MaxValue());
            player.mana.RawValue += 10000;
            Assert.IsTrue(player.mana.Value > player.mana.MaxValue());
        }
    }
}
