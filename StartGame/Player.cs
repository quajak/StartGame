using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlayerCreator;

namespace StartGame
{
    internal class Player
    {
        public PlayerType type;
        public string Name;
        public Troop troop;
        public double actionPoints = 0;
        public int maxActionPoints = 4;
        public bool active = false;
        public Map map;
        private Player[] enemies;

        public Player(PlayerType Type, string Name, Map Map, Player[] Enemies)
        {
            enemies = Enemies;
            type = Type;
            this.Name = Name;
            map = Map;
        }

        public void PlayTurn(Button actionDescriber, MainGameWindow main)
        {
            foreach (Weapon weapon in troop.weapons)
            {
                weapon.attacks = weapon.maxAttacks;
            }
            if (type == PlayerType.localHuman)
            {
                actionPoints = maxActionPoints;
                active = true;
                MessageBox.Show("It is your turn!");
                actionDescriber.Text = "End Turn";
            }
            else
            {
                actionPoints = maxActionPoints;
                actionDescriber.Enabled = false;
                DistanceGraphCreator distanceGraph = new DistanceGraphCreator(troop.position.X, troop.position.Y, enemies[0].troop.position.X, enemies[0].troop.position.Y, map);
                Thread path = new Thread(distanceGraph.CreateGraph);
                path.Start();
                active = false;
                MessageBox.Show($"It is {Name}'s turn!");
                actionDescriber.Text = "Next Turn";
                path.Join();

                int damageDealt = 0;

                while (actionPoints > 0)
                {
                    int[] playerPos = new int[2] {
                        enemies[0].troop.position.X, enemies[0].troop.position.Y };
                    //Check if it can attack player
                    int playerDistance = Math.Abs(playerPos[0] - troop.position.X) + Math.Abs(playerPos[1] -
                        troop.position.Y);
                    if (playerDistance <= troop.activeWeapon.range &&
                        troop.activeWeapon.attacks > 0)
                    {
                        //Attack
                        var (damage, killed) = main.Attack(this, enemies[0]);
                        damageDealt += damage;

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
                    int closestDistance = int.MaxValue;
                    int[] closestField = new int[2];
                    for (int x = 0; x <= distanceGraph.graph.GetUpperBound(0); x++)
                    {
                        for (int y = 0; y <= distanceGraph.graph.GetUpperBound(1); y++)
                        {
                            int playerDis = Math.Abs(playerPos[0] - x) + Math.Abs(playerPos[1] - y);
                            if (playerDis < closestDistance && distanceGraph.graph[x, y] <= actionPoints
                                && !map.troops.Exists(t => t.position.X == x && t.position.Y == y))
                            {
                                closestDistance = playerDis;
                                closestField = new int[2] { x, y };
                            }
                        }
                    }
                    if (closestDistance >= playerDistance) break;
                    //Move to closest field
                    troop.position.X = closestField[0];
                    troop.position.Y = closestField[1];
                    actionPoints -= closestDistance;
                    map.DrawTroops();
                    distanceGraph = new DistanceGraphCreator(troop.position.X, troop.position.Y,
                        playerPos[0], playerPos[1], map);
                    distanceGraph.CreateGraph();
                }
                if (damageDealt != 0)
                    map.overlayObjects.Add(new OverlayText(enemies[0].troop.position.X * MapCreator.fieldSize, enemies[0].troop.position.Y * MapCreator.fieldSize, System.Drawing.Color.Red, $"-{damageDealt}"));
                actionDescriber.Enabled = true;
            }
        }

        public void ActionButtonPressed(MainGameWindow mainGameWindow)
        {
            mainGameWindow.NextTurn();
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

        public DistanceGraphCreator(int SX, int SY, int EX, int EY, Map Map)
        {
            map = Map;
            sX = SX;
            sY = SY;
            eX = EX;
            eY = EY;
        }

        public void CreateGraph()
        {
            double[,] mapValues;
            lock (this)
            {
                lock (map)
                {
                    graph = new double[map.map.GetUpperBound(0), map.map.GetUpperBound(1)];
                    mapValues = new double[map.map.GetUpperBound(0), map.map.GetUpperBound(1)];
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
                    if (checking[1] != 0 && (graph[checking[0], checking[1] - 1] == 0 || graph[checking[0], checking[1] - 1] > graph[checking[0], checking[1]] + mapValues[checking[0], checking[1] - 1]))
                        sorrounding.Add(new int[] { checking[0], checking[1] - 1 });
                    //Bottom
                    if (checking[1] != mapValues.GetUpperBound(1) && (graph[checking[0], checking[1] + 1] == 0 || graph[checking[0], checking[1] + 1] > graph[checking[0], checking[1]] + mapValues[checking[0], checking[1] + 1]))
                        sorrounding.Add(new int[] { checking[0], checking[1] + 1 });
                    //Left
                    if (checking[0] != 0 && (graph[checking[0] - 1, checking[1]] == 0 || graph[checking[0] - 1, checking[1]] > graph[checking[0] - 1, checking[1]] + mapValues[checking[0] - 1, checking[1]]))
                        sorrounding.Add(new int[] { checking[0] - 1, checking[1] });
                    //Right
                    if (checking[0] != mapValues.GetUpperBound(0) && (graph[checking[0] + 1, checking[1]] == 0 || graph[checking[0] + 1, checking[1]] > graph[checking[0] + 1, checking[1]] + mapValues[checking[0] + 1, checking[1]]))
                        sorrounding.Add(new int[] { checking[0] + 1, checking[1] });

                    foreach (int[] field in sorrounding)
                    {
                        graph[field[0], field[1]] = graph[checking[0], checking[1]] + mapValues[field[0], field[1]];
                    }
                    toCheck.AddRange(sorrounding);
                }
            }
        }
    }
}