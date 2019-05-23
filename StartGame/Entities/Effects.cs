using StartGame.PlayerData;
using StartGame.Properties;
using StartGame.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using StartGame.GameMap;


namespace StartGame.Entities
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
            if (Name + ID.ToString() != base.Name) throw new Exception("Setting of effect name went wrong!");

            map = Map;
            map.entities.Add(this);

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
            map.entities.Remove(this);
            map.RemoveEntityRenderObject(Name);

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

    internal class IceSpike : Effect
    {
        public IceSpike(int Turns, Point point, Point SpawnPosition, Map map, MainGameWindow main) : base("Ice Spike", SpawnPosition, Resources.IceSpike, map, Turns, main, render: false)
        {
            //TODO: Do this for every single step of the animation
            int dX = SpawnPosition.X - point.X;
            int dY = SpawnPosition.Y - point.Y;
            if (Math.Abs(dX) > Math.Abs(dY))
            {
                if (dX < 0)
                {
                    Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
                else
                {
                    Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
            }
            else
            {
                if (dY < 0)
                {
                    Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                else
                {
                }
            }

            Position = point; //So animation is created
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
            List<Troop> f = Map.troops.Where(t => t.Position == Position).ToList();
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
                    if (!e.player.troop.statuses.Exists(s => s.name == Name))
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
                        e.player.troop.statuses.Find(s => s.name == Name).turns = turns + 1;
                    }
                }
            }
        }

        internal override void Delete()
        {
            main.PlayerMoved -= Main_PlayerMoved;
        }
    }

    public abstract class Status
    {
        public readonly string name;

        public int? turns;
        internal readonly Player player;
        //TODO: Automatically add to statuses

        public Status(string Name, int? Turns, Player Player)
        {
            turns = Turns;
            player = Player;
            name = Name;
        }

        public abstract string Description();
    }

    public class DebuffStatus : Status
    {
        private readonly int strength;
        private readonly MainGameWindow main;

        public DebuffStatus(int Turns, int Strength, MainGameWindow main, Player player) : base("Debuff", Turns, player)
        {
            strength = Strength;
            this.main = main;
            this.main.Turn += Main_Turn;
            player.actionPoints.rawMaxValue -= strength;
            player.troop.statuses.Add(this);
            main.UpdatePlayerView();
        }

        private void Main_Turn(object sender, MainGameWindow.TurnData e)
        {
            MainGameWindow main = sender as MainGameWindow;
            if (e.active.Name != player.Name) return;

            //Handle turn countdown
            if (turns == null) return;

            turns--;
            if (turns == 0)
            {
                player.actionPoints.rawMaxValue += strength;
                player.troop.statuses.Remove(this);
                main.UpdatePlayerView();
                this.main.Turn -= Main_Turn;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Turns"></param>
        /// <param name="Damage"></param>
        /// <param name="main"></param>
        /// <param name="affected">The player who was affected</param>
        public FireStatus(int? Turns, int Damage, MainGameWindow main, Player affected) : base("Fire", Turns, affected)
        {
            main.Turn += Main_Turn;
            main.PlayerMoved += Main_PlayerMoved;
            damage = Damage;
        }

        private void Main_PlayerMoved(object sender, MainGameWindow.PlayerMovementData e)
        {
            MainGameWindow main = sender as MainGameWindow;
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
            MainGameWindow main = sender as MainGameWindow;
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

    internal class OverweightStatus : Status
    {
        private int level;

        public OverweightStatus(Player player) : base("Overweight", null, player)
        {
            player.troop.statuses.Add(this);
            player.InitialiseTurnHandler += Player_InitialiseTurnHandler;
        }

        private void Player_InitialiseTurnHandler(object sender, EventArgs e)
        {
            CalculateLevel();
            player.actionPoints.RawValue -= level;
        }

        private void CalculateLevel()
        {
            level = (player.gearWeight.Value - player.gearWeight.MaxValue().Value) / (player.gearWeight.MaxValue().Value / 4) + 1;
        }

        public override string Description()
        {
            CalculateLevel();
            return $"You are overweight as you are using too much armour right now. You have {level} less action points each turn.";
        }

        public void RemoveEffect()
        {
            player.InitialiseTurnHandler -= Player_InitialiseTurnHandler;
            player.troop.statuses.Remove(this);
        }
    }
}