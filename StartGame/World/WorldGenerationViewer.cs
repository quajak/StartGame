using StartGame.Rendering;
using System;
using System.Windows.Forms;

namespace StartGame.World
{
    public partial class WorldGenerationViewer : Form
    {
        private WorldRenderer worldRenderer;

        public WorldGenerationViewer()
        {
            worldRenderer = new WorldRenderer(World.Instance);
            InitializeComponent();
            Render();
        }

        private void WorldGenerationViewer_Load(object sender, EventArgs e)
        {
        }

        private void Render()
        {
            int size = 1;
            worldMapBox.Image = worldRenderer.Render(400, 400, size * 2);
            rainfallMapBox.Image = worldRenderer.DrawRainfallMap(size);
            temperatureMapBox.Image = worldRenderer.DrawTemperatureMap(size);
            heightMapBox.Image = worldRenderer.DrawHeightMap(size);
            rawHeightMapBox.Image = worldRenderer.DrawRawHeightMap(size);
            islandMapBox.Image = worldRenderer.DrawIslands(size);
            nationMapBox.Image = worldRenderer.DrawNationMap(size);
            mineralMapBox.Image = worldRenderer.DrawMineralMap(size);
            agriculturalMapBox.Image = worldRenderer.DrawAgriculturalMap(size);
            valueMapBox.Image = worldRenderer.DrawValueMap(size);
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            ReCalc();
        }

        private void Recalculate_Click(object sender, EventArgs e)
        {
            ReCalc();
        }

        private void ReCalc()
        {
            label5.Text = $"{trackBar1.Value} {trackBar2.Value} {trackBar3.Value} {trackBar5.Value} {trackBar4.Value}";
            World.NewWorld(trackBar1.Value / 100d, trackBar2.Value / 100d, trackBar3.Value / 100d, trackBar5.Value, trackBar4.Value / 100d);
            worldRenderer = new WorldRenderer(World.Instance);
            Render();
        }

        private void TrackBar3_Scroll(object sender, EventArgs e)
        {
            ReCalc();
        }

        private void TrackBar2_Scroll(object sender, EventArgs e)
        {
            ReCalc();
        }

        private void DesignateIsland_Click(object sender, EventArgs e)
        {
            World.Instance.DesignateIsland(true);
            Render();
        }

        private void TrackBar5_Scroll(object sender, EventArgs e)
        {
            ReCalc();
        }

        private void MineralMapBox_Click(object sender, EventArgs e)
        {

        }
    }
}