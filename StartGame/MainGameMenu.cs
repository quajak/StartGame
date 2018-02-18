using StartGame.Properties;
using System;
using System.Windows.Forms;
using PlayerCreator;
using System.Threading;

namespace StartGame
{
    public partial class MainGameMenu : Form
    {
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
            if (map == null)
            {
                if (MessageBox.Show("You have no map selected! \n Starting now will mean using a random map!", "Alert", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    //Start game with random map
                    Random rnd = new Random();
                    map = new Map(10, 10); //TODO set global constant for the size
                    Thread mapThread;
                    do
                    {
                        mapThread = new Thread(() => map.SetupMap(new Tuple<double, double, double>(0.1, rnd.NextDouble() * 100, 0)))
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
                playerTroop = new Troop(Settings.Default.Name, 10, new Weapon(5, AttackType.magic, 1, "Punch", 2, false), Resources.playerTroop, 0);
                playerTroop.weapons.Add(new Weapon(50, AttackType.magic, 40, "GOD", 10, true));
                //return;
            }
            Player player = new Player(PlayerType.localHuman, Settings.Default.Name, null, null)
            {
                troop = playerTroop
            };
            Hide();
            MainGameWindow mainGameWindow = new MainGameWindow(map, player);
            mainGameWindow.ShowDialog();
            Show();

            //Reset all variables
            map = null;
            player = null;
        }

        private void PlayerSetup_Click(object sender, EventArgs e)
        {
            PlayerProfile playerProfile = new PlayerCreator.PlayerProfile();
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
            Player player = new Player(PlayerType.localHuman, Settings.Default.Name, null, null)
            {
                troop = playerTroop
            };
            CampaignController campaignCreator = new CampaignController(player);
            campaignCreator.ShowDialog();
            Show();
        }
    }
}