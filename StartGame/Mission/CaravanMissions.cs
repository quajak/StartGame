using StartGame.Entities;
using StartGame.GameMap;
using StartGame.Items;
using StartGame.PlayerData;
using StartGame.Properties;
using StartGame.Rendering;
using StartGame.World;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StartGame.Mission
{
    public class CaravanDefense : Mission
    {
        public CaravanDefense() : base("Caravan Defense", Forced: true, canPlayerEscape: false, lootDead: false)
        {
        }

        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions,
            string description) GenerateMission(int difficulty, int Round, ref Map map, Player player)
        {
            string desc = "Defend the caravan. ";

            #region Player Creation

            List<Player> players = new List<Player>() { player };

            //Set player position
            List<Point> startPos = map.DeterminSpawnPoint(1, SpawnType.centralRoad);
            if (startPos.Count == 0) startPos = map.DeterminSpawnPoint(1, SpawnType.randomLand);
            players[0].troop.Position = startPos[0];

            // Add wagon
            Building caravanWagon = new Building("Wagon", startPos[0], Resources.CaravanWagon, map);
            map.entities.Add(caravanWagon);
            map.renderObjects.Add(new EntityRenderObject(caravanWagon, new TeleportPointAnimation(new Point(0, 0), caravanWagon.Position)));

            //Generate enemies and set position
            int enemyNumber = difficulty / 2 + 2;


            List<Point> spawnPoints = map.DeterminSpawnPoint(enemyNumber,
                SpawnType.randomRoughCircle);

            for (int i = 0; i < enemyNumber; i++)
            {
                Bitmap image = Resources.enemyScout;
                Weapon weapon = new Weapon(3 + difficulty / 4 + World.World.random.Next(-2, 4),
                        BaseAttackType.melee, BaseDamageType.sharp, 1, "Sword", 1, false);
                if (World.World.random.Next(5) == 1)
                {
                    weapon = new RangedWeapon(4 + difficulty / 6, BaseDamageType.sharp, 8, "Bow", 12, false, AmmoType.Arrow, 2);
                    image = Resources.Archer;
                }
                string name = PlayerDataResource.GetMaleName();
                BanditAI item = new BanditAI(PlayerType.computer, name, map, new Player[] { player }, 2 + difficulty / 5 + World.World.random.Next(-1, 2));
                item.troop = new Troop(name,
                    weapon,
                    image, 0, map, item) {
                    armours = new List<Armour>
                        {
                            new Armour("Shirt", 32, new List<BodyParts>{ BodyParts.LeftUpperArm, BodyParts.RightUpperArm, BodyParts.Torso}, Material.Materials.First(m => m.name == "Cloth"), Quality.Broken, ArmourLayer.clothing),
                            new Armour("Hose", 60, new List<BodyParts> { BodyParts.UpperLegs, BodyParts.LeftLowerLeg, BodyParts.RightLowerLeg, BodyParts.LeftShin, BodyParts.RightShin }, Material.Materials.First(m => m.name == "Cloth"), Quality.Simple, ArmourLayer.clothing)
                        }
                };
                item.troop.health.RawValue += World.World.random.Next(2, 5);
                players.Add(item);
                players[i + 1].troop.Position = spawnPoints[i];
            }

            #endregion Player Creation

            #region WinCodition Creation

            List<WinCheck> wins = new List<WinCheck>
            {
                deathCheck
            };
            desc += "Kill all the enemies. ";

            #endregion WinCodition Creation

            #region DeathCondition Creation

            List<WinCheck> deaths = new List<WinCheck>
            {
                playerDeath
            };

            desc += "Do not die. ";

            #endregion DeathCondition Creation

            desc += "Good luck! ";
            return (players, wins, deaths, desc);
        }

        public override bool MapValidity(Map map)
        {
            if (map.mapBiome != null && map.mapBiome is DesertMapBiome)
                return true;
            
            for (int x = 0; x < map.width; x++)
            {
                for (int y = 0; y < map.height; y++)
                {
                    if (map.map[x, y].type.type == MapTileTypeEnum.road)
                        return true;
                }
            }
            return false;
        }

        public override Reward Reward()
        {
            bool jewleryReward = World.World.random.Next(3) == 1;
            bool weaponReward = World.World.random.Next(4) == 1;
            return new Reward(
                jewleryReward ? new JewelryReward(1, Quality.Poor) : null,
                weaponReward ? new WeaponReward(true, 0, null) : null,
                6 + World.World.random.Next(4),
                null,
                12,
                null
            );
        }

        public override bool MissionAllowed(int Round)
        {
            return Round < 5;
        }
        public override bool MissionAllowed(WorldTileType type, int wealth)
        {
            return type != WorldTileType.Ocean;
        }
    }

    public class CaravanAttack : Mission
    {
        //TODO: Give some of the loot from the caravan to the player
        //TODO: Trigger aggresive AI when casting spell to hurt bandit
        public CaravanAttack() : base("Caravan Attack", canPlayerEscape: true)
        {

        }

        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions, string description)
            GenerateMission(int difficulty, int Round, ref Map map, Player player)
        {
            string desc = "Raid the caravan! ";

            #region Player Creation

            List<Player> players = new List<Player>() { player };

            //Find position of player
            players[0].troop.Position = map.DeterminSpawnPoint(1, SpawnType.randomLand)[0];

            int enemyNumber = GetEnemyNumber(difficulty);
            //Find position of caravan
            List<Point> caravanPoint = map.DeterminSpawnPoint(1 + enemyNumber, SpawnType.randomRoughRoadCircle);
            //Make caravan
            Building caravanWagon = new Building("Caravan", caravanPoint[0], Resources.CaravanWagon, map);
            map.entities.Add(caravanWagon);
            map.renderObjects.Add(new EntityRenderObject(caravanWagon, new TeleportPointAnimation(new Point(0, 0), caravanWagon.Position)));


            //Generate basic bandits and set position
            for (int i = 0; i < enemyNumber; i++)
            {
                string name = PlayerDataResource.GetMaleName();
                DefensiveBanditAI item = new DefensiveBanditAI(PlayerType.computer, name, map, new Player[] { player }, caravanPoint[0]);
                item.troop = new Troop(name, new Weapon(3 + difficulty / 5, BaseAttackType.melee, BaseDamageType.sharp, 1, "Dagger", 2, false),
                Resources.enemyScout, 0, map, item, Dodge: 25) {
                    armours = new List<Armour>
                        {
                            new Armour("Cap", 10, new List<BodyParts>{BodyParts.Head}, Material.Materials.First(m => m.name == "Cloth"),Quality.Simple, ArmourLayer.clothing),
                            new Armour("Shirt", 20, new List<BodyParts>{BodyParts.Torso}, Material.Materials.First(m => m.name == "Cloth"),Quality.Poor, ArmourLayer.clothing),
                            new Armour("Hose", 25, new List<BodyParts>{BodyParts.UpperLegs,BodyParts.LeftLowerLeg,BodyParts.RightLowerLeg}, Material.Materials.First(m => m.name == "Cloth"),Quality.Common, ArmourLayer.clothing),
                            new Armour("Cloak", 35, new List<BodyParts>{BodyParts.LeftUpperArm,BodyParts.RightUpperArm,BodyParts.Torso,BodyParts.UpperLegs,BodyParts.LeftLowerLeg,BodyParts.RightLowerLeg}, Material.Materials.First(m => m.name == "Wool"),Quality.Common, ArmourLayer.light)
                        }
                };
                item.troop.health.RawValue -= 10;
                players.Add(item);

                players[i + 1].troop.Position = caravanPoint[i];
            }

            #endregion Player Creation

            #region WinCodition Creation

            //Must kill spider nest to win

            List<WinCheck> wins = new List<WinCheck>
            {
                deathCheck
            };
            desc += "Kill all the guards to steal their caravan. ";

            #endregion WinCodition Creation

            #region DeathCondition Creation

            List<WinCheck> deaths = new List<WinCheck>
            {
                playerDeath
            };

            desc += "Do not die. ";

            #endregion DeathCondition Creation

            desc += "Good luck! ";
            return (players, wins, deaths, desc);
        }

        internal override int GetEnemyNumber(int difficulty)
        {
            return 2 + difficulty / 3;
        }

        public override bool MapValidity(Map map)
        {
            return map.goals.Count > 1;
        }

        public override Reward Reward()
        {
            return new Reward(
                new JewelryReward(1, Quality.Simple),
                new WeaponReward(true, 3, null),
                5,
                null,
                34 + World.World.random.Next(20),
                null
            );
        }

        public override bool MissionAllowed(int Round)
        {
            return Round > 2;
        }

        public override bool MissionAllowed(WorldTileType type, int wealth)
        {
            return type == WorldTileType.TemperateForest || type == WorldTileType.TemperateGrassland || type == WorldTileType.Alpine;
        }
    }
}