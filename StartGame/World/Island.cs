using System.Collections.Generic;
using System.Linq;

namespace StartGame.World
{
    public class Island
    {
        public static int ID = 0;
        public int id;
        public bool land;
        public List<WorldTile> tiles = new List<WorldTile>();
        public List<Island> connectedOceans = new List<Island>();
        public List<WorldTile> border = new List<WorldTile>();

        public Island(bool land)
        {
            this.land = land;
            id = ++ID;
        }

        public void DetermineOceans()
        {
            border = tiles.Where(t => t.sorroundingTiles.rawMaptiles.Select(r => r.island).Where(i => i != this).Count() != 0).ToList();
            connectedOceans = border.SelectMany(b => b.sorroundingTiles.rawMaptiles.Select(s => s.island).Where(i => i != this)).Distinct().Where(i => !i.land).ToList();
        }
    }
}