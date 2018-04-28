using StartGame;
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

            strengthUp.Enabled = points != 0;
            agilityUp.Enabled = points != 0;
            enduranceUp.Enabled = points != 0;
            vitalityUp.Enabled = points != 0;

            strengthDown.Enabled = StrengthUp != 0;
            agilityDown.Enabled = AgilityUp != 0;
            enduranceDown.Enabled = EnduranceUp != 0;
            vitalityDown.Enabled = VitatlityUp != 0;

            playerMaxHealth.Text = $"Max Health: {player.troop.maxHealth} ({2 * (VitatlityUp + player.vitality)})";
            playerActionPoints.Text = $"Action Points: {player.maxActionPoints} ({4 + (player.endurance + EnduranceUp) / 10})";
            playerDefense.Text = $"Defense: {player.troop.defense} ({player.endurance + EnduranceUp})";
            playerDodge.Text = $"Dodge: {player.troop.dodge} ({player.troop.baseDodge + (player.agility + AgilityUp) * 2})";
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
            player.CalculateStats();
            Close();
        }
    }
}