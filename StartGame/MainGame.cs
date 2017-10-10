using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StartGame
{
    partial class MainGameWindow : Form
    {
        //Debug section
        private bool showRawPerlin = true; //Will show the perlin multiplier on black - not different terrain

        //End

        private Random random;
        private TroopType[,] TroopTypeSquare;
        private Bitmap troopCreatorOverlay;
        private Image troopCreatorBitmap;

        private Map map;

        public Troop[] troops;
        public TroopType[] troopTypes;

        private Point selected;

        private Player[] players;
        private Player activePlayer;
        private Player humanPlayer;
        private int activePlayerCounter = 0;

        Troop toPlace;

        public MainGameWindow(Map Map, Player Player, int AINumber)
        {
            Troop infantry = new Troop(new bool[,] { { false, false, false }, { false, true, false }, { false, false, false } },
                TroopTypeEnum.infantry,
                "Infantry");
            Troop tank = new Troop(new bool[,] { { true, true, false }, { true, true, false }, { false, false, false } },
                TroopTypeEnum.cavalary,
                "Tank");
            Troop outpost = new Troop(new bool[,] { { false, false, false }, { false, true, false }, { false, false, false } },
                TroopTypeEnum.building,
                "Outpost");
            Troop Base = new Troop(new bool[,] { { false, true, false }, { true, true, true }, { false, true, false } },
                TroopTypeEnum.building,
                "Base");
            troops = new Troop[] { infantry, tank, outpost, Base };

            map = Map;

            selected = new Point(-1, 0);
            random = new Random();

            //Setup players
            players = new Player[AINumber + 1];
            players[0] = Player;
            humanPlayer = Player;
            //Generate AIs
            short botNumber = Convert.ToInt16(Resources.BOTAmount);
            List<string> botNames = new List<string>();
            for (int i = 0; i < botNumber; i++)
            {
                botNames.Add(Resources.ResourceManager.GetString("BOTName" + i));
            }
            for (int i = 0; i < AINumber; i++)
            {
                string name = botNames[random.Next(botNames.Count)];
                players[i + 1] = new Player(PlayerType.computer, name);
                botNames.Remove(name);
            }
            activePlayer = players[0];


            InitializeComponent();

            //Start work to update information in GUI

            //Add players to list
            for (int i = 0; i < players.Length; i++)
            {
                playerList.Items.Add(players[i].Name);
            }

            //Initialise information about player
            playerName.Text = humanPlayer.Name;

            //As it is first turn - set action button to start the game
            nextAction.Text = "Start game!";
        }

        private void MainGameWindow_Load(object sender, EventArgs e)
        {
            SetupTroopGame();
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height);
        }

        private void SetupTroopGame()
        {
            //Generate the field
            int xNum = troopCreator.Width / 20;
            int yNum = troopCreator.Height / 20;
            Color[] troopTypeFillColors = { Color.Red, Color.SandyBrown, Color.Blue, Color.DimGray };
            Color[] troopTypeBorderColors = { Color.DarkRed, Color.SaddleBrown, Color.DarkBlue, Color.DarkGray };
            TroopTypeSquare = new TroopType[xNum, yNum];

            troopTypes = new TroopType[4];

            for (int i = 0; i < 4; i++)
            {
                troopTypes[i] = new TroopType
                {
                    troopType = (TroopTypeEnum)i,
                    borderColor = troopTypeBorderColors[i],
                    fillColor = troopTypeFillColors[i]
                };
            }

            for (int x = 0; x < xNum; x++)
            {
                for (int y = 0; y < yNum; y++)
                {
                    TroopTypeEnum troopTypeEnum = (TroopTypeEnum)random.Next(0, 4);
                    TroopType troopType = new TroopType
                    {
                        troopType = troopTypeEnum,
                        fillColor = troopTypeFillColors[(int)troopTypeEnum],
                        borderColor = troopTypeBorderColors[(int)troopTypeEnum]
                    };
                    TroopTypeSquare[x, y] = troopType;
                }
            }
            //Initialise the graphics
            troopCreatorBitmap = new Bitmap(troopCreator.Width, troopCreator.Height);
            troopCreatorOverlay = new Bitmap(troopCreator.Width, troopCreator.Height);

            DrawTroopCreatorImage();
        }

        private void UpdateGameBoard()
        {
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, showRawPerlin);
        }

        private void DrawTroopCreatorImage()
        {
            //Draw the grapics
            using (Graphics g = Graphics.FromImage(troopCreatorBitmap))
            {
                g.Clear(Color.Black);
                for (int x = 0; x < troopCreator.Width / 20; x++)
                {
                    for (int y = 0; y < troopCreator.Height / 20; y++)
                    {
                        g.FillRectangle(new SolidBrush(TroopTypeSquare[x, y].fillColor), x * 20, y * 20, 19, 19);
                        g.DrawRectangle(new Pen(TroopTypeSquare[x, y].borderColor), x * 20, y * 20, 19, 19);
                    }
                }
            }
            UpdateTroopCreatorImage();
        }

        private void UpdateTroopCreatorImage()
        {
            troopCreator.Image = new Bitmap(troopCreator.Width, troopCreator.Height);
            using (Graphics g = Graphics.FromImage(troopCreator.Image))
            {
                g.DrawImage(troopCreatorBitmap, new PointF(0, 0));
                g.DrawImage(troopCreatorOverlay, new PointF(0, 0));
            }
        }

        private void TroopCreator_Click(object sender, EventArgs e)
        {
        }

        private void TroopCreator_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / 20;
            int y = e.Y / 20;
            using (Graphics g = Graphics.FromImage(troopCreatorOverlay))
            {
                g.Clear(Color.Transparent);
                if (this.selected.X == -1)
                {
                    this.selected = new Point(x, y);
                    g.DrawRectangle(Pens.Gold, x * 20, y * 20, 19, 19);
                }
                else
                {
                    TroopType troopType = TroopTypeSquare[selected.X, selected.Y];
                    TroopType troopType1 = TroopTypeSquare[x, y];
                    TroopTypeSquare[x, y] = troopType; ;
                    TroopTypeSquare[selected.X, selected.Y] = troopType1; ;
                    DrawTroopCreatorImage();
                    this.selected.X = -1;
                }
            }
            UpdateTroopCreatorImage();
        }

        private void SpawnTroop_Click(object sender, EventArgs e)
        {
            if (selected.X != -1)
            {
                //Find troop type of middle
                TroopTypeEnum troopMiddle = TroopTypeSquare[selected.X, selected.Y].troopType;
                //Create bool array
                bool[,] boolArray = new bool[3, 3];
                for (int diffX = -1; diffX < 2; diffX++)
                {
                    int x = diffX + selected.X;
                    if (x < 0 || x > 19) continue;
                    for (int diffY = -1; diffY < 2; diffY++)
                    {
                        int y = diffY + selected.Y;
                        if (y < 0 || y > 19) continue;
                        boolArray[diffX + 1, diffY + 1] = TroopTypeSquare[x, y].troopType == troopMiddle;
                    }
                }
                //Compare with troops
                foreach (Troop troop in troops)
                {
                    if (troop.type == troopMiddle)
                    {
                        bool equal = true;
                        //used to check if 2d bool arrays are equal
                        for (int x = 0; x < 3; x++)
                        {
                            for (int y = 0; y < 3; y++)
                            {
                                if (troop.form[x, y] == boolArray[x, y])
                                {

                                }
                                else
                                {
                                    equal = false;
                                }
                            }
                            if (!equal) break;
                        }
                        if (!equal) continue;
                        //Spawn
                        nextAction.Text = "Spawn building!";
                        toPlace = troop;
                        //Delete in the square
                        for (int x = -1; x < 2; x++)
                        {
                            for (int y = -1; y < 2; y++)
                            {
                                if (troop.form[x, y])
                                {
                                    int X = x + selected.X;
                                    int Y = y + selected.Y;
                                    if (X < 0 || X > 19) continue;
                                    if (Y < 0 || Y > 19) continue;

                                    TroopTypeSquare[X, Y] = new TroopType
                                    {
                                        borderColor = Color.White,
                                        fillColor = Color.White,
                                        troopType = TroopTypeEnum.empty
                                    };
                                }
                            }
                        }
                        //let fall squares 
                        for (int x = 0; x < 3; x++)
                        {
                            for (int y = 3; y > 0; y++)
                            {
                                if (y != 0 && TroopTypeSquare[x, y].troopType == TroopTypeEnum.empty)
                                {
                                    TroopTypeSquare[x, y] = TroopTypeSquare[x, y - 1];
                                    TroopTypeSquare[x, y - 1] = new TroopType
                                    {
                                        troopType = TroopTypeEnum.empty,
                                        borderColor = Color.White,
                                        fillColor = Color.White
                                    };
                                }
                            }
                        }
                        //Update square
                        DrawTroopCreatorImage();
                    }
                }
            }
        }

        private void SpawnTroop_MouseClick(object sender, MouseEventArgs e)
        {
        }

        public void NextTurn()
        {
            activePlayer = players[activePlayerCounter];
            activePlayer.PlayTurn(nextAction, activateSpell, spawnTroop);
            activePlayerCounter = activePlayerCounter == players.Length - 1 ? 0 : activePlayerCounter + 1;
        }

        private void NextAction_Click(object sender, EventArgs e)
        {
            activePlayer.ActionButtonPressed(this);
        }

        private void ShowTroopTypes_Click(object sender, EventArgs e)
        {
            Hide();
            TroopViewer troopViewer = new TroopViewer(this);
            troopViewer.ShowDialog();
            Show();
        }

        private void PlayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (playerList.SelectedIndex == -1) return;
            Player player = players[playerList.SelectedIndex];
            if (player.Name == activePlayer.Name)
            {
                //Clear all data
                enemyName.Text = "";
            }
            else
            {
                enemyName.Text = player.Name;
            }
        }

        private void MainGameWindow_MouseMove(object sender, MouseEventArgs e)
        {
        }
    }
}