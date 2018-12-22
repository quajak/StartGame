using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StartGame.Mission;
using StartGame.PlayerData;

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
    }
}
