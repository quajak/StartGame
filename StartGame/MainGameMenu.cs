using StartGame.Properties;
using System;
using System.Windows.Forms;

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
                    map = new Map(31, 31); //TODO set global constant for the size
                    map.SetupMap(0.1, rnd.NextDouble() * 100, ((double)rnd.Next(8) - 4) / 20, 1);
                }
                else
                {
                    //Allow user to create a map
                    return;
                }
            }
            //Load player
            Player player = new Player(PlayerType.localHuman, Settings.Default.Name)
            {
                troop = playerTroop
            };
            Hide();
            MainGameWindow mainGameWindow = new MainGameWindow(map, player);
            mainGameWindow.ShowDialog();
            Show();
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
    }
}