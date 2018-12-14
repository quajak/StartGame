using StartGame.Entities;
using StartGame.PlayerData;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartGame.Dungeons
{
    //TODO: Add dialog to choose dungeon
    public partial class DungeonCreator : Form
    {
        private Dungeon dungeon;
        private MapTileTypeEnum activeTileSet = MapTileTypeEnum.land;
        private double setHeight = 0;
        private EntityFactory entityFactory = null;
        public bool secondaryClick = false;
        public SecondaryClickPurpose secondaryClickPurpose;
        private List<Control> entityOverviewControls = new List<Control>();

        public enum SecondaryClickPurpose { EntityFactory, SpawnPoint };

        private List<string> roomNames = new List<string>();

        private bool showSelected = false;
        private string dungeonValidityMessage = "";

        #region Properties

        public double SetHeight
        {
            get => setHeight; set
            {
                tileHeight.Text = $"Height: {value}";
                setHeight = value;
            }
        }

        internal EntityFactory EntityFactory
        {
            get => entityFactory; set
            {
                finishEntityCreation.Enabled = value != null;
                entityFactory = value;
            }
        }

        public bool ShowSelected
        {
            get => showSelected; set
            {
                selectedData.Visible = value;
                showSelected = value;
                if (!value)
                {
                    selectedEntities.Visible = false;
                    selectedEntityData.Visible = false;
                }
            }
        }

        #endregion Properties

        public DungeonCreator()
        {
            InitializeComponent();
            dungeon = new Dungeon(dungeonName.Text, (int)mapWidth.Value, (int)mapHeight.Value);

            //Initialse the tile chooser
            foreach (var type in Enum.GetValues(typeof(MapTileTypeEnum)))
            {
                tileChooser.Items.Add(((MapTileTypeEnum)type).Description());
            }
            tileChooser.SelectedIndex = 0;

            //Initialise the entity chooser
            entityChooser.Items.AddRange(new[] {
                "Door"
            });

            //Initialse the room list
            roomList.Items.AddRange(dungeon.dungeonRooms.ToArray());

            //The dungeon list
            dungeonList.Items.AddRange(Dungeon.GetDungeons().ToArray());

            //Base text for new room name is set to Room
            newRoomName.Text = "Room";

            tilePaintPartSelector.Items.AddRange(new[] {
                "Type", "Height", "Free"
            });

            SetHeight = 0;
            InitialiseCommandBar();
            FullRender();
            CheckValidity();
        }

        #region Rendering

        public void FullRender(bool forceEntityDrawing = false, bool forceBackgroundDrawing = false)
        {
            roomName.Text = dungeon.active.name;
            mapHeight.Value = dungeon.active.Height;
            mapWidth.Value = dungeon.active.Width;
            if (dungeon.start.room != null)
            {
                setPlayerEntryPoint.Text = $"Player Spawn: {dungeon.start.room} at {dungeon.start.position}";
            }
            else
            {
                setPlayerEntryPoint.Text = "Set Player Entry Point";
            }

            UpdateRoomList();
            RenderMap(forceEntityDrawing, forceBackgroundDrawing);
        }

        private void UpdateRoomList()
        {
            //Update the room list if the number changed or the name has changed
            if (roomList.Items.Count != dungeon.dungeonRooms.Count || dungeon.dungeonRooms.Exists(d => !roomNames.Exists(r => r == d.ToString())))
            {
                roomNames = dungeon.dungeonRooms.ConvertAll(d => d.ToString());
                int selectedIndex = roomList.SelectedIndex;
                roomList.Items.Clear();
                roomList.Items.AddRange(dungeon.dungeonRooms.ToArray());
                if (selectedIndex < roomList.Items.Count)
                    roomList.SelectedIndex = selectedIndex;
            }
        }

        private void ShowNewDungeon()
        {
            EntityFactory?.CleanUp();
            EntityFactory = null;
            entityOverviewControls.ForEach(en => Controls.Remove(en));
            entityOverviewControls.Clear();
            entityChooser.Items.AddRange(dungeon.customEntities.Select(c => c.Name).ToArray());
            FullRender();
            CheckValidity();
        }

        private List<Bitmap> frames = new List<Bitmap>();
        private Thread animator;
        private bool animating = false;
        public bool actionOccuring = false;

        public void RenderMap(bool forceEntityRedrawing = false, bool forceDrawBackground = false)
        {
            if (IsActive(Command.ShowBlocking))
            {
                Color fill = Color.FromArgb(120, Color.Brown);
                lock (dungeon.active.map.RenderController)
                {
                    for (int X = 0; X <= dungeon.active.map.map.GetUpperBound(0); X++)
                    {
                        for (int Y = 0; Y <= dungeon.active.map.map.GetUpperBound(1); Y++)
                        {
                            if (!dungeon.active.map.map[X, Y].free)
                                dungeon.active.map.overlayObjects.Add(
                                    new OverlayRectangle(X * MapCreator.fieldSize,
                                    Y * MapCreator.fieldSize, MapCreator.fieldSize,
                                    MapCreator.fieldSize, Color.Brown, true, FillColor: fill));
                        }
                    }
                }
            }
            if (!animating && !actionOccuring)
            {
                animating = true;
                frames = new List<Bitmap>();
                animator = new Thread(() => dungeon.active.map.Render(dungeonPicture, frames,
                    forceEntityRedrawing, frameTime: 100, debug: false,
                    forceDrawBackground: forceDrawBackground)) {
                    Name = "Animator Thread"
                };
                animator.Start();
                System.Timers.Timer timer = new System.Timers.Timer(100) {
                    Enabled = true
                };
                timer.Elapsed += ChangeImage;
                timer.Start();
            }
            else
            {
            }
        }

        private void ChangeImage(object sender, System.Timers.ElapsedEventArgs e)
        {
            Bitmap newFrame = null;
            lock (frames)
            {
                Bitmap loc = frames.FirstOrDefault();
                if (loc is null) return;
                lock (loc)
                {
                    frames.Remove(loc);
                    if (loc is null)
                        newFrame = null;
                    else
                        newFrame = new Bitmap(loc);
                }
            }
            if (newFrame != null)
            {
                dungeonPicture.Image = newFrame;
            }
            if (!(animator.IsAlive || frames.Count != 0))
            {
                animating = false;
                (sender as System.Timers.Timer).Stop();
            }
        }

        #endregion Rendering

        #region Game Logic

        //TODO: Run this after every change
        private void CheckValidity()
        {
            (bool functional, string message) = dungeon.active.Valid();
            errorMessage = message;
            isDungeonRoomValid.Checked = functional;
            showRoomInvalidityMessage.Enabled = !functional;
            (bool dungeonValid, string dungeonErrorMessage) = dungeon.IsValid();
            dungeonValidityMessage = dungeonErrorMessage;
            isDungeonValid.Checked = dungeonValid;
            showDungeonInvalidityMessage.Enabled = !dungeonValid;
        }

        private void CreateEntityFactory()
        {
            if (dungeon.customEntities.Exists(c => c.Name == entityChooser.SelectedItem as string))
            {
                CustomPlayer c = dungeon.customEntities.Find(ce => ce.Name == entityChooser.SelectedItem as string);
                EntityFactory = new EntityFactory(EntityTemplate.CustomPlayer, dungeon, (CustomPlayer)c.Clone());
                EntityFactory.InitialiseEntityFactoryGUI(this, new Point(549, 180));
                return;
            }

            switch (entityChooser.SelectedItem)
            {
                case "Door":
                    EntityFactory = new EntityFactory(EntityTemplate.Door, dungeon);
                    EntityFactory.InitialiseEntityFactoryGUI(this, new Point(549, 180));
                    return;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion Game Logic

        #region Event Handler

        private void TileChooser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tileChooser.SelectedIndex != -1)
            {
                activeTileSet = MapTileType.MapTileTypeEnumFromString(tileChooser.SelectedItem as string);
            }
        }

        private void DungeonPicture_Click(object sender, EventArgs e)
        {
            return;
        }

        private void DungeonPicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                DungeonPicture_MouseClick(sender, e);
        }

        private void DungeonPicture_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / MapCreator.fieldSize;
            int y = e.Y / MapCreator.fieldSize;

            entityOverviewControls.ForEach(en => Controls.Remove(en));
            entityOverviewControls.Clear();

            ShowSelected = activeCommand == Command.Select && !secondaryClick;
            switch (activeCommand)
            {
                case Command.Paint:
                    if (x < dungeon.active.map.width && y < dungeon.active.map.height)
                    {
                        foreach (object item in tilePaintPartSelector.CheckedItems)
                        {
                            switch (item as string)
                            {
                                case "Type":
                                    MapTile mapTile1 = dungeon.active.map.map[x, y];
                                    dungeon.active.map.map[x, y] = new MapTile(x, y, new MapTileType() { type = activeTileSet },
                                        mapTile1.Height, mapTile1.free);
                                    break;

                                case "Height":
                                    dungeon.active.map.map[x, y].Height = SetHeight;
                                    break;

                                case "Free":
                                    dungeon.active.map.map[x, y].free = !setMapTileFree.Checked;
                                    break;

                                default:
                                    throw new NotImplementedException();
                            }
                        }
                        dungeon.active.map.UpdateMapTileData();
                        RenderMap(forceDrawBackground: true);
                    }

                    CheckValidity();
                    break;

                case Command.AddEntity:
                    if (EntityFactory is null)
                    {
                        if (x < dungeon.active.map.width && y < dungeon.active.map.height && entityChooser.SelectedItem != null)
                        {
                            CreateEntityFactory();
                            SetActive(Command.Select);
                            EntityFactory.SetValue("Position", new Point(x, y));
                        }
                    }
                    else
                    {
                        throw new Exception("Cannot create a new entity factory if one is still active");
                    }
                    break;

                case Command.Select:
                    if (!secondaryClick && x < dungeon.active.map.width && y < dungeon.active.map.height)
                    {
                        Map map = dungeon.active.map;
                        MapTile mapTile = map.map[x, y];
                        selectedData.Text = $"{mapTile.ToString()} Height {mapTile.Height} Free {mapTile.free}";
                        List<Entity> entities = map.GetEntities(x, y);
                        selectedEntities.Visible = entities.Count != 0;
                        selectedEntityData.Visible = entities.Count != 0;
                        if (entities.Count != 0)
                        {
                            selectedEntities.Items.Clear();
                            selectedEntities.Items.AddRange(entities.ToArray());
                        }
                    }
                    else if (x < dungeon.active.map.width && y < dungeon.active.map.height)
                    {
                        secondaryClick = false;
                        switch (secondaryClickPurpose)
                        {
                            case SecondaryClickPurpose.EntityFactory:
                                EntityFactory.TriggerMapClicked(sender, e);
                                break;

                            case SecondaryClickPurpose.SpawnPoint:
                                if (dungeon.active.map.map[x, y].free)
                                {
                                    dungeon.start = (dungeon.active, new Point(x, y));
                                    setPlayerEntryPoint.Text = $"Player Spawn: {dungeon.start.room} at {dungeon.start.position}";
                                }
                                else
                                {
                                    MessageBox.Show("Can not spawn player on blocked field!");
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void TileSetHeight_Scroll(object sender, EventArgs e)
        {
            SetHeight = tileSetHeight.Value / 10d;
        }

        private void AddEntity_Click(object sender, EventArgs e)
        {
            if (entityChooser.SelectedIndex != -1)
            {
                CreateEntityFactory();
                return;
            }
        }

        private void FinishEntityCreation_Click(object sender, EventArgs e)
        {
            Entity entity = null;
            if (EntityFactory.CreateEntity(ref entity))
            {
                if (entity is null)//assume that it is a custom player
                {
                    CustomPlayer player = EntityFactory.customPlayer ?? throw new NoNullAllowedException();
                    int num = 0;
                    while (dungeon.active.players.Exists(p => p.Name == player.Name + num))
                    {
                        num++;
                    }
                    player.Name += num;
                    dungeon.active.AddEntity(player.troop);
                }
                else
                {
                    dungeon.active.AddEntity(entity);
                }
                EntityFactory.CleanUp();
                EntityFactory = null;
                RenderMap();
                CheckValidity();
            }
            else
                MessageBox.Show("Please finish the entity creation!");
        }

        private void RoomName_TextChanged(object sender, EventArgs e)
        {
            if (roomName.Text != "")
                dungeon.active.name = roomName.Text;
            UpdateRoomList();
        }

        private void RoomList_SelectedIndexChanged(object sender, EventArgs e)
        {
            dungeon.active = roomList.SelectedItem as Room;
            if (dungeon.active is null) return;
            //Clean up any active gui parts
            entityOverviewControls.ForEach(c => Controls.Remove(c));
            entityOverviewControls.Clear();
            EntityFactory?.CleanUp();
            EntityFactory = null;
            selectedEntityData.Visible = false;
            selectedData.Visible = false;
            selectedEntities.Visible = false;

            FullRender();
        }

        private void AddRoom_Click(object sender, EventArgs e)
        {
            string roomName = newRoomName.Text;
            if (roomName == "") return;

            if (dungeon.dungeonRooms.Exists(d => d.name == roomName))
            {
                MessageBox.Show("A room with this name already exists!");
                return;
            }
            Room item = new Room(10, 10, roomName);
            dungeon.dungeonRooms.Add(item);

            FullRender();
        }

        private void SelectedEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedEntityData.Visible = selectedEntities.SelectedItem != null;
            selectedEntityDelete.Visible = selectedEntities.SelectedItem != null;
            if (selectedEntities.SelectedItem != null)
            {
                Entity entity = selectedEntities.SelectedItem as Entity;
                selectedEntityData.Text = entity.ToString();
                Point location = selectedEntityData.Location;
                location.X += selectedEntityData.Width + 10;
                entityOverviewControls = entity.GenerateFieldEditors(location, this);
            }
        }

        private void EntityChooser_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private string errorMessage = "";

        private void ShowRoomInvalidityMessage_Click(object sender, EventArgs e)
        {
            CheckValidity();
            MessageBox.Show("Error: " + errorMessage);
        }

        private void MapHeight_ValueChanged(object sender, EventArgs e)
        {
            dungeon.active.Height = (int)mapHeight.Value;
            FullRender();
        }

        private void MapWidth_ValueChanged(object sender, EventArgs e)
        {
            dungeon.active.Width = (int)mapWidth.Value;
            FullRender();
        }

        private void ShowDungeonInvalidityMessage_Click(object sender, EventArgs e)
        {
            CheckValidity();
            MessageBox.Show("Error: " + dungeonValidityMessage);
        }

        private void SetPlayerEntryPoint_Click(object sender, EventArgs e)
        {
            secondaryClick = true;
            secondaryClickPurpose = SecondaryClickPurpose.SpawnPoint;
        }

        private void DungeonName_TextChanged(object sender, EventArgs e)
        {
            if (dungeonName.Text != "")
            {
                //Serialise name
                string text = dungeonName.Text;
                text = text.Trim().Replace(" ", "");
                dungeon.name = text;
            }
        }

        private void LoadDungeon_Click(object sender, EventArgs e)
        {
            if (dungeonList.SelectedItem is null) return;
            dungeon = Dungeon.Load(dungeonList.SelectedItem as string);
            ShowNewDungeon();
        }

        private void SaveDungeon_Click(object sender, EventArgs e)
        {
            if (dungeon.Save())
            {
                MessageBox.Show("Saving succeeded!");
            }
            else
            {
                MessageBox.Show("Saving failed!");
            }

            //Update the dungeon list
            dungeonList.Items.Clear();
            dungeonList.Items.AddRange(Dungeon.GetDungeons().ToArray());
        }

        private void LoadExternalDungeon_Click(object sender, EventArgs e)
        {
            externalDungeonFolderBrowser.SelectedPath = Directory.GetCurrentDirectory();
            externalDungeonFolderBrowser.Description = "Directory of external dungeon";
            DialogResult dialog = externalDungeonFolderBrowser.ShowDialog();
            if (dialog == DialogResult.OK && externalDungeonFolderBrowser.SelectedPath != Directory.GetCurrentDirectory())
            {
                dungeon = Dungeon.LoadPath(externalDungeonFolderBrowser.SelectedPath);
                ShowNewDungeon();
            }
        }

        private void CreateNewEntittyTemplate_Click(object sender, EventArgs e)
        {
            PlayerCreator playerCreator = new PlayerCreator();
            playerCreator.ShowDialog();
            CustomPlayer p = playerCreator.player;
            if (p != null)
            {
                if (!entityChooser.Items.Contains(p.Name))
                {
                    entityChooser.Items.Add(p.Name);
                    dungeon.customEntities.Add(p);
                }
            }
        }

        private void SelectedEntityDelete_Click(object sender, EventArgs e)
        {
            Entity entity = selectedEntities.SelectedItem as Entity;
        }

        #endregion Event Handler

        #region Command Bar

        public enum Command
        { Paint, AddEntity, Select, ShowBlocking }

        private (Bitmap image, bool active, Command command)[] commandBarItems = new[] {
            (Resources.PaintTool, false, Command.Paint),
            (Resources.EntityAdderTool, false, Command.AddEntity),
            (Resources.SelectionTool, true, Command.Select),
            (Resources.ShowBlocking, false, Command.ShowBlocking)
        };

        public Command activeCommand = Command.Select;

        private Bitmap baseImage;

        private void InitialiseCommandBar()
        {
            Bitmap image = new Bitmap(commandBar.Width, commandBar.Height);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.Clear(Color.LightGray);

                int xPos = 0;
                foreach (var item in commandBarItems)
                {
                    g.DrawImage(item.image, new Point(xPos, 0));
                    xPos += 20;
                }
            }
            baseImage = image;
            CommandBarDrawActive();
        }

        private void CommandBarDrawActive()
        {
            Bitmap image = new Bitmap(baseImage);
            using (Graphics g = Graphics.FromImage(image))
            {
                int xPos = 0;
                foreach (var item in commandBarItems)
                {
                    if (item.active)
                    {
                        g.DrawRectangle(Pens.Red, xPos, 0, 19, 19);
                    }
                    xPos += 20;
                }
            }
            commandBar.Image = image;
        }

        private void CommandBar_Click(object sender, EventArgs e)
        {
        }

        private void CommandBar_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / 20;
            if (x < commandBarItems.Count())
            {
                for (int i = 0; i < commandBarItems.Length; i++)
                {
                    if (commandBarItems[i].command != Command.ShowBlocking)
                        commandBarItems[i].active = false;
                }
                activeCommand = commandBarItems[x].command;
                commandBarItems[x].active = !commandBarItems[x].active;

                if (commandBarItems[x].command == Command.ShowBlocking)
                {
                    SetActive(Command.Select);
                    RenderMap();
                }
            }
            CommandBarDrawActive();
        }

        private bool IsActive(Command command)
        {
            return commandBarItems.First(c => c.command == command).active;
        }

        private void SetActive(Command command)
        {
            if (command != Command.ShowBlocking)
                activeCommand = command;
            for (int i = 0; i < commandBarItems.Length; i++)
            {
                if (commandBarItems[i].command != Command.ShowBlocking)
                    commandBarItems[i].active = commandBarItems[i].command == command;
                if (command == Command.ShowBlocking && commandBarItems[i].command == command)
                {
                    commandBarItems[i].active = !commandBarItems[i].active;
                }
            }
            RenderMap();
            CommandBarDrawActive();
        }
        #endregion Command Bar

        private void DungeonCreator_Load(object sender, EventArgs e)
        {

        }
    }
}