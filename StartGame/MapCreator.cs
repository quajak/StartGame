using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

        public double Seed
        {
            get => seed; set
            {
                seed = value;
                if (seedInput != null)
                    seedInput.Text = seed.ToString();
            }
        }

        public MapCreator()
        {
            rng = new Random();
            Seed = 0;
            InitializeComponent();
        }

        private void MapCreator_Load(object sender, EventArgs e)
        {
            map = new Map(gameBoard.Width / fieldSize, gameBoard.Height / fieldSize);
            Recalulate(true);
        }

        private void Recalulate(bool allowSeedChange)
        {
            int times = 0;
            Thread thread;
            bool finished = false;
            do
            {
                if (times > 0)
                {
                    if (allowSeedChange)
                    {
                        Seed++;
                    }
                    else
                    {
                        if (MessageBox.Show("Unable to create map with current values. Create a map with different seed?", "Map creation error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            Seed++;
                            allowSeedChange = true;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                times++;
                double heightDif = (heightDifference.Value - 4) / 20d;
                thread = new Thread(() => map.SetupMap(new Tuple<double, double, double>(0.1, Seed, heightDif)));
                thread.Start();
                finished = thread.Join(Map.creationTime);
            } while (times < 10 && (!map.created || !finished));

            UpdateMap();
            mapType.Text = map.Stats();
        }

        private void UpdateMap()
        {
            seedInput.Text = Seed.ToString();
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, continentAlpha: (int)numericUpDown1.Value, showGoal: (int)goalChooser.Value, Debug: debug.Checked);
        }

        private void SeedInput_TextChanged(object sender, EventArgs e)
        {
            string text = seedInput.Text;
            string cleaned = Regex.Replace(text, "[^.0-9]", "");
            if (text != cleaned)
                seedInput.Text = cleaned;
            Seed = Int32.Parse(cleaned);
        }

        private void HeightDifference_Scroll(object sender, EventArgs e)
        {
            Recalulate(false);
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
                Seed = rng.Next();
                Recalulate(true);
                data[i] = map.RawStats();
            }

            System.IO.File.WriteAllLines("WriteLines.txt", data);
        }

        private void Randomise_Click(object sender, EventArgs e)
        {
            Seed = rng.Next();
            Recalulate(true);
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
            Recalulate(false);
        }

        private void GameBoard_MouseClick(object sender, MouseEventArgs e)
        {
            pos.Text = $"{e.X / fieldSize} : {e.Y / fieldSize} - {map.map[e.X / fieldSize, e.Y / fieldSize].type.type}";
        }
    }
}