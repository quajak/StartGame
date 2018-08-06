using StartGame.Dungeons;
using StartGame.PlayerData;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartGame.Entities
{
    public abstract class Entity
    {
        public readonly string name;
        private Point position;
        public readonly Bitmap image;
        public readonly bool blocking;
        private Map map;
        public bool finishedInitialisation = true;

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

        public virtual Entity Load()
        {
            throw new NotImplementedException();
        }

        public virtual string RawValue()
        {
            return $"{GetType().Name} {name} {position.X} {position.Y} {blocking}";
        }

        public virtual List<Control> GenerateFieldEditors(Point position, Form form)
        {
            return new List<Control>();
        }

        public override string ToString()
        {
            return $"{GetType().Name} - {Position}";
        }
    }

    public class Building : Entity
    {
        public Building(string Name, Point Position, Bitmap Image, Map map, bool Blocking = true) : base(Name, Position, Image, Blocking, map)
        {
        }

        virtual public void PlayerEnter(Player player)
        {
        }

        public override string RawValue()
        {
            return base.RawValue();
        }
    }

    public class Door : Building
    {
        private readonly Dungeon dungeon;
        private (Room room, Door door) next;
        public readonly Room from;
        public bool unlinked = false;
        private EntityEditorOverview controller;
        public int Id;

        public (Room room, Door door) Next
        {
            get => next; set
            {
                if (value.door == next.door) return;

                if (value.room is null)
                {
                    if (value.door != null) throw new Exception();
                    if (next.room != null)
                    {
                        Door door = next.door;
                        next = value;
                        door.unlinked = true;
                        door.next = (null, null);
                    }
                }
                else
                {
                    if (next.room != null)
                    {
                        Door door1 = next.door;
                        next = value;
                        door1.unlinked = true;
                        door1.next = (null, null);
                    }
                    else
                    {
                        next = value;
                    }
                    next.door.unlinked = false;
                    next.door.Next = (from, this);
                }
            }
        }

        public Door(Dungeon dungeon, (Room, Door) next, Room from, Point position, bool assingID = true) : base("Door", position, Resources.Door, from.map, false)
        {
            var (room, door) = next;
            unlinked = door == null;
            this.dungeon = dungeon;
            this.from = from;
            Next = next;
            if (assingID)
            {
                Id = from.DoorID++;
            }
        }

        public Door(Dungeon dungeon, (Room, Door) next, Room from, Point position, int id) : this(dungeon, next, from, position, false)
        {
            Id = id;
            if (from.DoorID <= id) from.DoorID = id + 1;
            finishedInitialisation = false;
        }

        public override void PlayerEnter(Player player)
        {
            if (unlinked) throw new Exception();

            if (player is HumanPlayer humanPlayer)
                dungeon.MoveTo(from, Next);
        }

        public override List<Control> GenerateFieldEditors(Point position, Form form)
        {
            //Just create classes from entity editor
            List<EntityParameter> entityParameters = new List<EntityParameter>() {
                    new EntityParameter("Next", EntityParameterType.door) {
                        value = Next
                    }
            };
            controller = new EntityEditorOverview(dungeon, entityParameters, position, form, this);
            return controller.GenerateGUI();
        }

        public override Entity Load()
        {
            return base.Load();
        }

        public override string RawValue()
        {
            return $"{base.RawValue()} {Id} {unlinked} {(Next.room is null ? "null" : Next.room.name.ToString())} " +
                $"{(Next.door is null ? "null" : Next.door.Id.ToString())}";
        }
    }
}