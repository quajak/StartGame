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

namespace StartGame.User_Controls
{
    partial class PlayerBaseStats : UserControl
    {
        private HumanPlayer player;

        public void Activate(HumanPlayer player)
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
            playerAttackDamage.Text = player.troop.activeWeapon.attackDamage.ToString();
            playerAttackRange.Text = player.troop.activeWeapon.range.ToString();
            playerAttackType.Text = player.troop.activeWeapon.type.ToString();
            playerActionPoints.Text = $"{player.actionPoints} / {player.maxActionPoints}";
            playerHealth.Text = $"{player.troop.health} / {player.troop.maxHealth}";
            playerWeaponAttacks.Text = $"Attacks: {player.troop.activeWeapon.attacks} / {player.troop.activeWeapon.maxAttacks}";
            playerDefense.Text = $"Defense: {player.troop.defense}";
            playerStrength.Text = $"Strength: {player.strength}";
            playerAgility.Text = $"Agility: {player.agility}";
            playerEndurance.Text = $"Endurance: {player.endurance}";
            playerVitatlity.Text = $"Vitality: {player.vitality}";
            playerLevel.Text = $"Level: {player.level} + ({player.storedLevelUps})";
            playerXP.Text = $"XP: {player.xp} / {player.levelXP}";
            playerMana.Text = $"Mana: {player.mana} / {player.maxMana}";
            playerWisdom.Text = $"Wisdom: {player.wisdom}";
            playerIntelligence.Text = $"Intelligence: {player.intelligence}";
            playerMoney.Text = $"Money: {player.money}";
            playerGearWeight.Text = $"Gear weight: {player.GearWeight} / {player.MaxGearWeight}";
            if (player.GearWeight > player.MaxGearWeight)
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
    }
}