using StartGame.Mission;
using StartGame.PlayerData;
using StartGame.PlayerData.Players;
using System;
using System.Drawing;

namespace StartGame.GameMap
{
    public abstract class MapBiome
    {
        /// <summary>
        /// Returns default parameters for this enviroment for map creation
        /// </summary>
        /// <returns></returns>
        public abstract (double PerlinDiff, double HeightDiff) DefaultParameters();
        public abstract MapTileTypeEnum DetermineType(double height);

        public virtual void ManipulateMission(MainGameWindow mainGame, Mission.Mission mission)
        {

        }
    }

    public class GrasslandMapBiome : MapBiome
    {
        public override (double, double) DefaultParameters()
        {
            return (0.1, 0);
        }
        public override MapTileTypeEnum DetermineType(double height)
        {
            if (height < 0.2) return MapTileTypeEnum.deepWater;
            else if (height < 0.3) return MapTileTypeEnum.shallowWater;
            else if (height < 0.6) return MapTileTypeEnum.land;
            else if (height < 0.8) return MapTileTypeEnum.hill;
            else if (height <= 1.0) return MapTileTypeEnum.mountain;
            throw new Exception();
        }

        public override void ManipulateMission(MainGameWindow mainGame, Mission.Mission mission)
        {
            int num = World.World.random.Next(0, 10);
            Point[] spawn = mainGame.map.DeterminSpawnPoint(num, SpawnType.randomLand).ToArray();
            foreach (var point in spawn)
            {
                mainGame.AddPlayer(new PlayerData.Players.Tree(point, mainGame.map));
            }
        }
    }
    public class ForestMapBiome : MapBiome
    {
        public override (double, double) DefaultParameters()
        {
            return (0.11, 0);
        }
        public override MapTileTypeEnum DetermineType(double height)
        {
            if (height < 0.2) return MapTileTypeEnum.deepWater;
            else if (height < 0.4) return MapTileTypeEnum.shallowWater;
            else if (height < 0.6) return MapTileTypeEnum.land;
            else if (height < 0.8) return MapTileTypeEnum.hill;
            else if (height <= 1.0) return MapTileTypeEnum.mountain;
            throw new Exception();
        }

        public override void ManipulateMission(MainGameWindow mainGame, Mission.Mission mission)
        {
            int num = World.World.random.Next(10, 30);
            Point[] spawn = mainGame.map.DeterminSpawnPoint(num, SpawnType.randomLand).ToArray();
            foreach (var point in spawn)
            {
                mainGame.AddPlayer(new PlayerData.Players.Tree(point, mainGame.map));
            }
        }
    }

    public class RainforestMapBiome : MapBiome
    {
        public override (double, double) DefaultParameters()
        {
            return (0.15, 0);
        }
        public override MapTileTypeEnum DetermineType(double height)
        {
            if (height < 0.3) return MapTileTypeEnum.deepWater;
            else if (height < 0.4) return MapTileTypeEnum.shallowWater;
            else if (height < 0.7) return MapTileTypeEnum.land;
            else if (height < 0.9) return MapTileTypeEnum.hill;
            else if (height <= 1.0) return MapTileTypeEnum.mountain;
            throw new Exception();
        }

        public override void ManipulateMission(MainGameWindow mainGame, Mission.Mission mission)
        {
            int num = World.World.random.Next(20, 40);
            Point[] spawn = mainGame.map.DeterminSpawnPoint(num, SpawnType.randomLand).ToArray();
            foreach (var point in spawn)
            {
                mainGame.AddPlayer(new RainforestTree(point, mainGame.map));
            }
        }
    }

    public class AlpineMapBiome : MapBiome
    {
        public override (double, double) DefaultParameters()
        {
            return (0.08, 0.2);
        }
        public override MapTileTypeEnum DetermineType(double height)
        {
            if (height < 0.1) return MapTileTypeEnum.deepWater;
            else if (height < 0.2) return MapTileTypeEnum.shallowWater;
            else if (height < 0.5) return MapTileTypeEnum.land;
            else if (height < 0.7) return MapTileTypeEnum.hill;
            else if (height < 0.9) return MapTileTypeEnum.mountain;
            else if (height <= 1.0) return MapTileTypeEnum.snow;
            throw new Exception();
        }
    }

    public class TundraMapBiome : MapBiome
    {
        public override (double, double) DefaultParameters()
        {
            return (0.07, 0);
        }
        public override MapTileTypeEnum DetermineType(double height)
        {
            if (height < 0.2) return MapTileTypeEnum.ice;
            else if (height < 0.5) return MapTileTypeEnum.snowyLand;
            else if (height < 0.7) return MapTileTypeEnum.snow;
            else if (height <= 1.0) return MapTileTypeEnum.mountain;
            throw new Exception();
        }

        public override void ManipulateMission(MainGameWindow mainGame, Mission.Mission mission)
        {
            int num = World.World.random.Next(0, 20);
            Point[] spawn = mainGame.map.DeterminSpawnPoint(num, SpawnType.randomLand).ToArray();
            foreach (var point in spawn)
            {
                mainGame.AddPlayer(new SnowyTree(point, mainGame.map));
            }
        }
    }

    public class DesertMapBiome : MapBiome
    {
        public override (double, double) DefaultParameters()
        {
            return (0.12, -0.2);
        }
        public override MapTileTypeEnum DetermineType(double height)
        {
            if (height < 0.2) return MapTileTypeEnum.looseSand;
            else if (height < 0.4) return MapTileTypeEnum.sand;
            else if (height < 0.6) return MapTileTypeEnum.dune;
            else if (height < 0.8) return MapTileTypeEnum.hill;
            else if (height <= 1.0) return MapTileTypeEnum.mountain;
            throw new Exception();
        }
        public override void ManipulateMission(MainGameWindow mainGame, Mission.Mission mission)
        {
            int num = World.World.random.Next(0,8);
            Point[] spawn = mainGame.map.DeterminSpawnPoint(num, SpawnType.randomLand).ToArray();
            foreach (var point in spawn)
            {
                mainGame.AddPlayer(new Cactus(point, mainGame.map));
            }
        }
    }

    public class SavannaMapBiome : MapBiome
    {
        public override (double, double) DefaultParameters()
        {
            return (0.12, -0.1);
        }
        public override MapTileTypeEnum DetermineType(double height)
        {
            if (height < 0.2) return MapTileTypeEnum.sand;
            else if (height < 0.5) return MapTileTypeEnum.land;
            else if (height < 0.7) return MapTileTypeEnum.dune;
            else if (height < 0.8) return MapTileTypeEnum.hill;
            else if (height <= 1.0) return MapTileTypeEnum.mountain;
            throw new Exception();
        }
    }
}