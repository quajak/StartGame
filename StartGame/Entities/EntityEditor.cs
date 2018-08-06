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
    internal class EntityEditorOverview
    {
        private readonly Dungeon dungeon;
        private readonly List<EntityParameter> parameters;
        private Point point;
        private readonly Form form;
        private readonly Entity entity;
        private ListBox overview;
        private List<EntityEditorPiece> entityEditorPieces;
        private EntityEditorPiece active;

        public EntityEditorOverview(Dungeon dungeon, List<EntityParameter> parameters, Point point, Form form, Entity entity)
        {
            this.dungeon = dungeon;
            this.parameters = parameters;
            this.point = point;
            this.form = form;
            this.entity = entity;
        }

        public List<Control> GenerateGUI()
        {
            List<Control> controls = new List<Control>();
            overview = new ListBox() {
                Location = point
            };
            overview.Items.AddRange(parameters.ToArray());
            overview.SelectedIndexChanged += Overview_SelectedIndexChanged;

            point.X += overview.Width;

            form.Controls.Add(overview);
            controls.Add(overview);

            entityEditorPieces = new List<EntityEditorPiece>();
            foreach (var parameter in parameters)
            {
                switch (parameter.type)
                {
                    case EntityParameterType.position:
                        throw new Exception(); //This should not be used to change the position of an entity
                    case EntityParameterType.door:
                        DoorChooser item = new DoorChooser(dungeon, point, form, entity as Door); //TODO: Do not depend directly on door
                        entityEditorPieces.Add(item);
                        controls.AddRange(item.GenerateGUI());
                        break;

                    default:
                        break;
                }
            }
            return controls;
        }

        private void Overview_SelectedIndexChanged(object sender, EventArgs e)
        {
            active?.HideGUI();
            active = entityEditorPieces[(sender as ListBox).SelectedIndex];
            active?.StartGUI();
        }
    }

    /// <summary>
    /// This is the class used to change the values of an entity later. All derived classes should take the most accurate entity class as an argument
    /// and edit this in place
    /// </summary>
    internal abstract class EntityEditorPiece
    {
        internal Point point;
        internal Form form;

        public EntityEditorPiece(Point point, Form form)
        {
            this.point = point;
            this.form = form;
        }

        public abstract List<Control> GenerateGUI();

        public abstract void StartGUI();

        public abstract void HideGUI();
    }

    //TODO: Generalise this for all tyes of entities
    internal class DoorChooser : EntityEditorPiece
    {
        private readonly Dungeon dungeon;
        private ListBox doorList;
        private ListBox roomList;
        private Door door;

        public DoorChooser(Dungeon dungeon, Point point, Form form, Door entity) : base(point, form)
        {
            this.dungeon = dungeon;
            door = entity;
        }

        public override void StartGUI()
        {
            roomList.Visible = true;
            if (door.unlinked)
            {
                roomList.SelectedIndex = roomList.Items.Count - 1;
            }
            else
            {
                roomList.SelectedIndex = roomList.Items.IndexOf(door.Next.room);
            }
        }

        public override void HideGUI()
        {
            roomList.Visible = false;
            doorList.Visible = false;
        }

        public override List<Control> GenerateGUI()
        {
            //First generate list of rooms, and then one for the list of doors in it
            roomList = new ListBox() {
                Location = point,
                Visible = false
            };
            roomList.Items.AddRange(dungeon.dungeonRooms.Where(d => d != dungeon.active).ToArray());
            roomList.Items.Add("Unlinked");
            roomList.SelectedIndexChanged += RoomList_SelectedIndexChanged;
            form.Controls.Add(roomList);

            point.X += roomList.Width;

            doorList = new ListBox() {
                Location = point,
                Visible = false
            };
            doorList.SelectedIndexChanged += DoorList_SelectedIndexChanged;
            form.Controls.Add(doorList);

            return new List<Control> { roomList, doorList };
        }

        private void DoorList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox doorList = sender as ListBox;
            Door nDoor = doorList.SelectedItem as Door;

            door.unlinked = false;
            door.Next = (nDoor.from, nDoor);
        }

        private void RoomList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox roomList = sender as ListBox;
            if (roomList.SelectedIndex != -1)
            {
                if (roomList.SelectedItem is Room selected)
                {
                    doorList.Visible = selected.doors.Count != 0;
                    if (selected.doors.Count != 0)
                    {
                        doorList.Items.Clear();
                        doorList.Items.AddRange(selected.doors.ToArray());
                        doorList.SelectedIndex = selected.doors.IndexOf(door.Next.door);
                    }
                    else
                    {
                        door.unlinked = true;
                        door.Next = (null, null);
                    }
                }
                else
                {
                    door.unlinked = true;
                    door.Next = (null, null);
                    doorList.Visible = false;
                }
            }
        }
    }
}