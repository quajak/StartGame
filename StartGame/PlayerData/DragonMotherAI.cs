using StartGame.Entities;
using StartGame.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using static StartGame.MainGameWindow;

namespace StartGame.PlayerData
{
    internal class DragonMotherAI : Player
    {
        private readonly Point egg;
        private bool enraged = false;
        private readonly int fireDamage = 3;

        private int fireLineCoolDown = 0;
        private int fireBombCoolDown = 0;
        private readonly int maxFireLineCooldown = 2;
        private readonly int maxFireBombCooldown = 4;

        private int bombNumber = 3;
        private List<Point> fireBombPlaces = new List<Point>();

        private readonly int jumpDamage = 3;
        private int jumpCooldown = 3;
        private readonly int maxJumpCooldown = 3;
        private readonly int jumpDistance = 15;

        private MainGameWindow main;

        public DragonMotherAI(PlayerType Type, string Name, Map Map, Player[] Enemies, Point Camp, int Round, int Difficulty) : base(Type, Name, Map, Enemies, 3, 5 + Difficulty + Round / 2, 8, 10, 7, 6, 25)
        {
            egg = Camp;
        }

        public override void Initialise(MainGameWindow Main)
        {
            main = Main;
            main.Combat += Update;
        }

        private void Update(object sender, CombatData combatData)
        {
            main.Combat -= Update;
            enraged = true;
            main.WriteConsole($"{Name} has become enraged!");
        }

        public override void PlayTurn(MainGameWindow main, bool SingleTurn)
        {
            DistanceGraphCreator distanceGraph = new DistanceGraphCreator(this, troop.Position, enemies[0].troop.Position, map, true);
            Thread path = new Thread(distanceGraph.CreateGraph);
            DistanceGraphCreator eggGraph = new DistanceGraphCreator(this, troop.Position, egg, map, true);
            Thread pathE = new Thread(eggGraph.CreateGraph);
            path.Start();
            pathE.Start();

            int damageDealt = 0;
            int dodged = 0;

            Point player = enemies[0].troop.Position;

            Random rng = new Random();

            int playerDistance = AIUtility.Distance(player, troop.Position);

            if (!enraged && AIUtility.Distance(troop.Position, player) < 30 && AIUtility.Distance(egg, player) < 20)
            {
                enraged = true;
                main.WriteConsole($"{Name} has become enraged!");
                main.Combat -= Update;
            }

            void AttackPlayer(bool continues = false)
            {
                bool attacked = false;
                //Check if it can attack player
                foreach (var weapon in troop.weapons)
                {
                    if (playerDistance <= weapon.range &&
                        weapon.attacks > 0)
                    {
                        //Attack
                        troop.activeWeapon = weapon;
                        var (damage, killed, hit) = main.Attack(this, enemies[0]);
                        damageDealt += damage;
                        if (!hit) dodged++;

                        if (killed)
                        {
                            map.overlayObjects.Add(new OverlayText(enemies[0].troop.Position.X * MapCreator.fieldSize, enemies[0].troop.Position.Y * MapCreator.fieldSize, System.Drawing.Color.Red, $"-{damageDealt}"));
                            main.PlayerDied($"You have been killed by {Name} using {weapon.name}!");
                        }
                        actionPoints.rawValue--;
                        weapon.attacks--;
                        attacked = true;
                    }
                }
                if (attacked)
                {
                    AttackPlayer(true);
                }
            }

            if (enraged)
            {
                if (fireLineCoolDown == 0 && playerDistance > 5)
                {
                    //Spew fire

                    //Shoot fire line - will most likely not kill but be in the area
                    Point end = new Point(player.X + (rng.Next(0, 2) == 1 ? -2 : 2), player.Y + (rng.Next(0, 2) == 1 ? -2 : 2));

                    Point diff = end.Sub(troop.Position);
                    Point start = troop.Position.Add(diff.Div(5));

                    int diffX = diff.X;
                    int diffY = diff.Y;
                    int time = Math.Max(Math.Abs(diffX), Math.Abs(diffY));
                    main.actionOccuring = true;
                    lock (map.RenderController)
                    {
                        for (int i = 0; i < time; i++)
                        {
                            Point position = new Point(Math.Max(start.X + (int)(((double)diffX / time) * i), 0), Math.Max(start.Y + (int)(((double)diffY / time) * i), 0));
                            new Fire(rng.Next(5) + 2, 5, position, troop.Position, map, main);
                        }
                    }
                    main.actionOccuring = false;
                    fireLineCoolDown = maxFireLineCooldown;
                }
                else
                {
                    fireLineCoolDown = fireLineCoolDown == 0 ? 0 : fireLineCoolDown - 1;
                }

                if (fireBombPlaces.Count != 0)
                {
                    lock (map.RenderController)
                    {
                        main.actionOccuring = true;
                        int appliedRadius = 2;
                        //Fire real explosions
                        foreach (var hit in fireBombPlaces)
                        {
                            for (int x = 0; x < appliedRadius * 2 + 1; x++)
                            {
                                for (int y = 0; y < appliedRadius * 2 + 1; y++)
                                {
                                    //Check in bounds
                                    Point point = new Point(x + hit.X - appliedRadius, y + hit.Y - appliedRadius);
                                    if ((point.X < 0 || point.X >= map.map.GetUpperBound(0) - 1) || (point.Y < 0 || point.Y >= map.map.GetUpperBound(1) - 1)) continue;

                                    int dis = AIUtility.Distance(point, hit);
                                    if (dis <= appliedRadius)
                                    {
                                        //Damage fields
                                        new Fire(rng.Next(0, 6), fireDamage, point, point, map, main);
                                    }
                                }
                            }
                        }
                        main.actionOccuring = false;
                    }
                    fireBombPlaces.Clear();
                }

                if (fireBombCoolDown == 0)
                {
                    //Summon fire explosions at certain positions - first one then ring

                    int tries = 0; //Longerm: Create deterministic version
                    while (tries != 100 && fireBombPlaces.Count < bombNumber)
                    {
                        Point test = new Point(rng.Next(0, map.map.GetUpperBound(0)), rng.Next(map.map.GetUpperBound(1)));
                        if (AIUtility.Distance(test, player) < 20 && AIUtility.Distance(test, troop.Position) > 3)
                        {
                            fireBombPlaces.Add(test);
                        }
                        tries++;
                    }

                    if (tries == 100) E.LogInfo("Did not find locations for fire bombs!");
                    else bombNumber++;

                    lock (map.RenderController)
                    {
                        foreach (var point in fireBombPlaces)
                        {
                            new Fire(rng.Next(3, 8), fireDamage, point, troop.Position, map, main);
                        }
                    }
                    fireBombCoolDown = maxFireBombCooldown;
                }
                else
                {
                    fireBombCoolDown--;
                }

                AttackPlayer();
            }

            //Find field to move to
            //If enraged go to player
            //If player is close to camp go to player
            //Else stay around camp
            if (actionPoints.Value == 0)
            {
                path.Abort();
                pathE.Abort();
                return;
            }

            Point closestField;
            path.Join();
            pathE.Join();

            DistanceGraphCreator graph = enraged || AIUtility.Distance(egg, player) < 8 ? distanceGraph : eggGraph;
            Point goalPos = enraged || AIUtility.Distance(egg, player) < 8 ? player : egg;

            if (jumpCooldown == 0 && enraged)
            {
                //Jump towards player and create earthquake
                closestField = AIUtility.FindClosestField(graph, goalPos, jumpDistance, map,
                (List<(Point point, double cost, double height)> list) => {
                    list.Sort((o1, o2) => {
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

                //Generate path of movement
                main.MovePlayer(closestField, troop.Position, this, MovementType.teleport);

                //Generate earthquake
                main.actionOccuring = true;
                foreach (var point in AIUtility.GetCircle(2, troop.Position, map, false))
                {
                    new EarthQuakeField(2, point, map, main);
                    main.DamageAtField(jumpDamage, DamageType.earth, point);
                }
                main.actionOccuring = false;

                jumpCooldown = maxJumpCooldown;
            }
            else
            {
                jumpCooldown = jumpCooldown == 0 ? 0 : jumpCooldown - 1;

                closestField = AIUtility.FindClosestField(graph, goalPos, movementPoints.Value, map,
                    (List<(Point point, double cost, double height)> list) => {
                        list.Sort((o1, o2) => {
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

                //Generate path of movement
                DistanceGraphCreator movementGraph = new DistanceGraphCreator(this, troop.Position.X, troop.Position.Y, closestField.X, closestField.Y, map, true, false);
                movementGraph.CreateGraph();

                List<Point> movement = new List<Point>() { };
                Point pointer = closestField;
                Point last = closestField;
                while (pointer != troop.Position)
                {
                    pointer = AIUtility.GetFields(pointer, movementGraph).Aggregate((min, point) => movementGraph.graph.Get(point) < movementGraph.graph.Get(min) ? point : min);
                    movement.Add(last.Sub(pointer)); //Opposite order as we will reverse the array later
                    last = pointer;
                    if (movement.Count > 100) throw new Exception();
                }
                movement.Reverse();

                main.MovePlayer(closestField, troop.Position, this, MovementType.walk, path: movement);
            }

            AttackPlayer(true);

            main.RenderMap();

            if (SingleTurn)
            {
                if (damageDealt != 0)
                    map.overlayObjects.Add(new OverlayText(enemies[0].troop.Position.X * MapCreator.fieldSize, enemies[0].troop.Position.Y * MapCreator.fieldSize, System.Drawing.Color.Red, $"-{damageDealt}" + (dodged != 0 ? $" and dodged {dodged} times!" : "")));
                else if (dodged != 0)
                    map.overlayObjects.Add(new OverlayText(enemies[0].troop.Position.X * MapCreator.fieldSize, enemies[0].troop.Position.Y * MapCreator.fieldSize, System.Drawing.Color.Red, $" Doged {dodged} {(dodged > 1 ? "times" : "time")}!"));
            }
            else
            {
                main.playerDamage += damageDealt;
                main.playerDoged += dodged;
            }
        }
    }
}