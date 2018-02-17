using PlayerCreator.Properties;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PlayerCreator
{
    public partial class PlayerProfile : Form
    {
        public Troop troop;

        public PlayerProfile()
        {
            InitializeComponent();
            playerStatsPanel.Hide();
            weaponCreatorPanel.Hide();
            Finish.Hide();
            name.Text = Settings.Default.Name;
            WeaponCreatorLoad();
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
            troop = new Troop(name.Text, 10, new Weapon(5, AttackType.melee, 1, "Fists", 2, false), Resources.playerTroop, 0);
            UpdatePlayerStats();
            UpdateWeaponStats(troop.WeaponIndex);
            playerStatsPanel.Show();
            weaponCreatorPanel.Show();
            Finish.Show();
        }

        private void UpdatePlayerStats()
        {
            playerName.Text = troop.name;
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
        }

        private void WeaponCreatorLoad()
        {
            weaponCreatorType.Items.AddRange(Enum.GetValues(typeof(AttackType)).Cast<AttackType>().Select(E => E.ToString()).ToArray());
            weaponCreatorType.SelectedIndex = 0;
        }

        private void AddNewWeaponClick(object sender, EventArgs e)
        {
            troop.weapons.Add(new Weapon((int)weaponCreatorDamage.Value,
                (AttackType)weaponCreatorType.SelectedIndex, (int)weaponCreatorRange.Value,
                createWeaponName.Text, (int)attacksPerTurn.Value, false)); //TODO: Allow weapon creator to say that a weapon can be discarded
            UpdateWeaponStats(troop.WeaponIndex);
        }

        private void UpdateWeaponStats(int index)
        {
            troop.WeaponIndex = index;
            activeWeaponName.Text = troop.activeWeapon.name;
            playerActiveWeaponDamage.Text = troop.activeWeapon.attackDamage.ToString();
            playerActiveWeaponRange.Text = troop.activeWeapon.range.ToString();
            playerActiveWeaponType.Text = troop.activeWeapon.type.ToString();
            if (troop.weapons.Count == 0)
            {
                lastActiveWeapon.Enabled = false;
                nextActiveWeapon.Enabled = false;
            }
            else
            {
                lastActiveWeapon.Enabled = true;
                nextActiveWeapon.Enabled = true;
            }
            if (index == troop.weapons.Count - 1) nextActiveWeapon.Enabled = false;
            if (index == 0) lastActiveWeapon.Enabled = false;
        }

        private void LastActiveWeapon_Click(object sender, EventArgs e)
        {
            UpdateWeaponStats(--troop.WeaponIndex);
        }

        private void NextActiveWeapon_Click(object sender, EventArgs e)
        {
            UpdateWeaponStats(++troop.WeaponIndex);
        }

        private void Finish_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}