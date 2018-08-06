using StartGame.Dungeons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartGame.Entities
{
    internal enum EntityParameterType
    { position, door }

    internal class EntityParameter
    {
        public object value;
        public string name;
        public EntityParameterType type;

        public EntityParameter(string Name, EntityParameterType type)
        {
            name = Name;
            this.type = type;
            value = null;
            if (type == EntityParameterType.position)
                value = new Point(0, 0);
        }

        public override string ToString()
        {
            return name;
        }
    }

    internal enum EntityTemplate
    { Door }

    internal class EntityFactory
    {
        private ListBox overview;
        private Point location;
        private EntityParameter active;
        private readonly List<EntityParameter> entityParameters;
        private readonly EntityTemplate type;
        private readonly Dungeon dungeon;
        private DungeonCreator dungeonC;
        private List<Control> inputs = new List<Control>();

        public EntityFactory(EntityTemplate type, Dungeon dungeon)
        {
            switch (type)
            {
                case EntityTemplate.Door:
                    entityParameters = new List<EntityParameter> {
                        new EntityParameter("Next", EntityParameterType.door),
                        new EntityParameter("Position", EntityParameterType.position)
                    };
                    break;

                default:
                    throw new NotImplementedException();
            }

            this.type = type;
            this.dungeon = dungeon;
        }

        public EntityFactory(EntityTemplate type, Dungeon dungeon, Point point) : this(type, dungeon)
        {
            entityParameters.Find(e => e.name == "Position").value = point;
        }

        public void InitialiseEntityFactoryGUI(DungeonCreator dungeonCreator, Point point)
        {
            location = point;
            dungeonC = dungeonCreator;
            //Create list to keep all the enitty parameter names
            overview = new ListBox() {
                Location = point
            };
            overview.Items.AddRange(entityParameters.ToArray());
            location.X += overview.Width + 10;
            dungeonC.Controls.Add(overview);
            overview.SelectedIndexChanged += Overview_IndexChanged;
        }

        private void Overview_IndexChanged(object sender, EventArgs e)
        {
            if (overview.SelectedIndex != -1)
            {
                //Clean up position event handlers
                if (active != null && active.type == EntityParameterType.position)
                    (inputs[0] as Button).MouseClick -= PositionClick;

                active = overview.SelectedItem as EntityParameter;
            }
            else
                active = null;
            RenderActive();
        }

        private void RenderActive()
        {
            if (active != null)
            {
                inputs.ForEach(i => dungeonC.Controls.Remove(i));
                inputs.Clear();

                switch (active.type)
                {
                    case EntityParameterType.position:
                        Button button = new Button() {
                            Name = active.name,
                            Text = $"{active.name}: {(Point)active.value}",
                            Location = location,
                            Width = 150
                        };
                        inputs.Add(button);
                        dungeonC.Controls.Add(button);
                        button.MouseClick += PositionClick;
                        break;

                    case EntityParameterType.door:
                        ListBox listBox = new ListBox() {
                            Name = active.name,
                            Location = location
                        };
                        listBox.Items.AddRange(dungeon.dungeonRooms.Where(d => d != dungeon.active).ToArray());
                        listBox.Items.Add("Unlinked");
                        inputs.Add(listBox);
                        listBox.SelectedIndexChanged += RoomListBox_SelectedIndexChanged;
                        dungeonC.Controls.Add(listBox);
                        break;

                    default:
                        break;
                }
            }
        }

        private void RoomListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox listBox = (inputs[0] as ListBox);
            if (listBox.SelectedItem is null) return;
            if (listBox.SelectedItem is string && (string)listBox.SelectedItem == "Unlinked")
            {
                (bool, Door) temp = (true, null as Door);
                active.value = temp;
                return;
            }

            location.Y += 100;
            //Create listbox to show doors in that room
            ListBox doorList = new ListBox() {
                Location = location
            };
            location.Y -= 100;
            doorList.SelectedIndexChanged += RoomList_SelectedIndexChanged;
            doorList.Items.AddRange(((Room)listBox.SelectedItem).doors.ToArray());
            dungeonC.Controls.Add(doorList);
            inputs.Add(doorList);
        }

        private void RoomList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox doorList = sender as ListBox;
            if (doorList.SelectedItem != null)
            {
                active.value = (false, doorList.SelectedItem as Door);
            }
        }

        private void PositionClick(object sender, MouseEventArgs e)
        {
            dungeonC.secondaryClick = true;
            dungeonC.secondaryClickPurpose = DungeonCreator.SecondaryClickPurpose.EntityFactory;
            dungeonC.activeCommand = DungeonCreator.Command.Select;
            MapClicked += PositionChanger;
        }

        private void PositionChanger(object sender, MouseEventArgs e)
        {
            int x = e.X / MapCreator.fieldSize;
            int y = e.Y / MapCreator.fieldSize;

            if (x >= 0 && x < dungeon.active.map.width)
            {
                if (y >= 0 && dungeon.active.map.height > y)
                {
                    if (active.type == EntityParameterType.position)
                    {
                        if (dungeon.active.map.map[x, y].free) //TODO: Allow placing of non-blocking entities on blocking ones
                        {
                            active.value = new Point(x, y);
                            inputs[0].Text = $"{active.name}: {(Point)active.value}";
                        }
                        else
                        {
                            active.value = null;
                            inputs[0].Text = "Invalid location!";
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
        }

        public bool CreateEntity(ref Entity toCreate)
        {
            if (entityParameters.Exists(e => e is null))
                return false;

            switch (type)
            {
                case EntityTemplate.Door:
                    object value1 = entityParameters.Find(e => e.name == "Next").value;
                    if (value1 is null) return false;
                    (bool, Door) real = (false, null);
                    //if (value1.GetType().GetGenericArguments()[1].Name == "Object") -- this code might be useles if no error occurs with code in else block this can be removed
                    //{
                    //    (bool, object) value = ((bool, object))value1;
                    //    real = (value.Item1, value.Item2 as Door);
                    //}
                    //else
                    //{
                    (bool, Door) value = ((bool, Door))value1;
                    real = (value.Item1, value.Item2 as Door);
                    //}
                    toCreate = new Door(dungeon, (real.Item2?.from, real.Item2), dungeon.active,
                        (Point)entityParameters.Find(e => e.name == "Position").value);
                    if (real.Item2 != null)
                    {
                        real.Item2.unlinked = false;
                        real.Item2.Next = ((toCreate as Door).from, toCreate as Door);
                    }
                    return true;

                default:
                    throw new NotImplementedException();
            }
        }

        public void SetValue(string identifier, object value)
        {
            foreach (var item in entityParameters)
            {
                if (item.name == identifier)
                {
                    item.value = value;
                    return;
                }
            }
            throw new KeyNotFoundException();
        }

        public void CleanUp()
        {
            dungeonC.Controls.Remove(overview);
            inputs.ForEach(i => dungeonC.Controls.Remove(i));
        }

        //TODO: Move this to dungeon creator and trigger the event there
        public event MouseEventHandler MapClicked;

        public void TriggerMapClicked(object sender, MouseEventArgs mouseEventArgs)
        {
            MapClicked(sender, mouseEventArgs);
        }
    }
}