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
    partial class MapCreator : Form
    {
        public Map map;
        private double seed;
        private Random rng;

        public MapCreator()
        {
            rng = new Random();
            seed = 0;
            InitializeComponent();
        }

        private void MapCreator_Load(object sender, EventArgs e)
        {
            map = new Map(gameBoard.Width / 10, gameBoard.Height / 10);
            Recalulate();
        }

        private void Recalulate()
        {
            map.SetupMap(0.1, seed, ((double)heightDifference.Value - 4) / 20, 1);
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, size: 10);
            mapType.Text = map.Stats();
        }

        private void SeedInput_TextChanged(object sender, EventArgs e)
        {
            seed = 0;
            foreach (char letter in seedInput.Text)
            {
                double numericValue = Char.GetNumericValue(letter);
                if (numericValue != -1)
                {
                    seed += numericValue;
                }
                else seed += Convert.ToInt32(letter);
            }
            Recalulate();
        }

        private void HeightDifference_Scroll(object sender, EventArgs e)
        {
            Recalulate();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Finished_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void getData_Click(object sender, EventArgs e)
        {
            int length = 1000;
            string[] data = new string[length];
            for (int i = 0; i < length; i++)
            {
                seed = rng.NextDouble() * 1000;
                Recalulate();
                data[i] = map.RawStats();
            }

            System.IO.File.WriteAllLines("WriteLines.txt", data);
        }

        private void Randomise_Click(object sender, EventArgs e)
        {
            seed = rng.NextDouble() * 1000;
            Recalulate();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, size: 10, continentAlpha: (int)numericUpDown1.Value);
        }

        private void goalChooser_ValueChanged(object sender, EventArgs e)
        {
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, size: 10, continentAlpha: (int)numericUpDown1.Value, showGoal: (int)goalChooser.Value);
        }
    }
}