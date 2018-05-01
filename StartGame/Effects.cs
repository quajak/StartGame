using PlayerCreator;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame
{
    internal abstract class Effect : Entity
    {
        private readonly Map map;
        internal int turns;
        internal readonly MainGameWindow main;

        public Effect(string Name, Point Position, Bitmap Image, Map Map, int Turns, MainGameWindow main, bool Blocking = false) : base(Name, Position, Image, Blocking, Map)
        {
            Map.entites.Add(this);
            main.UpdateGameBoard();
            map = Map;
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
                map.entites.Remove(this);

                main.Turn -= Main_Turn;
            }
        }
    }

    internal class Fire : Effect
    {
        private readonly int damage;

        public Fire(int Turns, int Damage, Point Position, Map Map, MainGameWindow main) : base("Fire", Position, Resources.Fire, Map, Turns, main)
        {
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
                    main.UpdateStatusList();
            }
        }

        private void Main_PlayerMoved(object sender, MainGameWindow.PlayerMovementData e)
        {
            if (e.goal.position == Position)
            {
                if (!e.player.troop.statuses.Exists(s => s.name == name))
                {
                    main.WriteConsole($"{e.player.Name} has been put on fire!");
                    e.player.troop.statuses.Add(new FireStatus(turns + 1, damage, main, e.player));
                    if (e.player.Name == main.humanPlayer.Name)
                    {
                        main.UpdateStatusList();
                    }
                }
                else
                {
                    e.player.troop.statuses.Find(s => s.name == name).turns = turns + 1;
                }
            }
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
            if (e.player.Name == player.Name && e.goal.type.FType == FieldType.water)
            {
                RemoveEffect(sender as MainGameWindow);
            }
        }

        private void Main_Turn(object sender, MainGameWindow.TurnData e)
        {
            var main = sender as MainGameWindow;
            if (e.active.Name != player.Name) return;

            //Do effect
            main.DamagePlayer(damage, player);

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
            main.UpdateStatusList();
        }

        public override string Description()
        {
            return $"This troop is on fire for {(turns != null ? $"the next {turns} {(turns > 1 ? "turns" : "turn")}" : "for ever")}. It deals {damage} per turn.";
        }
    }
}