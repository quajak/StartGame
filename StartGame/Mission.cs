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
    internal struct Reward
    {
        public WeaponReward? weaponReward;
        public int XP;
        public SpellReward? spellReward;
    }

    internal struct SpellReward
    {
        public Spell spell;
    }

    internal struct WeaponReward
    {
        public bool random;
        public int rarity;
        public Weapon reward;
    }

    internal abstract class Mission
    {
        public double heightDiff = 0;
        public bool EnemyMoveTogether = false;

        public abstract (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions, string description) GenerateMission(int difficulty, int Round, Map map, Player player);

        public static WinCheck deathCheck = ((_map, main) => main.players.Count == 1 && main.players.Exists(p => p == main.humanPlayer));

        public static WinCheck playerDeath = ((_map, main) => main.humanPlayer == null || main.humanPlayer.troop.health <= 0);

        public abstract bool MapValidity(Map map);

        public abstract Reward Reward();

        public abstract bool MissionAllowed(int Round);
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
            players[0].troop.Position = startPos[0];

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
                    Resources.enemyScout, 0, map)
                });
                players[i + 1].troop.Position = spawnPoints[i];
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
                    map.overlayObjects.Add(new OverlayRectangle(goal.X * MapCreator.fieldSize, goal.Y * MapCreator.fieldSize, MapCreator.fieldSize, MapCreator.fieldSize, Color.Gold, Once: false));
                    wins.Add((_map, main) => goal == main.humanPlayer.troop.Position);
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

        public override bool MapValidity(Map map)
        {
            return true;
        }

        public override Reward Reward()
        {
            return new Reward()
            {
                weaponReward = new WeaponReward() { random = true, rarity = 0, reward = null }
                ,
                XP = 5,
                spellReward = new SpellReward() { spell = new TeleportSpell(2, 0) }
            };
        }

        public override bool MissionAllowed(int Round)
        {
            return Round < 5;
        }
    }

    internal class SpiderNestMission : Mission
    {
        //Long term: Allow more than one spider nest to spawn
        //Long term: When first hit, spawn a few spiders
        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions, string description)
            GenerateMission(int difficulty, int Round, Map map, Player player)
        {
            Random rng = new Random();
            rng.Next();

            EnemyMoveTogether = true;

            string desc = "Welcome to the mission. ";

            #region Player Creation

            List<Player> players = new List<Player>() { player };

            //Set player position
            List<Point> startPos = map.DeterminSpawnPoint(1, SpawnType.road);
            if (startPos.Count == 0) startPos = map.DeterminSpawnPoint(1, SpawnType.randomLand);
            players[0].troop.Position = startPos[0];

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
                    Resources.spiderWarrior, 0, map, Dodge: 25)
                });
                players[i + 1].troop.Position = spawnPoints[i];
            }

            //Generate the spider nest
            //Choose the land tile which is the furthest away, which is sorrounded by land tiles and is a land tile
            IOrderedEnumerable<MapTile> orderedEnumerable = map.map.Cast<MapTile>().ToList().Where(p => map.map[p.position.X, p.position.Y].type.type == MapTileTypeEnum.land && map.map[p.position.X, p.position.Y].neighbours.rawMaptiles.All(m => m.type.type == MapTileTypeEnum.land))
               .OrderByDescending(p => (Math.Abs(p.position.X - player.troop.Position.X) + Math.Abs(p.position.Y - player.troop.Position.Y)));
            Point spawnPoint = orderedEnumerable.FirstOrDefault().position;

            players.Add(new SpiderNestAI(PlayerType.computer, "Spider Nest", map, new Player[] { player }, difficulty, Round)
            {
                troop = new Troop("Spider Nest", 10 + (difficulty / 2), null, Resources.spiderNest, 2, map, 0)
                {
                    Position = spawnPoint
                }
            });

            #endregion Player Creation

            #region WinCodition Creation

            //Must kill spider nest to win

            WinCheck nestState = new WinCheck((_map, main) => !_map.troops.Exists(t => t.name == "Spider Nest"));

            List<WinCheck> wins = new List<WinCheck>
            {
                nestState
            };
            desc += "Destroy the spider nest. ";

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
            return true;
        }

        public override Reward Reward()
        {
            return new Reward()
            {
                weaponReward = new WeaponReward() { random = true, rarity = 0, reward = null },
                XP = 2,
                spellReward = null
            };
        }

        public override bool MissionAllowed(int Round)
        {
            return Round > 1;
        }
    }

    internal class ElementalWizardFight : Mission
    {
        public ElementalWizardFight()
        {
        }

        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions, string description)
            GenerateMission(int difficulty, int Round, Map map, Player player)
        {
            Random rng = new Random();
            rng.Next();

            string desc = "Welcome to the mission. ";

            #region Player Creation

            List<Player> players = new List<Player>() { player };

            //Find position of player
            players[0].troop.Position = map.DeterminSpawnPoint(1, SpawnType.road)[0];

            List<Point> startPos = map.DeterminSpawnPoint(1, SpawnType.heighestField);
            string name = $"Elemental Wizard";
            players.Add(new ElementalWizard(PlayerType.computer, name, map, new Player[] { player }, difficulty, Round)
            {
                troop = new Troop(name, (difficulty / 3) + (int)(Round * 1.5) + 5,
                new Weapon(3 + difficulty / 5 + Round / 2,
                    AttackType.melee, 1, "Dagger", 2, false),
                Resources.enemyScout, 0, map, Dodge: 25)
            });
            players[1].troop.Position = startPos[0];

            #endregion Player Creation

            #region WinCodition Creation

            //Must kill spider nest to win

            List<WinCheck> wins = new List<WinCheck>
            {
                deathCheck
            };
            desc += "Kill the wizard to survive. ";

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
            return true;
        }

        public override Reward Reward()
        {
            return new Reward()
            {
                weaponReward = new WeaponReward() { random = true, rarity = 3, reward = null },
                XP = 5,
                spellReward = new SpellReward() { spell = new FireBall(10, 2, 3, 0) }
            };
        }

        public override bool MissionAllowed(int Round)
        {
            return Round > 3;
        }
    }

    internal class AttackCampMission : Mission
    {
        public AttackCampMission()
        {
            heightDiff = -0.2;
        }

        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions, string description)
            GenerateMission(int difficulty, int Round, Map map, Player player)
        {
            Random rng = new Random();
            rng.Next();

            string desc = "Welcome to the mission. ";

            #region Player Creation

            List<Player> players = new List<Player>() { player };

            //Find position of player
            players[0].troop.Position = map.DeterminSpawnPoint(1, SpawnType.road)[0];

            //Find position of campsite
            Point camp;
            camp = map.map.Cast<MapTile>().ToList().Where(t => t.neighbours.rawMaptiles.Count(n => n.type.FType == FieldType.land) == 4).OrderByDescending(m => AIUtility.Distance(m.position, player.troop.Position)).First().position;
            //Make camp fire
            map.entites.Add(new Building("Fireplace", camp, Resources.firePlace, map));

            int enemyNumber = Round / 5 + 2 * difficulty / 3;
            List<Point> startPos = new List<Point>();
            List<Point> toCheck = map.map[camp.X, camp.Y].neighbours.rawMaptiles.ToList().ConvertAll(m => m.position);
            //Add a few more than neccessary so ones which are path can be removed
            while (startPos.Count <= enemyNumber + 10 && toCheck.Count != 0)
            {
                Point checking = toCheck[0];
                toCheck.Remove(checking);
                startPos.Add(checking);

                map.map[checking.X, checking.Y].neighbours.rawMaptiles.ToList().ForEach(m =>
                {
                    //Check if it does not exist, if land and is not blocked
                    if (!startPos.Exists(f => f.X == m.position.X && f.Y == m.position.Y) && m.type.FType == FieldType.land && m.free)
                    {
                        toCheck.Add(m.position);
                    }
                });
            }

            enemyNumber = Math.Min(enemyNumber, startPos.Count); //in case there are too few positions

            //If extra, remove path
            while (startPos.Count > enemyNumber)
            {
                if (startPos.Exists(f => map.map[f.X, f.Y].type.type == MapTileTypeEnum.path))
                {
                    startPos.Remove(startPos.First(f => map.map[f.X, f.Y].type.type == MapTileTypeEnum.path));
                }
                //If no path left remove random
                else
                {
                    startPos.RemoveAt(rng.Next(startPos.Count));
                }
            }

            //Generate basic bandits and set position
            for (int i = 0; i < enemyNumber; i++)
            {
                string name = $"Bandit {i + 1}";
                players.Add(new DefensiveBanditAI(PlayerType.computer, name, map, new Player[] { player }, camp)
                {
                    troop = new Troop(name, (difficulty / 3) + (int)(Round * 1.5) + 5,
                    new Weapon(3 + difficulty / 5 + Round / 2,
                        AttackType.melee, 1, "Dagger", 2, false),
                    Resources.enemyScout, 0, map, Dodge: 25)
                });
                players[i + 1].troop.Position = startPos[i];
            }

            #endregion Player Creation

            #region WinCodition Creation

            //Must kill spider nest to win

            List<WinCheck> wins = new List<WinCheck>
            {
                deathCheck
            };
            desc += "Kill all the bandits to steal their treasure. ";

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
            return map.goals.Count > 1 && map.map.Cast<MapTile>().ToList().Exists(t => t.neighbours.rawMaptiles.Count(n => n.type.FType == FieldType.land) == 4);
        }

        public override Reward Reward()
        {
            return new Reward()
            {
                weaponReward = new WeaponReward() { random = true, rarity = 3, reward = null },
                XP = 5,
                spellReward = null
            };
        }

        public override bool MissionAllowed(int Round)
        {
            return Round > 2;
        }
    }
}