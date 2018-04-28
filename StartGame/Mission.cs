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
        public abstract (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions, string description) GenerateMission(int difficulty, int Round, Map map, Player player);

        public static WinCheck deathCheck = ((_map, main) => main.players.Count == 1 && main.players.Exists(p => p == main.humanPlayer));

        public static WinCheck playerDeath = ((_map, main) => main.humanPlayer == null || main.humanPlayer.troop.health <= 0);
    }

    internal class BanditMission : Mission
    {
        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions, string description) GenerateMission(int difficulty, int Round, Map map, Player player)
        {
            Random rng = new Random();
            rng.Next();

            string desc = "Welcome to the mission. ";

            #region Player Creation

            List<Player> players = new List<Player>() { player };

            //Set player position
            List<Point> startPos = map.DeterminSpawnPoint(1, SpawnType.road);
            if (startPos.Count == 0) startPos = map.DeterminSpawnPoint(1, SpawnType.randomLand);
            players[0].troop.position = startPos[0];

            //Generate enemies and set position
            int enemyNumber = Round + map.width / 15;

            List<string> botNames = new List<string>() { "Hal", "Test", "Steve", "John", "Duff", "Mike", "Pence", "Alfred", "Bob", "Donald" };

            List<Point> spawnPoints = map.DeterminSpawnPoint(enemyNumber,
                SpawnType.randomLand);

            for (int i = 0; i < enemyNumber; i++)
            {
                string name = botNames[rng.Next(botNames.Count)];
                botNames.Remove(name);
                players.Add(new BanditAI(PlayerType.computer, name, map, new Player[] { player })
                {
                    troop = new Troop(name, 10 + (difficulty / 2) + (int)(Round * 1.5) - 4,
                    new Weapon(4 + difficulty / 4 + Round - 1,
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
            desc += "Kill all the enemies. ";

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
                if (distance(goal, playerPosition) > map.width / 2)
                {
                    desc += $"You can also reach the field [{goal.X}, {goal.Y}] to win.";
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

            desc += "Do not die. ";

            #endregion DeathCondition Creation

            desc += "Good luck! ";
            return (players, wins, deaths, desc);
        }
    }

    internal class SpiderNestMission : Mission
    {
        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions, string description)
            GenerateMission(int difficulty, int Round, Map map, Player player)
        {
            Random rng = new Random();
            rng.Next();

            string desc = "Welcome to the mission. ";

            #region Player Creation

            List<Player> players = new List<Player>() { player };

            //Set player position
            List<Point> startPos = map.DeterminSpawnPoint(1, SpawnType.road);
            if (startPos.Count == 0) startPos = map.DeterminSpawnPoint(1, SpawnType.randomLand);
            players[0].troop.position = startPos[0];

            //Generate basic spiders and set position
            int enemyNumber = Round * 2 + 10;

            List<Point> spawnPoints = map.DeterminSpawnPoint(enemyNumber,
                SpawnType.randomLand);

            for (int i = 0; i < enemyNumber; i++)
            {
                string name = $"Spider Warrior {i}";
                players.Add(new WarriorSpiderAI(PlayerType.computer, name, map, new Player[] { player })
                {
                    troop = new Troop(name, (difficulty / 3) + (int)(Round * 1.5) + 2,
                    new Weapon(1 + difficulty / 5 + Round / 2,
                        AttackType.melee, 1, "Fangs", 1, false),
                    Resources.spiderWarrior, 0, Dodge: 25)
                });
                players[i + 1].troop.position = spawnPoints[i];
            }

            //Generate the spider nest
            //Choose the land tile which is the furthest away, which is sorrounded by land tiles and is a land tile
            IOrderedEnumerable<MapTile> orderedEnumerable = map.map.Cast<MapTile>().ToList().Where(p => map.map[p.position.X, p.position.Y].type.type == MapTileTypeEnum.land && map.map[p.position.X, p.position.Y].neighbours.rawMaptiles.All(m => m.type.type == MapTileTypeEnum.land))
               .OrderByDescending(p => (Math.Abs(p.position.X - player.troop.position.X) + Math.Abs(p.position.Y - player.troop.position.Y)));
            Point spawnPoint = orderedEnumerable.FirstOrDefault().position;

            players.Add(new SpiderNestAI(PlayerType.computer, "Spider Nest", map, new Player[] { player }, difficulty, Round)
            {
                troop = new Troop("Spider Nest", 10 + (difficulty / 2), null, Resources.spiderNest, 2, 0)
                {
                    position = spawnPoint
                }
            });

            #endregion Player Creation

            #region WinCodition Creation

            //Must kill spider nest to win

            WinCheck nestState = new WinCheck((_map, main) => !_map.troops.Exists(t => t.name == "Spider Nest"));

            List<WinCheck> wins = new List<WinCheck>
            {
                deathCheck,
                nestState
            };
            desc += "Kill all the enemies. Destroy the spider nest. ";

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
    }
}