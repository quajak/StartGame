using StartGame.AI;
using StartGame.GameMap;
using StartGame.Items;
using StartGame.Mission;
using StartGame.PlayerData;
using StartGame.Properties;
using StartGame.Rendering;
using StartGame.World.Cities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StartGame.World
{
    public partial class WorldView : Form
    {
        private bool running = false;
        private World world = World.Instance;
        private WorldRenderer worldRenderer;
        private Timer controller = new Timer();

        //Todo: Use better zoom system
        private int zoom = 0; //Should be between -15 and 30

        public WorldView(HumanPlayer player = null)
        {
            if (player is null)
            {
                player = new HumanPlayer(PlayerType.localHuman, "Player", null, new Player[0], null, 0);
                player.Money.RawValue = 1000;
                Troop playerTroop = new Troop("Player", new Weapon(5, BaseAttackType.melee, BaseDamageType.blunt, 1, "Punch", 2, false), Resources.playerTroop, 0
                        , null, player) {
                    armours = new List<Armour>
                    {
                        new Armour("Woolen Tunic", 50, new List<BodyParts>{BodyParts.LeftUpperArm,BodyParts.RightUpperArm,BodyParts.Torso}, Material.Materials.First(m => m.name == "Wool"),Quality.Common, ArmourLayer.clothing),
                        new Armour("Old Pants", 40, new List<BodyParts> { BodyParts.UpperLegs, BodyParts.LeftLowerLeg, BodyParts.RightLowerLeg, BodyParts.LeftShin, BodyParts.RightShin }, Material.Materials.First(m => m.name == "Cloth"), Quality.Poor, ArmourLayer.clothing),
                        new Armour("Wooden Shoes", 32, new List<BodyParts> { BodyParts.LeftFoot, BodyParts.RightFoot }, Material.Materials.First(m => m.name == "Wood"), Quality.Poor, ArmourLayer.light)
                    }
                };
                playerTroop.weapons.Add(new RangedWeapon(4, BaseDamageType.blunt, 5, "Rock", 1, true, AmmoType.Rock));
                playerTroop.items.Add(new Ammo(AmmoType.Rock, Buff.Zero, "Special rock", "rock", 10));
                playerTroop.armours.ForEach(a => a.active = true);
                playerTroop.weapons.Add(new Weapon(50, BaseAttackType.magic, BaseDamageType.magic, 40, "GOD", 10, true));
                player.troop = playerTroop;
            }
            this.player = player;
            //determine player spawnpoint
            worldRenderer = new WorldRenderer(world);
            List<City> small = world.nation.cities.Where(c => c is SmallCity && c.IsPort).ToList();
            Point point = new Point(0, 0);
            if(small.Count != 0)
            {
                point = small.GetRandom().position;
            }
            else
            {
                Trace.TraceWarning("Unable to determine good spawnpoint for player!");
                point = world.nation.cities.GetRandom().position;
            }
            Point spawn = point.Copy();
            do
            {
                spawn = point.Copy();
                spawn.X += World.random.Next(10) - 5;
                spawn.Y += World.random.Next(10) - 5;
                spawn.X = spawn.X.Cut(5, World.WORLD_SIZE - 5);
                spawn.Y = spawn.Y.Cut(5, World.WORLD_SIZE - 5);
            } while (!World.IsLand(world.worldMap.Get(spawn).type));
            player.WorldPosition = spawn;

            player.worldRenderer = worldRenderer;
            world.InitialisePlayer(player);
            world.actors.Add(player);
            InitializeComponent();
            playerView.Activate(player, null, false);
            Render();
            worldMapView.MouseWheel += WorldMapView_MouseWheel;
            controller.Interval = 500;
            controller.Tick += Controller_Tick;
            FocusOnPlayer();
        }

        private void Controller_Tick(object sender, EventArgs e)
        {
            world.ProgressTime(new TimeSpan(0, 2, 0, 0));
            worldRenderer.Redraw = true; //Is there a way we can avoid this?
            Render();
            startMission.Visible = player.availableActions.Exists(a => a is StartMission);
            enterCity.Visible = player.availableActions.Exists(a => a is InteractCity);
        }

        private void WorldMapView_MouseWheel(object sender, MouseEventArgs e)
        {
            //pre zoom snap to grid and center on mouse
            int cornerX = worldRenderer.Position.X / (20 + zoom);
            int mouseX = (e.X - worldMapView.Width / 2) / (20 + zoom);
            int posX = cornerX + mouseX;
            int cornerY = worldRenderer.Position.Y / (20 + zoom);
            int mouseY = (e.Y - worldMapView.Height / 2 )/ (20 + zoom) ;
            int posY = cornerY + mouseY;

            //Zoom in or out
            zoom += 2 * e.Delta / Math.Abs(e.Delta);
            zoom = zoom < -18 ? -18 : zoom;
            zoom = zoom > 30 ? 30 : zoom;

            worldRenderer.Position = new Point(posX * (20 + zoom), posY * (20 + zoom));
            worldRenderer.Position = worldRenderer.Position.Cut(0, (20 + zoom) * World.WORLD_SIZE - worldMapView.Width, 0, (20 + zoom) * World.WORLD_SIZE - worldMapView.Height);
            Render();
        }

        private void WorldView_Load(object sender, EventArgs e)
        {
        }

        public void Render()
        {
            worldMapView.Image?.Dispose();
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

        private Point mouseDownPosition;
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
            FocusOnPlayer();
        }

        public void FocusOnPlayer()
        {
            zoom = 0;
            worldRenderer.Position.X = player.WorldPosition.X * (20 + zoom) - worldMapView.Width / 2;
            worldRenderer.Position.Y = player.WorldPosition.Y * (20 + zoom) - worldMapView.Height / 2;
            worldRenderer.Position = Cut();
            Render();
        }

        private Point selected;

        private void WorldMapView_Click(object sender, EventArgs e)
        {
        }

        private void WorldMapView_MouseClick(object sender, MouseEventArgs e)
        {
            int tileSize = (20 + zoom);
            int x = (worldRenderer.Position.X + e.X) / tileSize;
            int y = (worldRenderer.Position.Y + e.Y) / tileSize;
            coords.Text = $"{x} {y}";
        }

        private void WorldMapView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int tileSize = (20 + zoom);
            int x = (worldRenderer.Position.X + e.X) / tileSize;
            int y = (worldRenderer.Position.Y + e.Y) / tileSize;
            selected = new Point(x, y);
            if (x != x.Cut(0, World.WORLD_SIZE) || y != y.Cut(0, World.WORLD_SIZE))
                return;
            //TODO: If entity at position show info

            //Find route from player
            Point[] route = AStar.FindOptimalRoute(world.MovementCost(), player.WorldPosition, selected);
            if (route.Length > 1)
            {
                Point previous = route[1];
                foreach (var point in player.toMove)
                {
                    worldRenderer.overlayObjects.RemoveAll(o => (o is OverlayLine l) && l.start == point.Mult(20).Add(10, 10));
                }
                foreach (var point in route.Skip(1).ToList())
                {
                    worldRenderer.overlayObjects.Add(new OverlayLine(previous.Mult(20).Add(10, 10), point.Mult(20).Add(10, 10), Color.Red, false));
                    previous = point;
                }
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

        private readonly Dictionary<WorldTileType, MapBiome> mapBiomes = new Dictionary<WorldTileType, MapBiome> {
            {WorldTileType.TemperateGrassland, new GrasslandMapBiome() },
            {WorldTileType.Alpine, new AlpineMapBiome() },
            {WorldTileType.Tundra, new TundraMapBiome() },
            {WorldTileType.Desert, new DesertMapBiome() },
            {WorldTileType.Savanna, new SavannaMapBiome() },
            {WorldTileType.Rainforest, new RainforestMapBiome() }
        };

        private void StartMission_Click(object sender, EventArgs e)
        {
            StartMission startMission1 = (player.availableActions.Find(a => a is StartMission) as StartMission);
            Mission.Mission selected = startMission1.mission;
            world.campaign.mission = selected;
            MapBiome biome = new GrasslandMapBiome();
            if (mapBiomes.ContainsKey(world.worldMap[player.WorldPosition.X, player.WorldPosition.Y].type))
            {
                biome = mapBiomes[world.worldMap[player.WorldPosition.X, player.WorldPosition.Y].type];
            }
            Map map = world.campaign.GenerateMap(biome);
            player.map = map;
            player.troop.Map = map;
            MainGameWindow mainGame = new MainGameWindow(map, player, selected, world.trees, World.WORLD_DIFFICULTY, startMission1.difficulty);
            biome.ManipulateMission(mainGame, selected);
            mainGame.RenderMap(true, true, true);
            controller.Stop();
            mainGame.ShowDialog();
            if (mainGame.dead)
            {
                //as player is dead campaign is over
                Close();
                return;
            }
            if (mainGame.giveReward)
            {
                //Now give reward
                MissionResult r = CampaignController.GenerateRewardAndHeal(player, mainGame, selected, player.vitality.Value, (player.level / 25d).Cut(0,1), "Close");
                r.ShowDialog();
            }
            if (running)
                controller.Start();
            world.missionsCompleted++;
            player.availableActions.Remove(startMission1);
            player.possibleActions.Remove(startMission1);
            world.actors.RemoveAll(p => (p is MissionWorldPlayer wp) && wp.WorldPosition == player.WorldPosition);
            world.GenerateNewMission();
            Render();
        }

        private void GenerateNewWorld_Click(object sender, EventArgs e)
        {
            World.NewWorld();
            world = World.Instance;
            worldRenderer = new WorldRenderer(World.Instance);
            Render();
        }

        private void CityView_Click(object sender, EventArgs e)
        {
            City city = (player.availableActions.First(a => a is InteractCity) as InteractCity).city;
            CityView cityView = new CityView(city, player, this);
            controller.Stop();
            cityView.ShowDialog();
            if (running)
                controller.Start();
        }

        private void WorldView_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.Stop();
            controller.Enabled = false;
            controller.Dispose();
            controller = null;
        }
    }
}