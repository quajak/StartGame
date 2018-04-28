using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StartGame.MainGameWindow;

namespace StartGame
{
    internal abstract class Tree
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
    }

    internal abstract class Skill : Tree
    {
        public Skill(string Name, string Description, string Reason) : base(Name, Description, Reason)
        {
        }
    }

    internal abstract class Title : Tree
    {
        public Title(string Name, string Description, string Reason) : base(Name, Description, Reason)
        {
        }
    }

    internal class SpiderKiller : Title
    {
        private readonly MainGameWindow mainGame;
        private int toKill = 5;

        public SpiderKiller(MainGameWindow mainGame) : base("Spider Killer", "Do 1 extra damage against spiders.", "Kill 5 spiders.")
        {
            this.mainGame = mainGame;
            mainGame.Combat += Update;
        }

        public void Update(object sender, CombatData data)
        {
            if (data.killed && data.attacked.GetType() == typeof(WarriorSpiderAI))
            {
                toKill--;
                if (toKill == 0)
                {
                    //Pop up for title notice
                    mainGame.TreeGained(this);

                    //Add effect
                    mainGame.PlayerAttack.Add((da) => (da.attacked.GetType() == typeof(WarriorSpiderAI) ? da.damage + 1 : da.damage));
                    mainGame.humanPlayer.trees.Add(this);
                    mainGame.UpdateTreeView();

                    //Remove listener
                    mainGame.Combat -= Update;
                }
            }
        }
    }

    internal class Sprinter : Skill
    {
        private static int distanceNeeded = 2;
        private int totalDistance = 0;
        private MainGameWindow MainGame;

        public Sprinter(MainGameWindow mainGame) : base("Sprinter", "Improved ability to run long distances. When moving longer than 4 squares, move one extra.", $"Moving {distanceNeeded} fields!")
        {
            mainGame.PlayerMoved += Update;
            MainGame = mainGame;
        }

        public void Update(object sender, PlayerMovementData e)
        {
            totalDistance += e.distance;
            if (totalDistance > distanceNeeded)
            {
                //Pop uo for skill notice
                MainGame.TreeGained(this);

                //Add effect
                MainGame.CalculateCost.Add((m, d, c) => (d == 5) ? 0 : c);
                MainGame.humanPlayer.trees.Add(this);
                MainGame.UpdateTreeView();

                //Remove listener
                MainGame.PlayerMoved -= Update;
            }
        }
    }
}