using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        }
    }
}