using StartGame.AI;
using StartGame.GameMap;
using StartGame.Mission;
using StartGame.Properties;
using StartGame.World;
using StartGame.World.Cities;
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

        internal virtual string Report()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class PureWorldPlayer : WorldPlayer
    {
        /// <summary>
        /// Used for players which do not fight
        /// </summary>
        /// <param name="name"></param>
        /// <param name="image"></param>
        public PureWorldPlayer(string name, Bitmap image, Point worldPosition) : base(PlayerType.computer, name, null, new Player[0], 0, 0, 0, 0, 0, 0, 0)
        {
            troop = new Troop(name, null, image, 0, null, this);
            WorldPosition = worldPosition;
        }

        /// <summary>
        /// Triggered when the player is on the same tile as this player
        /// </summary>
        public virtual void OnPlayerEntry()
        {

        }
        public override void PlayTurn(MainGameWindow main, bool singleTurn)
        {
            throw new NotImplementedException();
        }
    }

    public class Caravan : PureWorldPlayer
    {
        private readonly CaravanRoute route;
        readonly CaravanMission mission;

        public Caravan(CaravanRoute route) : base("Caravan: " + route.ToString(), Resources.Caravan, route.start.position)
        {
            Point[] moveRoute = AStar.FindOptimalRoute(World.World.Instance.MovementCost(), route.start.position, route.end.position);
            toMove = moveRoute.Skip(1).ToList();
            this.route = route;
            mission = new CaravanMission(this, route.items.Select(i => i.Cost * i.Amount).Sum() / 100, route.start.position);
            World.World.Instance.GetHumanPlayer().possibleActions.Add(mission);
            World.World.Instance.actors.Add(this);
            
        }

        public override void WorldAction(double newWorldActionPoints)
        {
            worldActionPoints += newWorldActionPoints / 2; //Caravans move slower
            HandleMovement();
            mission.position = WorldPosition;
            if(toMove.Count == 0)
            {
                route.End();
                World.World.Instance.GetHumanPlayer().possibleActions.Remove(mission);
                World.World.Instance.ToChange.Add(this);
            }
        }

        internal override string Report()
        {
            return $"The caravan is travelling from {route.start.name} to {route.end.name}. It is being protected by {mission.GetEnemyNumber()} guards " +
                $"and carries approximately goods worth {route.items.Select(i => i.Cost * i.Amount).Sum().WriteSignigicantFigures(1)}.";
        }
    }
}