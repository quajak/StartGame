using StartGame.Dungeons;
using StartGame.PlayerData;
using System.Drawing;
using System.Linq;

namespace StartGame.Entities
{
    public class PlayerPlaceHolder : EntityPlaceHolder
    {
        public readonly CustomPlayer player;

        public PlayerPlaceHolder(CustomPlayer player)
        {
            this.player = player;
        }
    }

    public abstract class EntityPlaceHolder
    {
    }

    public class DoorPlaceHolder : EntityPlaceHolder
    {
        private readonly (string, int) next;
        private readonly string roomBase;
        private readonly Point point;
        private readonly int id;

        public DoorPlaceHolder((string, int) next, string roomBase, Point point, int id)
        {
            this.next = next;
            this.roomBase = roomBase;
            this.point = point;
            this.id = id;
        }

        public Door Initialise(Dungeon dungeon)
        {
            Room baseR = dungeon.dungeonRooms.Find(d => d.name == roomBase);
            Room to = dungeon.dungeonRooms.Find(d => d.name == next.Item1);
            Door door = to?.doors.FirstOrDefault(d => d.Id == next.Item2);
            Door door1 = new Door(dungeon, (to, door), baseR, point, id);
            if (door != null)
            {
                door.unlinked = false;
                door.Next = (baseR, door1);
            }
            return door1;
        }
    }
}