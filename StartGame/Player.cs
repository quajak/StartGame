using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PlayerCreator;
using StartGame;
using StartGame.Properties;
using static StartGame.MainGameWindow;

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
        internal readonly List<Spell> spells;

        //Derived stats
        public int maxActionPoints = 4;

        public Troop troop;

        public bool Dead { get => troop.health == 0; }

        public Player(PlayerType Type, string name, Map Map, Player[] Enemies, int XP, List<Spell> spells = null)
        {
            this.XP = XP;
            if (spells != null)
            {
                this.spells = spells;
            }
            else
            {
                this.spells = new List<Spell>();
            }
            enemies = Enemies;
            type = Type;
            Name = name;
            map = Map;
        }

        public abstract void PlayTurn(MainGameWindow main, bool singleTurn);

        public virtual void Initialise(MainGameWindow main)
        {
        }

        public void ActionButtonPressed(MainGameWindow mainGameWindow)
        {
            mainGameWindow.NextTurn();
        }

        /// <summary>
        /// Function list used to calculate cost to move to certain field.
        /// Input: Start, Active, Next Tile, Total already Distance moved, Cost of movement, type
        /// Ouput: Cost
        /// </summary>
        public List<Func<MapTile, MapTile, MapTile, int, double, MovementType, double>> CalculateStepCost = new List<Func<MapTile, MapTile, MapTile, int, double, MovementType, double>>()
        {
            (path, active, next, distance, cost, type) =>
            {
                switch (type)
                {
                    case MovementType.walk:
                        cost = next.Cost;
                     break;

                    case MovementType.teleport:
                        cost = 1;
                     break;

                    default:
                     throw new NotImplementedException();
                }

                return cost;
            }
        };

        /// <summary>
        /// Function used to execute the calculateMovementCost list and calculate the cost of a movement
        /// </summary>
        /// <param name="path">Path of movement</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public double CalculateStep(MapTile start, MapTile active, MapTile next, int distance, MovementType type, bool allowBlockedEnd = false)
        {
            Contract.Assert(start != null);
            Contract.Assert(next != null);
            Contract.Assert(type != MovementType.teleport || start == active);

            switch (type)
            {
                case MovementType.walk:
                    break;

                case MovementType.teleport:
                    distance = AIUtility.Distance(start, next);
                    break;

                default:
                    throw new NotImplementedException();
            }
            if (distance == 0) throw new Exception("A unit can not travel for a distance of 0");

            double cost = 0;
            foreach (var func in CalculateStepCost)
            {
                cost = func.Invoke(start, active, next, distance, cost, type);
            }
            return cost;
        }
    }

    internal class HumanPlayer : Player
    {
        //Base stats
        public int strength; //Bonus damage dealt

        public int agility; //Chance to dodge
        public int endurance; //How many action points + defense
        public int vitality; //How much health
        public int wisdom; //how much mana
        public int intelligence; //improves the effectivness of spells

        public int mana;
        public int maxMana;

        public int level = 1;
        public int xp = 0;
        public int levelXP = 5;
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
            wisdom = 1;
            intelligence = 1;
        }

        public void CalculateStats()
        {
            maxActionPoints = 4 + endurance / 10;

            int healthDifference = troop.health - troop.maxHealth;
            troop.maxHealth = 2 * vitality;
            troop.health = troop.maxHealth - healthDifference;

            troop.defense = endurance / 5;

            troop.dodge = troop.baseDodge + agility * 2;

            int manaDifference = mana - maxMana;
            maxMana = wisdom * 2 + 10;
            mana = maxMana - manaDifference;
        }

        public void GainXP(int XP)
        {
            xp += XP;
            if (xp >= levelXP)
            {
                xp -= levelXP;
                storedLevelUps += 1;
                levelXP = Math.Max((int)(1.1 * levelXP), levelXP + 1);

                //Set Level up
                main.SetUpdateState(storedLevelUps > 0);

                //Immediate effect
                troop.health = troop.maxHealth;
                main.WriteConsole("Level Up! Healing to max hp!");
            }
        }

        public override void PlayTurn(MainGameWindow main, bool SingleTurn)
        {
        }
    }

    internal class BanditAI : Player
    {
        public BanditAI(PlayerType Type, string Name, Map Map, Player[] Enemies) : base(Type, Name, Map, Enemies, 3)
        {
        }

        public override void PlayTurn(MainGameWindow main, bool SingleTurn)
        {
            DistanceGraphCreator distanceGraph = new DistanceGraphCreator(this, troop.Position.X, troop.Position.Y, enemies[0].troop.Position.X,
                enemies[0].troop.Position.Y, map, true);
            Thread path = new Thread(distanceGraph.CreateGraph);
            path.Start();
            path.Join();

            int damageDealt = 0;
            int dodged = 0;

            while (actionPoints > 0)
            {
                Point playerPos = enemies[0].troop.Position;

                //Check if it can attack player
                int playerDistance = AIUtility.Distance(playerPos, troop.Position);
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
                    actionPoints--;
                    troop.activeWeapon.attacks--;
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

    internal class DefensiveBanditAI : Player
    {
        private readonly Point camp;
        private bool enraged = false;

        private MainGameWindow main;

        public DefensiveBanditAI(PlayerType Type, string Name, Map Map, Player[] Enemies, Point Camp) : base(Type, Name, Map, Enemies, 3)
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

            while (actionPoints > 0)
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
                        actionPoints--;
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

                closestField = AIUtility.FindClosestField(graph, goalPos, actionPoints, map,
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

    internal class DragonMotherAI : Player
    {
        private readonly Point egg;
        private bool enraged = false;
        private int fireDamage = 3;

        private int fireLineCoolDown = 0;
        private int fireBombCoolDown = 0;
        private readonly int maxFireLineCooldown = 2;
        private readonly int maxFireBombCooldown = 4;

        private int bombNumber = 3;
        private List<Point> fireBombPlaces = new List<Point>();

        private int jumpDamage = 3;
        private int jumpCooldown = 3;
        private int maxJumpCooldown = 3;
        private int jumpDistance = 15;

        private MainGameWindow main;

        public DragonMotherAI(PlayerType Type, string Name, Map Map, Player[] Enemies, Point Camp) : base(Type, Name, Map, Enemies, 3)
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
                        actionPoints--;
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

                    if (tries == 100) Extensions.LogInfo("Did not find locations for fire bombs!");
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
            if (actionPoints == 0)
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

                closestField = AIUtility.FindClosestField(graph, goalPos, actionPoints, map,
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

    internal class WarriorSpiderAI : Player
    {
        public WarriorSpiderAI(PlayerType Type, string Name, Map Map, Player[] Enemies) : base(Type, Name, Map, Enemies, 1)
        {
        }

        public override void PlayTurn(MainGameWindow main, bool SingleTurn)
        {
            DistanceGraphCreator distanceGraph = new DistanceGraphCreator(this, troop.Position.X, troop.Position.Y, enemies[0].troop.Position.X, enemies[0].troop.Position.Y, map, false);
            Thread path = new Thread(distanceGraph.CreateGraph);
            path.Start();
            path.Join();

            int damageDealt = 0;
            int dodged = 0;

            while (actionPoints > 0)
            {
                Point playerPos = enemies[0].troop.Position;

                //Check if it can attack player
                int playerDistance = AIUtility.Distance(playerPos, troop.Position);
                if (playerDistance <= troop.activeWeapon.range &&
                    troop.activeWeapon.attacks > 0)
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
                    actionPoints--;
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

                //Generate path of movement
                DistanceGraphCreator movementGraph = new DistanceGraphCreator(this, troop.Position.X, troop.Position.Y, closestField.X, closestField.Y, map, false, false);
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

                distanceGraph = new DistanceGraphCreator(this, troop.Position.X, troop.Position.Y,
                    playerPos.X, playerPos.Y, map, false);
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

    internal class SpiderNestAI : Player
    {
        private int numSpawned = 0;
        private int difficulty;
        private int turn;
        private int maxSpawned;
        private readonly int round;

        public SpiderNestAI(PlayerType Type, string Name, Map Map, Player[] Enemies, int Difficulty, int Round) : base(Type, Name, Map, Enemies, 5)
        {
            map = Map;
            turn = 7 - (Difficulty / 2);
            round = Round;
            difficulty = Difficulty;
            maxSpawned = 5 + Difficulty + Round;
        }

        public override void PlayTurn(MainGameWindow main, bool singleTurn)
        {
            turn = turn == 0 ? 0 : turn - 1;
            if (turn == 0 && numSpawned < maxSpawned)
            {
                //Find the position for the new spider to spawn
                Point pos = new Point(-1, -1);
                foreach (MapTile tile in map.map[troop.Position.X, troop.Position.Y].neighbours.rawMaptiles)
                {
                    //Check if empty and not water
                    if (tile.type.type != MapTileTypeEnum.deepWater || tile.type.type != MapTileTypeEnum.shallowWater && !map.troops.Exists(t => t.Position.X == tile.position.X && t.Position.Y == tile.position.Y))
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
                        AttackType.melee, 1, "Fangs", 1, false), Resources.spiderWarrior, 0, map, 25)
                    {
                        Position = pos
                    }
                };

                main.AddPlayer(spider);

                turn = 5 - (difficulty / 2);
            }
        }
    }

    internal class ElementalWizard : Player
    {
        //TODO: Make the AI more difficult by adding more spells, or making him move
        private readonly int difficulty;

        private int lastHealth;
        private readonly int round;

        public ElementalWizard(PlayerType Type, string Name, Map Map, Player[] Enemies, int Difficulty, int Round) : base(Type, Name, Map, Enemies, 10
            , new List<Spell>() { new FireBall(Difficulty / 2 + Round + 1, Difficulty / 3 + 1 ,6 - Difficulty / 2, 0)
                , new TeleportSpell(8- Difficulty/2, 0)
            })
        {
            map = Map;
            round = Round;
            difficulty = Difficulty;
        }

        public override void Initialise(MainGameWindow main)
        {
            spells.ForEach(s => s.Initialise(main, map));
            lastHealth = troop.health;
        }

        public override void PlayTurn(MainGameWindow main, bool singleTurn)
        {
            //Decrease spell cooldown
            foreach (Spell spell in spells)
            {
                spell.coolDown = spell.coolDown == 0 ? 0 : spell.coolDown - 1;
            }

            //Panic if he has been hit or player is close and he is on low health
            if (troop.health != lastHealth || (AIUtility.Distance(troop.Position, enemies[0].troop.Position) < 4 && troop.health != troop.maxHealth))
            {
                lastHealth = troop.health;
                //If teleport spell is ready
                if (spells[1].Ready)
                {
                    //Find heighest free space
                    var HeightSorted = from field in map.map.Cast<MapTile>()
                                       where field.free
                                       where AIUtility.Distance(field.position, enemies[0].troop.Position) > 10
                                       orderby field.height descending
                                       select field;
                    //Teleport
                    spells[1].Activate(new SpellInformation() { positions = new List<Point>() { troop.Position, HeightSorted.Take(1).ToList()[0].position }, mage = this });
                    return; //Finish turn
                }
                else
                {
                    main.WriteConsole("The wizard wimpers");
                    return;
                }
            }

            //Attack
            if (spells[0].Ready)
            {
                spells[0].Activate(new SpellInformation() { positions = new List<Point> { enemies[0].troop.Position }, mage = this });
            }
        }
    }

    internal delegate Point FieldOptimiser(List<(Point point, double cost, double height)> list);

    internal static class AIUtility
    {
        public static IEnumerable<Point> GetCircle(int radius, Point center, Map map, bool withCenter)
        {
            for (int x = 0; x < radius * 2 + 1; x++)
            {
                for (int y = 0; y < radius * 2 + 1; y++)
                {
                    //Check in bounds
                    Point point = new Point(x + center.X - radius, y + center.Y - radius);
                    if ((point.X < 0 || point.X >= map.map.GetUpperBound(0) - 1) || (point.Y < 0 || point.Y >= map.map.GetUpperBound(1) - 1)) continue;

                    if (!withCenter && point == center) continue;

                    int dis = AIUtility.Distance(point, center);
                    if (dis <= radius)
                    {
                        //Damage fields
                        yield return point;
                    }
                }
            }
        }

        public static int Distance(MapTile A, MapTile B)
        {
            Contract.Assert(A != null);
            Contract.Assert(B != null);

            return Distance(A.position, B.position);
        }

        public static int Distance(Point A, Point B)
        {
            return Math.Abs(A.X - B.X) + Math.Abs(A.Y - B.Y);
        }

        public static Point FindClosestField(DistanceGraphCreator distanceGraph, Point goal, double actionPoints, Map map, FieldOptimiser chooser)
        {
            int closestDistance = int.MaxValue;
            List<(Point point, double points, double height)> closestPoints = new List<(Point point, double points, double height)>();
            for (int x = 0; x <= distanceGraph.graph.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= distanceGraph.graph.GetUpperBound(1); y++)
                {
                    int playerDis = Distance(goal, new Point(x, y));
                    if (distanceGraph.graph[x, y] <= actionPoints
                        && map.map[x, y].free)
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
            if (closestPoints.Count == 0) throw new Exception("Did not find any close field!");
            return chooser.Invoke(closestPoints);
        }

        public static List<Point> GetFields(int[] point, DistanceGraphCreator distanceGraph)
        {
            Contract.Assert(point.Length == 2);

            return GetFields(new Point(point[0], point[1]), distanceGraph);
        }

        public static List<Point> GetFields(Point point, DistanceGraphCreator distanceGraph)
        {
            List<Point> points = new List<Point>();
            if (point.Y != 0)
                points.Add(new Point(point.X, point.Y - 1));
            if (point.Y != distanceGraph.graph.GetUpperBound(1))
                points.Add(new Point(point.X, point.Y + 1));
            if (point.X != 0)
                points.Add(new Point(point.X - 1, point.Y));
            if (point.X != distanceGraph.graph.GetUpperBound(0))
                points.Add(new Point(point.X + 1, point.Y));
            return points;
        }
    }

    internal enum PlayerType
    { localHuman, computer };

    internal class DistanceGraphCreator
    {
        private const MovementType walk = MovementType.walk;
        private readonly Player player;
        private readonly int sX;
        private readonly int sY;
        private Map map;
        private int eX;
        private int eY;
        public double[,] graph;
        private bool allowWater;
        private readonly bool needFree;

        /// <summary>
        /// Create a double map of the distance cost to get to field from a certain point
        /// </summary>
        /// <param name="EX"></param>
        /// <param name="EY"></param>
        /// <param name="Map"></param>
        /// <param name="AllowWater"></param>
        /// <param name="needFree"></param>
        public DistanceGraphCreator(Player Player, int SX, int SY, int EX, int EY, Map Map, bool AllowWater, bool needFree = true)
        {
            map = Map;
            player = Player;
            sX = SX;
            sY = SY;
            eX = EX;
            eY = EY;
            allowWater = AllowWater;
            this.needFree = needFree;
        }

        public DistanceGraphCreator(Player Player, Point S, Point E, Map Map, bool AllowWater, bool needFree = true)
        {
            player = Player;
            map = Map;
            sX = S.X;
            sY = S.Y;
            eX = E.X;
            eY = E.Y;
            allowWater = AllowWater;
            this.needFree = needFree;
        }

        private double[,] mapValues;
        private bool[,] free;

        public void CreateGraph()
        {
            lock (this) lock (map)
                {
                    graph = new double[map.map.GetUpperBound(0) + 1, map.map.GetUpperBound(1) + 1];
                    mapValues = new double[map.map.GetUpperBound(0) + 1, map.map.GetUpperBound(1) + 1];
                    free = new bool[map.map.GetUpperBound(0) + 1, map.map.GetUpperBound(1) + 1];
                    for (int x = 0; x <= mapValues.GetUpperBound(0); x++)
                    {
                        for (int y = 0; y <= mapValues.GetUpperBound(1); y++)
                        {
                            mapValues[x, y] = map.map[x, y].MovementCost;
                            free[x, y] = map.map[x, y].free;
                            graph[x, y] = 100;
                        }
                    }

                    graph[sX, sY] = 0;

                    List<int[]> toCheck = new List<int[]>() { new int[] { sX, sY } };
                    graph[sX, sY] = 0;
                    Point start = new Point(sX, sY);
                    MapTile startTile = map.map.Get(start);
                    while (toCheck.Count != 0)
                    {
                        int[] checking = toCheck[0];
                        toCheck.Remove(checking);

                        List<int[]> sorrounding = new List<int[]>();
                        //Top
                        if (checking[1] != 0)
                            sorrounding.Add(new int[] { checking[0], checking[1] - 1 });
                        //Bottom
                        if (checking[1] != mapValues.GetUpperBound(1))
                            sorrounding.Add(new int[] { checking[0], checking[1] + 1 });
                        //Left
                        if (checking[0] != 0)
                            sorrounding.Add(new int[] { checking[0] - 1, checking[1] });
                        //Right
                        if (checking[0] != mapValues.GetUpperBound(0))
                            sorrounding.Add(new int[] { checking[0] + 1, checking[1] });

                        List<int[]> toAdd = new List<int[]>();
                        foreach (int[] field in sorrounding)
                        {
                            if (!allowWater && (map.map[field[0], field[1]].type.type == MapTileTypeEnum.deepWater
                                    || map.map[field[0], field[1]].type.type == MapTileTypeEnum.shallowWater))
                            {
                            }
                            else if (!free[field[0], field[1]])
                            {
                            }
                            else
                            {
                                MapTile[] path = GeneratePath(new Point(field[0], field[1]), start, map);

                                double newCost = graph.Get(checking) + player.CalculateStep(startTile, map.map.Get(checking), map.map.Get(field), path.Length - 1, walk, true);
                                if (newCost < graph[field[0], field[1]])
                                {
                                    graph[field[0], field[1]] = newCost;
                                    toAdd.Add(field);
                                }
                                else
                                {
                                }
                            }
                        }
                        toCheck.AddRange(toAdd);
                    }
                }
        }

        public MapTile[] GeneratePath(Point start, Point end, Map map)
        {
            List<MapTile> path = new List<MapTile>();
            //DEBUG
            int counter = 0;

            Point active = start;
            path.Add(map.map.Get(active));

            while (active != end)
            {
                //DEBUG
                if (++counter == 100) throw new Exception("Something went wrong!");

                //Find field with lowest value for graph (lowest movement cost from start)
                List<Point> fields = AIUtility.GetFields(active, this);
                active = fields.Aggregate((best, field) =>
                {
                    double b = graph[best.X, best.Y]; //Do not use Get method for optimization
                    double n = graph[field.X, field.Y];
                    if (n == b)
                    {
                        int bestDistance = AIUtility.Distance(best, end);
                        int fieldDistance = AIUtility.Distance(field, end);
                        //if they have the same cost take the one which is closer
                        return bestDistance > fieldDistance ? field : best;
                    }
                    else
                    {
                        if (b > n)
                        {
                            return field;
                        }
                        else
                        {
                            return best;
                        }
                    }
                });

                path.Add(map.map.Get(active));
            }
            return path.ToArray();
        }
    }
}