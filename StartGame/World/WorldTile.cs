using StartGame.Functions;
using System;
using System.Diagnostics;
using System.Drawing;
using static StartGame.World.World;

namespace StartGame.World
{
    public enum WorldTileType { Ocean, River, TemperateGrassland, Rainforest, Desert, Tundra, TemperateForest, Savanna, Alpine, SeaIce }

    public class WorldTile
    {
        public Point position;
        public WorldFeature worldFeature;
        public WorldTileType type;
        public double height;
        public double movementCost;
        public Island island;
        public SorroundingTiles<WorldTile> sorroundingTiles;

        public double forest;
        public WorldRockMaterial rock;
        public double waterChange;

        /// <summary>
        /// The water which is not being moved, but is used for vegetation on this tile
        /// </summary>
        public double landWater;

        // Stats for biome decision
        public double averageTemp;
        public double measurements;

        public WorldTile(double Height, WorldTileType type, double cost, Point position, double rainFall)
        {
            height = Height;
            this.type = type;
            movementCost = cost;
            this.position = position;
            switch (type)
            {
                case WorldTileType.Ocean:
                    forest = 0;
                    break;
                case WorldTileType.River:
                    forest = 0;
                    break;
                case WorldTileType.TemperateGrassland:
                    forest = 10 * (1 + rainFall);
                    break;
                case WorldTileType.Rainforest:
                    forest = 100;
                    break;
                case WorldTileType.Desert:
                    forest = 0;
                    break;
                case WorldTileType.Tundra:
                    forest = 40 * (1 + rainFall);
                    break;
                case WorldTileType.TemperateForest:
                    forest = 60 * (1 + rainFall / 2);
                    break;
                case WorldTileType.Savanna:
                    forest = 10 * (1 + rainFall);
                    break;
                case WorldTileType.Alpine:
                    forest = 20 * (1 + rainFall);
                    break;
                case WorldTileType.SeaIce:
                    forest = 0;
                    break;
                default:
                    throw new System.NotImplementedException();
            }
            forest.Cut(0, 100);
            // TODO: Find a better way to determine these values
            rock = new WorldRockMaterial(80, 0.01, 0.3);
        }


        /// <summary>
        /// Once an ingame hour, update water movement and erosion
        /// </summary>
        public void DoUpdate()
        {
            if ((rock.WaterAmount == 0 && rock.waterCreation == 0) || !IsLand(type))
                return;

            //if (rock.WaterAmount < 0 || rock.WaterAmount > 10000)
            //    throw new Exception();
            double initialWater = rock.WaterAmount;
            rock.WaterAmount += rock.waterCreation;
            double waterKept = FastMath.Floor(rock.WaterAmount * rock.waterLoss,3);
            rock.WaterAmount -= waterKept;
            //if (rock.WaterAmount < 0 || rock.WaterAmount > 10000)
            //    throw new Exception();
            landWater += waterKept;

            WorldTile left = position.X != 0 ? Instance.worldMap[position.X - 1, position.Y] : null;
            WorldTile right = position.X != WORLD_SIZE - 1 ? Instance.worldMap[position.X + 1, position.Y] : null;
            WorldTile top = position.Y != 0 ? Instance.worldMap[position.X, position.Y - 1] : null;
            WorldTile bottom = position.Y != WORLD_SIZE - 1 ? Instance.worldMap[position.X, position.Y + 1] : null;
            double xGradient = (left?.height ?? height) - (right?.height ?? height);
            double yGradient = (bottom?.height ?? height) - (top?.height ?? height);
            WorldTile next;
            if (Math.Abs(xGradient) > Math.Abs(yGradient))
            {
                if (xGradient > 0)
                {
                    next = left;
                    rock.xDirection = -1;
                }
                else
                {
                    next = right;
                    rock.xDirection = 1;
                }
                rock.yDirection = null;
            }
            else
            {
                if (yGradient > 0)
                {
                    next = bottom;
                    rock.yDirection = -1;
                }
                else
                {
                    next = top;
                    rock.yDirection = 1;
                }
                rock.xDirection = null;
            }
            if(next != null)
            {
                double accel = height - next.height;
                if(accel > 0 && rock.WaterAmount > 0)
                {
                    rock.waterSpeed = rock.waterInertia + 400 * accel - rock.friction * rock.waterSpeed;
                    if (double.IsNaN(rock.waterSpeed) || double.IsInfinity(rock.waterSpeed))
                        throw new Exception();
                    rock.waterInertia = 0; // Reset for next calculation
                    next.rock.waterInertia += rock.waterSpeed;
                    double change = FastMath.Floor(Math.Min(FastMath.Floor(Math.Abs(rock.waterSpeed) * rock.WaterAmount, 3), rock.WaterAmount / 5) * EROSION_RATE, 1);
                    next.rock.WaterAmount += change;
                    //rock.WaterAmount -= change;
                    double diffH = (rock.waterSpeed * rock.WaterAmount * (1 + rock.friction)) / (rock.strength * 100);
                    if (diffH * 100 > height)
                    {
                        diffH = height / 100;
                    }
                    height -= diffH;
                    if(double.IsNaN(rock.WaterAmount) || double.IsNaN(height))
                        throw new Exception();
                    //if (rock.WaterAmount < 0 || rock.WaterAmount > 1000)
                    //    throw new Exception();
                }
                if (double.IsNaN(rock.WaterAmount)|| double.IsNaN(height))
                    throw new Exception();
                //if (rock.WaterAmount < 0 || rock.WaterAmount > 10000)
                //    throw new Exception();
            }

            waterChange = rock.WaterAmount - initialWater;
        }

        public void DetermineBiome()
        {
            WorldTileType oldType = type;
            //Determine average averageTemp and water
            double aTemp = 0;
            double aWater = 0;
            int scanSize = 3;
            for (int y = 0; y < scanSize; y++)
            {
                for (int x = 0; x < scanSize; x++)
                {
                    int lX = (position.X - (x - scanSize / 2)).Cut(0, WORLD_SIZE - 1);
                    int lY = (position.Y - (y - scanSize / 2)).Cut(0, WORLD_SIZE - 1);
                    aTemp += Instance.worldMap[lX, lY].averageTemp;
                    aWater += Instance.worldMap[lX, lY].landWater;
                }
            }
            aTemp /= scanSize * scanSize;
            aWater /= scanSize * scanSize;

            if (height < 0.42)
            {
                if (aTemp < 0)
                {
                    type = WorldTileType.SeaIce;
                }
                else
                {
                    type = WorldTileType.Ocean;
                }
                Instance.costMap[position.X, position.Y] = 20;
            }
            else if (height > 0.7)
            {
                type = WorldTileType.Alpine;
                Instance.costMap[position.X, position.Y] = 4;
            }
            else if (aTemp < 5)
            {
                type = WorldTileType.Tundra;
                Instance.costMap[position.X, position.Y] =  2;
            }
            else if (aWater > 40 && aTemp > 15)
            {
                type = WorldTileType.Rainforest;
                Instance.costMap[position.X, position.Y]  = 4;
            }
            else if (aWater < 15 && aTemp > 20)
            {
                type = WorldTileType.Desert;
                Instance.costMap[position.X, position.Y]  = 3;
            }
            else if (aTemp > 18)
            {
                type = WorldTileType.Savanna;
                Instance.costMap[position.X, position.Y]  = 2;
            }
            else if (aWater > 20)
            {
                type = WorldTileType.TemperateForest;
                Instance.costMap[position.X, position.Y]  = 1;
            }
            else
            {
                type = WorldTileType.TemperateGrassland;
                Instance.costMap[position.X, position.Y] = 1;
            }
            if(type != oldType)
            {
                Instance.lost[oldType]++;
                Instance.gained[type]++;
                Instance.GetAtmosphereValue(position.X, position.Y, 0).baseEvaporationRate = GetEvaporationRate(type);
                Instance.GetAtmosphereValue(position.X, position.Y, 0).albedo = GetAlebdo(type);
                Instance.GetAtmosphereValue(position.X, position.Y, 0).groundHeatCapacity = GetGroundHeatCapacity(type);
                Instance.GetAtmosphereValue(position.X, position.Y, 0).baseMinimumHumditiy = GetMinimumHumdity(type);
            }
            landWater = 0;
        }

        public string RawValue()
        {
            return $"{position} {height} {type} {worldFeature.id}";
        }
    }

    public class WorldRockMaterial
    {
        /// <summary>
        /// Between 0 -> 100, where 100 is the strongest. Determines erosion
        /// </summary>
        public double strength;
        /// <summary>
        /// Determines how easily water can pass through a tile. 0 = ocenan 1 = pure rock
        /// </summary>
        public double friction;
        /// <summary>
        /// Between 0 -> 1 is proportion of water lost to ground
        /// </summary>
        public double waterLoss;

        public double waterSpeed;
        /// <summary>
        /// Inertia of water entering this tile
        /// </summary>
        public double waterInertia = 0;

        private double waterAmount = 0;

        /// <summary>
        /// Is non-zero if this is a water source
        /// </summary>
        public double waterCreation = 0;

        // Used for rendering
        public int? xDirection = 0;
        public int? yDirection = 0;

        public double WaterAmount
        {
            get => waterAmount; set
            {
                if (value < 0)
                    throw new Exception();
                waterAmount = value;
            }
        }

        public WorldRockMaterial(double strength, double waterLoss, double friction, double waterCreation = 0)
        {
            this.strength = strength;
            this.waterLoss = waterLoss;
            this.waterCreation = waterCreation;
            this.friction = friction;
        }
    }
}