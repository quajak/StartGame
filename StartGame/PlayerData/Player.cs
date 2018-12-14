using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using StartGame.Items;
using static StartGame.MainGameWindow;

namespace StartGame.PlayerData
{
    public abstract class Player
    {
        public PlayerType type;
        private string name;
        public ActionPoint actionPoints;
        public bool active = false;
        public Map map;
        internal Player[] enemies;
        public int XP;
        internal readonly List<Spell> spells;

        //Derived stats
        public Troop troop;

        public bool Dead => troop.health.Value == 0;

        public string Name { get => name; set => name = value; }

        public List<JewelryType> JewelryTypes = new List<JewelryType>()
        {
            new JewelryType("Necklace", 2),
            new JewelryType("Ring", 10),
            new JewelryType("Earring", 4)
        };

        public JewelryType GetJewelryType(JewelryType type)
        {
            return GetJewelryType(type.name);
        }

        public JewelryType GetJewelryType(string name)
        {
            return JewelryTypes.First(j => j.name == name);
        }

        //Base stats
        public Attribute strength; //Bonus damage dealt

        public Attribute agility; //Chance to dodge
        public Attribute endurance; //How many action points + defense
        public Attribute vitality; //How much health
        public Attribute wisdom; //how much mana
        public Attribute intelligence; //improves the effectivness of spells

        public Mana mana;
        public Attribute money;
        public MovementPoints movementPoints;

        public List<Tree> trees = new List<Tree>();

        public GearWeight gearWeight;

        public Player(PlayerType Type, string name, Map Map, Player[] Enemies, int XP, int Intelligence,
            int Strength, int Endurance, int Wisdom, int Agility, int Vitality, List<Spell> spells = null)
        {
            this.XP = XP;
            actionPoints = new ActionPoint(this, 4);
            intelligence = new Attribute(Intelligence, "Intelligence");
            strength = new Attribute(Strength, "Strength");
            agility = new Attribute(Agility, "Agility");
            endurance = new Attribute(Endurance, "Endurance");
            vitality = new Attribute(Vitality, "Vitality");
            wisdom = new Attribute(Wisdom, "Wisdom");
            mana = new Mana(this, 10);
            movementPoints = new MovementPoints(this, 0);
            gearWeight = new GearWeight(this, 5000);
            if (spells != null)
            {
                this.spells = spells;
            }
            else
            {
                this.spells = new List<Spell>();
            }
            enemies = Enemies;
            type = Type;
            Name = name;
            map = Map;
        }

        public abstract void PlayTurn(MainGameWindow main, bool singleTurn);

        public virtual void Initialise(MainGameWindow main)
        {
        }

        public void ActionButtonPressed(MainGameWindow mainGameWindow)
        {
            mainGameWindow.NextTurn();
        }

        /// <summary>
        /// Called when turn starts used to set action points
        /// </summary>
        public event EventHandler InitialiseTurnHandler = (sender, e) => {
            Player player = (sender as Player);
            player.actionPoints.rawValue = player.actionPoints.MaxValue().Value;
            player.movementPoints.Reset();
        };

        public void NextTurn()
        {
            InitialiseTurnHandler(this, null);
        }

        /// <summary>
        /// Function list used to calculate cost to move to certain field.
        /// Input: Start, Active, Next Tile, Total already Distance moved, Cost of movement, type
        /// Ouput: Cost
        /// </summary>
        public List<Func<MapTile, MapTile, MapTile, int, double, MovementType, double>> CalculateStepCost = new List<Func<MapTile, MapTile, MapTile, int, double, MovementType, double>>()
        {
            (path, active, next, distance, cost, type) =>
            {
                switch (type)
                {
                    case MovementType.walk:
                        cost = next.MovementCost;
                     break;

                    case MovementType.teleport:
                        cost = 1;
                     break;

                    default:
                     throw new NotImplementedException();
                }

                return cost;
            }
        };

        /// <summary>
        /// Function used to execute the calculateMovementCost list and calculate the cost of a movement
        /// </summary>
        /// <param name="path">Path of movement</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public double CalculateStep(MapTile start, MapTile active, MapTile next, int distance, MovementType type, bool allowBlockedEnd = false)
        {
            Contract.Assert(start != null);
            Contract.Assert(next != null);
            Contract.Assert(type != MovementType.teleport || start == active);

            switch (type)
            {
                case MovementType.walk:
                    break;

                case MovementType.teleport:
                    distance = AIUtility.Distance(start, next);
                    break;

                default:
                    throw new NotImplementedException();
            }
            if (distance == 0) throw new Exception("A unit can not travel for a distance of 0");

            double cost = 0;
            foreach (var func in CalculateStepCost)
            {
                cost = func.Invoke(start, active, next, distance, cost, type);
            }
            return cost;
        }
    }

    internal delegate Point FieldOptimiser(List<(Point point, double cost, double height)> list);

    public enum PlayerType
    { localHuman, computer };
}