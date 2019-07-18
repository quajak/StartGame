using StartGame.PlayerData;
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
            atmosphereRatio.Items.AddRange(new object[] { 1, 2, 4, 5, 8 });
            atmosphereRatio.SelectedIndexChanged += AtmosphereRatio_SelectedIndexChanged;
            InitialiseData();
            worldMapBox.MouseWheel += WorldMapBox_MouseWheel;
            World.Instance.TimeChange += (o, e) => {
                if (World.Instance.time.DayOfYear % 7 == 0 && World.Instance.time.Hour == 0)
                {
                    AddDataPoints();
                }
            };
            Render();
        }

        private void AtmosphereRatio_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = (int)atmosphereRatio.SelectedItem;
            World.Instance.InitialiseAtmosphere(selected);
            Render();
        }

        int zoom = -16;
        private void WorldMapBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta == 0)
                return;
            //pre zoom snap to grid and center on mouse
            int cornerX = worldRenderer.Position.X / (20 + zoom);
            int mouseX = (e.X - worldMapBox.Width / 2) / (20 + zoom);
            int posX = (cornerX + mouseX).Cut(0, worldMapBox.Width);
            int cornerY = worldRenderer.Position.Y / (20 + zoom);
            int mouseY = (e.Y - worldMapBox.Height / 2) / (20 + zoom);
            int posY = (cornerY + mouseY).Cut(0, worldMapBox.Height);

            //Zoom in or out
            zoom += 2 * e.Delta / Math.Abs(e.Delta);
            zoom = zoom < -16 ? -16 : zoom;
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
            worldMapBox.Image = worldRenderer.Render(800, 800, 20 + zoom, showTime.Checked);
            rainfallMapBox.Image = worldRenderer.DrawRainfallMap(size);
            humidityMapBox.Image = worldRenderer.DrawHumidityMap(size);
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
            radiationMap.Image = worldRenderer.DrawRadiationMap(size);
            waterMap.Image = worldRenderer.DrawWaterMap(size);
            waterkeptMap.Image = worldRenderer.DrawWaterkeptMap(size);
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
            ProgressTime(new TimeSpan(10 * 365, 0, 0, 0));
            Render();
        }

        private void RunDay_Click(object sender, EventArgs e)
        {
            ProgressTime(new TimeSpan(1, 0, 0, 0, 0), false);
            Render();
        }


        void ProgressTime(TimeSpan time, bool allowScale = false)
        {
            World.Instance.ProgressTime(time, false, allowScale);  
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
            if (delta.Mag() < 5)
                return;
            worldRenderer.Position = worldRenderer.Position.Sub(delta);
            worldRenderer.Position = Cut();
            Render();       

        }
        Point mouseDownPosition;
        private void WorldMapBox_MouseDown(object sender, MouseEventArgs e)
        {
            int x = (worldRenderer.Position.X + e.X)/(20 + zoom);
            int y = (worldRenderer.Position.Y + e.Y)/(20 + zoom);
            WeatherPoint wP = World.Instance.atmosphere[x * World.MaxZ / World.RATIO + y / World.RATIO * World.MaxZ * World.WORLD_SIZE / World.RATIO];
            WorldTile point = World.Instance.worldMap[x, y];
            pointInformation.Text = $"Co-ords: {x} {y} Temp: {wP.temperature} Pressure: {wP.pressure} Humidity: {wP.humidity} Lon Wind {wP.v} Lat Wind {wP.u} " +
                $"Height {point.height} Water: {point.rock.WaterAmount} Average Temp: {point.averageTemp} Landwater {point.landWater}";
            if (e.Button == MouseButtons.Left)
                mouseDownPosition = e.Location;
        }
        private Point Cut()
        {
            return worldRenderer.Position.Cut(0, (20 + zoom) * World.WORLD_SIZE - worldMapBox.Width, 0, (20 + zoom) * World.WORLD_SIZE - worldMapBox.Height);
        }

        private void Run2Hour_Click(object sender, EventArgs e)
        {
            ProgressTime(new TimeSpan(2, 0, 0), false);
            Render();
        }

        private void RunSingleStep_Click(object sender, EventArgs e)
        {
            ProgressTime(new TimeSpan((int)World.atmosphereTimeStep, (int)(World.atmosphereTimeStep * 60) - (60 * (int)World.atmosphereTimeStep) , 0), false);
            Render();
        }
    }
}