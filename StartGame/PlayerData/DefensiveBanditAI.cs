using StartGame.Items;
using StartGame.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using static StartGame.MainGameWindow;

namespace StartGame.PlayerData
{
    internal class DefensiveBanditAI : Player
    {
        private readonly Point camp;
        private bool enraged = false;

        private MainGameWindow main;

        public DefensiveBanditAI(PlayerType Type, string Name, Map Map, Player[] Enemies, Point Camp) : base(Type, Name, Map, Enemies, 3, 0, 3, 5, 0, 4, 10)
        {
            camp = Camp;
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
            DistanceGraphCreator distanceGraph = new DistanceGraphCreator(this, troop.Position.X, troop.Position.Y, enemies[0].troop.Position.X,
                enemies[0].troop.Position.Y, map, true);
            Thread path = new Thread(distanceGraph.CreateGraph);
            path.Start();
            DistanceGraphCreator campGraph = new DistanceGraphCreator(this, troop.Position.X, troop.Position.Y, camp.X,
                camp.Y, map, true);
            Thread campPath = new Thread(campGraph.CreateGraph);
            campPath.Start();
            path.Join();
            campPath.Join();

            int damageDealt = 0;
            int dodged = 0;

            while (actionPoints.Value > 0)
            {
                Point playerPos = enemies[0].troop.Position;

                int playerDistance = AIUtility.Distance(playerPos, troop.Position);
                if (enraged)
                {
                    //Check if it can attack player
                    if (playerDistance <= troop.activeWeapon.range &&
                        troop.activeWeapon.attacks > 0)
                    {
                        //Attack
                        var (damage, killed, hit) = main.Attack(this, enemies[0]);
                        damageDealt += damage;
                        if (!hit) dodged++;

                        if (killed)
                        {
                            map.overlayObjects.Add(new OverlayText(enemies[0].troop.Position.X * MapCreator.fieldSize, enemies[0].troop.Position.Y * MapCreator.fieldSize, System.Drawing.Color.Red, $"-{damageDealt}"));
                            main.PlayerDied($"You have been killed by {Name}!");
                            break;
                        }
                        actionPoints.rawValue--;
                        troop.activeWeapon.attacks--;
                        main.RenderMap();
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
                }

                //Find field to move to
                //If enraged go to player
                //If player is close to camp go to player
                //Else stay around camp
                Point closestField;
                DistanceGraphCreator graph = enraged || AIUtility.Distance(camp, playerPos) < 8 ? distanceGraph : campGraph;
                Point goalPos = enraged || AIUtility.Distance(camp, playerPos) < 8 ? playerPos : camp;

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

                //Move to closest field
                int closestDistance = AIUtility.Distance(closestField, goalPos);
                if (closestDistance >= AIUtility.Distance(troop.Position, goalPos)) break;

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

                campGraph = new DistanceGraphCreator(this, troop.Position.X, troop.Position.Y, camp.X, camp.Y, map, true);
                campGraph.CreateGraph();
                distanceGraph = new DistanceGraphCreator(this, troop.Position.X, troop.Position.Y,
                    playerPos.X, playerPos.Y, map, true);
                distanceGraph.CreateGraph();
            }
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