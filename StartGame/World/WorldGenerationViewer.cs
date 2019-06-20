using StartGame.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace StartGame.World
{
    public partial class WorldGenerationViewer : Form
    {
        private WorldRenderer worldRenderer;
        Dictionary<string, List<int>> CityPopulation = new Dictionary<string, List<int>>();

        public WorldGenerationViewer()
        {
            worldRenderer = new WorldRenderer();
            InitializeComponent();
            InitialiseData();
            worldMapBox.MouseWheel += WorldMapBox_MouseWheel;
            Render();
        }

        int zoom = -16;
        private void WorldMapBox_MouseWheel(object sender, MouseEventArgs e)
        {
            //pre zoom snap to grid and center on mouse
            int cornerX = worldRenderer.Position.X / (20 + zoom);
            int mouseX = (e.X - worldMapBox.Width / 2) / (20 + zoom);
            int posX = cornerX + mouseX;
            int cornerY = worldRenderer.Position.Y / (20 + zoom);
            int mouseY = (e.Y - worldMapBox.Height / 2) / (20 + zoom);
            int posY = cornerY + mouseY;

            //Zoom in or out
            zoom += 2 * e.Delta / Math.Abs(e.Delta);
            zoom = zoom < -18 ? -18 : zoom;
            zoom = zoom > 30 ? 30 : zoom;

            worldRenderer.Position = new Point(posX * (20 + zoom), posY * (20 + zoom));
            worldRenderer.Position = worldRenderer.Position.Cut(0, (20 + zoom) * World.WORLD_SIZE - worldMapBox.Width, 0, (20 + zoom) * World.WORLD_SIZE - worldMapBox.Height);
            Render();
        }

        private void WorldGenerationViewer_Load(object sender, EventArgs e)
        {
        }

        private void Render()
        {
            int size = 1;
            worldMapBox.Image = worldRenderer.Render(800, 800, 20 + zoom);
            rainfallMapBox.Image = worldRenderer.DrawRainfallMap(size);
            temperatureMapBox.Image = worldRenderer.DrawTemperatureMap(size);
            heightMapBox.Image = worldRenderer.DrawHeightMap(size);
            rawHeightMapBox.Image = worldRenderer.DrawRawHeightMap(size);
            nationMapBox.Image = worldRenderer.DrawNationMap(size);
            mineralMapBox.Image = worldRenderer.DrawMineralMap(size);
            agriculturalMapBox.Image = worldRenderer.DrawAgriculturalMap(size);
            valueMapBox.Image = worldRenderer.DrawValueMap(size);
            temperatureMap.Image = worldRenderer.DrawAtmosphereTemperatureMap(size);
            pressureMap.Image = worldRenderer.DrawPressureMap(size);
            latWindMap.Image = worldRenderer.DrawLatWindMap(size);
            lonWindMap.Image = worldRenderer.DrawLonWindMap(size);
            dPressureMap.Image = worldRenderer.DrawDPMap(size);
            dTemperatureMap.Image = worldRenderer.DrawDTMap(size);
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
            worldRenderer = new WorldRenderer();
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

        private void RunMonth_Click(object sender, EventArgs e)
        {
            ProgressTime(new TimeSpan(31, 0, 0, 0, 0));
            Render();
        }

        private void RunYear_Click(object sender, EventArgs e)
        {
            ProgressTime(new TimeSpan(365, 0, 0, 0, 0));
            Render();
        }
        private void RunTenYears_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                ProgressTime(new TimeSpan(365, 0, 0, 0, 0));
            }
            Render();
        }

        private void RunDay_Click(object sender, EventArgs e)
        {
            ProgressTime(new TimeSpan(1, 0, 0, 0, 0));
            Render();
        }


        void ProgressTime(TimeSpan time)
        {
            for (int i = 0; i < time.TotalHours; i++)
            {
                World.Instance.ProgressTime(new TimeSpan(0, 1, 0, 0));
                if(World.Instance.time.DayOfYear % 7 == 0)
                {
                    AddDataPoints();
                }
            }    
        }

        void InitialiseData()
        {
            CityPopulation = new Dictionary<string, List<int>>();
            populationChart.Series.Clear();
            foreach (var city in World.Instance.nation.cities)
            {
                CityPopulation.Add(city.name, new List<int> { city.Population });
                populationChart.Series.Add(new Series(city.name) { ChartType = SeriesChartType.Line });
                populationChart.Series.FindByName(city.name).Points.Add(city.Population);
            }
        }

        void AddDataPoints()
        {
            foreach (var city in World.Instance.nation.cities)
            {
                CityPopulation[city.name].Add(city.Population);
                populationChart.Series.FindByName(city.name).Points.Add(city.Population);
            }
        }

        private void WorldMapBox_MouseUp(object sender, MouseEventArgs e)
        {
            Point delta = e.Location.Sub(mouseDownPosition);
            worldRenderer.Position = worldRenderer.Position.Sub(delta);
            worldRenderer.Position = Cut();
            Render();       

        }
        Point mouseDownPosition;
        private void WorldMapBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                mouseDownPosition = e.Location;
        }
        private Point Cut()
        {
            return worldRenderer.Position.Cut(0, (20 + zoom) * World.WORLD_SIZE - worldMapBox.Width, 0, (20 + zoom) * World.WORLD_SIZE - worldMapBox.Height);
        }

        private void Run6Min_Click(object sender, EventArgs e)
        {
            ProgressTime(new TimeSpan(0, 6, 0));
            Render();
        }
    }
}