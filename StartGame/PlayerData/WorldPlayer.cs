using StartGame.GameMap;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StartGame.PlayerData
{
    public abstract class WorldPlayer : Player
    {
        public Point WorldPosition;
        public List<Point> toMove = new List<Point>();
        public double worldActionPoints = 0;

        public WorldPlayer(PlayerType playerType, string name, Map map, Player[] enemies, int XP, int intelligence, int strength, 
            int endurance, int wisdom, int agility, int vitality, List<Spell> spells = null, bool constantMovementFunction = true)
            : base(playerType, name, map, enemies, XP, intelligence, strength, endurance, wisdom, agility, vitality, spells, constantMovementFunction)
        {
        }

        public abstract void WorldAction(double newWorldActionPoints);

        internal virtual void HandleMovement(double? maxPointUsed = null)
        {
            if (toMove.Count == 0) return;

            double points = worldActionPoints;
            if (maxPointUsed != null)
                points = Math.Min(points, maxPointUsed.Value);

            worldActionPoints -= points;
            while (toMove.Count != 0 && points >= World.World.Instance.worldMap.Get(toMove.First()).movementCost)
            {
                Point point = toMove.First();
                points -= World.World.Instance.worldMap.Get(point).movementCost;
                World.World.Instance.Move(this, point);
                toMove.RemoveAt(0);
            }
            worldActionPoints += points;
        }
    }
}