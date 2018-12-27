using StartGame.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StartGame.World.Cities
{
    public abstract class City : WorldFeature
    {
        private bool isPort = false;
        static List<string> Names = new List<string> { "Edagas","Aqewell","Chueginia","Navine","Cofield","Shans","Cance","Vlerton","Ekaton","Illevale","Epimore","Bromery","Qrurstin","Vrimont","Mifton","Jeley","Ylaka","Ekesa","Ingdence","Ardginia","Foibert","Kissa","Stroccester","Driland","Craland","Doni","Vrares","Soit","Oriaby","Oremond","Wuayson","Vruchester","Mehledo","Sophia","Seymond","Frila","Ylanbu","Yheka","Odonfield","Anbushire","Zasburn","Neprough","Clinard","Crafield","Vrudsea","Tring","Nico","Neley","Andorough","Olnphia","Zruhull","Birora","Zinstead","Plaeburgh","Polas","Pence","Srouver","Flolk","Oriagow","Eighcester","Zruhull","Birora","Zinstead","Plaeburgh","Polas","Pence","Srouver","Flolk","Oriagow","Eighcester","Miahull","Wrafstead","Gluilby","Modiff","Fesford","Hora","Ylille","Gin","Ariocaster","Andover","Yrocaster","Wipus","Meham","Comouth","Wrokcester","Floit","Sranta","Hares","Ouisford","Urydale","Vriwood","Sudford","Druson","Crieta","Krumont","Grego","Cego","Xock","Agopolis","Atheland","Biphia","Bleurgend","Nuuxvine","Brosall","Cristead","Plurgh","Baco","Srork","Akaham","Inmouth","Bruuburn","Silas","Sliburg","Vlahpus","Luport","Ihont","Gline","Xok","Athemont","Irierith"};
        public string name;
        public Island Located => World.Instance.worldMap.Get(position).island;

        public bool IsPort
        {
            get => isPort; set
            {
                if (value && !isPort)
                {
                    buildings.Add(new Port(this));
                }
                else if(!value)
                {
                    buildings = buildings.Where(b => !(b is Port)).ToList();
                }
                isPort = value;
            }
        }

        public List<Island> access = new List<Island>();
        public List<City> portConnections = new List<City>();
        public List<City> roadConnections = new List<City>();
        public readonly int connections;
        public double[,] costMap = null;
        public int priority;
        public readonly int value;

        public List<CityBuilding> buildings = new List<CityBuilding>();

        public City(Point position, Bitmap bitmap, int connections, int priority, int value) : base(++ID, position, bitmap, 1)
        {
            name = Names[World.random.Next(Names.Count)];
            Names.Remove(name);
            foreach (var tile in World.Instance.worldMap.Get(position).sorroundingTiles.rawMaptiles)
            {
                if (!access.Contains(tile.island))
                    access.Add(tile.island);
            }

            this.connections = connections;
            this.priority = priority;
            this.value = value;
            size = new Point(3, 2);
            alignment = RenderingAlignment.Center;
            for (int i = 0; i < World.random.Next(value / 10) + 1; i++)
            {
                int type = World.random.Next(2);
                if (type == 0)
                    buildings.Add(ConvenienceStore.GenerateStore(value));
                else if (type == 1)
                    buildings.Add(JewleryStore.GenerateStore(value));
            }
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class SmallCity : City
    {
        public SmallCity(Point position, int value) : base(position, Resources.SmallCity, 1, 1, value)
        {
        }
    }

    public class MediumCity : City
    {
        public MediumCity(Point position, int value) : base(position, Resources.MediumCity, 1, 3, value)
        {
        }
    }

    public class LargeCity : City
    {
        public LargeCity(Point position, int value) : base(position, Resources.BigCity, 2, 6, value)
        {
        }
    }

    public class CapitalCity : City
    {
        public CapitalCity(Point position, int value) : base(position, Resources.CapitalCity, 3, 10, value)
        {
        }
    }
}