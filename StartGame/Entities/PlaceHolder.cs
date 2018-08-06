using StartGame.Dungeons;
using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Entities
{
    public abstract class PlayerPlaceHolder : Entity
    {
        public PlayerPlaceHolder(Point position, string name, Bitmap image, Map map) : base(name, position, image, false, map)
        {
        }

        public abstract Player InitialisePlayer();

        public static Bitmap AddShade(Bitmap bitmap)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawRectangle(new Pen(Color.FromArgb(40, 40, 40, 40), 1), 0, 0, bitmap.Width, bitmap.Height);
                return bitmap;
            }
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