using StartGame.Mission;
using StartGame.PlayerData;
using StartGame.World.Cities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StartGame.World
{
    public abstract class WorldAction
    {
        public int cost;
        internal Point position;

        public WorldAction(Point Position, int Cost)
        {
            position = Position;
            cost = Cost;
        }

        public abstract bool Available(WorldPlayer player);
    }

    public class StartMission : WorldAction
    {
        public readonly Mission.Mission mission;
        public readonly int difficulty;
        
        public bool Forced => mission.forced;

        public StartMission(Mission.Mission mission, int difficulty, Point position) : base(position, 0)
        {
            this.mission = mission;
            this.difficulty = difficulty;
        }

        public override bool Available(WorldPlayer player)
        {
            return position == player.WorldPosition;
        }

        public virtual void MissionEnded(bool won)
        {

        }
    }

    public class CaravanMission : StartMission
    {
        private readonly Caravan caravan;

        public CaravanMission(Caravan caravan, int difficulty, Point position) : base(new CaravanAttack(), difficulty, position)
        {
            this.caravan = caravan;
        }

        public override void MissionEnded(bool won)
        {
            if (won)
            {
                // Player has captured the caravan
                World.Instance.ToChange.Add(caravan);
            }
        }

        internal int GetEnemyNumber()
        {
            return mission.GetEnemyNumber(difficulty);
        }
    }

    public class InteractCity : WorldAction
    {
        public Point[] points;
        public City city;

        public InteractCity(City city) : base(city.position, 0)
        {
            this.city = city;   
            List<Point> tP = new List<Point>();
            for (int x = 0; x < city.size.X; x++)
            {
                for (int y = 0; y < city.size.Y; y++)
                {
                    tP.Add(new Point(city.position.X - city.size.X / 2 + x, city.position.Y - city.size.Y / 2 + y));
                }
            }
            points = tP.ToArray();
        }

        public override bool Available(WorldPlayer player)
        {
            return points.Contains(player.WorldPosition);
        }
    }
}