﻿using PlayerCreator;
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
        public HumanPlayer player;
        private List<MainGameWindow> playedGames = new List<MainGameWindow>();
        private int numberOfGames;
        public MainGameWindow activeGame;
        private Random random = new Random();
        public int difficulty;
        public int Round { get { return playedGames.Count + 1; } }

        public readonly int healthRegen;

        public Campaign(HumanPlayer _player, int gameNumber, int Difficulty)
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

        private Mission DecideMission()
        {
            List<Mission> missions = new List<Mission> { new SpiderNestMission(), new BanditMission() };
            return missions[random.Next(missions.Count - 1)];
        }

        /// <summary>
        /// Initialises campaign and generates map and enemies for first mission
        /// </summary>
        public void Start()
        {
            //Setup map
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

            Mission mission = new SpiderNestMission();

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

            //Setup map
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

            Mission mission = new BanditMission();

            //Finish initialisation
            player.active = false;
            activeGame = new MainGameWindow(map, player, mission, this);

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