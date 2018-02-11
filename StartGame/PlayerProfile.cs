using StartGame.Properties;
using System;
using System.Linq;
using System.Windows.Forms;
using PlayerCreator;

namespace StartGame
{
    partial class PlayerProfile : Form
    {
        public Troop troop;

        public PlayerProfile()
        {
            InitializeComponent();
            name.Text = Settings.Default.Name;
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            if (name.Text.Length == 0)
            {
                MessageBox.Show("You're name can not be empty!");
                return;
            }
            Settings.Default.Name = name.Text;
            Settings.Default.Save();
            troop = new Troop(name.Text, 10, new Weapon((int)weaponAttack.Value, (AttackType)weaponType.SelectedIndex, (int)weaponRange.Value, "Fists"), Resources.playerTroop);
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PlayerProfile_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (name.Text.Length == 0)
            {
                Settings.Default.Name = Resources.BasePlayerName;
                Settings.Default.Save();
            }
        }

        private void PlayerProfile_Load(object sender, EventArgs e)
        {
            weaponType.Items.AddRange(Enum.GetValues(typeof(AttackType)).Cast<AttackType>().Select(E => E.ToString()).ToArray());
        }
    }
}