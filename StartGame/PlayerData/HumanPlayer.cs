﻿using StartGame.GameMap;
using StartGame.Rendering;
using StartGame.World;
using StartGame.World.Cities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StartGame.PlayerData
{
    public class HumanPlayer : WorldPlayer
    {
        public MainGameWindow main;
        public WorldRenderer worldRenderer;

        public List<WorldAction> possibleActions = new List<WorldAction>();
        public List<WorldAction> availableActions = new List<WorldAction>();

        public int level = 1;
        public int xp = 0;
        public int levelXP = 5;
        public int storedLevelUps = 0;

        public PlayerView playerView;

        /// <summary>
        /// If in spectator mode the user can only watch the character move on the world stage and cannot change the path. 
        /// This is used when he is part of a caravan.
        /// </summary>
        public bool spectatorMode = false;

        public CaravanRoute caravan;

        public HumanPlayer(PlayerType Type, string Name, Map Map, Player[] Enemies, MainGameWindow window, int Money) 
            : base(Type, Name, Map, Enemies, 0, 1, 1, 1, 1, 1, 10, constantMovementFunction: false)
        {
            main = window;
            base.Money = new Attribute(Money, "Money", "Coins");
            mana.RawValue = (int)mana.MaxValue();
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
                troop.health.RawValue = troop.health.MaxValue().Value;
                main.WriteConsole("Level Up! Healing to max hp!");
            }
        }

        public override void PlayTurn(MainGameWindow main, bool SingleTurn)
        {
        }

        public override void WorldAction(double newWorldActionPoints)
        {
            worldActionPoints += newWorldActionPoints;
            if (toMove.Count == 0) worldActionPoints = 0; //TODO: Find a better system to handle actions which need more action points than available in one step

            //What action to do?
            if (availableActions.Exists(a => (a is StartMission m) && m.Forced))
                return;

            HandleMovement();
            availableActions = possibleActions.Where(a => a.Available(this)).ToList();
        }

        internal override void HandleMovement(double? maxPointUsed = null)
        {
            if (toMove.Count == 0) return;

            double points = worldActionPoints;
            if (caravan != null)
                points *= 2;
            if (maxPointUsed != null)
                points = Math.Min(points, maxPointUsed.Value);

            worldActionPoints -= points;
            while (toMove.Count != 0 && points >= World.World.Instance.worldMap.Get(toMove.First()).movementCost)
            {
                Point point = toMove.First();
                points -= World.World.Instance.worldMap.Get(point).movementCost;
                World.World.Instance.Move(this, point);
                if (worldRenderer != null)
                {
                    worldRenderer.overlayObjects.RemoveAll(o => (o is OverlayLine l) && l.start == point.Mult(20).Add(10, 10));
                }
                toMove.RemoveAt(0);
                availableActions = possibleActions.Where(a => a.Available(this)).ToList();
                if (availableActions.Exists(a => (a is StartMission m) && m.Forced))
                    return;
            }
            worldActionPoints += points;
        }

        public Tree GetTree(string name)
        {
            return trees.Find(t => t.name == name);
        }
    }
}