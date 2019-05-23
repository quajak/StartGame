using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace StartGame.PlayerData
{
    public partial class PlayerCreator : Form
    {
        public CustomPlayer player;

        public PlayerCreator()
        {
            player = new CustomPlayer("Custom", "enemyScout",
                new Items.Weapon(5, Items.BaseAttackType.melee, Items.BaseDamageType.blunt, 1, "Hit", 1, false), 0);
            InitializeComponent();
            playerView1.Activate(player, null, false);
            ShowImage();
        }

        private void StrengthValue_ValueChanged(object sender, EventArgs e)
        {
            player.strength.RawValue = (int)strengthValue.Value;
            playerView1.Render();
        }

        private void IntelligenceValue_ValueChanged(object sender, EventArgs e)
        {
            player.intelligence.RawValue = (int)intelligenceValue.Value;
            playerView1.Render();
        }

        private void VitalityValue_ValueChanged(object sender, EventArgs e)
        {
            player.vitality.RawValue = (int)vitalityValue.Value;
            playerView1.Render();
        }

        private void EnduranceValue_ValueChanged(object sender, EventArgs e)
        {
            player.endurance.RawValue = (int)enduranceValue.Value;
            playerView1.Render();
        }

        private void AgilityValue_ValueChanged(object sender, EventArgs e)
        {
            player.agility.RawValue = (int)agilityValue.Value;
            playerView1.Render();
        }

        private void WisdomValue_ValueChanged(object sender, EventArgs e)
        {
            player.wisdom.RawValue = (int)wisdomValue.Value;
            playerView1.Render();
        }

        private void PlayerName_TextChanged(object sender, EventArgs e)
        {
            if (playerName.Text.Trim() != "")
            {
                player.Name = playerName.Text.Trim();
                player.troop.Name = playerName.Text.Trim();
                playerView1.Render();
            }
        }

        private void PlayerXP_ValueChanged(object sender, EventArgs e)
        {
            player.XP = (int)playerXP.Value;
            playerView1.Render();
        }

        private int imageIndex = 0;

        private readonly List<(Bitmap, string)> images = new List<(Bitmap, string)>() {
            (Resources.spiderWarrior, "spiderWarrior"),
            (Resources.enemyScout, "enemyScout"),
            (Resources.Slime, "Slime")
        };

        private void ImageNext_Click(object sender, EventArgs e)
        {
            imageIndex = (imageIndex + 1) % images.Count;
            ShowImage();
        }

        private void ImageLast_Click(object sender, EventArgs e)
        {
            imageIndex = Math.Abs((imageIndex - 1) % images.Count);
            ShowImage();
        }

        private void ShowImage()
        {
            imageSelected.Image = images[imageIndex].Item1;
            player.troop.Image = images[imageIndex].Item1;
            player.bitmap = images[imageIndex].Item2;
        }
    }
}