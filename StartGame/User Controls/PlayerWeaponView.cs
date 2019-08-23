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
        private bool allowAction = false;

        public PlayerWeaponView()
        {
            InitializeComponent();
        }

        public void Activate(Player Player, bool AllowAction)
        {
            allowAction = AllowAction;
            player = Player;
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
                playerPossibleWeaponAttacks.Text = $"Attacks: {weapon.Attacks()} / {weapon.MaxAttacks()}";
                dumpWeapon.Enabled = weapon.discardeable && player.active;
                changeWeapon.Enabled = (weapon != player.troop.activeWeapon) && player.active;
                if(weapon is RangedWeapon w)
                {
                    ammoList.Visible = true;
                    List<Ammo> diffAmmo = ammoList.Items.Cast<Ammo>().Except(w.Ammo).ToList();
                    foreach (var d in diffAmmo)
                    {
                        ammoList.Items.Remove(d);
                    }
                    diffAmmo = w.Ammo.Except(ammoList.Items.Cast<Ammo>()).ToList();
                    foreach (var d in diffAmmo)
                    {
                        ammoList.Items.Add(d);
                    }
                    int index = w.Ammo.IndexOf(w.Ammo.Find(a => a.Selected.ContainsKey(w) && a.Selected[w]));
                    if(ammoList.SelectedIndex != index)
                        ammoList.SelectedIndex = index;
                }
                else
                {
                    ammoList.Visible = false;
                }
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
                ammoList.Visible = false;
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

        private void AmmoList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ammoList.SelectedItem != null && player.troop.weapons[playerWeaponList.SelectedIndex] is RangedWeapon w)
            {
                w.DeselectCurrent();
                (ammoList.SelectedItem as Ammo).Select(w);
                Render();
            }
        }
    }
}