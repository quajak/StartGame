using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlayerCreator;
using StartGame;
using StartGame.Properties;

namespace StartGame
{
    internal abstract class Player
    {
        public PlayerType type;
        public string Name;
        public double actionPoints = 0;
        public bool active = false;
        public Map map;
        internal Player[] enemies;
        public int XP;

        //Derived stats
        public int maxActionPoints = 4;

        public Troop troop;

        public bool singleTurn;

        public Player(PlayerType Type, string name, Map Map, Player[] Enemies, int XP, bool SingleTurn = true)
        {
            this.XP = XP;
            enemies = Enemies;
            type = Type;
            Name = name;
            map = Map;
            singleTurn = SingleTurn;
        }

        public abstract void PlayTurn(MainGameWindow main);

        public void ActionButtonPressed(MainGameWindow mainGameWindow)
        {
            mainGameWindow.NextTurn();
        }
    }

    internal class HumanPlayer : Player
    {
        //Base stats
        public int strength; //Bonus damage dealt

        public int agility; //Chance to dodge
        public int endurance; //How many action points + defense
        public int vitality; //How much health

        public int level = 1;
        public int xp = 0;
        public int levelXP = 10;
        public int storedLevelUps = 0;

        public MainGameWindow main;

        public List<Tree> trees = new List<Tree>();

        public HumanPlayer(PlayerType Type, string Name, Map Map, Player[] Enemies, MainGameWindow window) : base(Type, Name, Map, Enemies, 0)
        {
            main = window;
            strength = 1;
            agility = 1;
            endurance = 1;
            vitality = 10;
        }

        public void CalculateStats()
        {
            maxActionPoints = 4 + endurance / 10;

            int healthDifference = troop.health - troop.maxHealth;
            troop.maxHealth = 2 * vitality;
            troop.health = troop.maxHealth - healthDifference;

            troop.defense = endurance / 5;

            troop.dodge = troop.baseDodge + agility * 2;
        }

        public void GainXP(int XP)
        {
            xp += XP;
            if (xp >= levelXP)
            {
                xp -= levelXP;
                storedLevelUps += 1;
                levelXP = (int)(1.1 * levelXP);

                //Set Level up
                main.SetUpdateState(storedLevelUps > 0);
            }
        }

        public override void PlayTurn(MainGameWindow main)
        {
        }
    }

    internal class BanditAI : Player
    {
        public BanditAI(PlayerType Type, string Name, Map Map, Player[] Enemies) : base(Type, Name, Map, Enemies, 3)
        {
        }

        public override void PlayTurn(MainGameWindow main)
        {
            DistanceGraphCreator distanceGraph = new DistanceGraphCreator(troop.position.X, troop.position.Y, enemies[0].troop.position.X,
                enemies[0].troop.position.Y, map, true);
            Thread path = new Thread(distanceGraph.CreateGraph);
            path.Start();
            path.Join();

            int damageDealt = 0;
            int dodged = 0;

            while (actionPoints > 0)
            {
                Point playerPos = enemies[0].troop.position;

                //Check if it can attack player
                int playerDistance = AIUtility.Distance(playerPos, troop.position);
                if (playerDistance <= troop.activeWeapon.range &&
                    troop.activeWeapon.attacks > 0)
                {
                    //Attack
                    var (damage, killed, hit) = main.Attack(this, enemies[0]);
                    damageDealt += damage;
                    if (!hit) dodged++;

                    if (killed)
                    {
                        map.overlayObjects.Add(new OverlayText(enemies[0].troop.position.X * MapCreator.fieldSize, enemies[0].troop.position.Y * MapCreator.fieldSize, System.Drawing.Color.Red, $"-{damageDealt}"));
                        main.PlayerDied($"You have been killed by {Name}!");
                        break;
                    }
                    actionPoints--;
                    troop.activeWeapon.attacks--;
                    map.DrawTroops();
                    continue;
                }
                else if (troop.weapons.Exists(t => t.range >= playerDistance && t.attacks > 0))
                {
                    //Change weapon
                    Weapon best = troop.weapons.FindAll(t => t.range >= playerDistance)
                        .Aggregate((t1, t2) => t1.range > t2.range ? t1 : t2);
                    troop.activeWeapon = best;
                    continue;
                }

                //Find closest field to player
                Point closestField;
                closestField = AIUtility.FindClosestField(distanceGraph, playerPos, actionPoints, map,
                    (List<(Point point, double cost, double height)> list) =>
                    {
                        list.Sort((o1, o2) =>
                        {
                            double diffCost = o1.cost - o2.cost;
                            double heightDiff = o1.height - o2.height;
                            if (Math.Abs(diffCost) >= 1) //assume that using the weapon costs 1 action point
                            {
                                return diffCost < 0 ? -1 : 1;
                            }
                            else if (heightDiff != 0)
                            {
                                return diffCost < 0 ? -1 : 1;
                            }
                            return 0;
                        });
                        return list.First().point;
                    });

                //Move to closest field
                int closestDistance = AIUtility.Distance(closestField, playerPos);
                if (closestDistance >= playerDistance) break;
                troop.position.X = closestField.X;
                troop.position.Y = closestField.Y;
                actionPoints -= closestDistance;
                map.DrawTroops();
                distanceGraph = new DistanceGraphCreator(troop.position.X, troop.position.Y,
                    playerPos.X, playerPos.Y, map, true);
                distanceGraph.CreateGraph();
            }
            if (damageDealt != 0)
                map.overlayObjects.Add(new OverlayText(enemies[0].troop.position.X * MapCreator.fieldSize, enemies[0].troop.position.Y * MapCreator.fieldSize, System.Drawing.Color.Red, $"-{damageDealt}" + (dodged != 0 ? $" and dodged {dodged} times!" : "")));
            else if (dodged != 0)
                map.overlayObjects.Add(new OverlayText(enemies[0].troop.position.X * MapCreator.fieldSize, enemies[0].troop.position.Y * MapCreator.fieldSize, System.Drawing.Color.Red, $" Doged {dodged} {(dodged > 1 ? "times" : "time")}!"));
        }
    }

    internal class WarriorSpiderAI : Player
    {
        public WarriorSpiderAI(PlayerType Type, string Name, Map Map, Player[] Enemies) : base(Type, Name, Map, Enemies, 1, false)
        {
        }

        public override void PlayTurn(MainGameWindow main)
        {
            DistanceGraphCreator distanceGraph = new DistanceGraphCreator(troop.position.X, troop.position.Y, enemies[0].troop.position.X, enemies[0].troop.position.Y, map, false);
            Thread path = new Thread(distanceGraph.CreateGraph);
            path.Start();
            path.Join();

            int damageDealt = 0;
            int dodged = 0;

            while (actionPoints > 0)
            {
                Point playerPos = enemies[0].troop.position;

                //Check if it can attack player
                int playerDistance = AIUtility.Distance(playerPos, troop.position);
                if (playerDistance <= troop.activeWeapon.range &&
                    troop.activeWeapon.attacks > 0)
                {
                    //Attack
                    var (damage, killed, hit) = main.Attack(this, enemies[0]);
                    damageDealt += damage;
                    if (!hit) dodged++;

                    if (killed)
                    {
                        map.overlayObjects.Add(new OverlayText(enemies[0].troop.position.X * MapCreator.fieldSize, enemies[0].troop.position.Y * MapCreator.fieldSize, System.Drawing.Color.Red, $"-{damageDealt}"));
                        main.PlayerDied($"You have been killed by {Name}!");
                        break;
                    }
                    actionPoints--;
                    troop.activeWeapon.attacks--;
                    map.DrawTroops();
                    continue;
                }
                else if (troop.weapons.Exists(t => t.range >= playerDistance && t.attacks > 0))
                {
                    //Change weapon
                    Weapon best = troop.weapons.FindAll(t => t.range >= playerDistance)
                        .Aggregate((t1, t2) => t1.range > t2.range ? t1 : t2);
                    troop.activeWeapon = best;
                    continue;
                }

                //Find closest field to player
                Point closestField;
                closestField = AIUtility.FindClosestField(distanceGraph, playerPos, actionPoints, map,
                    (List<(Point point, double cost, double height)> list) =>
                    {
                        list.Sort((o1, o2) =>
                        {
                            double diffCost = o1.cost - o2.cost;
                            double heightDiff = o1.height - o2.height;
                            if (Math.Abs(diffCost) >= 1) //assume that using the weapon costs 1 action point
                            {
                                return diffCost < 0 ? -1 : 1;
                            }
                            else if (heightDiff != 0)
                            {
                                return diffCost < 0 ? -1 : 1;
                            }
                            return 0;
                        });
                        return list.First().point;
                    });

                //Move to closest field
                int closestDistance = AIUtility.Distance(closestField, playerPos);
                if (closestDistance >= playerDistance) break;
                troop.position.X = closestField.X;
                troop.position.Y = closestField.Y;
                actionPoints -= closestDistance;
                map.DrawTroops();
                distanceGraph = new DistanceGraphCreator(troop.position.X, troop.position.Y,
                    playerPos.X, playerPos.Y, map, false);
                distanceGraph.CreateGraph();
            }
            if (singleTurn)
            {
                if (damageDealt != 0)
                    map.overlayObjects.Add(new OverlayText(enemies[0].troop.position.X * MapCreator.fieldSize, enemies[0].troop.position.Y * MapCreator.fieldSize, System.Drawing.Color.Red, $"-{damageDealt}" + (dodged != 0 ? $" and dodged {dodged} times!" : "")));
                else if (dodged != 0)
                    map.overlayObjects.Add(new OverlayText(enemies[0].troop.position.X * MapCreator.fieldSize, enemies[0].troop.position.Y * MapCreator.fieldSize, System.Drawing.Color.Red, $" Doged {dodged} {(dodged > 1 ? "times" : "time")}!"));
            }
            else
            {
                main.playerDamage += damageDealt;
                main.playerDoged += dodged;
            }
        }
    }

    internal class SpiderNestAI : Player
    {
        private int numSpawned = 0;
        private int difficulty;
        private int turn;
        private readonly int round;

        public SpiderNestAI(PlayerType Type, string Name, Map Map, Player[] Enemies, int Difficulty, int Round) : base(Type, Name, Map, Enemies, 5, false)
        {
            map = Map;
            turn = 7 - (Difficulty / 2);
            round = Round;
            difficulty = Difficulty;
        }

        public override void PlayTurn(MainGameWindow main)
        {
            turn = turn == 0 ? 0 : turn - 1;
            if (turn == 0)
            {
                //Find the position for the new spider to spawn
                Point pos = new Point(-1, -1);
                foreach (MapTile tile in map.map[troop.position.X, troop.position.Y].neighbours.rawMaptiles)
                {
                    //Check if empty and not water
                    if (tile.type.type != MapTileTypeEnum.deepWater || tile.type.type != MapTileTypeEnum.shallowWater && !map.troops.Exists(t => t.position.X == tile.position.X && t.position.Y == tile.position.Y))
                    {
                        pos = tile.position;
                        break;
                    }
                }

                if (pos.X == -1) // No space available wait for next turn
                    return;

                //Spawn a new spider
                numSpawned++;
                WarriorSpiderAI spider = new WarriorSpiderAI(PlayerType.computer, "Spider Spawn " + numSpawned, map, main.players.ToArray())
                {
                    troop = new Troop("Spider Spawn " + numSpawned, (difficulty / 3) + (int)(round * 1.5) + 1, new Weapon(1 + difficulty / 5 + round / 2,
                        AttackType.melee, 1, "Fangs", 1, false), Resources.spiderWarrior, 0, 25)
                    {
                        position = pos
                    }
                };

                main.AddPlayer(spider);

                turn = 5 - (difficulty / 2);
            }
        }
    }

    internal delegate Point FieldOptimiser(List<(Point point, double cost, double height)> list);

    internal static class AIUtility
    {
        public static int Distance(Point A, Point B)
        {
            return Math.Abs(A.X - B.X) + Math.Abs(A.Y - B.Y);
        }

        public static Point FindClosestField(DistanceGraphCreator distanceGraph, Point A, double actionPoints, Map map, FieldOptimiser chooser)
        {
            int closestDistance = int.MaxValue;
            List<(Point point, double points, double height)> closestPoints = new List<(Point point, double points, double height)>();
            for (int x = 0; x <= distanceGraph.graph.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= distanceGraph.graph.GetUpperBound(1); y++)
                {
                    int playerDis = Math.Abs(A.X - x) + Math.Abs(A.Y - y);
                    if (distanceGraph.graph[x, y] <= actionPoints
                        && !map.troops.Exists(t => t.position.X == x && t.position.Y == y))
                    {
                        if (playerDis < closestDistance)
                        {
                            closestDistance = playerDis;
                            closestPoints.Clear();
                            closestPoints.Add((new Point(x, y), distanceGraph.graph[x, y], map.map[x, y].height));
                        }
                        else if (playerDis == closestDistance)
                        {
                            closestPoints.Add((new Point(x, y), distanceGraph.graph[x, y], map.map[x, y].height));
                        }
                    }
                }
            }
            if (closestPoints.Count == 0) return new Point(-20, -20);
            return chooser.Invoke(closestPoints);
        }
    }

    internal enum PlayerType
    { localHuman, computer };

    internal class DistanceGraphCreator
    {
        private Map map;
        private int sX;
        private int sY;
        private int eX;
        private int eY;
        public double[,] graph;
        private bool allowWater;

        public DistanceGraphCreator(int SX, int SY, int EX, int EY, Map Map, bool AllowWater)
        {
            map = Map;
            sX = SX;
            sY = SY;
            eX = EX;
            eY = EY;
            allowWater = AllowWater;
        }

        public void CreateGraph()
        {
            double[,] mapValues;
            lock (this)
            {
                lock (map)
                {
                    graph = new double[map.map.GetUpperBound(0) + 1, map.map.GetUpperBound(1) + 1];
                    mapValues = new double[map.map.GetUpperBound(0) + 1, map.map.GetUpperBound(1) + 1];
                    for (int x = 0; x <= mapValues.GetUpperBound(0); x++)
                    {
                        for (int y = 0; y <= mapValues.GetUpperBound(1); y++)
                        {
                            mapValues[x, y] = map.map[x, y].MovementCost;
                        }
                    }
                }
                List<int[]> toCheck = new List<int[]>() { new int[] { sX, sY } };
                graph[sX, sY] = 0;
                while (toCheck.Count != 0)
                {
                    int[] checking = toCheck[0];
                    toCheck.Remove(checking);

                    List<int[]> sorrounding = new List<int[]>();
                    //Top
                    if (checking[1] != 0 && (graph[checking[0], checking[1] - 1] == 0 ||
                            graph[checking[0], checking[1] - 1] > graph[checking[0], checking[1]] + mapValues[checking[0], checking[1] - 1]))
                        sorrounding.Add(new int[] { checking[0], checking[1] - 1 });
                    //Bottom
                    if (checking[1] != mapValues.GetUpperBound(1) && (graph[checking[0], checking[1] + 1] == 0 ||
                            graph[checking[0], checking[1] + 1] > graph[checking[0], checking[1]] + mapValues[checking[0], checking[1] + 1]))
                        sorrounding.Add(new int[] { checking[0], checking[1] + 1 });
                    //Left
                    if (checking[0] != 0 && (graph[checking[0] - 1, checking[1]] == 0 ||
                            graph[checking[0] - 1, checking[1]] > graph[checking[0] - 1, checking[1]] + mapValues[checking[0] - 1, checking[1]]))
                        sorrounding.Add(new int[] { checking[0] - 1, checking[1] });
                    //Right
                    if (checking[0] != mapValues.GetUpperBound(0) && (graph[checking[0] + 1, checking[1]] == 0 ||
                            graph[checking[0] + 1, checking[1]] > graph[checking[0] + 1, checking[1]] + mapValues[checking[0] + 1, checking[1]]))
                        sorrounding.Add(new int[] { checking[0] + 1, checking[1] });

                    foreach (int[] field in sorrounding)
                    {
                        if (!allowWater && map.map[field[0], field[1]].type.type == MapTileTypeEnum.deepWater
                                || map.map[field[0], field[1]].type.type == MapTileTypeEnum.shallowWater)
                            graph[field[0], field[1]] = 20;
                        else
                            graph[field[0], field[1]] = graph[checking[0], checking[1]] + mapValues[field[0], field[1]];
                    }
                    toCheck.AddRange(sorrounding);
                }
            }
        }
    }
}