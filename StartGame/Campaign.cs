using StartGame.GameMap;
using StartGame.Items;
using StartGame.Mission;
using StartGame.PlayerData;
using StartGame.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace StartGame
{
    public class Campaign
    {
        public Mission.Mission mission;
        public HumanPlayer player;
        private readonly List<MainGameWindow> playedGames = new List<MainGameWindow>();
        public readonly int numberOfGames;
        public MainGameWindow activeGame;
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
            if (player != null && Difficulty < 6)
            {
                player.vitality.RawValue += 6 - Difficulty;
            }
        }

        public Mission.Mission DecideMission(int Round)
        {
            List<Mission.Mission> missions = new List<Mission.Mission> { new BearMission(), new ElementalWizardFight(), new AttackCampMission(), new SpiderNestMission(), new BanditMission(), new DragonFight()
            };
            missions = missions.Where(m => m.MissionAllowed(Round)).ToList();
            int id = World.World.random.Next(missions.Count);
            return missions[id];
        }

        public Mission.Mission DecideMission(WorldTileType type, int Wealth)
        {
            List<Mission.Mission> missions = new List<Mission.Mission> { new BearMission(), new ElementalWizardFight(), new AttackCampMission(), new SpiderNestMission(), new BanditMission(), new DragonFight()
            };
            missions = missions.Where(m => m.MissionAllowed(type, Wealth)).ToList();
            return missions.GetRandom();
        }

        /// <summary>
        /// Initialises campaign and generates map and enemies for first mission
        /// </summary>
        public void Start()
        {
            //Setup map
            mission = DecideMission(Round);
            Map map = GenerateMap();
            player.map = map;
            player.troop.Map = map;

            //Finish initialisation
            activeGame = new MainGameWindow(map, player, mission, trees, difficulty, Round);
        }

        public Map GenerateMap(MapBiome mapBiome = null)
        {
            if (mapBiome is null)
                mapBiome = new GrasslandMapBiome();
            Map map = new Map() {
                mapBiome = mapBiome
            };
            Thread mapCreator = new Thread(() => map.SetupMap(mapBiome.DefaultParameters().PerlinDiff, World.World.random.Next(), mapBiome.DefaultParameters().HeightDiff + mission.heightDiff, mapBiome));
            mapCreator.Start();
            mapCreator.Priority = ThreadPriority.Highest;
            bool validMap = false;
            bool finished = mapCreator.Join(Map.creationTime);
            int counter = 20;
            while (!(finished && validMap))
            {
                int seed = World.World.random.Next();
                mapCreator = new Thread(() => map.SetupMap(mapBiome.DefaultParameters().PerlinDiff, World.World.random.Next(), mapBiome.DefaultParameters().HeightDiff + mission.heightDiff, mapBiome));
                mapCreator.Start();
                mapCreator.Priority = ThreadPriority.Highest;
                finished = mapCreator.Join(Map.creationTime);
                validMap = mission.MapValidity(map);
                counter--;
                if (counter == 0)
                    throw new Exception("Can not find valid map!");
            }

            return map;
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
                mapCreator = new Thread(() => map.SetupMap(0.1, World.World.random.Next(), 0)) {
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

        public static Weapon CalculateWeaponReward(WeaponReward weaponReward, int playedGames, int numberOfGames)
        {
            if (!weaponReward.random) return weaponReward.reward;

            List<Weapon> weapons = Weapon.GetWeaponTypes();
            weapons.Sort((w1, w2) => (w1.attackDamage.Value * w1.Attacks() * w1.range / 4) > (w2.attackDamage.Value * w2.Attacks() * w2.range / 4) ? 1 : -1);
            return weapons[World.World.random.Next(weapons.Count / numberOfGames * playedGames + weaponReward.rarity,
                Math.Min(weapons.Count / numberOfGames * (playedGames + 1) + weaponReward.rarity, weapons.Count))];
        }

        public bool IsFinished()
        {
            return playedGames.Count + 1 == numberOfGames;
        }
    }

    public delegate bool WinCheck(Map map, MainGameWindow main);
}