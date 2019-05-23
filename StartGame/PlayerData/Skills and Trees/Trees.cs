using StartGame.PlayerData;
using System.Collections.Generic;

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
                new Fighter(),
                new Mage(),
                new Acrobat()
            };
        }
    }
    

    
}