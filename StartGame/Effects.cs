using PlayerCreator;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame
{
    [DebuggerDisplay("Effect - {name}")]
    internal abstract class Effect : Entity
    {
        private readonly Map map;
        internal int turns;
        internal readonly MainGameWindow main;
        private readonly int ID;

        public static int IDcounter = 0; //The id of an effect is used to identify the render object it is linked to

        public Effect(string Name, Point Position, Bitmap Image, Map Map, int Turns, MainGameWindow main, bool Blocking = false, bool render = true) : base(Name + IDcounter.ToString(), Position, Image, Blocking, Map)
        {
            ID = IDcounter;
            IDcounter++;
            //DEBUG Test -- check that there is no race condition occuring and that names are being set correctly
            if (Name + ID.ToString() != name) throw new Exception("Setting of effect name went wrong!");

            map = Map;
            map.entites.Add(this);

            lock (map.RenderController)
            {
                map.renderObjects.Add(new EntityRenderObject(this, new TeleportPointAnimation(new Point(0, 0), Position)));
            }
            if (render)
                main.RenderMap();
            turns = Turns;
            this.main = main;
            main.Turn += Main_Turn;
        }

        internal virtual void Main_Turn(object sender, MainGameWindow.TurnData e)
        {
            turns--;
            if (turns <= 0)
            {
                //Delete
                BaseDelete();
                Delete();
            }
        }

        internal void BaseDelete()
        {
            map.entites.Remove(this);
            map.RemoveEntityRenderObject(name);

            main.Turn -= Main_Turn;
        }

        internal abstract void Delete();
    }

    internal class EarthQuakeField : Effect
    {
        public EarthQuakeField(int Turns, Point point, Map map, MainGameWindow main) : base("EarthQuakeField", point, Resources.EarthQuake, map, Turns, main, render: false)
        {
        }

        internal override void Delete()
        {
        }
    }

    internal class LightningBolt : Effect
    {
        public LightningBolt(int Turns, Point point, Map map, MainGameWindow main) : base("LightningBolt", point, Resources.LightningBolt, map, Turns, main, render: false)
        {
        }

        internal override void Delete()
        {
        }
    }

    internal class Fire : Effect
    {
        private readonly int damage;

        public Fire(int Turns, int Damage, Point Position, Point SpawnPosition, Map Map, MainGameWindow main) : base("Fire", SpawnPosition, Resources.Fire, Map, Turns, main, render: false)
        {
            //Can it actually exist?
            if (Map.map.Get(Position).type.FType == FieldType.water)
            {
                //Destroy it
                BaseDelete();
                Delete();
                return;
            }

            //Set position to actual position - to create effect that the mage created the fireball
            this.Position = Position;
            main.RenderMap();

            damage = Damage;
            this.main.PlayerMoved += Main_PlayerMoved;

            //Check if put on position of entity
            var f = Map.troops.Where(t => t.Position == Position).ToList();
            if (f.Count != 0)
            {
                Player player = main.players.First(p => p.troop == f[0]);
                string playerName = player.Name;
                main.WriteConsole($"{playerName} has been put on fire!");
                f[0].statuses.Add(new FireStatus(turns + 2, damage, main, player));
                if (playerName == main.humanPlayer.Name)
                    main.UpdatePlayerView();
            }
        }

        private void Main_PlayerMoved(object sender, MainGameWindow.PlayerMovementData e)
        {
            Point humanPoint = e.start.position;

            foreach (Point point in e.path)
            {
                humanPoint = humanPoint.Add(point);
                if (humanPoint == Position)
                {
                    if (!e.player.troop.statuses.Exists(s => s.name == name))
                    {
                        main.WriteConsole($"{e.player.Name} has been put on fire!");
                        e.player.troop.statuses.Add(new FireStatus(turns + 1, damage, main, e.player));
                        if (e.player.Name == main.humanPlayer.Name)
                        {
                            main.UpdatePlayerView();
                        }
                    }
                    else
                    {
                        e.player.troop.statuses.Find(s => s.name == name).turns = turns + 1;
                    }
                }
            }
        }

        internal override void Delete()
        {
            main.PlayerMoved -= Main_PlayerMoved;
        }
    }

    internal abstract class Status
    {
        public readonly string name;

        public int? turns;
        internal readonly Player player;

        public Status(string Name, int? Turns, Player Player)
        {
            turns = Turns;
            player = Player;
            name = Name;
        }

        public abstract string Description();
    }

    internal class DebuffStatus : Status
    {
        private readonly int strength;
        private readonly MainGameWindow main;

        public DebuffStatus(int Turns, int Strength, MainGameWindow main, Player player) : base("Debuff", Turns, player)
        {
            strength = Strength;
            this.main = main;
            this.main.Turn += Main_Turn;
            player.InitialiseTurnHandler += Player_InitialiseTurnHandler;
            player.troop.statuses.Add(this);
            main.UpdatePlayerView();
        }

        private void Player_InitialiseTurnHandler(object sender, EventArgs e)
        {
            (sender as Player).actionPoints -= strength;
        }

        private void Main_Turn(object sender, MainGameWindow.TurnData e)
        {
            var main = sender as MainGameWindow;
            if (e.active.Name != player.Name) return;

            //Handle turn countdown
            if (turns == null) return;

            turns--;
            if (turns == 0)
            {
                player.InitialiseTurnHandler -= Player_InitialiseTurnHandler;
                player.troop.statuses.Remove(this);
                main.UpdatePlayerView();
            }
        }

        public override string Description()
        {
            return $" You have been debuffed and have {strength} action points less every turn!";
        }
    }

    internal class FireStatus : Status
    {
        private readonly int damage;

        public FireStatus(int? Turns, int Damage, MainGameWindow main, Player player) : base("Fire", Turns, player)
        {
            main.Turn += Main_Turn;
            main.PlayerMoved += Main_PlayerMoved;
            damage = Damage;
        }

        private void Main_PlayerMoved(object sender, MainGameWindow.PlayerMovementData e)
        {
            var main = sender as MainGameWindow;
            if (e.player.Name == player.Name && e.goal.type.FType == FieldType.water)
            {
                Point start = e.start.position;
                foreach (var field in e.path)
                {
                    start = start.Add(field);
                    if (main.map.map.Get(start).type.FType == FieldType.water)
                    {
                        RemoveEffect(sender as MainGameWindow);
                        return;
                    }
                }
            }
        }

        private void Main_Turn(object sender, MainGameWindow.TurnData e)
        {
            var main = sender as MainGameWindow;
            if (e.active.Name != player.Name) return;

            //Do effect
            main.DamagePlayer(damage, DamageType.fire, player);

            //Handle turn countdown
            if (turns == null) return;

            turns--;
            if (turns == 0)
            {
                RemoveEffect(main);
            }
        }

        private void RemoveEffect(MainGameWindow main)
        {
            main.Turn -= Main_Turn;
            main.PlayerMoved -= Main_PlayerMoved;
            player.troop.statuses.Remove(this);
            main.UpdatePlayerView();
        }

        public override string Description()
        {
            return $"This troop is on fire for {(turns != null ? $"the next {turns} {(turns > 1 ? "turns" : "turn")}" : "for ever")}. It deals {damage} per turn.";
        }
    }
}