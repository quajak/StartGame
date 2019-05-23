using StartGame.Extra.Loading;
using StartGame.Items;
using StartGame.Properties;
using StartGame.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using static StartGame.MainGameWindow;

namespace StartGame.PlayerData
{
    public class CustomPlayer : Player, ICloneable
    {
        public string bitmap;
        public readonly Weapon weapon;
        public readonly int defense;

        public CustomPlayer(string Name, string bitmap, Weapon weapon, int defense) : base(PlayerType.computer, Name, null, null, 10,
            1, 1, 1, 1, 1, 1)
        {
            this.bitmap = bitmap;
            this.weapon = weapon;
            this.defense = defense;
            troop = new Troop(Name, weapon, (Bitmap)Resources.ResourceManager.GetObject(bitmap), defense, null, this);
        }

        public override void PlayTurn(MainGameWindow main, bool SingleTurn)
        {
            DistanceGraphCreator distanceGraph = new DistanceGraphCreator(this, troop.Position.X, troop.Position.Y, map, true);
            Thread path = new Thread(distanceGraph.CreateGraph);
            path.Start();
            path.Join();

            int damageDealt = 0;
            int dodged = 0;

            while (actionPoints.Value > 0)
            {
                Point playerPos = enemies[0].troop.Position;

                //Check if it can attack player
                int playerDistance = AIUtility.Distance(playerPos, troop.Position);
                if (playerDistance <= troop.activeWeapon.range &&
                    troop.activeWeapon.Attacks() > 0)
                {
                    //Attack
                    var (damage, killed, hit) = main.Attack(this, enemies[0]);
                    damageDealt += damage;
                    if (!hit) dodged++;

                    if (killed)
                    {
                        map.overlayObjects.Add(new OverlayText(enemies[0].troop.Position.X * MapCreator.fieldSize, enemies[0].troop.Position.Y * MapCreator.fieldSize, Color.Red, $"-{damageDealt}"));
                        main.PlayerDied($"You have been killed by {Name}!");
                        break;
                    }
                    actionPoints.RawValue--;
                    troop.activeWeapon.UseWeapon(enemies[0], main);
                    continue;
                }
                else if (troop.weapons.Exists(t => t.range >= playerDistance && t.Attacks() > 0))
                {
                    //Change weapon
                    Weapon best = troop.weapons.FindAll(t => t.range >= playerDistance)
                        .Aggregate((t1, t2) => t1.range > t2.range ? t1 : t2);
                    troop.activeWeapon = best;
                    continue;
                }

                //Generate map of left value
                double[,] movementCosts = new double[map.width, map.height];
                for (int x = 0; x <= movementCosts.GetUpperBound(0); x++)
                {
                    for (int y = 0; y <= movementCosts.GetUpperBound(1); y++)
                    {
                        movementCosts[x, y] = -1;
                    }
                }

                //Find closest field to player
                Point closestField = new Point(-1, -1);
                try
                {
                    closestField = AIUtility.FindClosestField(distanceGraph, playerPos, movementPoints.Value, map,
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
                }
                catch (Exception)
                {
                    //Most likely errors caused by being sorrounded by 1.5 cost tiles
                    //Or when any tiles forward are water
                    //Or when the enemy is already at the player

                    //Trace.TraceError($"Error during warrior spider error: {e.Message} \n {e.StackTrace}");
                    //MessageBox.Show($"An error occured during spider warrior AI at {troop.Position} turn. Distance to player {AIUtility.Distance(troop.Position, playerPos)} Action points: {actionPoints.Value} Check logs for more details!");
                    break;
                }
                //Move to closest field
                int closestDistance = AIUtility.Distance(closestField, playerPos);
                if (closestDistance >= playerDistance) break;

                //Generate path of movement
                DistanceGraphCreator movementGraph = new DistanceGraphCreator(this, troop.Position.X, troop.Position.Y, map, true);
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

                distanceGraph = new DistanceGraphCreator(this, troop.Position.X, troop.Position.Y, map, true);
                distanceGraph.CreateGraph();
            }
            if (SingleTurn)
            {
                if (damageDealt != 0)
                    map.overlayObjects.Add(new OverlayText(enemies[0].troop.Position.X * MapCreator.fieldSize, enemies[0].troop.Position.Y * MapCreator.fieldSize, Color.Red, $"-{damageDealt}" + (dodged != 0 ? $" and dodged {dodged} times!" : "")));
                else if (dodged != 0)
                    map.overlayObjects.Add(new OverlayText(enemies[0].troop.Position.X * MapCreator.fieldSize, enemies[0].troop.Position.Y * MapCreator.fieldSize, Color.Red, $" Doged {dodged} {(dodged > 1 ? "times" : "time")}!"));
            }
            else
            {
                main.playerDamage += damageDealt;
                main.playerDoged += dodged;
            }
        }

        public object Clone()
        {
            CustomPlayer toReturn = new CustomPlayer(Name, bitmap, troop.activeWeapon, defense);
            toReturn.strength.RawValue = strength.RawValue;
            toReturn.agility.RawValue = agility.RawValue;
            toReturn.vitality.RawValue = vitality.RawValue;
            toReturn.intelligence.RawValue = intelligence.RawValue;
            toReturn.wisdom.RawValue = wisdom.RawValue;
            toReturn.endurance.RawValue = endurance.RawValue;
            toReturn.XP = XP;
            toReturn.Money = Money;
            toReturn.enemies = enemies;

            return toReturn;
        }

        public bool Save(string path)
        {
            List<string> lines = new List<string>() {
                E.WriteAttribute(Name, "name"),
                E.WriteAttribute(bitmap, "bitmap"),
                E.WriteAttribute(weapon.RawValue(), "weapon"),
                E.WriteAttribute(defense, "defense"),
                E.WriteAttribute(XP, "xp")
            };
            File.WriteAllLines(path, lines);
            return true;
        }

        public static CustomPlayer Load(string path)
        {
            string[] lines = File.ReadAllLines(path);
            string name = lines[0].GetString();
            string bitmap = lines[1].GetString();
            Weapon weapon = Weapon.Load(lines[2]);
            int defense = lines[3].GetInt();
            int xp = lines[4].GetInt();
            return new CustomPlayer(name, bitmap, weapon, defense) { XP = xp };
        }
    }
}