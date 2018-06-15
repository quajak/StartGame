using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StartGame.MainGameWindow;

namespace StartGame
{
    internal enum DamageType
    { melee, fire, water, earth, air, unblockable };

    internal abstract class Spell
    {
        //Long term: Add spell shop
        //Long term: Add: Spells have a chance to fail
        //TODO: Make intelligence effect is somehow
        //BUG: Fix that all spells depend on human intelligence
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
            if (information.positions is null && format.Positions == 0)
            {
                return true;
            }
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
        public Player mage;
    }

    internal class HealingSpell : Spell
    {
        //TODO: Allow this to be done to none player targets
        private readonly int gainHealth;

        public HealingSpell(int GainHealth) : base("Healing Spell", 3, 0, new SpellInformationFormat() { Positions = 0 }, 2)
        {
            gainHealth = GainHealth;
        }

        public override string Activate(SpellInformation information)
        {
            if (!Ready) throw new Exception("Spell is not ready!");
            if (!CheckFormat(information)) throw new Exception("Information is incomplete!");

            coolDown = MaxCoolDown;

            main.humanPlayer.troop.health = Math.Min(main.humanPlayer.intelligence + gainHealth + main.humanPlayer.troop.health, main.humanPlayer.troop.maxHealth);
            return $"{main.humanPlayer.Name} has healed for {gainHealth}";
        }
    }

    internal class LightningBoltSpell : Spell
    {
        private readonly int damage;

        public LightningBoltSpell(int damage) : base("Lightning Bolt", 3, 0, new SpellInformationFormat() { Positions = 1 }, 8)
        {
            this.damage = damage;
        }

        public override string Activate(SpellInformation information)
        {
            if (!Ready) throw new Exception("Spell is not ready");
            if (!CheckFormat(information)) throw new Exception("Information is incomplete!");

            coolDown = MaxCoolDown;

            Point hit = information.positions[0];

            int appliedDamage = damage + main.humanPlayer.intelligence / 2;
            if (map.map.Get(hit).type.FType == FieldType.water) appliedDamage *= 2;

            new LightningBolt(1, hit, map, main);

            if (main.DamageAtField(appliedDamage, DamageType.air, hit))
            {
                return $"Hit troop at {hit} for {appliedDamage}!";
            }
            else
            {
                return "The spell missed!";
            }
        }
    }

    internal class EarthQuakeSpell : Spell
    {
        private readonly int damage;
        private readonly int radius;

        public EarthQuakeSpell(int damage, int radius) : base("Earthquake", 8, 0, new SpellInformationFormat() { Positions = 1 }, 10)
        {
            this.damage = damage;
            this.radius = radius;
        }

        public override string Activate(SpellInformation information)
        {
            if (!Ready) throw new Exception("Spell is not ready");
            if (!CheckFormat(information)) throw new Exception("Information is incomplete!");

            coolDown = MaxCoolDown;

            int appliedDamage = damage + main.humanPlayer.intelligence / 4;
            int appliedRadius = radius + main.humanPlayer.intelligence / 7;

            Point hit = information.positions[0];

            main.actionOccuring = true;
            for (int x = 0; x < appliedRadius * 2 + 1; x++)
            {
                for (int y = 0; y < appliedRadius * 2 + 1; y++)
                {
                    //Check in bounds
                    Point point = new Point(x + hit.X - appliedRadius, y + hit.Y - appliedRadius);
                    if ((point.X == 0 || point.X >= map.map.GetUpperBound(0) - 1) || (hit.Y == 0 || hit.Y >= map.map.GetUpperBound(1) - 1)) continue;

                    int dis = AIUtility.Distance(point, hit);
                    if (dis <= appliedRadius)
                    {
                        //Damage fields
                        main.DamageAtField(appliedDamage, DamageType.earth, point);
                        new EarthQuakeField(1, point, map, main);
                    }
                }
            }
            main.actionOccuring = false;

            main.RenderMap();
            return $"An earthquake has hit {hit}!";
        }
    }

    //TODO: Spell that decreases you action points
    //TODO: Blindness spell + effect - Eneies are unable to attack

    internal class FireBall : Spell
    {
        //TODO: Initial animation from player to enemy
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
            int appliedDamage = damage + main.humanPlayer.intelligence / 2;
            int appliedTurns = turns + main.humanPlayer.intelligence / 10;

            if (map.map[information.positions[0].X, information.positions[0].Y].type.FType != FieldType.water)
            {
                new Fire(turns, damage, information.positions[0], information.mage.troop.Position, map, main);
                return $"Created a Fireball at {information.positions[0]}";
            }
            else
            {
                main.DamageAtField(damage, DamageType.fire, information.positions[0]);
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

            Player player = main.players.Find(p => p.troop.Position == information.positions[0]);
            Point goal = information.positions[1];
            if (player != null) //Check for player to move
            {
                main.MovePlayer(goal, information.positions[0], player, MovementType.teleport, false);
                main.RenderMap();
                return $"Teleported {player.troop.name} from {information.positions[0]} to {goal}";
            }
            else
            {
                return "The teleport spell failed as there was nothign to teleport!";
            }
        }
    }
}