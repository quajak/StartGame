using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame;
using StartGame.GameMap;
using StartGame.Items;
using StartGame.Mission;
using StartGame.PlayerData;
using StartGame.Properties;

namespace Tests
{
    //Tests for the core game play
    [TestClass]
    public class PlayGameTest
    {

        [TestMethod]
        public void TestMapCreation()
        {
            Map map = new Map();
            map.SetupMap(0.1, 12d, 0d);
            Assert.IsNotNull(map.map);
        }
        [TestMethod]
        public void SimpleStart()
        {
            Initialise(out Map map, out List<Tree> trees, out HumanPlayer player);
            Assert.IsNotNull(player);
            MainGameWindow mainGame = new MainGameWindow(map, player, new DebugMission(), trees, 1, 1);
            Assert.IsNotNull(mainGame);
            Assert.IsNotNull(mainGame.humanPlayer);
            Assert.IsNotNull(mainGame.map);
            Assert.IsFalse(mainGame.dead);
        }

        [TestMethod]
        public void CheckAllMissions()
        {
            List<Mission> missions = new List<Mission> { new BearMission(), new ElementalWizardFight(), new AttackCampMission(), new SpiderNestMission(), new BanditMission(), new DragonFight() };

            foreach (var mission in missions)
            {
                Initialise(out Map map, out List<Tree> trees, out HumanPlayer player);
                MainGameWindow mainGame = new MainGameWindow(map, player, new DebugMission(), trees, 1, 1);
                Assert.IsNotNull(mainGame);
                Assert.IsNotNull(mainGame.humanPlayer);
                Assert.IsNotNull(mainGame.map);
                Assert.IsFalse(mainGame.dead);
            }
        }


        private static void Initialise(out Map map, out List<Tree> trees, out HumanPlayer player)
        {
            map = new Map();
            map.SetupMap(0.1, 12d, 0d);
            trees = Tree.GenerateTrees();
            player = new HumanPlayer(PlayerType.localHuman, "Test", null, null, null, 0);
            Troop playerTroop = new Troop("Test", new Weapon(5, BaseAttackType.melee, BaseDamageType.blunt, 1, "Punch", 2, false), Resources.playerTroop, 0, map, player)
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
            player.troop.armours.ForEach(a => a.active = true);
        }
    }
}
