using StartGame.PlayerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StartGame.MainGameWindow;

namespace StartGame
{
    public abstract class Tree
    {
        public string name;

        public readonly string description;
        public readonly string reason;

        public Tree(string Name, string Description, string Reason)
        {
            name = Name;
            description = Description;
            reason = Reason;
        }

        public abstract void Initialise(MainGameWindow mainGame);

        public abstract void Activate();

        public static List<Tree> GenerateTrees()
        {
            return new List<Tree>()
            {
                new Sprinter(),
                new Rampage(),
                new SpiderKiller(),
                new Fighter()
            };
        }
    }

    //TODO: Description of skills are not hard coded but a function
    public abstract class Skill : Tree
    {
        private readonly int minGrowth;
        public int level;
        private int xp;
        public int maxXP;
        internal MainGameWindow main;
        private readonly double growthFactor;
        internal bool activateOnLevelUp = false;

        public int Xp
        {
            get => xp;
            set
            {
                xp = value;
                bool levelup = false;
                while (xp >= maxXP)
                {
                    levelup = true;
                    xp -= maxXP;
                    level++;
                    maxXP = (int)Math.Max(maxXP + minGrowth, maxXP * growthFactor);
                }
                if (!(main is null)) main.UpdatePlayerView();
                if (levelup) main.SkillLevelUp(this);
                if (levelup && activateOnLevelUp) Activate();
            }
        }

        public Skill(string Name, string Description, string Reason, double growthFactor, int maxXP, int minGrowth = 1) : base(Name, Description, Reason)
        {
            this.growthFactor = growthFactor;
            this.maxXP = maxXP;
            Xp = 0;
            level = 0;
            this.minGrowth = minGrowth;
        }
    }

    internal abstract class Title : Tree
    {
        public Title(string Name, string Description, string Reason) : base(Name, Description, Reason)
        {
        }
    }

    internal class Rampage : Skill
    {
        private int succesiveTurns = 0;
        private readonly int goal = 3;
        private bool hasKilled = false;
        private bool active = false;

        public Rampage() : base("Rampage", "Get an action point back when killing an enemy.", "Kill enemies on 3 succesive turns.", 2, 2)
        {
        }

        public override void Initialise(MainGameWindow mainGame)
        {
            main = mainGame;
            main.Turn += TurnUpdate;
            main.Combat += CombatUpdate;
        }

        public void CombatUpdate(object sender, CombatData data)
        {
            if (data.killed && main.humanPlayer.Name == data.attacker.Name)
            {
                if (active)
                {
                    main.humanPlayer.actionPoints.rawValue += level;
                    main.ShowPlayerStats();
                }
                hasKilled = true;
            }
        }

        public override void Activate()
        {
            //Add effect
            active = true;
            main.UpdatePlayerView();
            level = level == 0 ? 1 : level;
        }

        public void TurnUpdate(object sender, TurnData data)
        {
            if (main.humanPlayer != null && data.active.Name == main.humanPlayer.Name)
            {
                if (hasKilled)
                {
                    succesiveTurns++;
                    if (succesiveTurns == goal && level == 0)
                    {
                        level = 1;
                        //pop up
                        main.TreeGained(this);

                        main.humanPlayer.trees.Add(this);
                        Activate();
                    }
                    else if (succesiveTurns >= goal)
                    {
                        Xp += succesiveTurns - goal;
                    }
                    hasKilled = false;
                }
                else
                {
                    succesiveTurns = 0;
                }
            }
        }
    }

    internal class SpiderKiller : Title
    {
        private MainGameWindow mainGame;
        private int toKill = 5;

        public SpiderKiller() : base("Spider Killer", "Do 1 extra damage against spiders.", "Kill 5 spiders.")
        {
        }

        public override void Initialise(MainGameWindow mainGame)
        {
            this.mainGame = mainGame;
            if (!mainGame.humanPlayer.trees.Exists(t => t.name == name))
                mainGame.Combat += Update;
        }

        public void Update(object sender, CombatData data)
        {
            if (data.killed && data.attacked.GetType() == typeof(WarriorSpiderAI))
            {
                toKill--;
                if (toKill == 0)

                {
                    mainGame.humanPlayer.trees.Add(this);
                    mainGame.UpdatePlayerView();
                    //Pop up for title notice
                    mainGame.TreeGained(this);

                    //Remove listener
                    mainGame.Combat -= Update;

                    Activate();
                }
            }
        }

        public override void Activate()
        {
            //Add effect
            mainGame.CalculatePlayerAttackDamage.Add((da) => (da.attacked.GetType() == typeof(WarriorSpiderAI) ? da.damage + 1 : da.damage));
        }
    }

    internal class Sprinter : Skill
    {
        private static int distanceNeeded = 10;
        private int totalDistance = 0;
        private int lastLevel = 0;

        public Sprinter() : base("Sprinter", "Improved ability to run long distances. Increases the movement points", $"Moving {distanceNeeded} fields!", 2, 20, 10)
        {
            activateOnLevelUp = true;
        }

        public override void Initialise(MainGameWindow mainGame)
        {
            main = mainGame;
            main.PlayerMoved += Update;
        }

        public void Update(object sender, PlayerMovementData e)
        {
            if (e.player.Name == main.humanPlayer.Name && e.movementType == MovementType.walk)
            {
                totalDistance += e.distance;
                if (totalDistance > distanceNeeded && level == 0)
                {
                    level = 1;
                    main.humanPlayer.trees.Add(this);
                    main.UpdatePlayerView();

                    //Pop up for skill notice
                    main.TreeGained(this);

                    Activate();
                }
                else
                {
                    Xp += e.distance;
                }
            }
        }

        public override void Activate()
        {
            //Add effect
            main.humanPlayer.movementPoints.maxExtraMovement += (level - lastLevel);
            lastLevel = level;
        }
    }

    internal class Fighter : Skill
    {
        private static int DamageNeeded = 40;
        private int damageDealt = 0;

        public Fighter() : base("Fighter", "Deal more damage against all enemies.", $"Deal {DamageNeeded} damage.", 1.2, 40, 5)
        {
        }

        public override void Initialise(MainGameWindow mainGame)
        {
            main = mainGame;
            main.Combat += Combat;
        }

        private void Combat(object sender, CombatData e)
        {
            if (e.attacker.Name == main.humanPlayer.Name && e.doged != false)
            {
                damageDealt += e.damage;
                if (damageDealt >= DamageNeeded && level == 0)
                {
                    level = 1;
                    main.humanPlayer.trees.Add(this);
                    main.UpdatePlayerView();

                    //Pop up
                    main.TreeGained(this);

                    Activate();
                }
                else
                {
                    Xp += e.damage;
                }
            }
        }

        public override void Activate()
        {
            main.CalculatePlayerAttackDamage.Add((cd) => cd.damage += level);
        }
    }
}