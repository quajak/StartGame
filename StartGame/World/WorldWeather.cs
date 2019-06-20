using StartGame.Functions;
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
        public double geopotential;
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

        //TODO: Take into account season and albedo
        public double EnergyBalance
        {
            get
            {
                if (Height != 0)
                {
                    return 0;
                }
                double outgoing = (5.670367e-10 * Math.Pow(273 + temperature, 4) * World.DX * World.DY);
                return income - outgoing;
            }
        }

        /// <summary>
        /// 0->1 depending on blocking of mountain
        /// </summary>
        public double mountain;

        public int Height { get; }
        public int X { get; }
        public int Y { get; }

        double income;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        /// <param name="temperature"></param>
        /// <param name="height"></param>
        /// <param name="latitude">Between -90 and 90</param>
        public WeatherPoint(double u, double v, double w, double temperature, int height, double latitude, int x, int y, double mountain, double albedo)
        {
            this.u = u;
            this.v = v;
            this.w = w;
            this.temperature = temperature;
            Height = height;
            X = x;
            Y = y;
            geopotential = (double)((decimal)6.673e-11 * (decimal)5.975e24 * 1 / (decimal)6.378e6 - 1 / (decimal)(6.37e6 + height * 1000));
            if (geopotential < 0)
                throw new Exception();
            if (height == 0)
            {
                income = (-0.03 * latitude * latitude + 400) * 6 * (1 - albedo);
            }
            else
            {
                income = 0;
            }
            f = 2 * 2 * Math.PI / 24 * World.atmosphereTimeStep * Math.Sin(latitude / 180d);
            pressure = 1013 * Math.Exp(-0.02896 * 9.807 / (World.R * 288.15) * (273 + temperature));//See https://www.math24.net/barometric-formula/
            if (pressure < 900)
                throw new Exception();
            if (pressure > 1400)
                throw new Exception();
            this.mountain = mountain;
            dP = 0;
            dT = 0;
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
        public const double atmosphereTimeStep = 2;
        //Weather variables
        public const int DX = 20; // Every box is 20 km wide
        public const int DY = 20; // Every box is 20 km up
        public const int DZ = 1; //Every box is 1 km up
        public const int MaxZ = 1;

        /// <summary>
        /// 3D array so index = z + x * 20 / DZ + y * WORLD_SIZE * 20
        /// </summary>
        public WeatherPoint[] atmosphere = new WeatherPoint[WORLD_SIZE * WORLD_SIZE * MaxZ / DZ];

        void InitialiseAtmosphere()
        {
            DateTime time = DateTime.Now;
            for (int x = 0; x < WORLD_SIZE; x++)
            {
                for (int y = 0; y < WORLD_SIZE; y++)
                {
                    for (int z = 0; z < MaxZ / DZ; z++)
                    {
                        //TODO: Get a correct temperature decrease with height
                        double albedo = GetAlebdo(worldMap[x, y].type);
                        atmosphere[z + x * MaxZ + y * WORLD_SIZE * MaxZ] = new WeatherPoint(0, 0, 0, temperatureMap[x, y] * 35 - z, z,
                            (y - WORLD_SIZE / 2) / ((double)WORLD_SIZE) * 180, x, y, heightMap[x, y], albedo);
                    }
                }
            }
            Trace.TraceInformation($"Generated atmosphere in {(time - DateTime.Now).TotalMilliseconds} miliseconds");
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
                    return 0.4d;
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
        void CalculateAtmosphereTimeStep(bool debug = false)
        {
            DateTime time = DateTime.Now;
            Forker forker = new Forker();
            for (int i = 0; i < 9; i++)
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
            //We simply calculate each one from the values around them, so they will have old and new value used
            for (int x = 0; x < WORLD_SIZE; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    for (int z = 0; z < MaxZ / DZ; z++)
                    {
                        //See https://en.wikipedia.org/wiki/Primitive_equations
                        int position = z + MaxZ * x + y * WORLD_SIZE * MaxZ;
                        WeatherPoint point = atmosphere[position];

                        // du/dt -fv = -d geopotential / dx
                        WeatherPoint right;
                        if (x == WORLD_SIZE - 1)
                        {
                            right = atmosphere[z + MaxZ * (WORLD_SIZE - 1) + y * WORLD_SIZE * MaxZ];
                        }
                        else
                        {
                            right = atmosphere[z + MaxZ * (x + 1) + y * WORLD_SIZE * MaxZ];
                        }
                        WeatherPoint left;
                        if (x == 0)
                        {
                            left = atmosphere[z + MaxZ * (WORLD_SIZE - 1) + y * WORLD_SIZE * MaxZ];
                        }
                        else
                        {
                            left = atmosphere[z + MaxZ * (x - 1) + y * WORLD_SIZE * MaxZ];
                        }
                        double moveU = (point.u * atmosphereTimeStep * 4) / DX;
                        if (point.u > 0)
                        {
                            right.u += moveU;
                        }
                        else
                        {
                            left.u += moveU;
                        }
                        point.u -= moveU;
                        double friction = point.u * 0.1 * atmosphereTimeStep;
                        double dU = atmosphereTimeStep * point.f - (1.5 - point.mountain) * (right.pressure - left.pressure) - friction;
                        point.u += dU;
                        // dv/dt + fu = -d geopotential / dy
                        WeatherPoint bottom;
                        if (y != WORLD_SIZE - 1)
                        {
                            bottom = atmosphere[z + MaxZ * x + (y + 1) * WORLD_SIZE * MaxZ];
                        }
                        else
                        {
                            bottom = point;
                        }
                        WeatherPoint top;
                        if (y != 0)
                        {
                            top = atmosphere[z + MaxZ * x + (y - 1) * WORLD_SIZE * MaxZ];
                        }
                        else
                        {
                            top = point;
                        }
                        double moveV = (point.v * atmosphereTimeStep * 4) / DY;
                        if (point.v > 0)
                        {
                            top.v += moveV;
                        }
                        else
                        {
                            left.v += moveV;
                        }
                        point.v -= moveV;
                        friction = point.v * 0.1 * atmosphereTimeStep;
                        double dV = (1.5 - point.mountain) * 10 * (bottom.pressure - top.pressure) - friction;
                        point.v += dV;

                        // p = air densiy * R * T
                        double pressure = point.pressure;
                        point.pressure = 1013 * Math.Exp(2.896 * (9.807 + point.temperature) / (R * (273 + point.temperature))); //See https://www.math24.net/barometric-formula/;
                        point.dP = pressure - point.pressure;
                        // d vertical = d pressure * ( -dU / dx -dV / dy)
                        double dW = point.dP * (-dU / DX - dV / DY);
                        point.w += dW;
                        // dT (1 /dt + u/dx + v/dy + w/dp - w*R*T/(p*cp)) = J/cp
                        // dT = J / (cp * (1 / dt + u/dx + v/dy +w /dp - w*R*T/(p * pc))
                        // I guess J is simply energy balance
                        double radiation = point.EnergyBalance * atmosphereTimeStep / 100d;
                        double latitudinalTemp = ((left.temperature + right.temperature) / 2 - point.temperature) / 10;
                        double longitudinalTemp = ((top.temperature + bottom.temperature) / 2 - point.temperature) / 10;
                        WeatherPoint below = new WeatherPoint();
                        if (z != 0) below = atmosphere[(z - 1) + MaxZ * x + y * WORLD_SIZE * MaxZ];
                        else below = point;
                        WeatherPoint above = new WeatherPoint();
                        if (z != MaxZ - 1) above = atmosphere[(z + 1) + MaxZ * x + y * WORLD_SIZE * MaxZ];
                        else above = point;
                        double verticalTemp = atmosphereTimeStep * (below.temperature - above.temperature) / (2 * DZ);
                        point.dT = radiation + latitudinalTemp + longitudinalTemp + verticalTemp;
                        if (!double.IsNaN(point.dT))
                        {
                            //if (Math.Abs(dT) > 40)
                            //    //point.temperature += Math.Sign(dT) * 4;
                            //    throw new Exception();
                            point.temperature += point.dT;
                        }
                        else
                            point.temperature += 0.0001;
                        if (point.temperature == 0 && point.pressure == 0 && point.u == 0 && point.v == 0 &&
                            point.geopotential == 0 && point.w == 0)
                            throw new Exception();
                        if (point.temperature < -60)
                            throw new Exception();
                        if (double.IsNaN(point.temperature))
                            throw new Exception();
                        if (double.IsNaN(point.pressure))
                            throw new Exception();
                        if (double.IsNaN(point.u))
                            throw new Exception();
                        if (double.IsNaN(point.v))
                            throw new Exception();
                        if (double.IsNaN(point.w))
                            throw new Exception();
                        atmosphere[position] = point;
                    }
                }
            }
        }
    }
}
