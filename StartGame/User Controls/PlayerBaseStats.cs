using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StartGame.Items;
using StartGame.PlayerData;

namespace StartGame.User_Controls
{
    partial class PlayerBaseStats : UserControl
    {
        private Player player;

        public void Activate(Player player)
        {
            InitializeComponent();
            this.player = player;
            Render();
        }
        
        private void PlayerBaseStats_Load(object sender, EventArgs e)
        {
        }

        public void Render()
        {
            playerName.Text = player.Name;
            playerAttackDamage.Text = $"Damage: {player.troop.activeWeapon.attackDamage}";
            playerAttackRange.Text = $"Attack range: {player.troop.activeWeapon.range}";
            playerAttackType.Text = $"Attack type: {player.troop.activeWeapon.type}";
            playerActionPoints.Text = player.actionPoints.ToString();
            playerMovementPoints.Text = player.movementPoints.ToString();
            playerHealth.Text = player.troop.health.ToString();
            playerWeaponAttacks.Text = $"Attacks: {player.troop.activeWeapon.Attacks()} / {player.troop.activeWeapon.MaxAttacks()}";
            playerDefense.Text = $"{player.troop.defense}";
            playerDodge.Text = player.troop.dodge.ToString();
            playerStrength.Text = player.strength.ToString();
            playerAgility.Text = player.agility.ToString();
            playerEndurance.Text = player.endurance.ToString();
            playerVitatlity.Text = player.vitality.ToString();
            if (player is HumanPlayer h && h != null)
            {
                playerLevel.Text = $"Level: {h.level} + ({h.storedLevelUps})";
                playerXP.Text = $"XP: {h.xp} / {h.levelXP}";
                playerMoney.Text = player.Money.ToString();
            }
            else
            {
                playerXP.Text = "";
                playerMoney.Text = "";
                playerLevel.Text = "";
            }
            playerMana.Text = player.mana.ToString();
            playerWisdom.Text = player.wisdom.ToString();
            playerIntelligence.Text = player.intelligence.ToString();
            playerGearWeight.Text = player.gearWeight.ToString();
            if (player.gearWeight.Value > player.gearWeight.MaxValue().Value)
            {
                playerGearWeight.ForeColor = Color.Red;
            }
            else
            {
                playerGearWeight.ForeColor = playerMoney.ForeColor;
            }
            playerTroopImage.Image = player.troop.body.Render(false, player.troop.armours.Where(a => a.active).ToList(), 16);

            if (active != null)
            {
                playerBodyPartArmourList.Visible = true;
                if (playerBodyPartName.Text != active.name)
                {
                    playerBodyPartName.Text = active.name;
                    playerBodyPartArmourList.Items.Clear();
                    playerBodyPartArmourList.Items.AddRange(
                        player.troop.armours.Where(a => a.affected.Exists(b => b == active.part) && a.active)
                        .ToList().ConvertAll(a => a.name).ToArray());
                    playerBodyPartArmourList.Visible = playerBodyPartArmourList.Items.Count != 0;
                    if (playerBodyPartArmourList.Items.Count == 1) playerBodyPartArmourList.SelectedIndex = 0;
                }

                if (playerBodyPartArmourList.SelectedIndex != -1)
                {
                    playerActiveArmourTitle.Visible = true;
                    playerActiveArmourDescription.Visible = true;
                    int selectedIndex = playerBodyPartArmourList.SelectedIndex;
                    Armour armour = player.troop.armours.Where(a => a.active && a.affected.Exists(b => b == active.part)).ToList()[selectedIndex];
                    playerActiveArmourTitle.Text = armour.name;
                    playerActiveArmourDescription.Text = armour.Description;
                }
                else
                {
                    playerActiveArmourTitle.Visible = false;
                    playerActiveArmourDescription.Visible = false;
                }
            }
            else
            {
                playerBodyPartName.Text = "";
                playerBodyPartArmourList.Visible = false;
                playerActiveArmourTitle.Visible = false;
                playerActiveArmourDescription.Visible = false;
            }
        }

        private BodyPart active = null;

        private void PlayerTroopImage_Click(object sender, EventArgs e)
        {
        }

        private void PlayerBaseStats_Click(object sender, EventArgs e)
        {
            active = null;
        }

        private void PlayerTroopImage_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / 16;
            int y = e.Y / 16;
            active = player.troop.body.body[x, y];
            Render();
        }

        private void PlayerBodyPartArmourList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Render();
        }

        private void PlayerAttackDamage_Click(object sender, EventArgs e)
        {
        }

        private void PlayerMoney_Click(object sender, EventArgs e)
        {
        }

        private void PlayerMovementPoints_Click(object sender, EventArgs e)
        {
        }
    }
}