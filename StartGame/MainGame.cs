﻿using PlayerCreator;
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
        private Random random;

        private Map map;

        private Point selected;

        private Player[] players;
        private Player activePlayer;
        private Player humanPlayer;
        private int activePlayerCounter = 0;

        public MainGameWindow(Map Map, Player Player)
        {
            map = Map;

            selected = new Point(-1, 0);
            random = new Random();

            //Setup players
            players = new Player[2];

            //Setup human player
            players[0] = Player;
            humanPlayer = Player;
            map.troops.Add(humanPlayer.troop);
            //Get position for player troops
            List<Point> startPos = map.DeterminSpawnPoint(1, SpawnType.road);
            players[0].troop.position = startPos[0];

            //Generate AI
            //Get name of troop
            short botNumber = Convert.ToInt16(Resources.BOTAmount);
            List<string> botNames = new List<string>();
            for (int i = 0; i < botNumber; i++)
            {
                botNames.Add(Resources.ResourceManager.GetString("BOTName" + i));
            }
            string name = botNames[random.Next(botNames.Count)];
            players[1] = new Player(PlayerType.computer, name, map, new Player[] { humanPlayer })
            {
                troop = new Troop(name, new Weapon(2, AttackType.melee, 1, "Fists"), Resources.enemyScout)
            };
            players[1].troop.position = map.DeterminSpawnPoint(1, SpawnType.randomLand)[0];
            map.troops.Add(players[1].troop);

            //Setup game
            activePlayer = players[0];

            InitializeComponent();

            //Start work to update information in GUI

            //Add players to list
            for (int i = 0; i < players.Length; i++)
            {
                troopList.Items.Add(players[i].troop.name);
            }
            troopList.SelectedIndex = 0;

            //Initialise information about player
            ShowPlayerStats();
            //Initialise information about player weapons
            int c = 0;
            foreach (var weapon in humanPlayer.troop.weapons)
            {
                playerWeaponList.Items.Add(weapon.name);
                if (weapon == humanPlayer.troop.activeWeapon) playerWeaponList.SelectedIndex = c;
                c++;
            }

            //As it is first turn - set action button to start the game
            nextAction.Text = "Start game!";
        }

        private void ShowPlayerStats()
        {
            playerName.Text = humanPlayer.Name;
            playerAttackDamage.Text = humanPlayer.troop.activeWeapon.attackDamage.ToString();
            playerAttackRange.Text = humanPlayer.troop.activeWeapon.range.ToString();
            playerAttackType.Text = humanPlayer.troop.activeWeapon.type.ToString();
            playerActionPoints.Text = $"{humanPlayer.actionPoints} / {humanPlayer.maxActionPoints}";
        }

        private void MainGameWindow_Load(object sender, EventArgs e)
        {
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, continentAlpha: 0);
        }

        private void UpdateGameBoard()
        {
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, continentAlpha: 0);
        }

        public void NextTurn()
        {
            canMoveTo.Clear();
            activePlayer = players[activePlayerCounter];
            activePlayer.PlayTurn(nextAction);
            AddOverlay();
            activePlayerCounter = activePlayerCounter == players.Length - 1 ? 0 : activePlayerCounter + 1;
            ShowPlayerStats();
        }

        private void NextAction_Click(object sender, EventArgs e)
        {
            AddOverlay();
            activePlayer.ActionButtonPressed(this);
        }

        private void PlayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (troopList.SelectedIndex == -1) return;
            Player player = players[troopList.SelectedIndex];
            if (player.Name == activePlayer.Name)
            {
                //Clear all data
                enemyName.Text = "";
                enemyAttackDamage.Text = "";
                enemyAttackRange.Text = "";
                enemyAttackType.Text = "";
            }
            else
            {
                enemyName.Text = player.Name;
                enemyAttackDamage.Text = player.troop.activeWeapon.attackDamage.ToString();
                enemyAttackRange.Text = player.troop.activeWeapon.range.ToString();
                enemyAttackType.Text = player.troop.activeWeapon.type.ToString();
            }
        }

        private void MainGameWindow_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void PlayerWeaponList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pos = playerWeaponList.SelectedIndex;
            Weapon weapon = humanPlayer.troop.weapons[pos];
            playerPossibleAttackRange.Text = $"Range: {weapon.range}";
            playerPossibleWeaponDamage.Text = $"Damage: {weapon.attackDamage}";
            playerPossibleWeaponName.Text = $"{weapon.name}";
            playerPossibleWeaponType.Text = $"Type: {weapon.type}";
        }

        private void ChangeWeapon_Click(object sender, EventArgs e)
        {
            if (playerWeaponList.SelectedIndex >= 0)
            {
                humanPlayer.troop.activeWeapon = humanPlayer.troop.weapons[playerWeaponList.SelectedIndex];
            }
            ShowPlayerStats();
        }

        private void GameBoard_Click(object sender, EventArgs e)
        {
        }

        private List<MapTile> canMoveTo = new List<MapTile>();

        private void GameBoard_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X - e.X % MapCreator.fieldSize;
            int y = e.Y - e.Y % MapCreator.fieldSize;

            map.overlayObjects.Add(new OverlayRectangle(x, y, 20, 20, Color.Red, false));
            AddOverlay();

            int X = e.X / MapCreator.fieldSize;
            int Y = e.Y / MapCreator.fieldSize;
            if (humanPlayer.active)
            {
                //try to move player
                if (canMoveTo.Count != 0)
                {
                    try
                    {
                        MapTile moveTo = canMoveTo.Find(f => f.position.X == X
                            && Y == f.position.Y);
                        humanPlayer.actionPoints = moveTo.leftValue;
                        humanPlayer.troop.position.X = moveTo.position.X;
                        humanPlayer.troop.position.Y = moveTo.position.Y;
                        map.DrawTroops();
                        AddOverlay();
                        canMoveTo.Clear();
                        ShowPlayerStats();
                        return;
                    }
                    catch (Exception)
                    {
                    }
                }
                canMoveTo.Clear();
                //Find fields the player can move to
                if (humanPlayer.troop.position.X == X && humanPlayer.troop.position.Y == Y)
                {
                    //Reset movement cost
                    for (int _x = 0; _x <= map.map.GetUpperBound(0); _x++)
                    {
                        for (int _y = 0; _y <= map.map.GetUpperBound(1); _y++)
                        {
                            map.map[_x, _y].leftValue = -1;
                        }
                    }
                    //Find all fields it can move to
                    List<MapTile> possibleFields = new List<MapTile>();
                    List<MapTile> toCheck = new List<MapTile> { map.map[X, Y] };
                    map.map[X, Y].leftValue = humanPlayer.actionPoints;
                    while (toCheck.Count != 0)
                    {
                        MapTile checking = toCheck[0];
                        toCheck.Remove(checking);
                        if (checking.leftValue >= 0)
                        {
                            possibleFields.Add(checking);
                            List<MapTile> sorroundingTiles = new SorroundingTiles(checking.position, map).rawMaptiles.ToList();
                            sorroundingTiles = sorroundingTiles.Where(t => t.leftValue == -1 || t.leftValue < checking.leftValue - t.MovementCost).ToList();
                            sorroundingTiles.ForEach(t => t.leftValue = checking.leftValue - t.MovementCost);
                            toCheck.AddRange(sorroundingTiles);
                        }
                    }
                    //Add rectangles foreach to the overlay
                    possibleFields.ForEach(f => map.overlayObjects.Add(new OverlayRectangle(f.position.X * MapCreator.fieldSize, f.position.Y * MapCreator.fieldSize, MapCreator.fieldSize, MapCreator.fieldSize, Color.Green, false)));
                    canMoveTo.AddRange(possibleFields);
                    AddOverlay();
                    return;
                }
            }
        }

        private void AddOverlay()
        {
            Image image = new Bitmap(map.background);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.DrawImage(map.DrawOverlay(gameBoard.Width, gameBoard.Height), 0, 0);
            }
            gameBoard.Image = image;
        }
    }
}