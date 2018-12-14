using System.Drawing;
using StartGame.Items;
using StartGame.Properties;

namespace StartGame.PlayerData
{
    internal class SpiderNestAI : Player
    {
        private int numSpawned = 0;
        private readonly int difficulty;
        private int turn;
        private readonly int maxSpawned;
        private readonly int round;

        public SpiderNestAI(PlayerType Type, string Name, Map Map, Player[] Enemies, int Difficulty, int Round) : base(Type, Name, Map, Enemies, 5, 0, 0, 10, 0, 0, 10)
        {
            map = Map;
            turn = 7 - (Difficulty / 2);
            round = Round;
            difficulty = Difficulty;
            maxSpawned = 5 + Difficulty + Round;
        }

        public override void PlayTurn(MainGameWindow main, bool singleTurn)
        {
            turn = turn == 0 ? 0 : turn - 1;
            if (turn == 0 && numSpawned < maxSpawned)
            {
                //Find the position for the new spider to spawn
                Point pos = new Point(-1, -1);
                foreach (MapTile tile in map.map[troop.Position.X, troop.Position.Y].neighbours.rawMaptiles)
                {
                    //Check if empty and not water
                    if (tile.type.type != MapTileTypeEnum.deepWater || tile.type.type != MapTileTypeEnum.shallowWater && !map.troops.Exists(t => t.Position.X == tile.position.X && t.Position.Y == tile.position.Y))
                    {
                        pos = tile.position;
                        break;
                    }
                }

                if (pos.X == -1) // No space available wait for next turn
                    return;

                //Spawn a new spider
                numSpawned++;
                WarriorSpiderAI spider = new WarriorSpiderAI(PlayerType.computer, "Spider Spawn " + numSpawned, map, main.players.ToArray());
                spider.troop = new Troop("Spider Spawn " + numSpawned, new Weapon(1 + difficulty / 5 + round / 2,
                        BaseAttackType.melee, BaseDamageType.sharp, 1, "Fangs", 1, false), Resources.spiderWarrior, 0, map, spider, 25) {
                    Position = pos
                };

                main.AddPlayer(spider);

                turn = 5 - (difficulty / 2);
            }
        }
    }
}