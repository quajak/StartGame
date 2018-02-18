using PlayerCreator;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StartGame
{
    internal class Campaign
    {
        public Player player;
        private List<MainGameWindow> playedGames = new List<MainGameWindow>();
        private int numberOfGames;
        public MainGameWindow activeGame;
        private Random random = new Random();
        public int difficulty;
        public int Round { get { return playedGames.Count + 1; } }

        public readonly int healthRegen;

        public Campaign(Player _player, int gameNumber, int Difficulty)
        {
            player = _player;
            player.CalculateStats();
            numberOfGames = gameNumber;
            difficulty = Difficulty;
            healthRegen = 10 - Difficulty;
            if (Difficulty < 6)
            {
                player.vitality += 6 - Difficulty;
                player.CalculateStats();
            }
        }

        /// <summary>
        /// Initialises campaign and generates map and enemies for first mission
        /// </summary>
        public void Start()
        {
            Map map = new Map(10, 10);
            Thread mapCreator = new Thread(() => map.SetupMap(new Tuple<double, double, double>(0.1, random.Next(), 0)));
            mapCreator.Start();
            mapCreator.Priority = ThreadPriority.Highest;
            while (!mapCreator.Join(Map.creationTime))
            {
                mapCreator = new Thread(() => map.SetupMap(new Tuple<double, double, double>(0.1, random.Next(), 0)));
                mapCreator.Start();
                mapCreator.Priority = ThreadPriority.Highest;
            }

            player.map = map;

            Mission mission = new BanditMission();

            //Finish initialisation
            activeGame = new MainGameWindow(map, player, mission, this);
        }

        /// <summary>
        /// Initialises next mission in the campaign. Returns true if there is a next mission, false if the campaign is over
        /// </summary>
        public bool Next()
        {
            if (playedGames.Count + 1 == numberOfGames) return false;
            if (activeGame is null) throw new Exception();
            playedGames.Add(activeGame);

            Map map = new Map(10, 10);
            Thread mapCreator;
            do
            {
                mapCreator = new Thread(() => map.SetupMap(new Tuple<double, double, double>(0.1, random.Next(), 0)))
                {
                    Priority = ThreadPriority.Highest
                };
                mapCreator.Start();
            } while (!mapCreator.Join(Map.creationTime));

            //Level stats
            int PlayerNumber = Round + map.width / 10;

            //Generate enemies
            List<Player> enemies = new List<Player>();
            short botNumber = Convert.ToInt16(Resources.BOTAmount);
            List<string> botNames = new List<string>();
            for (int i = 0; i < botNumber; i++)
            {
                botNames.Add(Resources.ResourceManager.GetString("BOTName" + i));
            }

            List<Point> spawnPoints = map.DeterminSpawnPoint(PlayerNumber,
                SpawnType.randomLand);

            for (int i = 0; i < PlayerNumber; i++)
            {
                string name = botNames[random.Next(botNames.Count)];
                botNames.Remove(name);
                enemies.Add(new Player(PlayerType.computer, name, map, new Player[] { player })
                {
                    troop = new Troop(name, 10 + (difficulty / 2) + (int)(Round * 1.5) - 4,
                    new Weapon(4 + difficulty / 4 + Round - 1,
                        AttackType.melee, 1, "Fists", 1, false),
                    Resources.enemyScout, 0)
                });
                enemies[i].troop.position = spawnPoints[i];
            }
            //Finish initialisation
            player.active = false;
            activeGame = new MainGameWindow(map, player, enemies, this);

            return true;
        }

        public Weapon CalculateReward()
        {
            List<Weapon> weapons = new List<Weapon>
            {
                new Weapon(6, AttackType.melee, 2, "Stick", 2, true),
                new Weapon(8, AttackType.melee, 1, "Large rock", 1, true),
                new Weapon(4, AttackType.range, 5, "Rock", 1, true),
                new Weapon(11, AttackType.melee, 1, "Dagger", 1, true),
                new Weapon(9, AttackType.melee, 2, "Sword", 2, true),
                new Weapon(8, AttackType.melee, 4, "Spear", 2, true),
                new Weapon(12, AttackType.melee, 2, "Long sword", 2, true),
                new Weapon(7, AttackType.melee, 1, "Short Sword", 4, true),
                new Weapon(15, AttackType.magic, 7, "Firewand", 1, true, 2),
                new Weapon(11, AttackType.range, 11, "Bow", 8, true, 2),
                new Weapon(15, AttackType.range, 20, "Long bow", 5, true, 3)
            };
            weapons.Sort((w1, w2) => (w1.attackDamage * w1.attacks * w1.range / 4) > (w2.attackDamage * w2.attacks * w2.range / 4) ? 1 : -1);
            return weapons[random.Next(weapons.Count / numberOfGames * playedGames.Count, weapons.Count / numberOfGames * (playedGames.Count + 1))];
        }

        public bool IsFinished()
        {
            return playedGames.Count + 1 == numberOfGames;
        }
    }

    internal delegate bool WinCheck(Map map, MainGameWindow main);
}