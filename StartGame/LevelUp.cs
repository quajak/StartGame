﻿using StartGame;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlayerCreator
{
    partial class LevelUp : Form
    {
        private HumanPlayer player;

        private int StrengthUp = 0;
        private int AgilityUp = 0;
        private int EnduranceUp = 0;
        private int VitatlityUp = 0;
        private int WisdomUp = 0;
        private int IntelligenceUp = 0;

        private int points;

        public LevelUp(HumanPlayer Player, int Points)
        {
            InitializeComponent();
            player = Player;
            playerName.Text = player.Name;
            points = Points;

            Render();
        }

        private void Render()
        {
            playerStrength.Text = $"Strength: {player.strength} ({StrengthUp + player.strength})";
            playerAgiltiy.Text = $"Agility: {player.agility} ({AgilityUp + player.agility})";
            playerEndurance.Text = $"Endurance: {player.endurance} ({EnduranceUp + player.endurance})";
            playerVitality.Text = $"Vitality: {player.vitality} ({VitatlityUp + player.vitality})";
            playerWisdom.Text = $"Wisdom: {player.wisdom} ({player.wisdom + WisdomUp})";
            playerIntelligence.Text = $"Intelligence: {player.intelligence} ({player.intelligence + IntelligenceUp})";

            strengthUp.Enabled = points != 0;
            agilityUp.Enabled = points != 0;
            enduranceUp.Enabled = points != 0;
            vitalityUp.Enabled = points != 0;
            wisdomUp.Enabled = points != 0;
            intelligenceUp.Enabled = points != 0;

            strengthDown.Enabled = StrengthUp != 0;
            agilityDown.Enabled = AgilityUp != 0;
            enduranceDown.Enabled = EnduranceUp != 0;
            vitalityDown.Enabled = VitatlityUp != 0;
            wisdomDown.Enabled = WisdomUp != 0;
            intelligenceDown.Enabled = IntelligenceUp != 0;

            playerMaxHealth.Text = $"Max Health: {player.troop.maxHealth} ({2 * (VitatlityUp + player.vitality)})";
            playerActionPoints.Text = $"Action Points: {player.maxActionPoints} ({4 + (player.endurance + EnduranceUp) / 10})";
            playerDefense.Text = $"Defense: {player.troop.defense} ({player.endurance + EnduranceUp / 5})";
            playerDodge.Text = $"Dodge: {player.troop.dodge} ({player.troop.baseDodge + (player.agility + AgilityUp) * 2})";
            playerMana.Text = $"Mana: {player.maxMana} ({player.maxMana + WisdomUp * 2})";
        }

        private void StrengthUp_Click(object sender, EventArgs e)
        {
            StrengthUp++;
            points--;
            Render();
        }

        private void StrengthDown_Click(object sender, EventArgs e)
        {
            StrengthUp--;
            points++;
            Render();
        }

        private void AgilityDown_Click(object sender, EventArgs e)
        {
            AgilityUp--;
            points++;
            Render();
        }

        private void AgilityUp_Click(object sender, EventArgs e)
        {
            AgilityUp++;
            points--;
            Render();
        }

        private void EnduranceDown_Click(object sender, EventArgs e)
        {
            EnduranceUp--;
            points++;
            Render();
        }

        private void VitalityDown_Click(object sender, EventArgs e)
        {
            VitatlityUp--;
            points++;
            Render();
        }

        private void VitalityUp_Click(object sender, EventArgs e)
        {
            VitatlityUp++;
            points--;
            Render();
        }

        private void EnduranceUp_Click(object sender, EventArgs e)
        {
            EnduranceUp++;
            points--;
            Render();
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            player.strength += StrengthUp;
            player.agility += AgilityUp;
            player.endurance += EnduranceUp;
            player.vitality += VitatlityUp;
            player.wisdom += WisdomUp;
            player.intelligence += IntelligenceUp;
            player.CalculateStats();
            Close();
        }

        private void WisdomDown_Click(object sender, EventArgs e)
        {
            WisdomUp--;
            points++;
            Render();
        }

        private void WisdomUp_Click(object sender, EventArgs e)
        {
            WisdomUp++;
            points--;
            Render();
        }

        private void IntelligenceUp_Click(object sender, EventArgs e)
        {
            IntelligenceUp++;
            points--;
            Render();
        }

        private void IntelligenceDown_Click(object sender, EventArgs e)
        {
            IntelligenceUp--;
            points++;
            Render();
        }
    }
}