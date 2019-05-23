using System;
using static StartGame.MainGameWindow;

namespace StartGame
{
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
                    main.humanPlayer.actionPoints.RawValue += level;
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

    internal class Tank : Skill
    {
        private int activation = 40;

        public Tank() : base("Tank", "Absorb more damage.", "Loose 40 health.", 2, 40)
        {
            activateOnLevelUp = true;
        }

        public override void Initialise(MainGameWindow mainGame)
        {
            main = mainGame;
            main.Combat += CombatUpdate;
        }

        public void CombatUpdate(object sender, CombatData data)
        {
            if (main.humanPlayer.Name == data.attacked.Name && !data.doged)
            {
                if(level == 0)
                {
                    activation -= data.damage;
                    if(activation <= 0)
                    {
                        Activate();
                        main.TreeGained(this);
                    }
                }
                else
                {
                    Xp += data.damage;
                }
            }
        }

        public override void Activate()
        {
            //Add effect
            main.humanPlayer.trees.Add(this);

            main.UpdatePlayerView();
            level = level == 0 ? 1 : level;
            if(main.humanPlayer.endurance.buffs.Exists(b => b.Name == name))
            {
                main.humanPlayer.endurance.buffs.Find(b => b.Name == name).value = level * 3;
            }
            else
            {
                main.humanPlayer.endurance.buffs.Add(new PlayerData.Buff(PlayerData.BuffType.Constant, 3, name));
            }
        }
    }
    internal class Sprinter : Skill
    {
        private static readonly int distanceNeeded = 100;
        private int totalDistance = 0;
        private int lastLevel = 0;

        public Sprinter() : base("Sprinter", "Improved ability to run long distances. Increases the movement points", $"Moving {distanceNeeded} fields!", 2, distanceNeeded, 10)
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
            if (main.humanPlayer is null) return;
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

    internal class Mage : Skill
    {
        private static int manaNeeded = 10;

        public Mage() : base("Mage", "Improved availability of mana.", $"Using {manaNeeded} mana!", 3, manaNeeded, 10)
        {
            activateOnLevelUp = true;
        }

        public override void Initialise(MainGameWindow mainGame)
        {
            main = mainGame;
            main.SpellCasted += Main_SpellCasted; ;
        }

        private void Main_SpellCasted(object sender, SpellCast e)
        {
            if(e.caster.Name == main.humanPlayer.Name)
            {
                manaNeeded -= e.spell.manaCost;
                if(manaNeeded <= 0 && level == 0)
                {
                    level = 1;
                    main.humanPlayer.trees.Add(this);
                    main.UpdatePlayerView();

                    main.TreeGained(this);
                } else if(manaNeeded <= 0)
                {
                    Xp += e.spell.manaCost;
                }
            }
        }

        public override void Activate()
        {
            //Add effect
            main.humanPlayer.mana.rawMaxValue += level;
        }
    }

    internal class Acrobat : Skill
    {
        private static int minimumDodges = 4;

        public Acrobat() : base("Acrobat", "Improved your dodging ability.", $"Dodge some attacks!", 1.4, minimumDodges, 5)
        {
            activateOnLevelUp = true;
        }

        public override void Initialise(MainGameWindow mainGame)
        {
            main = mainGame;
            main.Combat += Main_Combat;
        }

        private void Main_Combat(object sender, CombatData e)
        {
            if(e.attacked.Name == main.humanPlayer.Name && e.doged)
            {
                minimumDodges--;
                if(minimumDodges <= 0 && level == 0)
                {
                    level = 1;
                    main.humanPlayer.trees.Add(this);
                    main.UpdatePlayerView();

                    main.TreeGained(this);
                } else if( minimumDodges <= 0)
                {
                    Xp++;
                }
            }
        }

        private void Main_SpellCasted(object sender, SpellCast e)
        {
            if (e.caster.Name == main.humanPlayer.Name)
            {
                minimumDodges -= e.spell.manaCost;
                if (minimumDodges <= 0 && level == 0)
                {
                    level = 1;
                    main.humanPlayer.trees.Add(this);
                    main.UpdatePlayerView();

                    main.TreeGained(this);
                }
                else if (minimumDodges <= 0)
                {
                    Xp += e.spell.manaCost;
                }
            }
        }

        public override void Activate()
        {
            //Add effect
            if (!main.humanPlayer.agility.buffs.Exists(b => b.Name == name))
                main.humanPlayer.agility.buffs.Add(new PlayerData.Buff(PlayerData.BuffType.Constant, 3, name));
            else
                main.humanPlayer.agility.buffs.Find(b => b.Name == name).value++;
        }
    }



}