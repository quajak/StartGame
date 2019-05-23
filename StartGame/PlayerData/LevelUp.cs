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

namespace StartGame
{
    partial class LevelUp : Form
    {
        private readonly Player player;

        private int StrengthUp = 0;
        private int AgilityUp = 0;
        private int EnduranceUp = 0;
        private int VitatlityUp = 0;
        private int WisdomUp = 0;
        private int IntelligenceUp = 0;

        private int points;

        public LevelUp(Player Player, int Points)
        {
            InitializeComponent();
            player = Player;
            playerName.Text = player.Name;
            points = Points;

            Render();
        }

        private void Render()
        {
            int? actualMaxHealth = player.troop.health.MaxValue();
            double actualActionPoints = player.actionPoints.MaxValue().Value;
            Defense actualDefense = player.troop.defense;
            Dodge actualDodge = player.troop.dodge;
            int actualMana = player.mana.MaxValue().Value;
            string strength = player.strength.ToString();
            string agility = player.agility.ToString();
            string endurance = player.endurance.ToString();
            string vitality = player.vitality.ToString();
            string wisdom = player.wisdom.ToString();
            string intelligence = player.intelligence.ToString();

            //Now simulate player change
            player.vitality.RawValue += VitatlityUp;
            player.endurance.RawValue += EnduranceUp;
            player.agility.RawValue += AgilityUp;
            player.wisdom.RawValue += WisdomUp;
            player.intelligence.RawValue += IntelligenceUp;
            player.strength.RawValue += StrengthUp;

            playerStrength.Text = $"{strength} ({player.strength.Value})";
            playerAgiltiy.Text = $"{agility} ({player.agility.Value})";
            playerEndurance.Text = $"{endurance} ({player.endurance.Value})";
            playerVitality.Text = $"{vitality} ({player.vitality.Value})";
            playerWisdom.Text = $"{wisdom} ({player.wisdom.Value})";
            playerIntelligence.Text = $"{intelligence} ({player.intelligence.Value})";

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

            playerMaxHealth.Text = $"{actualMaxHealth} ({player.troop.health.MaxValue().Value})";
            playerActionPoints.Text = $"{actualActionPoints} ({player.actionPoints.MaxValue().Value})";
            playerDefense.Text = $"{actualDefense} ({player.troop.defense.Value})";
            playerDodge.Text = $"{actualDodge} ({player.troop.dodge.Value})";
            playerMana.Text = $"{actualMana} ({player.mana.MaxValue().Value})";

            //Undo changes
            player.vitality.RawValue -= VitatlityUp;
            player.endurance.RawValue -= EnduranceUp;
            player.agility.RawValue -= AgilityUp;
            player.wisdom.RawValue -= WisdomUp;
            player.intelligence.RawValue -= IntelligenceUp;
            player.strength.RawValue -= StrengthUp;

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
            player.strength.RawValue += StrengthUp;
            player.agility.RawValue += AgilityUp;
            player.endurance.RawValue += EnduranceUp;
            player.vitality.RawValue += VitatlityUp;
            player.wisdom.RawValue += WisdomUp;
            player.intelligence.RawValue += IntelligenceUp;
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

        private void PlayerMaxHealth_Click(object sender, EventArgs e)
        {
        }
    }
}