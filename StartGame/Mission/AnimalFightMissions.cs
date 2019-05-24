using StartGame.Items;
using StartGame.PlayerData;
using StartGame.Properties;
using StartGame.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StartGame.World;
using StartGame.GameMap;

namespace StartGame.Mission
{
    //TODO: Make one abstract class, which does most of the work
    public class BearMission : Mission
    {
        public BearMission() : base("Bear Attack", Forced: true)
        {
        }

        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions,
            string description) GenerateMission(int difficulty, int Round, ref Map map, Player player)
        {

            string desc = "You are being attacked by a bear! ";

            #region Player Creation

            List<Player> players = new List<Player>() { player };

            //Set player position
            List<Point> startPos = map.DeterminSpawnPoint(1, SpawnType.road);
            if (startPos.Count == 0) startPos = map.DeterminSpawnPoint(1, SpawnType.randomLand);
            players[0].troop.Position = startPos[0];

            //Generate enemies and set position
            BearAI bear = new BearAI(PlayerType.computer, map, new Player[] { player });
            bear.troop = new Troop("Bear",
                new Weapon(4 + difficulty / 4 + Round - 1,
                    BaseAttackType.melee, BaseDamageType.sharp, 2, "Claws", 1, false),
                Resources.Bear, 0, map, bear) {
                    armours = new List<Armour>{}
            };
            players.Add(bear);
            bear.troop.Position = map.DeterminSpawnPoint(1, SpawnType.random)[0];

            #endregion Player Creation

            #region WinCodition Creation

            List<WinCheck> wins = new List<WinCheck>
            {
                deathCheck
            };
            desc += "Kill the bear. ";

            #endregion WinCodition Creation

            #region DeathCondition Creation

            List<WinCheck> deaths = new List<WinCheck>
            {
                playerDeath
            };

            desc += "Survive!";

            #endregion DeathCondition Creation

            return (players, wins, deaths, desc);
        }

        public override bool MapValidity(Map map)
        {
            return true;
        }

        public override Reward Reward()
        {
            return new Reward(
                new JewelryReward(0, Quality.Broken),
                null,
                20,
                null,
                12,
                new ItemReward(new Food("Bear Meat", World.World.random.Next(3, 8), 8, 3))
            );
        }

        public override bool MissionAllowed(int Round)
        {
            return Round < 5;
        }

        public override bool MissionAllowed(WorldTileType type, int wealth)
        {
            return type == WorldTileType.Alpine || type == WorldTileType.Tundra;
        }
    }

    public class IceElementalMission : Mission
    {
        public IceElementalMission() : base("Ice Elemental Fight", Forced: true)
        {
        }

        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions,
            string description) GenerateMission(int difficulty, int Round, ref Map map, Player player)
        {

            string desc = "You are being attacked by a ice elemental! ";

            #region Player Creation

            List<Player> players = new List<Player>() { player };

            //Set player position
            List<Point> startPos = map.DeterminSpawnPoint(1, SpawnType.road);
            if (startPos.Count == 0) startPos = map.DeterminSpawnPoint(1, SpawnType.randomLand);
            players[0].troop.Position = startPos[0];

            //Generate enemies and set position
            IceElementalAI iceElementalAi = new IceElementalAI(PlayerType.computer, map, new Player[] { player }, Round + 1);
            iceElementalAi.troop = new Troop("Ice Elemental", null,
                Resources.IceElemental, 0, map, iceElementalAi) {
                armours = new List<Armour> { }
            };
            players.Add(iceElementalAi);
            iceElementalAi.troop.Position = map.DeterminSpawnPoint(1, SpawnType.random)[0];

            #endregion Player Creation

            #region WinCodition Creation

            List<WinCheck> wins = new List<WinCheck>
            {
                deathCheck
            };
            desc += "Kill the ice elemetal. ";

            #endregion WinCodition Creation

            #region DeathCondition Creation

            List<WinCheck> deaths = new List<WinCheck>
            {
                playerDeath
            };

            desc += "Survive!";

            #endregion DeathCondition Creation

            return (players, wins, deaths, desc);
        }

        public override bool MapValidity(Map map)
        {
            return true;
        }

        public override Reward Reward()
        {
            return new Reward(
                new JewelryReward(0, Quality.Broken),
                null,
                20,
                null,
                12,
                new ItemReward(new Resource("Ice Core", 1, 50))
            );
        }

        public override bool MissionAllowed(int Round)
        {
            return Round < 5;
        }

        public override bool MissionAllowed(WorldTileType type, int wealth)
        {
            return type == WorldTileType.Tundra;
        }
    }
}