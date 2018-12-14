using StartGame.Entities;
using StartGame.Items;
using StartGame.PlayerData;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame
{
    public class Reward
    {
        public JewelryReward jewelryReward;
        public WeaponReward weaponReward;
        public int XP;
        public SpellReward spellReward;
        public int Money;

        public Reward(JewelryReward jewelryReward, WeaponReward weaponReward, int xP, SpellReward spellReward, int money)
        {
            this.jewelryReward = jewelryReward;
            this.weaponReward = weaponReward;
            XP = xP;
            this.spellReward = spellReward;
            Money = money;
        }
    }

    public class JewelryReward
    {
        public List<Jewelry> jewelries;
        public int number;
        public Quality quality;

        public JewelryReward(Jewelry jewelry)
        {
            jewelries = new List<Jewelry>
            {
                jewelry
            };
        }

        public JewelryReward(List<Jewelry> jewelries)
        {
            this.jewelries = jewelries;
        }

        public JewelryReward(int number, Quality quality)
        {
            this.number = number;
            this.quality = quality;
        }
    }

    public class SpellReward
    {
        public Spell spell;

        public SpellReward(Spell spell)
        {
            this.spell = spell;
        }
    }

    public class WeaponReward
    {
        public bool random;
        public int rarity;
        public Weapon reward;

        public WeaponReward(bool random, int rarity, Weapon reward)
        {
            this.random = random;
            this.rarity = rarity;
            this.reward = reward;
        }
    }

    public abstract class Mission
    {
        public double heightDiff = 0;
        public bool EnemyMoveTogether = false;

        public abstract (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions,
            string description) GenerateMission(int difficulty, int Round, ref Map map, Player player);

        public static WinCheck deathCheck = ((_map, main) => main.players.Count == 1 && main.players.Exists(p => p == main.humanPlayer));

        public static WinCheck playerDeath = ((_map, main) => main.humanPlayer == null || main.humanPlayer.troop.health.Value <= 0);
        public readonly bool useWinChecks;

        public Mission(bool UseWinChecks = true)
        {
            useWinChecks = UseWinChecks;
        }

        public abstract bool MapValidity(Map map);

        public abstract Reward Reward();

        public abstract bool MissionAllowed(int Round);
    }

    //TODO: Underwater mission

    internal class DebugMission : Mission
    {
        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions,
            string description) GenerateMission(int difficulty, int Round, ref Map map, Player player)
        {
            Random rng = new Random();
            rng.Next();

            string desc = "Debug mission ";

            #region Player Creation

            List<Player> players = new List<Player>() { player };

            //Set player position
            players[0].troop.Position = new Point(0, 1);

            #endregion Player Creation

            #region WinCodition Creation

            List<WinCheck> wins = new List<WinCheck> {
            };
            WinCheck fail = ((_map, main) => false);
            wins.Add(fail);

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
            return new Reward(
                new JewelryReward(5, Quality.Common),
                new WeaponReward(true, 0, null),
                5,
                new SpellReward(World.Instance.GainSpell<TeleportSpell>()),
                0);
        }

        public override bool MissionAllowed(int Round)
        {
            return true;
        }
    }

    internal class DragonFight : Mission
    {
        private int xp;

        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions,
            string description) GenerateMission(int difficulty, int Round, ref Map map, Player player)
        {
            Random rng = new Random();
            rng.Next();

            string desc = "Welcome to an epic dragon fight! Reach the dragons nest to win! ";

            #region Player Creation

            List<Player> players = new List<Player>() { player };
            xp = (int)((player as HumanPlayer).levelXP * (rng.NextDouble() + 2));

            //Set player position
            players[0].troop.Position = map.DeterminSpawnPoint(1, SpawnType.road)[0];

            //Find highest peak
            Point peak = map.DeterminSpawnPoint(1, SpawnType.heighestField)[0];

            //Create the nest
            Building egg = new Building("Nest", peak, Resources.DragonEggNest, map, true);
            map.entities.Add(egg);
            map.renderObjects.Add(new EntityRenderObject(egg, new TeleportPointAnimation(new Point(0, 0), egg.Position)));

            //Spawn dragon close to the highest peak
            Point dragonSpawn = new Point(peak.X + rng.Next(3), peak.Y + rng.Next(3));
            while (dragonSpawn.X >= map.map.GetUpperBound(0) && dragonSpawn.Y >= map.map.GetUpperBound(1))
            {
                dragonSpawn.X = dragonSpawn.X >= map.map.GetUpperBound(0) ? dragonSpawn.X - 1 : dragonSpawn.X;
                dragonSpawn.Y = dragonSpawn.Y >= map.map.GetUpperBound(1) ? dragonSpawn.Y - 1 : dragonSpawn.Y;
            }

            Dictionary<DamageType, double> vurn = new Dictionary<DamageType, double>
            {
                { DamageType.fire, 0 }
            };

            DragonMotherAI dragonMother = new DragonMotherAI(PlayerType.computer, "Dragon", map, new Player[] { player }, peak, Round, difficulty);

            dragonMother.troop = new Troop("Dragon",
                    new Weapon(8 + difficulty / 4 + Round - 1,
                        BaseAttackType.melee, BaseDamageType.sharp, 3, "Claw", 1, false),
                    Resources.Dragon, 0, map, dragonMother, Vurneabilities: vurn);

            players.Add(dragonMother);
            players[1].troop.Position = dragonSpawn;
            players[1].troop.weapons.Add(new Weapon(6 + difficulty / 3 + Round, BaseAttackType.melee, BaseDamageType.blunt, 6, "Tail", 1, false, 1));

            #endregion Player Creation

            #region WinCodition Creation

            List<WinCheck> wins = new List<WinCheck>
            {
                deathCheck,
                (_map, _main) =>
                {
                    return AIUtility.Distance(_main.humanPlayer.troop.Position, peak) == 1;
                }
            };
            WinCheck fail = ((_map, main) => false);
            wins.Add(fail);

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
            return new Reward(
                new JewelryReward(3, Quality.Superior),
                new WeaponReward(true, 2, null),
                xp,
                new SpellReward(World.Instance.GainSpell<FireBall>()),
                80
            );
        }

        public override bool MissionAllowed(int Round)
        {
            return Round > 7;
        }
    }

    internal class BanditMission : Mission

    {
        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions,
            string description) GenerateMission(int difficulty, int Round, ref Map map, Player player)
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
                BanditAI item = new BanditAI(PlayerType.computer, name, map, new Player[] { player });
                item.troop = new Troop(name,
                    new Weapon(4 + difficulty / 4 + Round - 1,
                        BaseAttackType.melee, BaseDamageType.blunt, 1, "Fists", 1, false),
                    Resources.enemyScout, 0, map, item) {
                    armours = new List<Armour>
                        {
                            new Armour("Shirt", 32, new List<BodyParts>{BodyParts.LeftUpperArm,BodyParts.RightUpperArm,BodyParts.Torso}, Material.Materials.First(m => m.name == "Cloth"),Quality.Broken, ArmourLayer.clothing),
                            new Armour("Hose", 60, new List<BodyParts> { BodyParts.UpperLegs, BodyParts.LeftLowerLeg, BodyParts.RightLowerLeg, BodyParts.LeftShin, BodyParts.RightShin }, Material.Materials.First(m => m.name == "Cloth"), Quality.Simple, ArmourLayer.clothing)
                        }
                };
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

            int distance(Point A, Point B)
            {
                return Math.Abs(A.X - B.X) + Math.Abs(A.Y - B.Y);
            }

            //Find goal region
            Point playerPosition = startPos[0];
            List<MapTile> goalLocations = new List<MapTile>(map.goals);
            if (goalLocations.Count > 1)
            {
                goalLocations.Sort((l1, l2) => {
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
                    wins.Add((_map, main) => main.humanPlayer != null && goal == main.humanPlayer.troop.Position);
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
            return new Reward(
                new JewelryReward(1, Quality.Broken),
                new WeaponReward(true, 0, null),
                5,
                new SpellReward(World.Instance.GainSpell<DebuffSpell>()),
                12
            );
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
            GenerateMission(int difficulty, int Round, ref Map map, Player player)
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
                WarriorSpiderAI item = new WarriorSpiderAI(PlayerType.computer, name, map, new Player[] { player });
                item.troop = new Troop(name,
                    new Weapon(1 + difficulty / 5 + Round / 2,
                        BaseAttackType.melee, BaseDamageType.sharp, 1, "Fangs", 1, false),
                    Resources.spiderWarrior, 0, map, item, Dodge: 25);
                players.Add(item);
                players[i + 1].troop.Position = spawnPoints[i];
            }

            //Generate the spider nest
            //Choose the land tile which is the furthest away, which is sorrounded by land tiles and is a land tile
            Map _map = map;
            IOrderedEnumerable<MapTile> orderedEnumerable = map.map.Cast<MapTile>().ToList().Where(p => _map.map[p.position.X, p.position.Y].type.type == MapTileTypeEnum.land && _map.map[p.position.X, p.position.Y].neighbours.rawMaptiles.All(m => m.type.type == MapTileTypeEnum.land))
               .OrderByDescending(p => (Math.Abs(p.position.X - player.troop.Position.X) + Math.Abs(p.position.Y - player.troop.Position.Y)));
            Point spawnPoint = orderedEnumerable.FirstOrDefault().position;

            SpiderNestAI item1 = new SpiderNestAI(PlayerType.computer, "Spider Nest", map, new Player[] { player }, difficulty, Round);
            item1.troop = new Troop("Spider Nest", null, Resources.spiderNest, 2, map, item1, 0) {
                Position = spawnPoint
            };
            players.Add(item1);

            #endregion Player Creation

            #region WinCodition Creation

            //Must kill spider nest to win

            WinCheck nestState = new WinCheck((__map, main) => !__map.troops.Exists(t => t.Name == "Spider Nest"));

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
            return new Reward(
                null,
                new WeaponReward(true, 0, null),
                2,
                null,
                0
            );
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
            GenerateMission(int difficulty, int Round, ref Map map, Player player)
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
            ElementalWizard item = new ElementalWizard(PlayerType.computer, name, map, new Player[] { player }, difficulty, Round);
            item.troop = new Troop(name, new Weapon(3 + difficulty / 5 + Round / 2, BaseAttackType.melee, BaseDamageType.sharp, 1, "Dagger", 2, false),
            Resources.elementalWizard, 0, map, item, Dodge: 25) {
                armours = new List<Armour>
                    {
                        new Armour("Wizard's Cloak", 40, new List<BodyParts>{BodyParts.Head,BodyParts.Neck,BodyParts.LeftUpperArm,BodyParts.RightUpperArm,BodyParts.Torso,BodyParts.UpperLegs,BodyParts.LeftLowerLeg,BodyParts.RightLowerLeg}, Material.Materials.First(m => m.name == "Wool"),Quality.Superior, ArmourLayer.light)
                        {
                            magicDefense = 20
                        }
                    }
            };
            players.Add(item);
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
            return new Reward(
                null,
                new WeaponReward(true, 3, null),
                5,
                new SpellReward(World.Instance.GainSpell<TeleportSpell>()),
                48
            );
        }

        public override bool MissionAllowed(int Round)
        {
            return Round > 3;
        }
    }

    internal class AttackCampMission : Mission
    {
        //TODO: Trigger aggresive AI when casting spell to hurt bandit
        public AttackCampMission()
        {
            heightDiff = -0.2;
        }

        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions, string description)
            GenerateMission(int difficulty, int Round, ref Map map, Player player)
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
            Building fireplace = new Building("Fireplace", camp, Resources.firePlace, map);
            map.entities.Add(fireplace);
            map.renderObjects.Add(new EntityRenderObject(fireplace, new TeleportPointAnimation(new Point(0, 0), fireplace.Position)));

            int enemyNumber = Round / 5 + 2 * difficulty / 3;
            List<Point> startPos = new List<Point>();
            List<Point> toCheck = map.map[camp.X, camp.Y].neighbours.rawMaptiles.ToList().ConvertAll(m => m.position);
            //Add a few more than neccessary so ones which are path can be removed
            while (startPos.Count <= enemyNumber + 10 && toCheck.Count != 0)
            {
                Point checking = toCheck[0];
                toCheck.Remove(checking);
                startPos.Add(checking);

                map.map[checking.X, checking.Y].neighbours.rawMaptiles.ToList().ForEach(m => {
                    //Check if it does not exist, if land and is not blocked
                    if (!startPos.Exists(f => f.X == m.position.X && f.Y == m.position.Y) && m.type.FType == FieldType.land && m.free)
                    {
                        toCheck.Add(m.position);
                    }
                });
            }

            enemyNumber = Math.Min(enemyNumber, startPos.Count); //in case there are too few positions

            //If extra, remove path
            Map _map = map;
            while (startPos.Count > enemyNumber)
            {
                if (startPos.Exists(f => _map.map[f.X, f.Y].type.type == MapTileTypeEnum.path))
                {
                    startPos.Remove(startPos.First(f => _map.map[f.X, f.Y].type.type == MapTileTypeEnum.path));
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
                DefensiveBanditAI item = new DefensiveBanditAI(PlayerType.computer, name, map, new Player[] { player }, camp);
                item.troop = new Troop(name, new Weapon(3 + difficulty / 5 + Round / 2, BaseAttackType.melee, BaseDamageType.sharp, 1, "Dagger", 2, false),
                Resources.enemyScout, 0, map, item, Dodge: 25) {
                    armours = new List<Armour>
                        {
                            new Armour("Cap", 10, new List<BodyParts>{BodyParts.Head}, Material.Materials.First(m => m.name == "Cloth"),Quality.Simple, ArmourLayer.clothing),
                            new Armour("Shirt", 20, new List<BodyParts>{BodyParts.Torso}, Material.Materials.First(m => m.name == "Cloth"),Quality.Poor, ArmourLayer.clothing),
                            new Armour("Hose", 25, new List<BodyParts>{BodyParts.UpperLegs,BodyParts.LeftLowerLeg,BodyParts.RightLowerLeg}, Material.Materials.First(m => m.name == "Cloth"),Quality.Common, ArmourLayer.clothing),
                            new Armour("Cloak", 35, new List<BodyParts>{BodyParts.LeftUpperArm,BodyParts.RightUpperArm,BodyParts.Torso,BodyParts.UpperLegs,BodyParts.LeftLowerLeg,BodyParts.RightLowerLeg}, Material.Materials.First(m => m.name == "Wool"),Quality.Common, ArmourLayer.light)
                        }
                };
                players.Add(item);

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
            return new Reward(
                new JewelryReward(1, Quality.Simple),
                new WeaponReward(true, 3, null),
                5,
                null,
                34
            );
        }

        public override bool MissionAllowed(int Round)
        {
            return Round > 2;
        }
    }
}