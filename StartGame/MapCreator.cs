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
        public const int fieldSize = 20;

        public MapCreator()
        {
            rng = new Random();
            seed = 0;
            InitializeComponent();
        }

        private void MapCreator_Load(object sender, EventArgs e)
        {
            map = new Map(gameBoard.Width / fieldSize, gameBoard.Height / fieldSize);
            Recalulate();
        }

        private void Recalulate()
        {
            map.SetupMap(0.1, seed, ((double)heightDifference.Value - 4) / 20, 1);
            UpdateMap();
            mapType.Text = map.Stats();
        }

        private void UpdateMap()
        {
            seedInput.Text = seed.ToString();
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, continentAlpha: (int)numericUpDown1.Value, showGoal: (int)goalChooser.Value, Debug: debug.Checked);
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

        private void GetData_Click(object sender, EventArgs e)
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

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            UpdateMap();
        }

        private void GoalChooser_ValueChanged(object sender, EventArgs e)
        {
            UpdateMap();
        }

        private void Recalculate_Click(object sender, EventArgs e)
        {
            Recalulate();
        }

        private void GameBoard_MouseClick(object sender, MouseEventArgs e)
        {
            pos.Text = $"{e.X / fieldSize} : {e.Y / fieldSize} - {map.map[e.X / fieldSize, e.Y / fieldSize].type.type}";
        }
    }
}