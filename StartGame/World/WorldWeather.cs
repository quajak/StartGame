using StartGame.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

//This file is used to do the weather calculations

namespace StartGame.World
{
    public class WeatherPoint
    {
        /// <summary>
        /// -180 -> 180 Longitude of point
        /// </summary>
        public float longitude;

        /// <summary>
        /// Hour must be increased when time runs, insolation depends on hour
        /// </summary>
        public float hour = 0;

        /// <summary>
        /// 0 -> 100 How much sunlight the place receives due to current time
        /// </summary>
        public float BaseSunlight
        {
            get
            {
                float offset = longitude / 180 * 12;
                double baseSunlight = Math.Sin((hour + offset) / 24 * 2 * Math.PI);
                return (float)(baseSunlight / 2f + 0.5f).Cut(0, 1) * 100;
            }
        }

        internal float baseMinimumHumditiy;

        // At -30°C no water in air, at 10°C 100% and at 40°C 200%?
        public float MinimumHumditiy => baseMinimumHumditiy * (temperature.Cut(-30, 200) + 30) / 40;

        public float movedHumidity = 0;
        public float humidity = 0;

        /// <summary>
        /// % of sky covered 0 -> 100
        /// </summary>
        public float CloudCover => (humidity - (baseMinimumHumditiy / 2) / baseMinimumHumditiy * 2).Cut(0, 100);

        /// <summary>
        /// East-west speed
        /// </summary>
        public float u;

        /// <summary>
        /// North-south speed
        /// </summary>
        public float v;

        /// <summary>
        /// Vertical speed
        /// </summary>
        public float w;

        public float windTemperatureChange;
        public float temperature;
        private readonly float latitude;
        public float dT;

        //public float geopotential;
        /// <summary>
        /// Movement due to Coriolis effect
        /// </summary>
        public float f;

        public float pressure;
        public float dP;

        /// <summary>
        /// Seems to be stationary in the pressure range
        /// </summary>
        public const float specificHeat = 1.006837f;

        public float groundHeatCapacity;

        float lastTemp;
        float outgoing = 0;
        //TODO: Take into account season and albedo
        //TODO: Optimise
        public float EnergyBalance
        {
            get
            {
                if (Height != 0)
                {
                    return 0;
                }
                if (!temperature.AlmostEqual(lastTemp, 0.2f))
                {
                    //Cloud cover can not decrease outgoing
                    insolation = CalculateInsolation();
                    outgoing = (float)(3e-9f * Math.Pow(150 + temperature, 4) * World.DX * World.DY);
                    lastTemp = temperature;
                }
                //Cloud cover can at maximum decrease insolation by 25%
                float diff = (insolation * (1 - CloudCover / 200) * BaseSunlight / 100) - outgoing;
                if (float.IsNegativeInfinity(diff))
                    throw new Exception();
                return diff;
            }
        }

        /// <summary>
        /// 0->1 depending on blocking of mountain
        /// </summary>
        public float mountain;

        internal float albedo;

        public int Height { get; }
        public int X { get; }
        public int Y { get; }

        private float insolation;
        internal float baseEvaporationRate;
        public float EvaporationRate => baseEvaporationRate * Math.Max(temperature - 10, 0) / 10;

        public float precipitation;
        internal float Resistance
        {
            get
            {
                float resistance = ((mountain - 0.3f) * 4).Cut(0,1); // 1 - (0.5f * ((float)Math.Pow(2, mountain) - 1) + 0.25f * (1 - mountain) - 0.2f);
                return resistance;
            }
        }

        /// <summary>
        /// Determines if a tile is currently raining
        /// </summary>
        public bool Raining = false;

        /// <summary>
        ///
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <param name="temperature"></param>
        /// <param name="height"></param>
        /// <param name="latitude">Between -180 and 180</param>
        public WeatherPoint(float u, float v, float w, float temperature, int height, float latitude, int x, int y, float mountain, float albedo, float EvaporationRate, float minimumHumdity, float longitude, float groundHeatCapacity)
        {
            this.u = u;
            this.v = v;
            this.w = w;
            this.temperature = temperature;
            Height = height;
            this.latitude = latitude;
            X = x;
            Y = y;
            //geopotential = (float)((decimal)6.673e-11 * (decimal)5.975e24 * 1 / (decimal)6.378e6 - 1 / (decimal)(6.37e6 + height * 1000));
            //if (geopotential < 0)
            //    throw new Exception();
            insolation = CalculateInsolation();
            f = CalculateCoriolisStrength();
            pressure = 1000 + temperature * 1.8f;
            if (pressure < 900)
                throw new Exception();
            if (pressure > 1400)
                throw new Exception();
            this.mountain = mountain;
            this.albedo = albedo;
            baseEvaporationRate = EvaporationRate;
            dP = 0;
            dT = 0;
            baseMinimumHumditiy = minimumHumdity;
            this.longitude = longitude;
            this.groundHeatCapacity = groundHeatCapacity;
        }

        public WeatherPoint(float u, float v, float w, float temperature, int height, float latitude, int x, int y,
            float mountain, float albedo, float EvaporationRate, float minimumHumdity, float longitude, float groundHeatCapacity,
            float pressure, float humidity, float hour)
        {
            this.u = u;
            this.v = v;
            this.w = w;
            this.temperature = temperature;
            Height = height;
            this.latitude = latitude;
            X = x;
            Y = y;
            //geopotential = (float)((decimal)6.673e-11 * (decimal)5.975e24 * 1 / (decimal)6.378e6 - 1 / (decimal)(6.37e6 + height * 1000));
            //if (geopotential < 0)
            //    throw new Exception();
            if (height == 0)
            {
                insolation = (-0.05f * latitude * latitude + 700) * 8 * (1 - albedo);
            }
            else
            {
                insolation = 0;
            }
            f = CalculateCoriolisStrength();
            this.pressure = pressure;
            this.humidity = humidity;
            this.hour = hour;
            this.mountain = mountain;
            this.albedo = albedo;
            baseEvaporationRate = EvaporationRate;
            dP = 0;
            dT = 0;
            baseMinimumHumditiy = minimumHumdity;
            this.longitude = longitude;
            this.groundHeatCapacity = groundHeatCapacity;
        }

        private float CalculateCoriolisStrength()
        {
            return (float)FastMath.Round(600 * World.atmosphereTimeStep * Math.Cos(latitude / 360 * Math.PI), 3);
        }

        float CalculateInsolation()
        {
            return Height == 0 ? (-1f * Math.Abs(latitude) + 400) * 8 * (1 - albedo) : 0;
        }

        public WeatherPoint()
        {
        }
    }

    public partial class World
    {
        /// <summary>
        /// Gas Constant
        /// </summary>
        public const float R = 8.31f;

        /// <summary>
        /// Seems to be nearly a constant
        /// </summary>
        public const float airDensity = 1.2015f;

        /// <summary>
        /// In hours
        /// </summary>
        public const float atmosphereTimeStep = 0.3f;

        //Weather variables
        public static int DX = 20; // Every box is 20 km wide

        public static int DY = 20; // Every box is 20 km up
        public static int DZ = 1; //Every box is 1 km up
        public static int RATIO = 1;
        public const int MaxZ = 1;
        private const int WindStrength = 40; // Determines how strong winds are due to differences in pressuree
        const int MaxWindStrength = 60;
        public static int THREADS = 5; //Adding more threads does not improve performance. Lower number of threads is better because there will be less overhead at higher ratios

        /// <summary>
        /// 3D array so index = z + x * 20 / DZ + y * WORLD_SIZE * 20
        /// </summary>
        public WeatherPoint[] atmosphere = new WeatherPoint[WORLD_SIZE / RATIO * WORLD_SIZE / RATIO * MaxZ / DZ];

        public Dictionary<WorldTileType, int> lost;
        public Dictionary<WorldTileType, int> gained;

        //double totalHumidity = 0;

        /// <summary>
        ///
        /// </summary>
        /// <param name="ratio"></param>
        public void InitialiseAtmosphere(int ratio = 1)
        {
            lost = new Dictionary<WorldTileType, int>();
            gained = new Dictionary<WorldTileType, int>();
            ResetBiomeChangeData();
            DateTime time = DateTime.Now;
            WeatherPoint[] oldAtmosphere = atmosphere;
            if (oldAtmosphere[0] is null)
            {
                atmosphere = new WeatherPoint[WORLD_SIZE / ratio * WORLD_SIZE / ratio * MaxZ / DZ];
                //Create a new atmosphere, x and y are coords in atmosphere not ground coords
                for (int x = 0; x < WORLD_SIZE / ratio; x++)
                {
                    for (int y = 0; y < WORLD_SIZE / ratio; y++)
                    {
                        for (int z = 0; z < MaxZ / DZ; z++)
                        {
                            //TODO: Get a correct temperature decrease with height
                            float evaporationRate = z == 0 ? (float)worldMap.GetAverage(x * ratio, (x + 1) * ratio, y * ratio, (y + 1) * ratio, t => GetEvaporationRate(t.type)) : 0;
                            float minimumHumdity = z == 0 ? (float)worldMap.GetAverage(x * ratio, (x + 1) * ratio, y * ratio, (y + 1) * ratio, t => GetMinimumHumdity(t.type)) : 0;
                            float groundHeatCapacity = z == 0 ? (float)worldMap.GetAverage(x * ratio, (x + 1) * ratio, y * ratio, (y + 1) * ratio, t => GetGroundHeatCapacity(t.type)) : 0;
                            float albedo = (float)worldMap.GetAverage(x * ratio, (x + 1) * ratio, y * ratio, (y + 1) * ratio, t => GetAlebdo(t.type));
                            float height = (float)heightMap.GetAverage(x * ratio, (x + 1) * ratio, y * ratio, (y + 1) * ratio, h => h < 0.43 ? 0.43 : h);
                            float latitude = (y * ratio - WORLD_SIZE / 2) / ((float)WORLD_SIZE / 2) * 180;
                            atmosphere[z + x * MaxZ + y * WORLD_SIZE / ratio * MaxZ] = new WeatherPoint(0, 0, 0, (float)temperatureMap[x, y] * 35 - z, z,
                                latitude, x * ratio, y * ratio, height, albedo,
                                evaporationRate, minimumHumdity, (x * ratio - WORLD_SIZE / 2f) / (WORLD_SIZE / 2f) * 180f, groundHeatCapacity);
                        }
                    }
                }
            }
            else
            {
                //Scale the data from the old one
                if (RATIO > ratio)
                {
                    //The old one was less precise
                    atmosphere = new WeatherPoint[WORLD_SIZE / ratio * WORLD_SIZE / ratio * MaxZ / DZ];
                    for (int x = 0; x < WORLD_SIZE / ratio; x++)
                    {
                        for (int y = 0; y < WORLD_SIZE / ratio; y++)
                        {
                            for (int z = 0; z < MaxZ / DZ; z++)
                            {
                                int index = z + x * ratio / RATIO * MaxZ + y * ratio / RATIO * WORLD_SIZE / RATIO * MaxZ;
                                WeatherPoint reference = oldAtmosphere[index];
                                float evaporationRate = reference.baseEvaporationRate;
                                float minimumHumdity = reference.baseMinimumHumditiy;
                                float groundHeatCapacity = reference.groundHeatCapacity;
                                float albedo = reference.albedo;
                                float height = reference.Height;
                                float temperature = reference.temperature;
                                float pressure = reference.pressure;
                                float humidity = reference.humidity;
                                float v = reference.v;
                                float w = reference.w;
                                float u = reference.u;
                                float hour = reference.hour;
                                atmosphere[z + x * MaxZ + y * WORLD_SIZE / ratio * MaxZ] = new WeatherPoint(u, v, w, temperature, z,
                                    (y * ratio - WORLD_SIZE / 2) / ((float)WORLD_SIZE) * 180, x * ratio, y * ratio, height, albedo,
                                    evaporationRate, minimumHumdity, (x * ratio - WORLD_SIZE / 2f) / (WORLD_SIZE / 2f) * 180f, groundHeatCapacity,
                                    pressure, humidity, hour);
                            }
                        }
                    }
                }
                else if (ratio > RATIO)
                {
                    //The new one is less precise
                    atmosphere = new WeatherPoint[WORLD_SIZE / ratio * WORLD_SIZE / ratio * MaxZ / DZ];
                    for (int x = 0; x < WORLD_SIZE / ratio; x++)
                    {
                        for (int y = 0; y < WORLD_SIZE / ratio; y++)
                        {
                            for (int z = 0; z < MaxZ / DZ; z++)
                            {
                                //Converts global co-ordinates to zoomed
                                int getIndex(int _x, int _y) => _x / RATIO * MaxZ + _y / RATIO * MaxZ * WORLD_SIZE / RATIO;
                                int sX = x * ratio; // These values are in global co-ordinates
                                int eX = Math.Min((x + 1) * ratio, WORLD_SIZE);
                                int sY = y * ratio;
                                int eY = Math.Min((y + 1) * ratio, WORLD_SIZE);
                                float evaporationRate = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.baseEvaporationRate, getIndex);
                                float minimumHumdity = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.baseMinimumHumditiy, getIndex);
                                float groundHeatCapacity = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.groundHeatCapacity, getIndex);
                                float albedo = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.albedo, getIndex);
                                float height = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.Height, getIndex);
                                float temperature = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.temperature, getIndex);
                                float pressure = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.pressure, getIndex);
                                float humidity = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.humidity, getIndex);
                                float v = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.v, getIndex);
                                float w = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.w, getIndex);
                                float u = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.u, getIndex);
                                float hour = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.hour, getIndex);
                                if (float.IsNaN(albedo))
                                    throw new Exception();
                                if (float.IsNaN(minimumHumdity) || minimumHumdity == 0)
                                    throw new Exception();
                                int index = z + x * MaxZ + y * WORLD_SIZE / ratio * MaxZ;
                                atmosphere[index] = new WeatherPoint(u, v, w, temperature, z,
                                    (y * ratio - WORLD_SIZE / 2) / ((float)WORLD_SIZE) * 180, x * ratio, y * ratio, height, albedo,
                                    evaporationRate, minimumHumdity, (x * ratio - WORLD_SIZE / 2f) / (WORLD_SIZE / 2f) * 180f, groundHeatCapacity,
                                    pressure, humidity, hour);
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            RATIO = ratio;
            Trace.TraceInformation($"Generated atmosphere in {(DateTime.Now - time).TotalMilliseconds} miliseconds");
        }

        private void ResetBiomeChangeData()
        {
            lost = new Dictionary<WorldTileType, int>();
            gained = new Dictionary<WorldTileType, int>();
            foreach (object item in Enum.GetValues(typeof(WorldTileType)))
            {
                lost.Add((WorldTileType)item, 0);
                gained.Add((WorldTileType)item, 0);
            }
        }

        internal WeatherPoint GetAtmosphereValue(int x, int y, int z)
        {
            return atmosphere[z + x / RATIO * MaxZ + y / RATIO * WORLD_SIZE / RATIO * MaxZ];
        }

        #region Biome Values

        public static float GetGroundHeatCapacity(WorldTileType type)
        {
            switch (type)
            {
                case WorldTileType.Ocean:
                    return 10;

                case WorldTileType.River:
                    return 10;

                case WorldTileType.TemperateGrassland:
                    return 6;

                case WorldTileType.Rainforest:
                    return 8;

                case WorldTileType.Desert:
                    return 2;

                case WorldTileType.Tundra:
                    return 3;

                case WorldTileType.TemperateForest:
                    return 7;

                case WorldTileType.Savanna:
                    return 5;

                case WorldTileType.Alpine:
                    return 5;

                case WorldTileType.SeaIce:
                    return 8;

                default:
                    throw new NotImplementedException();
            }
        }

        public static int GetMinimumHumdity(WorldTileType type)
        {
            switch (type)
            {
                case WorldTileType.Ocean:
                    return 300;

                case WorldTileType.River:
                    return 120;

                case WorldTileType.TemperateGrassland:
                    return 60;

                case WorldTileType.Rainforest:
                    return WindStrength;

                case WorldTileType.Desert:
                    return 98;

                case WorldTileType.Tundra:
                    return 50;

                case WorldTileType.TemperateForest:
                    return 55;

                case WorldTileType.Savanna:
                    return 70;

                case WorldTileType.Alpine:
                    return 60;

                case WorldTileType.SeaIce:
                    return 95;

                default:
                    throw new NotImplementedException();
            }
        }

        public static float GetEvaporationRate(WorldTileType type)
        {
            switch (type)
            {
                case WorldTileType.Ocean:
                    return 0.8f;

                case WorldTileType.River:
                    return 0.8f;

                case WorldTileType.TemperateGrassland:
                    return 0.3f;

                case WorldTileType.Rainforest:
                    return 0.5f;

                case WorldTileType.Desert:
                    return 0;

                case WorldTileType.Tundra:
                    return 0.2f;

                case WorldTileType.TemperateForest:
                    return 0.35f;

                case WorldTileType.Savanna:
                    return 0.1f;

                case WorldTileType.Alpine:
                    return 0.1f;

                case WorldTileType.SeaIce:
                    return 0.1f;

                default:
                    throw new NotImplementedException();
            }
        }

        public static float GetAlebdo(WorldTileType type)
        {
            switch (type)
            {
                case WorldTileType.Ocean:
                    return 0.05f;

                case WorldTileType.River:
                    return 0.05f;

                case WorldTileType.TemperateGrassland:
                    return 0.27f;

                case WorldTileType.Rainforest:
                    return 0.15f;

                case WorldTileType.Desert:
                    return 0f; // To increase insolation
                case WorldTileType.Tundra:
                    return 0.21f;

                case WorldTileType.TemperateForest:
                    return 0.23f;

                case WorldTileType.Savanna:
                    return 0.3f;

                case WorldTileType.Alpine:
                    return 0.4f;

                case WorldTileType.SeaIce:
                    return 0.4f;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion Biome Values

        #region OldCode

        //void CalculateAtmosphereTimeStep(bool debug = false)
        //{
        //    DateTime time = DateTime.Now;
        //    //We simply calculate each one from the values around them, so they will have old and new value used
        //    for (int x = 0; x < WORLD_SIZE; x++)
        //    {
        //        for (int y = 0; y < WORLD_SIZE; y++)
        //        {
        //            for (int z = 0; z < 20 / DZ; z++)
        //            {
        //                //See https://en.wikipedia.org/wiki/Primitive_equations
        //                int position = z + 20 * x + y * WORLD_SIZE * 20;
        //                WeatherPoint point = atmosphere[position];

        //                // du/dt -fv = -d geopotential / dx
        //                WeatherPoint right = atmosphere[z + 20 * (x + 1) + y + WORLD_SIZE * 20];
        //                WeatherPoint left = atmosphere[z + 20 * (x - 1) + y + WORLD_SIZE * 20];
        //                float dU = atmosphereTimeStep * point.f * (point.v + 0.001) - (right.geopotential - left.geopotential) / (2 * DX);
        //                point.u += dU;
        //                // dv/dt + fu = -d geopotential / dy
        //                WeatherPoint bottom = atmosphere[z + 20 * x + (y + 1) + WORLD_SIZE * 20];
        //                WeatherPoint top = atmosphere[z + 20 * x + (y - 1) + WORLD_SIZE * 20];
        //                float dV = atmosphereTimeStep * point.f * point.u - (bottom.geopotential - top.geopotential) / (2 * DX);
        //                point.v += dV;
        //                // p = air densiy * R * T
        //                float pressure = point.pressure;
        //                point.pressure = 1013 * Math.Exp(-0.02896 * 9.807 / (R * 288.15) * (273 + point.temperature)); //See https://www.math24.net/barometric-formula/;
        //                float dPressures = pressure - point.pressure;
        //                // d geopotential = d pressure * ( R * T / p)
        //                float dGeoPotential = dPressures * (R * point.temperature / point.pressure);
        //                point.geopotential += dGeoPotential;
        //                // d vertical = d pressure * ( -dU / dx -dV / dy)
        //                float dW = dPressures * (-dU / DX - dV / DY);
        //                point.w += dW;
        //                // dT (1 /dt + u/dx + v/dy + w/dp - w*R*T/(p*cp)) = J/cp
        //                // dT = J / (cp * (1 / dt + u/dx + v/dy +w /dp - w*R*T/(p * pc))
        //                // I guess J is simply energy balance
        //                float dT = point.EnergyBalance / (10 * WeatherPoint.specificHeat *
        //                    (1d / atmosphereTimeStep + point.u / dU + point.v / dV + point.w / dPressures - point.w * R * point.temperature / (point.pressure * WeatherPoint.specificHeat)));
        //                if (!float.IsNaN(dT))
        //                {
        //                    if (Math.Abs(dT) > 4)
        //                        point.temperature += Math.Sign(dT) * 4;
        //                    //throw new Exception();
        //                    point.temperature += dT;
        //                }
        //                else
        //                    point.temperature += 0.0001;
        //                if (point.temperature == 0 && point.pressure == 0 && point.u == 0 && point.v == 0 &&
        //                    point.geopotential == 0 && point.w == 0)
        //                    throw new Exception();
        //                if (float.IsNaN(point.temperature))
        //                    throw new Exception();
        //                if (float.IsNaN(point.pressure))
        //                    throw new Exception();
        //                if (float.IsNaN(point.u))
        //                    throw new Exception();
        //                if (float.IsNaN(point.v))
        //                    throw new Exception();
        //                if (float.IsNaN(point.geopotential))
        //                    throw new Exception();
        //                if (float.IsNaN(point.w))
        //                    throw new Exception();
        //                atmosphere[position] = point;
        //            }
        //        }
        //    }
        //    if (debug)
        //        Trace.TraceInformation($"Atmosphere calculation in {(DateTime.Now - time).TotalMilliseconds}");
        //}

        #endregion OldCode

        private void CalculateAtmosphereTimeStep(bool debug = false)
        {
            DateTime time = DateTime.Now;
            //CalculateWeather(0, WORLD_SIZE);
            Forker forker = new Forker();
            //totalHumidity = 0;
            //TODO: Move the boundaries between threads, so that artifacts are cleaned up
            for (int i = 0; i < THREADS; i++)
            {
                int startY = i * WORLD_SIZE / THREADS;
                int endY = (i + 1) * WORLD_SIZE / THREADS;
                forker.Fork(() => CalculateWeather(startY, endY));
            }
            forker.Join();
            //Trace.TraceInformation("Total humidity: " + totalHumidity);
            if (debug)
                Trace.TraceInformation($"Atmosphere calculation in {(DateTime.Now - time).TotalMilliseconds}");
        }

        private void CalculateWeather(int startY, int endY)
        {
            //Create a new random instance as the global one is not thread safe
            Random random = new Random(startY + endY);
            //We simply calculate each one from the values around them, so they will have old and new value used
            for (int x = 0; x < WORLD_SIZE / RATIO; x++)
            {
                for (int y = startY / RATIO; y < endY / RATIO; y++)
                {
                    for (int z = 0; z < MaxZ / DZ; z++)
                    {
                        int position = z + MaxZ * x + y * WORLD_SIZE / RATIO * MaxZ;
                        WeatherPoint point = atmosphere[position];
                        point.hour += atmosphereTimeStep;

                        // Horizontal wind
                        WeatherPoint right;
                        if (x == WORLD_SIZE / RATIO - 1)
                        {
                            right = atmosphere[z + y * WORLD_SIZE / RATIO * MaxZ];
                        }
                        else
                        {
                            right = atmosphere[z + MaxZ * (x + 1) + y * WORLD_SIZE / RATIO * MaxZ];
                        }
                        WeatherPoint left;
                        if (x == 0)
                        {
                            left = atmosphere[z + MaxZ * (WORLD_SIZE / RATIO - 1) + y * WORLD_SIZE / RATIO * MaxZ];
                        }
                        else
                        {
                            left = atmosphere[z + MaxZ * (x - 1) + y * WORLD_SIZE / RATIO * MaxZ];
                        }
                        float moveU = (point.u * atmosphereTimeStep * 5) / (DX * RATIO);
                        right.u += moveU / 2;
                        left.u += moveU / 2;
                        point.u -= moveU;
                        //float friction = point.u * 0.05 * atmosphereTimeStep * point.mountain;
                        //Go from high pressure to low pressure
                        float pressureDiff = -(1 - point.Resistance) * WindStrength * 10 * (right.pressure - left.pressure) / (DX * RATIO);
                        float coriolis = atmosphereTimeStep * point.f;
                        float dU = coriolis + pressureDiff;// - friction;
                        if (Math.Sign(dU) == Math.Sign(point.u))
                            dU = Math.Sign(dU) * Math.Max(2 * Math.Abs(dU) - Math.Abs(point.u), 0);
                        point.u += dU;
                        point.u = point.u.Cut(-MaxWindStrength, MaxWindStrength);

                        // Vertical wind
                        WeatherPoint bottom;
                        if (y != WORLD_SIZE / RATIO - 1)
                        {
                            bottom = atmosphere[z + MaxZ * x + (y + 1) * WORLD_SIZE / RATIO * MaxZ];
                        }
                        else
                        {
                            bottom = point;
                        }
                        WeatherPoint top;
                        if (y != 0)
                        {
                            top = atmosphere[z + MaxZ * x + (y - 1) * WORLD_SIZE / RATIO * MaxZ];
                        }
                        else
                        {
                            top = point;
                        }
                        float moveV = (point.v * atmosphereTimeStep * 20) / (DY * RATIO);
                        top.v += moveV / 4 * 3;
                        bottom.v += moveV / 4;
                        point.v -= moveV;
                        //float friction = point.v * 0.05f * atmosphereTimeStep * point.mountain;
                        float dV = -(1 - point.Resistance) * WindStrength * (top.pressure - bottom.pressure) / (DY * RATIO); // - friction;
                        if (Math.Sign(dV) == Math.Sign(point.v))
                            dV = Math.Sign(dV) * Math.Max(5 * Math.Abs(dV) - Math.Abs(point.v), 0);

                        point.v += dV;
                        point.v = point.v.Cut(-MaxWindStrength, MaxWindStrength);

                        // Pressure
                        float pressure = point.pressure;
                        point.pressure = 1000 + point.temperature * 1.8f + point.dT * 10;// + point.humidity / 10;
                        point.dP = pressure - point.pressure;

                        // Vertical wind - as long as MaxZ is 1 this is not important
                        // float dW = point.dP * (-dU / DX - dV / DY);
                        // point.w += dW;

                        // Temperature
                        //TODO: Calculate the effect of the wind
                        float radiation = point.EnergyBalance * atmosphereTimeStep / (40f * point.groundHeatCapacity);
                        float latitudinalTemp = 20 * atmosphereTimeStep * ((left.temperature + right.temperature) / 2 - point.temperature) / (DX * (RATIO / 2).Cut(1, 10));
                        float longitudinalTemp = 20 * atmosphereTimeStep * ((top.temperature + bottom.temperature) / 2 - point.temperature) / (DY * (RATIO / 2).Cut(1, 10));
                        WeatherPoint below;

                        if (z != 0)
                            below = atmosphere[(z - 1) + MaxZ * x + y * WORLD_SIZE / RATIO * MaxZ];
                        else
                            below = point;
                        WeatherPoint above;
                        if (z != MaxZ - 1)
                            above = atmosphere[(z + 1) + MaxZ * x + y * WORLD_SIZE / RATIO * MaxZ];
                        else
                            above = point;
                        float verticalTemp = atmosphereTimeStep * (below.temperature - above.temperature) / (2 * DZ);
                        point.dT = radiation + point.windTemperatureChange + latitudinalTemp + longitudinalTemp + verticalTemp;
                        point.windTemperatureChange = 0;

                        point.temperature += point.dT;
                        point.temperature = point.temperature.Cut(-100, 100);
                        point.temperature = (float)FastMath.Round(point.temperature, 3);

                        // Calculate humidity
                        // First add new
                        float dH = point.EvaporationRate * atmosphereTimeStep * (1 - point.CloudCover / 100) / 4;
                        point.humidity += dH;
                        //point.humidity = point.humidity.Cut(0, 100); //Maybe not necessary anymore
                        //Change in humidity also affects temperature
                        float dTfromdH = -dH / 10;
                        point.temperature += dTfromdH;
                        point.dT += dTfromdH;

                        //Movement due to wind
                        int xMovement = (int)point.u / 10;
                        int yMovement = (int)point.v / 10;
                        int gX = x + xMovement;
                        while (0 > gX)
                        {
                            gX += WORLD_SIZE / RATIO;
                        }
                        while (gX >= WORLD_SIZE / RATIO)
                        {
                            gX -= WORLD_SIZE / RATIO - 1;
                        }
                        int gY = y + yMovement;
                        while (0 > gY)
                        {
                            gY += WORLD_SIZE / RATIO;
                        }
                        while (gY >= WORLD_SIZE / RATIO)
                        {
                            gY -= WORLD_SIZE / RATIO - 1;
                        }
                        WeatherPoint moveTo = atmosphere[z + gX * MaxZ + gY * MaxZ * WORLD_SIZE / RATIO];

                        //Move humidity
                        float proportion = Math.Min(2, Math.Abs(atmosphereTimeStep * (Math.Abs(point.u) + Math.Abs(point.v)) / (DX * RATIO))) / 2f;
                        float changed = (float)FastMath.Round((point.humidity - point.movedHumidity - 1).Cut(0, 100) * proportion, 3);
                        point.movedHumidity = 0;
                        if (moveTo.humidity < point.humidity * 2)
                        {
                            point.humidity -= changed;
                            if (point.humidity < 0)
                                throw new Exception();
                            moveTo.humidity += changed;
                            moveTo.movedHumidity += changed;
                        }
                        //Move temperature
                        //TODO: Find way to incorporate wind speed
                        float diff = point.temperature - (moveTo.temperature + moveTo.windTemperatureChange);
                        moveTo.windTemperatureChange += diff * 0.05f;

                        //Now rain
                        point.precipitation = 0;
                        if (point.humidity > point.MinimumHumditiy)
                        {
                            if(random.Next(3) == 0)
                                point.Raining = true;
                        }
                        if (point.Raining)
                        {
                            float delta = random.Next(Math.Min(20, (int)point.humidity));
                            if (delta > 5)
                            {
                                point.precipitation += delta;
                                point.humidity -= delta / 2;
                                if (point.humidity < 0)
                                    throw new Exception();

                                for (int X = x * RATIO; X < ((x + 1) * RATIO).Cut(0, WORLD_SIZE - 1); X++)
                                {
                                    for (int Y = y * RATIO; Y < ((y + 1) * RATIO).Cut(0, WORLD_SIZE - 1); Y++)
                                    {
                                        if (IsLand(worldMap[X, Y].type))
                                            worldMap[X, Y].rock.WaterAmount += delta * 0.3;
                                    }
                                }

                                dTfromdH = -delta / 10;
                                point.temperature += dTfromdH;
                                point.dT += dTfromdH;
                            }
                            if (point.humidity < 10)
                                point.Raining = false;
                        }

                        var tile = worldMap[x, y];
                        tile.averageTemp = (tile.averageTemp * tile.measurements + point.temperature) / (tile.measurements + 1);
                        tile.measurements++;
                        //totalHumidity += point.humidity;
#if DEBUG
                        if (point.humidity < 0)
                            throw new Exception();
                        if (CheckValdidity(point.EnergyBalance))
                            throw new Exception();
                        if (CheckValdidity(point.temperature) || new Range(-150, 100).Excludes(point.temperature))
                            throw new Exception();
                        if (CheckValdidity(point.dT))
                            throw new Exception();
                        if (CheckValdidity(point.pressure))
                            throw new Exception();
                        if (CheckValdidity(point.dP))
                            throw new Exception();
                        if (CheckValdidity(point.humidity))
                            throw new Exception();
                        if (CheckValdidity(point.CloudCover))
                            throw new Exception();
                        if (CheckValdidity(point.u))
                            throw new Exception();
                        if (CheckValdidity(point.v))
                            throw new Exception();
                        if (CheckValdidity(point.w))
                        {
                            throw new Exception();
                        }
#endif
                    }
                }
            }
        }

#pragma warning disable IDE0051 // Remove unused private members
        private static bool CheckValdidity(float value)
#pragma warning restore IDE0051 // Remove unused private members
        {
            return float.IsNaN(value) || float.IsInfinity(value) || float.IsNegativeInfinity(value);
        }

        internal void ProgressTime(bool debug = false)
        {
            ProgressTime(new TimeSpan((int)atmosphereTimeStep, (int)(60 * atmosphereTimeStep), 0), debug);
        }
    }
}