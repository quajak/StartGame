using StartGame.GameMap;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.PlayerData.Players
{
    class Cactus : Player
    {
        static int counter = 0;
        public Cactus(Point position, Map map) : base(PlayerType.computer, "Cactus" + ++counter, map, new Player[] { }, 1, 0, 0, 0, 0, 0, 5)
        {
            troop = new Troop("Cactus" + counter, new Items.Weapon(1, Items.BaseAttackType.melee, Items.BaseDamageType.sharp, 1, "Spike", 1, false),
                Resources.Cactus, 2, map, this, 0, new Dictionary<DamageType, double> { { DamageType.fire, 1.5 } }) {
                Position = position
            };
            Hidden = true;
            Dangerous = false;
        }

        public override void PlayTurn(MainGameWindow main, bool singleTurn)
        {
            
        }
    }

    class Tree : Player
    {
        static readonly List<Bitmap> pictures = new List<Bitmap> { Resources.Tree1, Resources.Tree2, Resources.Tree3 };
        static int counter = 0;
        public Tree(Point position, Map map) : base(PlayerType.computer, "Tree" + ++counter, map, new Player[] { }, 1, 0, 0, 0, 0, 0, 5)
        {
            troop = new Troop("Tree" + counter, new Items.Weapon(1, Items.BaseAttackType.melee, Items.BaseDamageType.sharp, 1, "Root", 1, false),
                pictures.GetRandom(), 2, map, this, 0, new Dictionary<DamageType, double> { { DamageType.fire, 1.5 } }) {
                Position = position
            };
            Hidden = true;
            Dangerous = false;
        }

        public override void PlayTurn(MainGameWindow main, bool singleTurn)
        {

        }
    }

    class SnowyTree : Player
    {
        static readonly List<Bitmap> pictures = new List<Bitmap> { Resources.snowyTree1, Resources.snowyTree2, Resources.snowyTree3};
        static int counter = 0;
        public SnowyTree(Point position, Map map) : base(PlayerType.computer, "Snowy Tree" + ++counter, map, new Player[] { }, 1, 0, 0, 0, 0, 0, 5)
        {
            troop = new Troop("Snowy Tree" + counter, new Items.Weapon(1, Items.BaseAttackType.melee, Items.BaseDamageType.sharp, 1, "Root", 1, false),
                pictures.GetRandom(), 2, map, this, 0, new Dictionary<DamageType, double> { { DamageType.fire, 1.5 } }) {
                Position = position
            };
            Hidden = true;
            Dangerous = false;
        }

        public override void PlayTurn(MainGameWindow main, bool singleTurn)
        {

        }
    }
    class RainforestTree : Player
    {
        static readonly List<Bitmap> pictures = new List<Bitmap> { Resources.RainforestTree1, Resources.RainforestTree2, Resources.RainforestTree3, Resources.RainforestTree4};
        static int counter = 0;
        public RainforestTree(Point position, Map map) : base(PlayerType.computer, "Rainforest Tree" + ++counter, map, new Player[] { }, 1, 0, 0, 0, 0, 0, 5)
        {
            troop = new Troop("Rainforest Tree" + counter, new Items.Weapon(1, Items.BaseAttackType.melee, Items.BaseDamageType.sharp, 1, "Root", 1, false),
                pictures.GetRandom(), 2, map, this, 0, new Dictionary<DamageType, double> { { DamageType.fire, 1.5 } }) {
                Position = position
            };
            Hidden = true;
            Dangerous = false;
        }

        public override void PlayTurn(MainGameWindow main, bool singleTurn)
        {

        }
    }


}
