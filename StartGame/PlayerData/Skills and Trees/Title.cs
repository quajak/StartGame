using StartGame.PlayerData;
using static StartGame.MainGameWindow;

namespace StartGame
{
    internal abstract class Title : Tree
    {
        public Title(string Name, string Description, string Reason) : base(Name, Description, Reason)
        {
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


    internal class Fighter : Skill
    {
        private static readonly int DamageNeeded = 40;
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