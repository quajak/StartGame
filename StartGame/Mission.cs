using PlayerCreator;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame
{
    internal abstract class Mission
    {
        public abstract (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions) GenerateMission(Campaign campaign, Map map, Player player);

        public static WinCheck deathCheck = ((_map, main) => main.players.Count == 1 && main.players.Exists(p => p == main.humanPlayer));

        public static WinCheck playerDeath = ((_map, main) => main.humanPlayer.troop.health <= 0);
    }

    internal class BanditMission : Mission
    {
        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions) GenerateMission(Campaign campaign, Map map, Player player)
        {
            Random rng = new Random();
            rng.Next();

            #region Player Creation

            List<Player> players = new List<Player>() { player };

            //Set player position
            List<Point> startPos = map.DeterminSpawnPoint(1, SpawnType.road);
            if (startPos.Count == 0) startPos = map.DeterminSpawnPoint(1, SpawnType.randomLand);
            players[0].troop.position = startPos[0];

            //Generate enemies and set position
            int enemyNumber = campaign.Round + map.width / 10;
            short botNumber = Convert.ToInt16(Resources.BOTAmount);
            List<string> botNames = new List<string>();
            for (int i = 0; i < botNumber; i++)
            {
                botNames.Add(Resources.ResourceManager.GetString("BOTName" + i));
            }

            List<Point> spawnPoints = map.DeterminSpawnPoint(enemyNumber,
                SpawnType.randomLand);

            for (int i = 0; i < enemyNumber; i++)
            {
                string name = botNames[rng.Next(botNames.Count)];
                botNames.Remove(name);
                players.Add(new Player(PlayerType.computer, name, map, new Player[] { player })
                {
                    troop = new Troop(name, 10 + (campaign.difficulty / 2) + (int)(campaign.Round * 1.5) - 4,
                    new Weapon(4 + campaign.difficulty / 4 + campaign.Round - 1,
                        AttackType.melee, 1, "Fists", 1, false),
                    Resources.enemyScout, 0)
                });
                players[i + 1].troop.position = spawnPoints[i];
            }

            #endregion Player Creation

            #region WinCodition Creation

            List<WinCheck> wins = new List<WinCheck>
            {
                deathCheck
            };

            int distance(Point A, Point B)
            {
                return Math.Abs(A.X - B.X) + Math.Abs(A.Y - B.Y);
            }

            //Find goal region
            Point playerPosition = startPos[0];
            List<MapTile> goalLocations = new List<MapTile>(map.goals);
            if (goalLocations.Count > 1)
            {
                goalLocations.Sort((l1, l2) =>
                {
                    int dis1 = distance(l1.position, playerPosition);
                    int dis2 = distance(l2.position, playerPosition);
                    if (dis1 == dis2) return 0;
                    else if (dis1 > dis2) return -1;
                    else return 1;
                });
                Point goal = goalLocations.First().position;
                //if (distance(goal, playerPosition) > map.width / 2)
                if (true)
                {
                    //Make the player go somewhere
                    map.overlayObjects.Add(new OverlayRectangle(goal.X * MapCreator.fieldSize, goal.Y * MapCreator.fieldSize, MapCreator.fieldSize, MapCreator.fieldSize, Color.Gold, false, false));
                    wins.Add((_map, main) => goal == main.humanPlayer.troop.position);
                }
            }

            #endregion WinCodition Creation

            #region DeathCondition Creation

            List<WinCheck> deaths = new List<WinCheck>
            {
                playerDeath
            };

            #endregion DeathCondition Creation

            return (players, wins, deaths);
        }
    }
}