using StartGame.PlayerData;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Mission
{
    class MissionWorldPlayer : WorldPlayer
    {
        public static int counter = 0;
        public readonly Mission mission;
        public readonly int difficulty;

        public MissionWorldPlayer(Mission mission, int difficulty, Point point) : base(PlayerType.computer, $"Mission {++counter}", null, null, 0, 0, 0, 0, 0, 0, 0)
        {
            troop = new Troop($"Mission {counter}", null, Resources.Mission, 0, null, this);
            WorldPosition = point;
            this.mission = mission;
            this.difficulty = difficulty;
        }

        public override void PlayTurn(MainGameWindow main, bool singleTurn)
        {
            throw new NotImplementedException();
        }

        public override void WorldAction(double newWorldActionPoints)
        {
            //If a mission moves this is handled here
        }
    }
}
