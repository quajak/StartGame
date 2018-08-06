using PlayerCreator;
using StartGame.Items;
using StartGame.PlayerData;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartGame
{
    public class Campaign
    {
        public Mission mission;
        public HumanPlayer player;
        private List<MainGameWindow> playedGames = new List<MainGameWindow>();
        private readonly int numberOfGames;
        public MainGameWindow activeGame;
        private Random random = new Random();
        public int difficulty;
        public int Round => playedGames.Count + 1;

        public readonly int healthRegen;

        public List<Tree> trees = Tree.GenerateTrees();

        public Campaign(HumanPlayer _player, int gameNumber, int Difficulty)
        {
            player = _player;
            numberOfGames = gameNumber;
            difficulty = Difficulty;
            healthRegen = 10 - Difficulty;
            if (Difficulty < 6)
            {
                player.vitality.rawValue += 6 - Difficulty;
            }
        }

        private Mission DecideMission(int Round)
        {
            List<Mission> missions = new List<Mission> { new ElementalWizardFight(), new AttackCampMission(), new SpiderNestMission(), new BanditMission(), new DragonFight()
            };
            missions = missions.Where(m => m.MissionAllowed(Round)).ToList();
            int id = random.Next(missions.Count);
            return missions[id];
        }

        /// <summary>
        /// Initialises campaign and generates map and enemies for first mission
        /// </summary>
        public void Start()
        {
            //Setup map
            mission = DecideMission(Round);
            Map map = new Map();
            Thread mapCreator = new Thread(() => map.SetupMap(new Tuple<double, double, double>(0.1, random.Next(), mission.heightDiff)));
            mapCreator.Start();
            mapCreator.Priority = ThreadPriority.Highest;
            bool validMap = false;
            bool finished = mapCreator.Join(Map.creationTime);
            while (!(finished && validMap))
            {
                int seed = random.Next();
                mapCreator = new Thread(() => map.SetupMap(new Tuple<double, double, double>(0.1, seed, mission.heightDiff)));
                mapCreator.Start();
                mapCreator.Priority = ThreadPriority.Highest;
                finished = mapCreator.Join(Map.creationTime);
                validMap = mission.MapValidity(map);
            }

            player.map = map;
            player.troop.Map = map;

            //Finish initialisation
            activeGame = new MainGameWindow(map, player, mission, trees, difficulty, Round);
        }

        /// <summary>
        /// Initialises next mission in the campaign. Returns true if there is a next mission, false if the campaign is over
        /// </summary>
        public bool Next()
        {
            if (playedGames.Count + 1 == numberOfGames) return false;
            if (activeGame is null) throw new Exception();
            playedGames.Add(activeGame);

            mission = DecideMission(Round);

            //Setup map
            Map map = new Map();
            Thread mapCreator;
            do
            {
                mapCreator = new Thread(() => map.SetupMap(new Tuple<double, double, double>(0.1, random.Next(), 0))) {
                    Priority = ThreadPriority.Highest
                };
                mapCreator.Start();
            } while (!mapCreator.Join(Map.creationTime) && !mission.MapValidity(map));

            player.map = map;
            player.troop.Map = map;

            //Finish initialisation
            player.active = false;
            activeGame = new MainGameWindow(map, player, mission, trees, difficulty, Round);

            return true;
        }

        public Weapon CalculateWeaponReward(WeaponReward weaponReward)
        {
            if (!weaponReward.random) return weaponReward.reward;

            List<Weapon> weapons = new List<Weapon>
            {
                new Weapon(6, BaseAttackType.melee, BaseDamageType.blunt, 2, "Stick", 2, true),
                new Weapon(8, BaseAttackType.melee,BaseDamageType.blunt, 1, "Large rock", 1, true),
                new Weapon(4, BaseAttackType.range, BaseDamageType.blunt, 5, "Rock", 1, true),
                new Weapon(11, BaseAttackType.melee,BaseDamageType.sharp,  1, "Dagger", 1, true),
                new Weapon(12, BaseAttackType.melee,BaseDamageType.sharp, 2, "Axe", 1, true, 2),
                new Weapon(9, BaseAttackType.melee,BaseDamageType.sharp, 2, "Sword", 2, true),
                new Weapon(8, BaseAttackType.melee, BaseDamageType.sharp, 4, "Spear", 2, true),
                new Weapon(12, BaseAttackType.melee,BaseDamageType.sharp, 2, "Long sword", 2, true),
                new Weapon(7, BaseAttackType.melee, BaseDamageType.sharp, 1, "Short Sword", 4, true),
                new Weapon(15, BaseAttackType.magic,BaseDamageType.magic, 7, "Firewand", 1, true, 2),
                new Weapon(11, BaseAttackType.range,BaseDamageType.sharp, 11, "Bow", 8, true, 2),
                new Weapon(15, BaseAttackType.range,BaseDamageType.sharp, 20, "Long bow", 5, true, 3),
                new Weapon(4, BaseAttackType.range, BaseDamageType.blunt, 8, "Slingshot", 7, true)
            };
            weapons.Sort((w1, w2) => (w1.attackDamage * w1.attacks * w1.range / 4) > (w2.attackDamage * w2.attacks * w2.range / 4) ? 1 : -1);
            return weapons[random.Next(weapons.Count / numberOfGames * playedGames.Count + weaponReward.rarity, Math.Min(weapons.Count / numberOfGames * (playedGames.Count + 1) + weaponReward.rarity, weapons.Count))];
        }

        public bool IsFinished()
        {
            return playedGames.Count + 1 == numberOfGames;
        }
    }

    public delegate bool WinCheck(Map map, MainGameWindow main);
}