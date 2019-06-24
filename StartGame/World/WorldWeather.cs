using StartGame.Functions;
using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//This file is used to do the weather calculations

namespace StartGame.World
{
    public class WeatherPoint
    {
        /// <summary>
        /// -180 -> 180 Longitude of point
        /// </summary>
        public double longitude;
        /// <summary>
        /// Hour must be increased when time runs, insolation depends on hour
        /// </summary>
        public double hour = 0;
        /// <summary>
        /// 0 -> 100 How much sunlight the place receives due to current time
        /// </summary>
        public double BaseSunlight
        {
            get
            {
                double offset = longitude / 180 * 12;
                return (Math.Sin((hour + offset) / 24 * 2 * Math.PI) * 0.5 + 0.5).Cut(0, 1) * 100;
            }
        }

        /// <summary>
        /// 0 -> 100
        /// </summary>
        internal readonly double baseMinimumHumditiy;
        // At -30°C no water in air, at 10°C 100% and at 40°C 200%?
        public double MinimumHumditiy => baseMinimumHumditiy * (temperature.Cut(-30, 200) + 30) / 40;
        public double movedHumidity = 0;
        public double humidity = 0;
        /// <summary>
        /// % of sky covered 0 -> 100
        /// </summary>
        public double CloudCover => (humidity - (baseMinimumHumditiy / 2) / baseMinimumHumditiy * 2).Cut(0, 100);
        /// <summary>
        /// East-west speed 
        /// </summary>
        public double u;
        /// <summary>
        /// North-south speed
        /// </summary>
        public double v;
        /// <summary>
        /// Vertical speed
        /// </summary>
        public double w;
        public double temperature;
        public double dT;
        //public double geopotential;
        /// <summary>
        /// Movement due to Coriolis effect
        /// </summary>
        public double f;
        public double pressure;
        public double dP;
        /// <summary>
        /// Seems to be stationary in the pressure range
        /// </summary>
        public const double specificHeat = 1.006837;

        public double groundHeatCapacity;

        //TODO: Take into account season and albedo
        public double EnergyBalance
        {
            get
            {
                if (Height != 0)
                {
                    return 0;
                }
                //Cloud cover can not decrease outgoing
                double outgoing = (5.670367e-10 * Math.Pow(273 + temperature, 4) * World.DX * World.DY);
                //Cloud cover can at maximum decrease insolation by 50%
                double diff = (insolation * (1 - CloudCover / 200) * BaseSunlight / 100) - outgoing;
                if (double.IsNegativeInfinity(diff))
                    throw new Exception();
                return diff;
            }
        }

        /// <summary>
        /// 0->1 depending on blocking of mountain
        /// </summary>
        public double mountain;
        internal readonly double albedo;

        public int Height { get; }
        public int X { get; }
        public int Y { get; }

        readonly double insolation;
        internal readonly double baseEvaporationRate;
        public double EvaporationRate => baseEvaporationRate * Math.Max(temperature - 10, 0) / 10;

        public double precipitation;
        internal double Resistance => 1 - (0.5 * (Math.Pow(2, mountain) - 1) + 0.25 * (1 - mountain) - 0.2);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <param name="temperature"></param>
        /// <param name="height"></param>
        /// <param name="latitude">Between -90 and 90</param>
        public WeatherPoint(double u, double v, double w, double temperature, int height, double latitude, int x, int y, double mountain, double albedo, double EvaporationRate, double minimumHumdity, double longitude, double groundHeatCapacity)
        {
            this.u = u;
            this.v = v;
            this.w = w;
            this.temperature = temperature;
            Height = height;
            X = x;
            Y = y;
            //geopotential = (double)((decimal)6.673e-11 * (decimal)5.975e24 * 1 / (decimal)6.378e6 - 1 / (decimal)(6.37e6 + height * 1000));
            //if (geopotential < 0)
            //    throw new Exception();
            if (height == 0)
            {
                insolation = (-0.04 * latitude * latitude + 500) * 8 * (1 - albedo);
            }
            else
            {
                insolation = 0;
            }
            f = CalculateCoriolisStrength(latitude);
            pressure = 1013 * Math.Exp(-0.02896 * 9.807 / (World.R * 288.15) * (273 + temperature)); //See https://www.math24.net/barometric-formula/
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

        public WeatherPoint(double u, double v, double w, double temperature, int height, double latitude, int x, int y,
            double mountain, double albedo, double EvaporationRate, double minimumHumdity, double longitude, double groundHeatCapacity,
            double pressure, double humidity, double hour)
        {
            this.u = u;
            this.v = v;
            this.w = w;
            this.temperature = temperature;
            Height = height;
            X = x;
            Y = y;
            //geopotential = (double)((decimal)6.673e-11 * (decimal)5.975e24 * 1 / (decimal)6.378e6 - 1 / (decimal)(6.37e6 + height * 1000));
            //if (geopotential < 0)
            //    throw new Exception();
            if (height == 0)
            {
                insolation = (-0.05 * latitude * latitude + 700) * 8 * (1 - albedo);
            }
            else
            {
                insolation = 0;
            }
            f = CalculateCoriolisStrength(latitude);
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

        private static double CalculateCoriolisStrength(double latitude)
        {
            return 100 * World.atmosphereTimeStep * Math.Sin(latitude / 180d * 2 * Math.PI);
        }

        public WeatherPoint() { }
    }

    public partial class World
    {
        /// <summary>
        /// Gas Constant
        /// </summary>
        public const double R = 8.31;

        /// <summary>
        /// Seems to be nearly a constant
        /// </summary>
        public const double airDensity = 1.2015;


        /// <summary>
        /// In hours
        /// </summary>
        public const double atmosphereTimeStep = 0.2;
        //Weather variables
        public static int DX = 20; // Every box is 20 km wide
        public static int DY = 20; // Every box is 20 km up
        public static int DZ = 1; //Every box is 1 km up
        public static int RATIO = 1;
        public const int MaxZ = 1;
        private const int WindStrength = 20; // Determines how strong winds are due to differences in pressuree

        /// <summary>
        /// 3D array so index = z + x * 20 / DZ + y * WORLD_SIZE * 20
        /// </summary>
        public WeatherPoint[] atmosphere = new WeatherPoint[WORLD_SIZE / RATIO * WORLD_SIZE / RATIO * MaxZ / DZ];
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ratio"></param>
        public void InitialiseAtmosphere(int ratio = 1)
        {
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
                            double evaporationRate = z == 0 ? worldMap.GetAverage(x * ratio, (x + 1) * ratio, y * ratio, (y + 1) * ratio, t => GetEvaporationRate(t.type)) : 0;
                            double minimumHumdity = z == 0 ? worldMap.GetAverage(x * ratio, (x + 1) * ratio, y * ratio, (y + 1) * ratio, t => GetMinimumHumdity(t.type)) : 0;
                            double groundHeatCapacity = z == 0 ? worldMap.GetAverage(x * ratio, (x + 1) * ratio, y * ratio, (y + 1) * ratio, t => GetGroundHeatCapacity(t.type)) : 0;
                            double albedo = worldMap.GetAverage(x * ratio, (x + 1) * ratio, y * ratio, (y + 1) * ratio, t => GetAlebdo(t.type));
                            double height = heightMap.GetAverage(x * ratio, (x + 1) * ratio, y * ratio, (y + 1) * ratio, h => h < 0.43 ? 0.43 : h);
                            atmosphere[z + x * MaxZ + y * WORLD_SIZE / ratio * MaxZ] = new WeatherPoint(0, 0, 0, temperatureMap[x, y] * 35 - z, z,
                                (y * ratio - WORLD_SIZE / 2) / ((double)WORLD_SIZE) * 180, x * ratio, y * ratio, height, albedo,
                                evaporationRate, minimumHumdity, (x * ratio - WORLD_SIZE / 2d) / (WORLD_SIZE / 2d) * 180d, groundHeatCapacity);
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
                                double evaporationRate = reference.baseEvaporationRate;
                                double minimumHumdity = reference.baseMinimumHumditiy;
                                double groundHeatCapacity = reference.groundHeatCapacity;
                                double albedo = reference.albedo;
                                double height = reference.Height;
                                double temperature = reference.temperature;
                                double pressure = reference.pressure;
                                double humidity = reference.humidity;
                                double v = reference.v;
                                double w = reference.w;
                                double u = reference.u;
                                double hour = reference.hour;
                                atmosphere[z + x * MaxZ + y * WORLD_SIZE / ratio * MaxZ] = new WeatherPoint(u, v, w, temperature, z,
                                    (y * ratio - WORLD_SIZE / 2) / ((double)WORLD_SIZE) * 180, x * ratio, y * ratio, height, albedo,
                                    evaporationRate, minimumHumdity, (x * ratio - WORLD_SIZE / 2d) / (WORLD_SIZE / 2d) * 180d, groundHeatCapacity,
                                    pressure, humidity, hour);
                            }
                        }
                    }
                }
                else
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
                                double evaporationRate = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.baseEvaporationRate, getIndex);
                                double minimumHumdity = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.baseMinimumHumditiy, getIndex);
                                double groundHeatCapacity = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.groundHeatCapacity, getIndex);
                                double albedo = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.albedo, getIndex);
                                double height = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.Height, getIndex);
                                double temperature = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.temperature, getIndex);
                                double pressure = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.pressure, getIndex);
                                double humidity = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.humidity, getIndex);
                                double v = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.v, getIndex);
                                double w = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.w, getIndex);
                                double u = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.u, getIndex);
                                double hour = oldAtmosphere.GetAverage(sX, eX, sY, eY, p => p.hour, getIndex);
                                if (double.IsNaN(albedo))
                                    throw new Exception();
                                if (double.IsNaN(minimumHumdity) || minimumHumdity == 0)
                                    throw new Exception();
                                int index = z + x * MaxZ + y * WORLD_SIZE / ratio * MaxZ;
                                atmosphere[index] = new WeatherPoint(u, v, w, temperature, z,
                                    (y * ratio - WORLD_SIZE / 2) / ((double)WORLD_SIZE) * 180, x * ratio, y * ratio, height, albedo,
                                    evaporationRate, minimumHumdity, (x * ratio - WORLD_SIZE / 2d) / (WORLD_SIZE / 2d) * 180d, groundHeatCapacity,
                                    pressure, humidity, hour);
                            }
                        }
                    }
                }
            }
            RATIO = ratio;
            Trace.TraceInformation($"Generated atmosphere in {(time - DateTime.Now).TotalMilliseconds} miliseconds");
        }

        internal WeatherPoint GetAtmosphereValue(int x, int y, int z)
        {
            return atmosphere[z + x / RATIO * MaxZ * y / RATIO * WORLD_SIZE * MaxZ];
        }

        #region Biome Values
        private double GetGroundHeatCapacity(WorldTileType type)
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
                    return 1;
                default:
                    throw new NotImplementedException();
            }
        }

        private int GetMinimumHumdity(WorldTileType type)
        {
            switch (type)
            {
                case WorldTileType.Ocean:
                    return 120;
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

        private double GetEvaporationRate(WorldTileType type)
        {
            switch (type)
            {
                case WorldTileType.Ocean:
                    return 0.8;
                case WorldTileType.River:
                    return 0.8;
                case WorldTileType.TemperateGrassland:
                    return 0.3;
                case WorldTileType.Rainforest:
                    return 0.5;
                case WorldTileType.Desert:
                    return 0;
                case WorldTileType.Tundra:
                    return 0.2;
                case WorldTileType.TemperateForest:
                    return 0.35;
                case WorldTileType.Savanna:
                    return 0.1;
                case WorldTileType.Alpine:
                    return 0.1;
                case WorldTileType.SeaIce:
                    return 0.1;
                default:
                    throw new NotImplementedException();
            }
        }
        private double GetAlebdo(WorldTileType type)
        {
            switch (type)
            {
                case WorldTileType.Ocean:
                    return 0.05;
                case WorldTileType.River:
                    return 0.05;
                case WorldTileType.TemperateGrassland:
                    return 0.27d;
                case WorldTileType.Rainforest:
                    return 0.15;
                case WorldTileType.Desert:
                    return 0d; // To increase insolation
                case WorldTileType.Tundra:
                    return 0.21;
                case WorldTileType.TemperateForest:
                    return 0.23;
                case WorldTileType.Savanna:
                    return 0.3d;
                case WorldTileType.Alpine:
                    return 0.4;
                case WorldTileType.SeaIce:
                    return 0.4;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

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
        //                double dU = atmosphereTimeStep * point.f * (point.v + 0.001) - (right.geopotential - left.geopotential) / (2 * DX);
        //                point.u += dU;
        //                // dv/dt + fu = -d geopotential / dy
        //                WeatherPoint bottom = atmosphere[z + 20 * x + (y + 1) + WORLD_SIZE * 20];
        //                WeatherPoint top = atmosphere[z + 20 * x + (y - 1) + WORLD_SIZE * 20];
        //                double dV = atmosphereTimeStep * point.f * point.u - (bottom.geopotential - top.geopotential) / (2 * DX);
        //                point.v += dV;
        //                // p = air densiy * R * T
        //                double pressure = point.pressure;
        //                point.pressure = 1013 * Math.Exp(-0.02896 * 9.807 / (R * 288.15) * (273 + point.temperature)); //See https://www.math24.net/barometric-formula/;
        //                double dPressures = pressure - point.pressure;
        //                // d geopotential = d pressure * ( R * T / p)
        //                double dGeoPotential = dPressures * (R * point.temperature / point.pressure);
        //                point.geopotential += dGeoPotential;
        //                // d vertical = d pressure * ( -dU / dx -dV / dy)
        //                double dW = dPressures * (-dU / DX - dV / DY);
        //                point.w += dW;
        //                // dT (1 /dt + u/dx + v/dy + w/dp - w*R*T/(p*cp)) = J/cp
        //                // dT = J / (cp * (1 / dt + u/dx + v/dy +w /dp - w*R*T/(p * pc))
        //                // I guess J is simply energy balance
        //                double dT = point.EnergyBalance / (10 * WeatherPoint.specificHeat *
        //                    (1d / atmosphereTimeStep + point.u / dU + point.v / dV + point.w / dPressures - point.w * R * point.temperature / (point.pressure * WeatherPoint.specificHeat)));
        //                if (!double.IsNaN(dT))
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
        //                if (double.IsNaN(point.temperature))
        //                    throw new Exception();
        //                if (double.IsNaN(point.pressure))
        //                    throw new Exception();
        //                if (double.IsNaN(point.u))
        //                    throw new Exception();
        //                if (double.IsNaN(point.v))
        //                    throw new Exception();
        //                if (double.IsNaN(point.geopotential))
        //                    throw new Exception();
        //                if (double.IsNaN(point.w))
        //                    throw new Exception();
        //                atmosphere[position] = point;
        //            }
        //        }
        //    }
        //    if (debug)
        //        Trace.TraceInformation($"Atmosphere calculation in {(DateTime.Now - time).TotalMilliseconds}");
        //}
        #endregion
        void CalculateAtmosphereTimeStep(bool debug = false)
        {
            DateTime time = DateTime.Now;
            //CalculateWeather(0, WORLD_SIZE);
            Forker forker = new Forker();
            //TODO: Move the boundaries between threads, so that artifacts are cleaned up
            for (int i = 0; i < 10; i++)
            {
                int startY = i * WORLD_SIZE / 10;
                int endY = (i + 1) * WORLD_SIZE / 10;
                forker.Fork(() => CalculateWeather(startY, endY));
            }
            forker.Join();
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
                        //See https://en.wikipedia.org/wiki/Primitive_equations
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
                        double moveU = (point.u * atmosphereTimeStep * 5) / (DX * RATIO);
                        right.u += moveU / 2;
                        left.u += moveU / 2;
                        point.u -= moveU;
                        //double friction = point.u * 0.05 * atmosphereTimeStep * point.mountain;
                        //Go from high pressure to low pressure
                        double pressureDiff = -point.Resistance * 2 * WindStrength * (right.pressure - left.pressure) / (DX * RATIO);
                        double coriolis = -atmosphereTimeStep * point.f;
                        double dU = coriolis + pressureDiff;// - friction;
                        if (Math.Sign(dU) == Math.Sign(point.u))
                            dU = Math.Sign(dU) * Math.Max(10 * Math.Abs(dU) - Math.Abs(point.u), 0);
                        point.u += dU;
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
                        double moveV = (point.v * atmosphereTimeStep * 5) / (DY * RATIO);
                        top.v += moveV / 2;
                        bottom.v += moveV / 2;
                        point.v -= moveV;
                        //friction = point.v * 0.05 * atmosphereTimeStep * point.mountain;
                        double dV = -point.Resistance * WindStrength / 2 * (top.pressure - bottom.pressure) / (DY * RATIO);// - friction;
                        if (Math.Sign(dV) == Math.Sign(point.v))
                            dV = Math.Sign(dV) * Math.Max(10 * Math.Abs(dV) - Math.Abs(point.v), 0);
                        point.v += dV;

                        // Pressure
                        double pressure = point.pressure;
                        point.pressure = 1013 * Math.Exp(4 * (9.807 + point.temperature) / (R * (273 + point.temperature))) + point.humidity / 15; //See https://www.math24.net/barometric-formula/;
                        point.dP = pressure - point.pressure;
                        // Vertical wind - as long as MaxZ is 1 this is not important
                        // double dW = point.dP * (-dU / DX - dV / DY);
                        // point.w += dW;

                        // Temperature
                        double radiation = point.EnergyBalance * atmosphereTimeStep / (300d * point.groundHeatCapacity);
                        double latitudinalTemp = 10 * ((left.temperature + right.temperature) / 2 - point.temperature) / (DX * (RATIO/2).Cut(1,10));
                        double longitudinalTemp = 10 * ((top.temperature + bottom.temperature) / 2 - point.temperature) / (DY * (RATIO/2).Cut(1,10));
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
                        double verticalTemp = atmosphereTimeStep * (below.temperature - above.temperature) / (2 * DZ);
                        point.dT = radiation + latitudinalTemp + longitudinalTemp + verticalTemp;

                        point.temperature += point.dT;
                        point.temperature = Math.Round(point.temperature, 3);

                        // Calculate humidity
                        // First add new
                        double dH = point.EvaporationRate * atmosphereTimeStep * (1 - point.CloudCover / 100);
                        dH = random.NextDouble() * dH;
                        point.humidity += dH;
                        //point.humidity = point.humidity.Cut(0, 100); //Maybe not necessary anymore
                        //Change in humidity also affects temperature
                        double dTfromdH = -dH / 10;
                        point.temperature += dTfromdH;
                        point.dT += dTfromdH;
                        //Now move it
                        //at speed < 2 all is moved over
                        //to make it more likely for larger clouds to grow, prevent small humidity to go into large
                        if (x == 118 && y == 98)
                        {

                            //throw new Exception();
                        }
                        int xMovement = Math.Abs(point.u) > 20 ? (point.u > 0 ? 1 : -1) : 0;
                        int yMovement = Math.Abs(point.v) > 20 ? (point.v > 0 ? -1 : 1) : 0;
                        int gX = x + xMovement;
                        gX = -1 == gX ? WORLD_SIZE / RATIO - 1 : gX;
                        gX = WORLD_SIZE / RATIO == gX ? 1 : gX;
                        int gY = y + yMovement;
                        gY = WORLD_SIZE / RATIO == gY ? 1 : gY;
                        gY = -1 == gY ? WORLD_SIZE / RATIO - 1 : gY;
                        WeatherPoint moveTo = atmosphere[z + gX * MaxZ + gY * MaxZ * WORLD_SIZE / RATIO];
                        double proportion = Math.Min(2, Math.Abs(atmosphereTimeStep * point.u * 10 / (DX * RATIO)) + Math.Abs(atmosphereTimeStep * point.v * 10/ (DY * RATIO))) / 8d;
                        double changed = Math.Round((point.humidity - point.movedHumidity).Cut(0, 100) * proportion, 3);
                        if (moveTo.humidity < point.humidity * 2)
                        {
                            point.movedHumidity = 0;
                            point.humidity -= changed;
                            moveTo.humidity += changed;
                            moveTo.movedHumidity += changed;
                        }

                        //Now rain
                        point.precipitation = 0;
                        if (point.humidity > point.MinimumHumditiy)
                        {
                            double v = random.NextDouble();
                            double delta = Math.Floor(v * (point.humidity - (point.MinimumHumditiy - 20).Cut(0, 200)));
                            if (delta > 5)
                            {
                                point.precipitation += delta;
                                point.humidity -= delta;
                                if (point.humidity < 0)
                                    throw new Exception();
                                dTfromdH = -delta / 10;
                                point.temperature += dTfromdH;
                                point.dT += dTfromdH;
                            }
                        }
                        //point.humidity = Math.Round(point.humidity, 1);
                        if (point.humidity < 0)
                            throw new Exception();
                        if (CheckValdidity(point.EnergyBalance))
                            throw new Exception();
                        if (CheckValdidity(point.temperature) || new Range(-100, 100).Excludes(point.temperature))
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
                            throw new Exception();
                        atmosphere[position] = point;
                    }
                }
            }
        }

        private static bool CheckValdidity(double value)
        {
            return double.IsNaN(value) || double.IsInfinity(value) || double.IsNegativeInfinity(value);
        }
    }
}
