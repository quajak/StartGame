using StartGame.Properties;
using System;
using System.Windows.Forms;
using PlayerCreator;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using StartGame.Items;
using System.Linq;
using StartGame.DebugViews;
using StartGame.PlayerData;

namespace StartGame
{
    public partial class MainGameMenu : Form
    {
        //Logger
        public static TraceSource log = new TraceSource("MainLog");

        private Map map;
        private Troop playerTroop;

        public MainGameMenu()
        {
            InitializeComponent();
        }

        private void SetMap_Click(object sender, EventArgs e)
        {
            MapCreator mC = new MapCreator();
            if (mC.ShowDialog() == DialogResult.OK)
            {
                map = mC.map;
            }
        }

        private void MainGameMenu_Load(object sender, EventArgs e)
        {
        }

        private void StartGame_Click(object sender, EventArgs e)
        {
            //Long term: allow map selection
            if (map == null)
            {
                if (MessageBox.Show("You have no map selected! \n Starting now will mean using a random map!", "Alert", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    //Start game with random map
                    Random rnd = new Random();
                    map = new Map();
                    Thread mapThread;
                    do
                    {
                        mapThread = new Thread(() => map.SetupMap(new Tuple<double, double, double>(0.1, rnd.Next(), -0.2)))
                        {
                            Priority = ThreadPriority.Highest
                        };
                        mapThread.Start();
                    } while (!mapThread.Join(TimeSpan.FromSeconds(Map.creationTime)));
                }
                else
                {
                    //Allow user to create a map
                    return;
                }
            }
            //Load player
            if (playerTroop is null)
            {
                MessageBox.Show("Please create your troop before starting the game!");
                playerTroop = new Troop(Settings.Default.Name, 10, new Weapon(5, BaseAttackType.melee, BaseDamageType.blunt, 1, "Punch", 2, false), Resources.playerTroop, 0, map)
                {
                    armours = new List<Armour>
                    {
                        new Armour("Woolen Tunic", 50, new List<BodyParts>{BodyParts.LeftUpperArm,BodyParts.RightUpperArm,BodyParts.Torso}, Material.Materials.First(m => m.name == "Wool"),Quality.Common, ArmourLayer.clothing),
                        new Armour("Old Pants", 40, new List<BodyParts> { BodyParts.UpperLegs, BodyParts.LeftLowerLeg, BodyParts.RightLowerLeg, BodyParts.LeftShin, BodyParts.RightShin }, Material.Materials.First(m => m.name == "Cloth"), Quality.Poor, ArmourLayer.clothing),
                        new Armour("Wooden Shoes", 32, new List<BodyParts> { BodyParts.LeftFoot, BodyParts.RightFoot }, Material.Materials.First(m => m.name == "Wood"), Quality.Poor, ArmourLayer.light)
                    }
                };
                playerTroop.weapons.Add(new Weapon(50, BaseAttackType.magic, BaseDamageType.magic, 40, "GOD", 10, true));
                //return;
            }
            HumanPlayer player = new HumanPlayer(PlayerType.localHuman, Settings.Default.Name, null, null, null, 0)
            {
                troop = playerTroop
            };
            player.troop.armours.ForEach(a => a.active = true);

            //player.agility = 5;
            //player.strength = 5;
            //player.vitality = 20;
            //player.intelligence = 5;
            //player.wisdom = 5;
            //player.endurance = 5;

            //player.CalculateStats();
            Hide();
            //Long term: Make form to allow use to choose mission and difficulty

            Mission mission = new BanditMission();

            List<Tree> trees = Tree.GenerateTrees();

            MainGameWindow mainGameWindow = new MainGameWindow(map, player, mission, trees);
            try
            {
                mainGameWindow.ShowDialog();
            }
            catch (Exception f)
            {
                Trace.TraceError(f.ToString());
                MessageBox.Show(f.ToString());
            }
            Show();

            //Reset all variables
            map = null;
            player = null;
        }

        private void PlayerSetup_Click(object sender, EventArgs e)
        {
            PlayerProfile playerProfile = new PlayerProfile();
            Hide();
            playerProfile.ShowDialog();
            playerTroop = playerProfile.troop;
            Show();
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Hide();
            HumanPlayer player = new HumanPlayer(PlayerType.localHuman, Settings.Default.Name, null, null, null, 0)
            {
                troop = playerTroop
            };

            CampaignController campaignCreator = new CampaignController(player);
            campaignCreator.ShowDialog();
            Show();
        }

        private void ArmourCreator_Click(object sender, EventArgs e)
        {
            Hide();
            ArmourCreator armourCreator = new ArmourCreator();
            armourCreator.ShowDialog();
            Show();
        }
    }
}