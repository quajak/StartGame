﻿using PlayerCreator;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace StartGame
{
    partial class MainGameWindow : Form
    {
        private const int fieldSize = MapCreator.fieldSize;
        private Random random;

        private Map map;

        private Point selected;

        public List<Player> players;
        private Player activePlayer;
        public HumanPlayer humanPlayer;
        private int activePlayerCounter = 0;

        private List<WinCheck> winConditions;
        private List<WinCheck> deathConditions;
        private bool useWinChecks = false;

        public bool dead = false;

        private string description;

        private Campaign campaign;

        private bool multiTurn = false;
        public int playerDamage;
        public int playerDoged;

        public MainGameWindow(Map Map, HumanPlayer player, Mission mission, Campaign Campaign = null)
        {
            player.main = this;
            map = Map;
            if (map.map is null)
                throw new Exception("Map of maptiles can not be undefined when starting the game. Please initialise before calling this function");

            campaign = Campaign;

            selected = new Point(-1, 0);
            random = new Random();

            //Setup mission
            int difficulty;
            if (campaign != null)
                difficulty = campaign.difficulty;
            else
                difficulty = 5;

            int round;
            if (campaign != null)
                round = campaign.Round;
            else
                round = 0;

            (players, winConditions, deathConditions, description) = mission.GenerateMission(difficulty, round, map, player);
            useWinChecks = true;

            //Setup game
            humanPlayer = player;
            activePlayer = players[0];

            players.ForEach(p => map.troops.Add(p.troop));
            InitializeComponent();

            //Start work to update information in GUI
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, continentAlpha: 0);
            //Add players to list
            UpdatePlayerList();

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
            changeWeapon.Enabled = false;

            ShowPositionStats();
            dumpWeapon.Enabled = false;

            levelUpButton.Enabled = false;
        }

        #region Game Loop

        public bool DoChecks()
        {
            if (useWinChecks)
            {
                bool won = false;
                foreach (var check in winConditions)
                {
                    won = check.Invoke(map, this);
                    if (won)
                    {
                        PlayerWins();
                        return true;
                    }
                }
                bool lost = false;
                foreach (var check in deathConditions)
                {
                    lost = check.Invoke(map, this);
                    if (lost)
                    {
                        dead = true;
                        Close();
                    }
                }
            }
            else
            {
                if (dead)
                {
                    Close();
                }
                if (players.Count == 1 && players.Exists(t => t == humanPlayer))
                {
                    //Won
                    PlayerWins("You have defeated all the enemies!");
                    return true;
                }
            }
            return false;
        }

        public void NextTurn()
        {
            if (DoChecks()) return;
            canMoveTo.Clear();

            //Change active player
            int activeIndex = players.FindIndex(p => p.Name == activePlayer.Name);
            activePlayerCounter = activeIndex == players.Count - 1 ?
                0 : activeIndex + 1;

            activePlayer.active = false;
            activePlayer = players[activePlayerCounter];
            activePlayer.active = true;

            //Reset the status of the player
            activePlayer.actionPoints = activePlayer.maxActionPoints;
            foreach (Weapon weapon in activePlayer.troop.weapons)
            {
                if (weapon is null)
                {
                    continue;
                }
                if (weapon.type != AttackType.range)
                {
                    weapon.attacks = weapon.maxAttacks;
                }
            }

            if (humanPlayer is null) return;
            activePlayer.PlayTurn(this);

            if (DoChecks()) return;

            if (activePlayer.singleTurn && multiTurn)
            {
                if (playerDamage != 0 || playerDoged != 0)
                {
                    map.overlayObjects.Add(new OverlayText(humanPlayer.troop.position.X * fieldSize, humanPlayer.troop.position.Y * fieldSize, Color.Red,
                        $"{(playerDamage != 0 ? $"-{playerDamage}" : "")} {(playerDoged != 0 && playerDamage != 0 ? "and" : "")}" +
                        $" {(playerDoged != 0 ? $"Doged {playerDoged} {(playerDoged != 1 ? "times" : "time")}" : "")}"));
                    playerDamage = 0;
                    playerDoged = 0;
                }

                multiTurn = false;
            }
            else if (!activePlayer.singleTurn) multiTurn = true;

            UpdateOverlay();
            ShowPlayerStats();

            if (!activePlayer.singleTurn)
            {
                NextTurn();
                return;
            }

            //GUI Updates
            if (humanPlayer.active)
            {
                if (humanPlayer.troop.activeWeapon != humanPlayer.troop.weapons[playerWeaponList.SelectedIndex])
                    changeWeapon.Enabled = true;
                if (humanPlayer.troop.weapons[playerWeaponList.SelectedIndex].discardeable)
                    dumpWeapon.Enabled = true;
                nextAction.Enabled = false;
                nextAction.Text = "End turn";
                MessageBox.Show("It is your turn!");
                nextAction.Enabled = true;
            }
            else
            {
                changeWeapon.Enabled = false;
                dumpWeapon.Enabled = false;
                nextAction.Enabled = false;
                nextAction.Text = "Next turn!";
                MessageBox.Show($"It is {activePlayer.Name}'s turn!");
                nextAction.Enabled = true;
            }
            nextAction.Focus();
        }

        #endregion Game Loop

        #region GUI Updates

        public void SetUpdateState(bool active)
        {
            levelUpButton.Enabled = active;
        }

        private void SelectField(int X, int Y)
        {
            map.overlayObjects.Add(new OverlayRectangle(X * fieldSize, Y * fieldSize, fieldSize, fieldSize, Color.Orange, false));
            UpdateOverlay();

            position = new Point(X, Y);
            ShowPositionStats();
        }

        private Point position;

        private void ShowPositionStats()
        {
            if (position == null)
            {
                fieldPosition.Text = "--";
                fieldHeight.Text = "--";
            }
            else
            {
                fieldPosition.Text = $"Co-ords: {position.X} : {position.Y}";
                fieldHeight.Text = "Height: " + map.map[position.X, position.Y].height.ToString("0.##");
            }
        }

        private void ShowWeaponStats()
        {
            int pos = playerWeaponList.SelectedIndex;
            if (pos != -1)
            {
                Weapon weapon = humanPlayer.troop.weapons[pos];
                playerPossibleAttackRange.Text = $"Range: {weapon.range}";
                playerPossibleWeaponDamage.Text = $"Damage: {weapon.attackDamage}";
                playerPossibleWeaponName.Text = $"{weapon.name}";
                playerPossibleWeaponType.Text = $"Type: {weapon.type}";
                playerPossibleWeaponAttacks.Text = $"Attacks: {weapon.attacks} / {weapon.maxAttacks}";
                dumpWeapon.Enabled = weapon.discardeable && humanPlayer.active;
                changeWeapon.Enabled = (weapon != humanPlayer.troop.activeWeapon) && humanPlayer.active;
            }
            else
            {
                playerPossibleAttackRange.Text = "";
                playerPossibleWeaponDamage.Text = "";
                playerPossibleWeaponName.Text = "";
                playerPossibleWeaponType.Text = "";
                playerPossibleWeaponAttacks.Text = "";
                dumpWeapon.Enabled = false;
                changeWeapon.Enabled = false;
            }
        }

        private void UpdatePlayerList()
        {
            troopList.Items.Clear();
            for (int i = 0; i < players.Count; i++)
            {
                troopList.Items.Add(players[i].troop.name);
            }
            troopList.SelectedIndex = 0;
        }

        private void ShowPlayerStats()
        {
            if (humanPlayer != null)
            {
                playerName.Text = humanPlayer.Name;
                playerAttackDamage.Text = humanPlayer.troop.activeWeapon.attackDamage.ToString();
                playerAttackRange.Text = humanPlayer.troop.activeWeapon.range.ToString();
                playerAttackType.Text = humanPlayer.troop.activeWeapon.type.ToString();
                playerActionPoints.Text = $"{humanPlayer.actionPoints} / {humanPlayer.maxActionPoints}";
                playerHealth.Text = $"{humanPlayer.troop.health} / {humanPlayer.troop.maxHealth}";
                playerWeaponAttacks.Text = $"Attacks: {humanPlayer.troop.activeWeapon.attacks} / {humanPlayer.troop.activeWeapon.maxAttacks}";
                playerHeight.Text = $"{map.map[humanPlayer.troop.position.X, humanPlayer.troop.position.Y].height.ToString("0.##")}";
                playerDefense.Text = $"Defense: {humanPlayer.troop.defense}";
                playerStrength.Text = $"Strength: {humanPlayer.strength}";
                playerAgility.Text = $"Agility: {humanPlayer.agility}";
                playerEndurance.Text = $"Endurance: {humanPlayer.endurance}";
                playerVitatlity.Text = $"Vitality: {humanPlayer.vitality}";
                playerLevel.Text = $"Level: {humanPlayer.level} + ({humanPlayer.storedLevelUps})";
                playerXP.Text = $"XP: {humanPlayer.xp} / {humanPlayer.levelXP}";
            }
        }

        private void UpdateGameBoard()
        {
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, continentAlpha: 0);
        }

        private void UpdateOverlay(bool keepAll = false)
        {
            keepAll = !activePlayer.singleTurn;
            Image image = new Bitmap(map.background);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.DrawImage(map.DrawOverlay(gameBoard.Width, gameBoard.Height, keepAll), 0, 0);
            }
            gameBoard.Image = image;
        }

        private void ShowEnemyStats(bool clicked = false)
        {
            if (troopList.SelectedIndex == -1) return;
            Player player = players[troopList.SelectedIndex];
            if (player.Name == humanPlayer.Name)
            {
                //Clear all data
                enemyName.Text = "";
                enemyAttackDamage.Text = "";
                enemyAttackRange.Text = "";
                enemyAttackType.Text = "";
                enemyHealth.Text = "";
                enemyPosition.Text = "";
                enemyHeight.Text = "";
                enemyDefense.Text = "";
            }
            else
            {
                enemyName.Text = player.Name;
                if (player.troop.activeWeapon is null)
                {
                    enemyAttackDamage.Text = "";
                    enemyAttackRange.Text = "";
                    enemyAttackType.Text = "";
                }
                else
                {
                    enemyAttackDamage.Text = player.troop.activeWeapon.attackDamage.ToString();
                    enemyAttackRange.Text = player.troop.activeWeapon.range.ToString();
                    enemyAttackType.Text = player.troop.activeWeapon.type.ToString();
                }
                enemyHealth.Text = $"{player.troop.health} / {player.troop.maxHealth}";
                enemyPosition.Text = $"{player.troop.position.X} : {player.troop.position.Y}";
                enemyHeight.Text = $"{map.map[player.troop.position.X, player.troop.position.Y].height.ToString("0.##")}";
                enemyDefense.Text = $"Defense: {player.troop.defense}";
            }
            if (clicked)
            {
                SelectField(player.troop.position.X, player.troop.position.Y);
            }
        }

        #endregion GUI Updates

        #region Event Handler

        private void LevelUpButton_Click(object sender, EventArgs e)
        {
            //Level up
            LevelUp levelUp = new LevelUp(humanPlayer, humanPlayer.storedLevelUps * 2);
            levelUp.ShowDialog();

            humanPlayer.level += humanPlayer.storedLevelUps;
            humanPlayer.storedLevelUps = 0;

            ShowPlayerStats();
            levelUpButton.Enabled = false;
        }

        private void DumpWeapon_Click(object sender, EventArgs e)
        {
            if (playerWeaponList.SelectedIndex != -1)
            {
                Weapon toRemove = humanPlayer.troop.weapons[playerWeaponList.SelectedIndex];
                humanPlayer.troop.weapons.Remove(toRemove);
                playerWeaponList.Items.Remove(toRemove.name);
                UpdateOverlay();
            }
        }

        private void ShowHeightDifference_CheckedChanged(object sender, EventArgs e)
        {
            if (showHeightDifference.Checked)
            {
                gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, continentAlpha: 0, colorAlpha: 50);
            }
            else
            {
                gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, continentAlpha: 0);
            }
            UpdateOverlay(keepAll: true);
        }

        private void MainGameWindow_Load(object sender, EventArgs e)
        {
        }

        private void PlayerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowEnemyStats(true);
        }

        private void MainGameWindow_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void PlayerWeaponList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowWeaponStats();
        }

        private void ChangeWeapon_Click(object sender, EventArgs e)
        {
            if (playerWeaponList.SelectedIndex >= 0)
            {
                humanPlayer.troop.activeWeapon = humanPlayer.troop.weapons[playerWeaponList.SelectedIndex];
            }
            ShowPlayerStats();
            UpdateOverlay();
        }

        private void GameBoard_Click(object sender, EventArgs e)
        {
        }

        private void NextAction_Click(object sender, EventArgs e)
        {
            if (humanPlayer is null) Close();
            UpdateOverlay();
            activePlayer.ActionButtonPressed(this);
        }

        private void MainGameWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool won = false;
            if (useWinChecks)
            {
                foreach (var check in winConditions)
                {
                    won = check.Invoke(map, this);
                    if (won) break;
                }
            }
            else
            {
                won = players.Count == 1 && players.Exists(t => t == humanPlayer);
            }
            if (!won && !dead)
            {
                if (MessageBox.Show("Closing the game, will mean giving up!",
                    "Do you want to close", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    dead = true;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void MainGameWindow_Shown(object sender, EventArgs e)
        {
            MessageBox.Show(description);
        }

        #region Player Game Board Interaction

        private List<MapTile> canMoveTo = new List<MapTile>();
        private List<MapTile> canAttack = new List<MapTile>();

        private void GameBoard_MouseClick(object sender, MouseEventArgs e)
        {
            int X = e.X / fieldSize;
            int Y = e.Y / fieldSize;

            if (map.map.GetUpperBound(0) < X && map.map.GetUpperBound(1) < Y) return;

            int x = e.X - e.X % fieldSize;
            int y = e.Y - e.Y % fieldSize;

            Troop p = map.troops.Find(t => t.position.X == X && t.position.Y == Y);
            if (p != null)
            {
                troopList.SelectedIndex = -1;
                Player selected = players.Find(P => P.troop == p);
                troopList.SelectedIndex = troopList.Items.IndexOf(selected.Name);
            }
            else
            {
                SelectField(X, Y);
            }

            if (humanPlayer.active)
            {
                //try to attack
                if (canAttack.Count != 0 && canAttack.Exists(m => m.position.X == X
                     && m.position.Y == Y))
                {
                    Player attacked = players.Find(t => t.troop.position.X == X && t.troop.position.Y == Y);
                    Attack(humanPlayer, attacked, show: true);
                    return;
                }
                canAttack.Clear();

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
                        UpdateOverlay();
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
                            if (!map.troops.Exists(t =>
                                checking.position.X == t.position.X &&
                                checking.position.Y == t.position.Y))
                                possibleFields.Add(checking);

                            List<MapTile> sorroundingTiles = new SorroundingTiles(checking.position, map).rawMaptiles.ToList();
                            sorroundingTiles = sorroundingTiles.Where(t => t.leftValue == -1 || t.leftValue < checking.leftValue - t.MovementCost).ToList();
                            sorroundingTiles.ForEach(t => t.leftValue = checking.leftValue - t.MovementCost);
                            toCheck.AddRange(sorroundingTiles);
                        }
                    }
                    //Add rectangles foreach to the overlay
                    possibleFields.ForEach(f =>
                        map.overlayObjects.Add(new OverlayRectangle(f.position.X * fieldSize, f.position.Y * fieldSize, fieldSize, fieldSize, Color.Green, false)));
                    //Add text with cost for each field to overlay
                    possibleFields.ForEach(f =>
                        map.overlayObjects.Add(new OverlayText(f.position.X * fieldSize, f.position.Y * fieldSize, Color.DarkGreen, $"{f.leftValue}")));
                    canMoveTo.AddRange(possibleFields);

                    //Show all enemies it might hit and damage dealt
                    Point center = humanPlayer.troop.position;
                    if (humanPlayer.actionPoints >= 1 && humanPlayer.troop.activeWeapon.attacks > 0)
                    {
                        foreach (var troop in map.troops)
                        {
                            if (troop.position != center)
                            {
                                int distance = Math.Abs(troop.position.X - center.X) + Math.Abs(troop.position.Y - center.Y);
                                if (distance <= humanPlayer.troop.activeWeapon.range)
                                {
                                    map.overlayObjects.Add(new OverlayRectangle(troop.position.X * fieldSize,
                                        troop.position.Y * fieldSize, fieldSize, fieldSize,
                                        Color.Red, false));
                                    canAttack.Add(map.map[troop.position.X, troop.position.Y]);
                                }
                            }
                        }
                    }

                    UpdateOverlay();
                    return;
                }
            }
        }

        #endregion Player Game Board Interaction

        #endregion Event Handler

        #region Player Events

        public void PlayerDied(string message = "You have died!")
        {
            if (humanPlayer is null) return;
            players.Remove(humanPlayer);
            map.troops.Remove(humanPlayer.troop);
            UpdatePlayerList();
            UpdateOverlay();
            ShowPlayerStats();
            canAttack.Clear();
            ShowEnemyStats();
            nextAction.Text = "Game Over";
            dead = true;
            MessageBox.Show(message);
            humanPlayer = null;
        }

        public void PlayerWins(string message = "You have won!")
        {
            MessageBox.Show(message, "Mission Completed");

            if (campaign != null)
            {
                Weapon received = campaign.CalculateReward();
                WeaponView weaponView = new WeaponView(received, true);
                weaponView.ShowDialog();

                if (weaponView.decision)
                {
                    humanPlayer.troop.weapons.Add(received);
                }
            }

            Close();
        }

        public int CalculateDamage(Player attacking, Player defending, Weapon weapon)
        {
            return CalculateDamage(attacking.troop.position, attacking, defending, weapon);
        }

        public int CalculateDamage(Point attackingPosition, Player attacking, Player defending, Weapon weapon)
        {
            int damage = weapon.attackDamage - defending.troop.defense;
            damage += attacking is HumanPlayer ? (attacking as HumanPlayer).strength : 0;

            //if melee code check for height difference
            if (weapon.type == AttackType.melee)
            {
                double attackingHeight = map.map[attackingPosition.X, attackingPosition.Y].height;
                double defendingHeight = map.map[defending.troop.position.X, defending.troop.position.Y].height;

                double difference = attackingHeight - defendingHeight;

                damage = damage + (int)Math.Ceiling(difference * damage);
            }

            damage = defending.troop.health - damage < 0 ? defending.troop.health : damage;
            return damage;
        }

        /// <summary>
        /// Function which handles attacks of different players
        /// </summary>
        /// <param name="attacking">Attacking player</param>
        /// <param name="attacked">Attacked player</param>
        /// <param name="show">If overlay should instantly be updated</param>
        /// <returns>Return tuple containg Damage dealt, If attacked troop is dead</returns>
        public (int damage, bool killed, bool hit) Attack(Player attacking, Player attacked, bool show = false)
        {
            if (humanPlayer is null) return (0, true, true);

            bool killed = false;
            int damage = CalculateDamage(attacking, attacked, attacking.troop.activeWeapon);

            attacking.troop.activeWeapon.attacks--;
            attacking.actionPoints -= attacking.troop.activeWeapon.attackCost;
            //Check if hit
            if (random.Next(100) < attacked.troop.dodge)
            {
                //dodged
                if (show)
                    map.overlayObjects.Add(new OverlayText(attacked.troop.position.X * fieldSize, attacked.troop.position.Y * fieldSize, Color.Red, "Dodged!"));
                UpdateOverlay();
                return (0, false, false);
            } //TODO: Add scraping hit

            attacked.troop.health -= damage;

            if (attacked.troop.health == 0)
            {
                killed = true;
                if (attacked.Name == humanPlayer.Name)
                {
                    //Do nothing allow player AI code to handle it
                }
                else
                {
                    //Enemy Died
                    map.troops.Remove(attacked.troop);
                    players.Remove(attacked);
                    UpdatePlayerList();

                    //Let player gain xp
                    humanPlayer.GainXP(attacked.XP);
                }
            }

            if (show)
            {
                map.overlayObjects.Add(new OverlayText(attacked.troop.position.X * fieldSize, attacked.troop.position.Y * fieldSize, Color.Red, $"-{damage}"));
                map.DrawTroops();
                UpdateOverlay();
                if (attacking == humanPlayer)
                {
                    ShowWeaponStats();
                }
            }

            canAttack.Clear();
            canMoveTo.Clear();

            ShowPlayerStats();
            ShowEnemyStats();

            return (damage, killed, true);
        }

        #endregion Player Events

        #region Program Interaction

        public void AddPlayer(Player player)
        {
            players.Add(player);
            map.troops.Add(player.troop);
            UpdatePlayerList();
        }

        #endregion Program Interaction
    }
}