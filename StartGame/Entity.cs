using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame
{
    internal abstract class Entity
    {
        public readonly string name;
        private Point position;
        public readonly Bitmap image;
        public readonly bool blocking;
        private Map map;

        public Point Position
        {
            get => position; set
            {
                if (Map != null && blocking)
                    Map.map[position.X, position.Y].free = true;
                position = value;
                if (Map != null && blocking)
                    Map.map[position.X, position.Y].free = false;

                //Update the position of the render entity
                if (Map != null)
                {
                    lock (Map.RenderController)
                    {
                        EntityRenderObject entity = Map.EntityRenderObjects.Find(o => o.Name == name);
                        if (entity != null)
                        {
                            //If an objects position is changed two or more times between two animations, the object will do the last animation
                            if (entity.toRender != entity.position)
                            {
                                entity.SetPosition(entity.toRender, PositionSetType.absolute);
                            }

                            entity.SetPosition(value, PositionSetType.goal);
                            //TODO: Generate a path not just set goal
                            entity.Animation = new LinearPointAnimation(entity.position, entity.toRender);
                            entity.movementAnimationPoints = (entity.Animation as LinearPointAnimation).Animate().GetEnumerator();
                        }
                    }
                }
            }
        }

        public Map Map
        {
            get => map; set
            {
                map = value;
                if (map != null && blocking)
                    map.map[position.X, position.Y].free = false;
            }
        }

        public Entity(string Name, Point Position, Bitmap Image, bool Blocking, Map map)
        {
            name = Name;
            Map = map;
            blocking = Blocking;
            this.Position = Position;
            image = Image;
        }
    }

    internal class Building : Entity
    {
        public Building(string Name, Point Position, Bitmap Image, Map map, bool Blocking = true) : base(Name, Position, Image, Blocking, map)
        {
        }
    }
}