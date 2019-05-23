using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StartGame.World
{
    public class Island
    {
        static List<string> Names = new List<string>() {};

        public static int ID = 0;
        public int id;
        public bool land;
        public List<WorldTile> tiles = new List<WorldTile>();
        public List<Island> connectedOceans = new List<Island>();
        public List<WorldTile> border = new List<WorldTile>();
        public string Name;

        public Island(bool land)
        {
            if(Names.Count == 0)
            {
                Names = File.ReadAllLines(@".\Resources\IslandNames.txt").ToList();
            }
            this.land = land;
            Name = Names.GetRandom();
            Names.Remove(Name);
            id = ++ID;
        }

        public void DetermineOceans()
        {
            border = tiles.Where(t => t.sorroundingTiles.rawMaptiles.Select(r => r.island).Where(i => i != this).Count() != 0).ToList();
            connectedOceans = border.SelectMany(b => b.sorroundingTiles.rawMaptiles.Select(s => s.island).Where(i => i != this)).Distinct().Where(i => !i.land).ToList();
        }
    }
}