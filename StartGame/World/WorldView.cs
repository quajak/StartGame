using StartGame.AI;
using StartGame.Items;
using StartGame.Mission;
using StartGame.PlayerData;
using StartGame.Properties;
using StartGame.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartGame.World
{
    public partial class WorldView : Form
    {
        bool running = false;
        World world = World.Instance;
        WorldRenderer worldRenderer;
        Timer controller = new Timer();
        //Todo: Use better zoom system
        int zoom = 0; //Should be between -15 and 30 
        public WorldView(HumanPlayer player = null)
        {
            if (player is null)
            {
                player = new HumanPlayer(PlayerType.localHuman, "Player", null, new Player[0], null, 0);
                Troop playerTroop = new Troop("Player", new Weapon(5, BaseAttackType.melee, BaseDamageType.blunt, 1, "Punch", 2, false), Resources.playerTroop, 0
                        , null, player) {
                armours = new List<Armour>
                    {
                        new Armour("Woolen Tunic", 50, new List<BodyParts>{BodyParts.LeftUpperArm,BodyParts.RightUpperArm,BodyParts.Torso}, Material.Materials.First(m => m.name == "Wool"),Quality.Common, ArmourLayer.clothing),
                        new Armour("Old Pants", 40, new List<BodyParts> { BodyParts.UpperLegs, BodyParts.LeftLowerLeg, BodyParts.RightLowerLeg, BodyParts.LeftShin, BodyParts.RightShin }, Material.Materials.First(m => m.name == "Cloth"), Quality.Poor, ArmourLayer.clothing),
                        new Armour("Wooden Shoes", 32, new List<BodyParts> { BodyParts.LeftFoot, BodyParts.RightFoot }, Material.Materials.First(m => m.name == "Wood"), Quality.Poor, ArmourLayer.light)
                    }
                };
                playerTroop.armours.ForEach(a => a.active = true);
                playerTroop.weapons.Add(new Weapon(50, BaseAttackType.magic, BaseDamageType.magic, 40, "GOD", 10, true));
                player.troop = playerTroop;
            }
            this.player = player;
            //determine player spawnpoint
            worldRenderer = new WorldRenderer(world, player);
            player.WorldPosition = new Point(10, 10);
            player.worldRenderer = worldRenderer;
            world.InitialisePlayer(player);
            world.actors.Add(player);
            InitializeComponent();
            playerView.Activate(player, null, false);
            Render();
            worldMapView.MouseWheel += WorldMapView_MouseWheel;
            controller.Interval = 1000;
            controller.Tick += Controller_Tick;
        }

        private void Controller_Tick(object sender, EventArgs e)
        {
            world.ProgressTime(new TimeSpan(0, 2, 0, 0));
            worldRenderer.Redraw = true; //Is there a way we can avoid this?
            Render();
            startMission.Visible = player.availableActions.Exists(a => a is StartMission);
        }

        private void WorldMapView_MouseWheel(object sender, MouseEventArgs e)
        {
            //pre zoom position
            double posX = (double)(e.X + worldRenderer.Position.X) / ((20 + zoom) * World.WORLD_SIZE);
            double posY = (double)(e.Y + worldRenderer.Position.Y) / ((20 + zoom) * World.WORLD_SIZE);
            
            //Zoom in or out
            zoom += e.Delta / 40;
            zoom = zoom < -19 ? -19 : zoom;
            zoom = zoom > 30 ? 30 : zoom;

            //We center around pixel the mouse is focused on
            //We know the offset from the top left corner
            //Solve for new position
            int positionX = (int)(posX * ((20 + zoom) * World.WORLD_SIZE) - e.X);
            int positionY = (int)(posY * ((20 + zoom) * World.WORLD_SIZE) - e.Y);
            worldRenderer.Position = new Point(positionX, positionY);
            worldRenderer.Position = worldRenderer.Position.Cut(0, (20 + zoom) * World.WORLD_SIZE - worldMapView.Width, 0, (20 + zoom) * World.WORLD_SIZE - worldMapView.Height);
            Render();
        }

        private void WorldView_Load(object sender, EventArgs e)
        {

        }

        void Render()
        {
            worldMapView.Image = worldRenderer.Render(worldMapView.Width, worldMapView.Height, 20 + zoom);
            if (running)
            {
                gameRunningControl.Text = "Pause";
            }
            else
            {
                gameRunningControl.Text = "Run";
            }
            worldTimeLabel.Text = world.time.ToString("MM/dd/yyyy H:mm");
        }

        private void WorldMapView_MouseMove(object sender, MouseEventArgs e)
        {

        }

        Point mouseDownPosition;
        public readonly HumanPlayer player;

        private void WorldMapView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mouseDownPosition = e.Location;
        }

        private void WorldMapView_MouseUp(object sender, MouseEventArgs e)
        {
            Point delta = e.Location.Sub(mouseDownPosition);
            worldRenderer.Position = worldRenderer.Position.Sub(delta);
            worldRenderer.Position = Cut();
            Render();
        }

        private Point Cut()
        {
            return worldRenderer.Position.Cut(0, (20 + zoom) * World.WORLD_SIZE - worldMapView.Width, 0, (20 + zoom) * World.WORLD_SIZE - worldMapView.Height);
        }

        private void FocusOnPlayer_Click(object sender, EventArgs e)
        {
            worldRenderer.Position.X = player.troop.Position.X * (20 + zoom) - worldMapView.Width / 2;
            worldRenderer.Position.Y = player.troop.Position.Y * (20 + zoom) - worldMapView.Height / 2;
            worldRenderer.Position = Cut();
            zoom = 0;
            Render();
        }

        Point selected;
        private void WorldMapView_Click(object sender, EventArgs e)
        {
        }

        private void WorldMapView_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void WorldMapView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int tileSize = (20 + zoom);
            int x = (worldRenderer.Position.X + e.X) / tileSize;
            int y = (worldRenderer.Position.Y + e.Y) / tileSize;
            selected = new Point(x, y);
            //TODO: If entity at position show info

            //Find route from player
            Point[] route = AStar.FindOptimalRoute(world.MovementCost(), player.WorldPosition, selected);
            Point previous = route[1];
            foreach (var point in player.toMove)
            {
                worldRenderer.overlayObjects.RemoveAll(o => (o is OverlayLine l) && l.start == point.Mult(20).Add(10, 10));
            }
            foreach (var point in route.Skip(1).ToList())
            {
                worldRenderer.overlayObjects.Add(new OverlayLine(previous.Mult(20).Add(10,10), point.Mult(20).Add(10,10), Color.Red, false));
                previous = point;
            }
            player.toMove = route.Skip(1).ToList();

            worldRenderer.Redraw = true;

            Render();

        }

        private void GameRunningControl_Click(object sender, EventArgs e)
        {
            running = !running;
            if (running) controller.Start();
            else controller.Stop();
            Render();
        }

        private void StartMission_Click(object sender, EventArgs e)
        {
            StartMission startMission1 = (player.availableActions.Find(a => a is StartMission) as StartMission);
            Mission.Mission selected = startMission1.mission;
            world.campaign.mission = selected;
            Map map = world.campaign.GenerateMap();
            player.map = map;
            player.troop.Map = map;
            MainGameWindow mainGame = new MainGameWindow(map, player, selected, world.trees, World.WORLD_DIFFICULTY, startMission1.difficulty);
            controller.Stop();
            mainGame.ShowDialog();
            controller.Start();
            world.missionsCompleted++;
            player.availableActions.Remove(startMission1);
            player.possibleActions.Remove(startMission1);
            world.actors.RemoveAll(p => (p is MissionWorldPlayer wp) && wp.WorldPosition == player.WorldPosition);
            world.GenerateNewMission();
            Render();
        }
    }
}
