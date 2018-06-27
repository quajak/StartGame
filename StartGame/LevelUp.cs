﻿using StartGame;
using StartGame.PlayerData;
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
            playerStrength.Text = $"{player.Strength.ToString()} ({StrengthUp + player.Strength.Value})";
            playerAgiltiy.Text = $"{player.Agility.ToString()} ({AgilityUp + player.Agility.Value})";
            playerEndurance.Text = $"{player.Endurance.ToString()} ({EnduranceUp + player.Endurance.Value})";
            playerVitality.Text = $"{player.Vitality.ToString()} ({VitatlityUp + player.Vitality.Value})";
            playerWisdom.Text = $"{player.Wisdom.ToString()} ({player.Wisdom.Value + WisdomUp})";
            playerIntelligence.Text = $"{player.Intelligence.ToString()} ({player.Intelligence.Value + IntelligenceUp})";

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

            playerMaxHealth.Text = $"Max Health: {player.troop.maxHealth} ({2 * (VitatlityUp + player.Vitality.Value)})";
            playerActionPoints.Text = $"Action Points: {player.MaxActionPoints} ({4 + (player.Endurance.Value + EnduranceUp) / 10})";
            playerDefense.Text = $"Defense: {player.troop.defense} ({player.Endurance.Value + EnduranceUp / 5})";
            playerDodge.Text = $"Dodge: {player.troop.dodge} ({player.troop.baseDodge + (player.Agility.Value + AgilityUp) * 2})";
            playerMana.Text = $"Mana: {player.maxMana} ({player.maxMana + WisdomUp * 2})";

            ok.Enabled = points == 0;
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

        private bool finished = false;

        private void Ok_Click(object sender, EventArgs e)
        {
            DistributePoints();
        }

        private void DistributePoints()
        {
            finished = true;
            player.Strength.rawValue += StrengthUp;
            player.Agility.rawValue += AgilityUp;
            player.Endurance.rawValue += EnduranceUp;
            player.Vitality.rawValue += VitatlityUp;
            player.Wisdom.rawValue += WisdomUp;
            player.Intelligence.rawValue += IntelligenceUp;
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

        private void LevelUp_Load(object sender, EventArgs e)
        {
        }

        private void LevelUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (points != 0)
            {
                MessageBox.Show("You cannot stop before you have distributed all your points");
                e.Cancel = true;
            }
            else
            {
                if (finished)
                {
                }
                else
                {
                    DistributePoints();
                }
            }
        }
    }
}