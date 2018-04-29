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

        public abstract void Activate();
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

    internal class Rampage : Skill
    {
        private readonly MainGameWindow mainGame;
        private int succesiveTurns = 0;
        private int goal = 3;
        private bool hasKilled = false;
        private bool active = false;

        public Rampage(MainGameWindow mainGame) : base("Rampage", "Get an action point back when killing an enemy.", "Kil enemies on 3 succesive turns.")
        {
            this.mainGame = mainGame;
            if (!mainGame.humanPlayer.trees.Exists(t => t.name == name))
            {
                mainGame.Turn += TurnUpdate;
                mainGame.Combat += CombatUpdate;
            }
        }

        public void CombatUpdate(object sender, CombatData data)
        {
            if (data.killed && mainGame.humanPlayer.Name == data.attacker.Name)
            {
                if (active)
                {
                    mainGame.humanPlayer.actionPoints++;
                    mainGame.ShowPlayerStats();
                }
                else
                    hasKilled = true;
            }
        }

        public override void Activate()
        {
            //Add effect
            active = true;
            mainGame.UpdateTreeView();
        }

        public void TurnUpdate(object sneder, TurnData data)
        {
            if (mainGame.humanPlayer != null && data.active.Name == mainGame.humanPlayer.Name)
            {
                if (hasKilled)
                {
                    succesiveTurns++;
                    if (succesiveTurns == goal)
                    {
                        //pop up
                        mainGame.TreeGained(this);

                        mainGame.humanPlayer.trees.Add(this);
                        Activate();

                        //Remove useless handler
                        mainGame.Turn -= TurnUpdate;
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
        private readonly MainGameWindow mainGame;
        private int toKill = 5;

        public SpiderKiller(MainGameWindow mainGame) : base("Spider Killer", "Do 1 extra damage against spiders.", "Kill 5 spiders.")
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
                    mainGame.UpdateTreeView();
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
            mainGame.PlayerAttack.Add((da) => (da.attacked.GetType() == typeof(WarriorSpiderAI) ? da.damage + 1 : da.damage));
        }
    }

    internal class Sprinter : Skill
    {
        private static int distanceNeeded = 20;
        private int totalDistance = 0;
        private MainGameWindow MainGame;

        public Sprinter(MainGameWindow mainGame) : base("Sprinter", "Improved ability to run long distances. When moving longer than 4 squares, move one extra.", $"Moving {distanceNeeded} fields!")
        {
            MainGame = mainGame;
            if (!mainGame.humanPlayer.trees.Exists(t => t.name == name))
                mainGame.PlayerMoved += Update;
        }

        public void Update(object sender, PlayerMovementData e)
        {
            totalDistance += e.distance;
            if (totalDistance > distanceNeeded)
            {
                MainGame.humanPlayer.trees.Add(this);
                MainGame.UpdateTreeView();

                //Remove listener
                MainGame.PlayerMoved -= Update;

                //Pop uo for skill notice
                MainGame.TreeGained(this);

                Activate();
            }
        }

        public override void Activate()
        {
            //Add effect
            MainGame.CalculateCost.Add((m, d, c) => (d == 5) ? 0 : c);
        }
    }
}