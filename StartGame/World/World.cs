using StartGame.MathFunctions;
using StartGame.Mission;
using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.World
{
    public class World
    {
        Random random;
        public const int WORLD_SIZE = 150;
        public const int WORLD_DIFFICULTY = 5;
        private static World world;
        public DateTime time = new DateTime(2000, 1, 1, 0, 0, 0, 0);
        public Campaign campaign; //Get a better system to generate mission
        public int missionsCompleted = 0;

        public static World Instance
        {
            get
            {
                if (world is null)
                    world = new World();
                return world;
            }
            set => world = value;
        }

        public List<Spell> Spells => spells;

        private List<Spell> spells;
        public List<Tree> trees = Tree.GenerateTrees();

        public WorldTile[,] worldMap;
        readonly double[,] costMap;

        public List<WorldPlayer> actors = new List<WorldPlayer>();

        private World()
        {
            spells = new List<Spell>()
            {
                new HealingSpell(5, 500),
                new EarthQuakeSpell(5, 5, 4500),
                new LightningBoltSpell(15, 4000),
                new DebuffSpell(2, 1, 5, 0, 800),
                new TeleportSpell(2, 0, 9600),
                new FireBall(8, 3, 4, 0, 2300)
            };

            //Generate the terrain
            //0. Setup variables
            campaign = new Campaign(null, 0, WORLD_DIFFICULTY);
            worldMap = new WorldTile[WORLD_SIZE, WORLD_SIZE];
            costMap = new double[WORLD_SIZE, WORLD_SIZE];
            PerlinNoise perlin = new PerlinNoise();
            double perlinDiff = 0.08;
            random = new Random();
            double Seed = random.NextDouble();
            //1. Set height for each tile
            for (int x = 0; x < WORLD_SIZE; x++)
            {
                for (int y = 0; y < WORLD_SIZE; y++)
                {
                    //Height
                    double mHeight = perlin.Perlin(x * perlinDiff, y * perlinDiff, Seed);
                    //First spit the world into ocean, grassland and mountains
                    WorldTileType type = WorldTileType.Grassland;
                    double cost = 1;
                    if (mHeight < 0.33)
                    {
                        type = WorldTileType.Ocean;
                        cost = 3;
                    }
                    else if (mHeight > 0.66)
                    {
                        type = WorldTileType.Mountain;
                        cost = 3;
                    }
                    worldMap[x, y] = new WorldTile(mHeight, type, cost);
                    costMap[x, y] = cost;
                }
            }

            //2. Generate missions
            for (int i = 0; i < WORLD_SIZE / 4; i++)
            {
                Point position = new Point(random.Next(0, WORLD_SIZE), random.Next(0, WORLD_SIZE));
                Mission.Mission mission = campaign.DecideMission(missionsCompleted + 1);
                actors.Add(new MissionWorldPlayer(mission, missionsCompleted + 1, position));
            }
        }

        public void InitialisePlayer(HumanPlayer player)
        {
            foreach (var actor in actors)
            {
                if(actor is MissionWorldPlayer missionPlayer)
                {
                    player.possibleActions.Add(new StartMission(missionPlayer.mission, missionPlayer.difficulty, missionPlayer.WorldPosition));
                }
            }
        }

        public Spell GainSpell<T>() where T : Spell
        {
            Spell spell = spells.Where(s => s is T).FirstOrDefault();
            spells.Remove(spell);
            return spell;
        }

        public Spell GainSpell(string name)
        {
            Spell spell = spells.Where(s => s.name == name).FirstOrDefault();
            spells.Remove(spell);
            return spell;
        }

        public double[,] MovementCost()
        {
            double[,] values = new double[WORLD_SIZE, WORLD_SIZE];
            for (int x = 0; x <= worldMap.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= worldMap.GetUpperBound(1); y++)
                {
                    values[x, y] = worldMap[x, y].movementCost;
                }
            }
            return values;
        }

        public void ProgressTime(TimeSpan timePassed)
        {
            time += timePassed;
            double actions = Math.Round(timePassed.TotalHours * 2, MidpointRounding.AwayFromZero) / 2;
            actors.ForEach(a => a.WorldAction(actions));
        }

        public void Move(WorldPlayer actor, Point position)
        {
            Trace.TraceInformation($"{actor.Name} moved to {position} at {time.ToString("MM/dd/yyyy H:mm")}");
            actor.WorldPosition = position;
            //How does the renderer know that something has happened?
        }

        public void GenerateNewMission()
        {
            Point position = new Point(random.Next(0, WORLD_SIZE), random.Next(0, WORLD_SIZE));
            Mission.Mission mission = campaign.DecideMission(missionsCompleted + 1);
            actors.Add(new MissionWorldPlayer(mission, missionsCompleted + 1, position));
        }
    }
}