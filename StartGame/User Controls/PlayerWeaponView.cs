using StartGame.Items;
using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace StartGame.User_Controls
{
    partial class PlayerWeaponView : UserControl
    {
        private Player player;
        private MainGameWindow main;
        private bool allowAction = false;

        public PlayerWeaponView()
        {
            InitializeComponent();
        }

        public void Activate(Player Player, MainGameWindow Main, bool AllowAction)
        {
            allowAction = AllowAction;
            player = Player;
            main = Main;
        }

        public void Render()
        {
            //Initialise information about player weapons
            List<string> playerWeaponNames = player.troop.weapons.ConvertAll(t => t.name);
            List<string> diff = playerWeaponList.Items.Cast<string>().Except(playerWeaponNames).ToList();
            foreach (string dif in diff)
            {
                playerWeaponList.Items.Remove(dif);
            }

            diff = playerWeaponNames.Except(playerWeaponList.Items.Cast<string>()).ToList();
            foreach (string dif in diff)
            {
                playerWeaponList.Items.Add(dif);
            }

            if (allowAction)
            {
                if (player != null && player.active)
                {
                    if (playerWeaponList.SelectedIndex != -1 && player.troop.activeWeapon != player.troop.weapons[playerWeaponList.SelectedIndex])
                        changeWeapon.Enabled = true;
                    if (playerWeaponList.SelectedIndex != -1 && player.troop.weapons[playerWeaponList.SelectedIndex].discardeable)
                        dumpWeapon.Enabled = true;
                }
                else
                {
                    changeWeapon.Enabled = false;
                    dumpWeapon.Enabled = false;
                }
            }
            else
            {
                changeWeapon.Visible = false;
                dumpWeapon.Visible = false;
            }

            int pos = playerWeaponList.SelectedIndex;
            if (pos != -1)
            {
                Weapon weapon = player.troop.weapons[pos];
                playerPossibleAttackRange.Text = $"Range: {weapon.range}";
                playerPossibleWeaponDamage.Text = $"Damage: {weapon.attackDamage}";
                playerPossibleWeaponName.Text = $"{weapon.name}";
                playerPossibleWeaponType.Text = $"Type: {weapon.type}";
                playerPossibleWeaponAttacks.Text = $"Attacks: {weapon.attacks} / {weapon.maxAttacks}";
                dumpWeapon.Enabled = weapon.discardeable && player.active;
                changeWeapon.Enabled = (weapon != player.troop.activeWeapon) && player.active;
            }
            else
            {
                playerPossibleAttackRange.Text = "";
                playerPossibleWeaponDamage.Text = "";
                playerPossibleWeaponName.Text = "";
                playerPossibleWeaponType.Text = "";
                playerPossibleWeaponAttacks.Text = "";
                dumpWeapon.Enabled = false;
                changeWeapon.Enabled = false;
            }
        }

        private void PlayerWeaponView_Load(object sender, EventArgs e)
        {
        }

        private void DumpWeapon_Click(object sender, EventArgs e)
        {
            if (playerWeaponList.SelectedIndex != -1)
            {
                Weapon toRemove = player.troop.weapons[playerWeaponList.SelectedIndex];
                player.troop.weapons.Remove(toRemove);
                playerWeaponList.Items.Remove(toRemove.name);
                Render();
            }
        }

        private void ChangeWeapon_Click(object sender, EventArgs e)
        {
            if (playerWeaponList.SelectedIndex >= 0)
            {
                player.troop.activeWeapon = player.troop.weapons[playerWeaponList.SelectedIndex];
            }
            Render();
        }

        private void PlayerWeaponList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Render();
        }
    }
}