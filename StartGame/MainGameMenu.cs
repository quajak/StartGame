using StartGame.DebugViews;
using StartGame.Dungeons;
using StartGame.Items;
using StartGame.Mission;
using StartGame.PlayerData;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace StartGame
{
    public partial class MainGameMenu : Form
    {
        //Logger
        public static TraceSource log = new TraceSource("MainLog");

        private Map map;
        private Troop playerTroop;

        public MainGameMenu()
        {
            InitializeComponent();
        }

        private void SetMap_Click(object sender, EventArgs e)
        {
            MapCreator mC = new MapCreator();
            if (mC.ShowDialog() == DialogResult.OK)
            {
                map = mC.map;
            }
        }

        private void MainGameMenu_Load(object sender, EventArgs e)
        {
        }

        private void StartGame_Click(object sender, EventArgs e)
        {
            //Long term: allow map selection
            if (map == null)
            {
                if (MessageBox.Show("You have no map selected! \n Starting now will mean using a random map!", "Alert", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    //Start game with random map
                    Random rnd = new Random();
                    map = new Map();
                    Thread mapThread;
                    do
                    {
                        mapThread = new Thread(() => map.SetupMap(new Tuple<double, double, double>(0.1, rnd.Next(), -0.2))) {
                            Priority = ThreadPriority.Highest
                        };
                        mapThread.Start();
                    } while (!mapThread.Join(TimeSpan.FromSeconds(Map.creationTime)));
                }
                else
                {
                    //Allow user to create a map
                    return;
                }
            }
            //Load player
            HumanPlayer player = new HumanPlayer(PlayerType.localHuman, Settings.Default.Name, null, null, null, 0);
            if (playerTroop is null)
            {
                MessageBox.Show("Please create your troop before starting the game!");
                playerTroop = new Troop(Settings.Default.Name, new Weapon(5, BaseAttackType.melee, BaseDamageType.blunt, 1, "Punch", 2, false), Resources.playerTroop, 0, map, player) {
                    armours = new List<Armour>
                    {
                        new Armour("Woolen Tunic", 50, new List<BodyParts>{BodyParts.LeftUpperArm,BodyParts.RightUpperArm,BodyParts.Torso}, Material.Materials.First(m => m.name == "Wool"),Quality.Common, ArmourLayer.clothing),
                        new Armour("Old Pants", 40, new List<BodyParts> { BodyParts.UpperLegs, BodyParts.LeftLowerLeg, BodyParts.RightLowerLeg, BodyParts.LeftShin, BodyParts.RightShin }, Material.Materials.First(m => m.name == "Cloth"), Quality.Poor, ArmourLayer.clothing),
                        new Armour("Wooden Shoes", 32, new List<BodyParts> { BodyParts.LeftFoot, BodyParts.RightFoot }, Material.Materials.First(m => m.name == "Wood"), Quality.Poor, ArmourLayer.light)
                    }
                };
                playerTroop.weapons.Add(new Weapon(50, BaseAttackType.magic, BaseDamageType.magic, 40, "GOD", 10, true));
            }
            player.troop = playerTroop;
            player.troop.armours.ForEach(a => a.active = true);

            player.agility.rawValue = 5;
            player.strength.rawValue = 5;
            player.vitality.rawValue = 20;
            player.intelligence.rawValue = 5;
            player.wisdom.rawValue = 5;
            player.endurance.rawValue = 5;

            Hide();
            //Long term: Make form to allow use to choose mission and difficulty

            Mission.Mission mission = new DragonFight();

            List<Tree> trees = Tree.GenerateTrees();

            MainGameWindow mainGameWindow = new MainGameWindow(map, player, mission, trees, 5, 1);
            try
            {
                mainGameWindow.ShowDialog();
            }
            catch (Exception f)
            {
                Trace.TraceError(f.ToString());
                MessageBox.Show(f.ToString());
            }
            Show();

            //Reset all variables
            map = null;
            player = null;
        }

        private void Quit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Hide();
            HumanPlayer player = new HumanPlayer(PlayerType.localHuman, Settings.Default.Name, null, null, null, 0) {
                troop = playerTroop
            };

            CampaignController campaignCreator = new CampaignController(player);
            campaignCreator.ShowDialog();
            Show();
        }

        private void ArmourCreator_Click(object sender, EventArgs e)
        {
            Hide();
            ItemCreator armourCreator = new ItemCreator();
            armourCreator.ShowDialog();
            Show();
        }

        private void EnterDungeon_Click(object sender, EventArgs e)
        {
            Hide();
            DungeonChooser chooser = new DungeonChooser();
            chooser.ShowDialog();
            if (chooser.selected != null)
            {
                List<Tree> trees = Tree.GenerateTrees();
                HumanPlayer player = new HumanPlayer(PlayerType.localHuman, Settings.Default.Name, null, null, null, 0);
                if (playerTroop is null)
                {
                    playerTroop = new Troop(Settings.Default.Name, new Weapon(5, BaseAttackType.melee, BaseDamageType.blunt, 1, "Punch", 2, false), Resources.playerTroop, 0, map, player) {
                        armours = new List<Armour>
                        {
                        new Armour("Woolen Tunic", 50, new List<BodyParts>{BodyParts.LeftUpperArm,BodyParts.RightUpperArm,BodyParts.Torso}, Material.Materials.First(m => m.name == "Wool"),Quality.Common, ArmourLayer.clothing),
                        new Armour("Old Pants", 40, new List<BodyParts> { BodyParts.UpperLegs, BodyParts.LeftLowerLeg, BodyParts.RightLowerLeg, BodyParts.LeftShin, BodyParts.RightShin }, Material.Materials.First(m => m.name == "Cloth"), Quality.Poor, ArmourLayer.clothing),
                        new Armour("Wooden Shoes", 32, new List<BodyParts> { BodyParts.LeftFoot, BodyParts.RightFoot }, Material.Materials.First(m => m.name == "Wood"), Quality.Poor, ArmourLayer.light)
                    }
                    };
                    playerTroop.weapons.Add(new Weapon(50, BaseAttackType.magic, BaseDamageType.magic, 40, "GOD", 10, true));
                }
                player.troop = playerTroop;
                player.troop.armours.ForEach(a => a.active = true);
                MainGameWindow mainGame = new MainGameWindow(chooser.selected.start.room.map, player, chooser.selected, trees, 1, 1);
                mainGame.ShowDialog();
            }
            Show();
        }

        private void DungeonCreator_Click(object sender, EventArgs e)
        {
            DungeonCreator dungeonCreator = new DungeonCreator();
            Hide();
            dungeonCreator.ShowDialog();
            Show();
        }
    }
}