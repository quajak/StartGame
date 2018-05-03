using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StartGame.MainGameWindow;

namespace StartGame
{
    internal abstract class Spell
    {
        //Long term: Add: Spells have a chance to fail
        internal readonly string name;

        internal int coolDown;
        public readonly SpellInformationFormat format;
        internal readonly int MaxCoolDown;
        internal MainGameWindow main;
        internal Map map;

        public int manaCost;

        public bool Ready { get => coolDown == 0; set => coolDown = value ? 0 : coolDown; }

        public Spell(string name, int MaxCoolDown, int? InitialCoolDown, SpellInformationFormat format, int manaCost)
        {
            this.manaCost = manaCost;
            this.name = name;
            this.MaxCoolDown = MaxCoolDown;
            coolDown = InitialCoolDown ?? MaxCoolDown;
            this.format = format;
        }

        public void Initialise(MainGameWindow main, Map Map)
        {
            this.main = main;
            map = Map;
        }

        public abstract string Activate(SpellInformation information);

        public bool CheckFormat(SpellInformation information)
        {
            return information.positions.Count == format.Positions;
        }
    }

    internal struct SpellInformationFormat
    {
        public int Positions;
    }

    internal struct SpellInformation
    {
        public List<Point> positions;
    }

    internal class FireBall : Spell
    {
        //TODO: Calculate damage using function list
        private readonly int damage;

        private readonly int turns;

        public FireBall(int damage, int turns, int MaxCoolDown, int? intialCoolDown) : base("Fireball", MaxCoolDown, intialCoolDown,
            new SpellInformationFormat() { Positions = 1 }, 5)
        {
            this.damage = damage;
            this.turns = turns;
        }

        public override string Activate(SpellInformation information)
        {
            if (!Ready) throw new Exception("Spell is not ready");
            if (!CheckFormat(information)) throw new Exception("Information is incomplete!");

            coolDown = MaxCoolDown;

            if (map.map[information.positions[0].X, information.positions[0].Y].type.FType != FieldType.water)
            {
                new Fire(turns, damage, information.positions[0], map, main);
                return $"Created a Fireball at {information.positions[0]}";
            }
            else
            {
                main.DamageAtField(damage, information.positions[0]);
                return $"Tried to create a fireball at {information.positions[0]}";
            }
        }
    }

    internal class TeleportSpell : Spell
    {
        public TeleportSpell(int MaxCoolDown, int? initalCoolDown) : base("Teleport", MaxCoolDown, initalCoolDown,
            new SpellInformationFormat() { Positions = 2 }, 10)
        {
        }

        public override string Activate(SpellInformation information)
        {
            if (!Ready) throw new Exception("Spell is not ready");
            if (!CheckFormat(information)) throw new Exception("Information is incomplete!");

            coolDown = MaxCoolDown;

            if (main.players.Exists(p => p.troop.Position == information.positions[0]))
            {
                main.MovePlayer(information.positions[1], information.positions[0], main.players.Find(p => p.troop.Position == information.positions[0]), MovementType.teleport, false);
                main.UpdateGameBoard();
                return $"Teleported {map.troops.Find(t => t.Position == information.positions[1]).name} from {information.positions[0]} to {information.positions[1]}";
            }
            else
            {
                return "The teleport spell failed as there was nothign to teleport!";
            }
        }
    }
}