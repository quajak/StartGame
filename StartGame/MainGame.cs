using PlayerCreator;
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
        //Long term: Add dialog
        //Long term: Add save feature
        //Long term: Add debug features
        //Long term: Add magic system

        private const int fieldSize = MapCreator.fieldSize;

        private Random random;

        private Map map;

        private Point selected;

        public List<Player> players;
        private Player activePlayer;
        public HumanPlayer humanPlayer;
        private readonly Mission mission;
        private int activePlayerCounter = 0;

        private List<WinCheck> winConditions;
        private List<WinCheck> deathConditions;
        private bool useWinChecks = false;

        public bool dead = false;

        private string description;

        private Campaign campaign;

        public int playerDamage;
        public int playerDoged;

        public MainGameWindow(Map Map, HumanPlayer player, Mission mission, List<Tree> trees, Campaign Campaign = null)
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

            bool first = true;
            do
            {
                if (!first)
                {
                    map = new Map();
                }
                else
                    first = false;
                (players, winConditions, deathConditions, description) = mission.GenerateMission(difficulty, round, map, player);
            } while (players is null);

            useWinChecks = true;

            //Setup game
            humanPlayer = player;
            this.mission = mission;
            activePlayer = players[0];

            players.ForEach(p => map.troops.Add(p.troop));
            players.ForEach(p => map.entites.Add(p.troop));
            InitializeComponent();

            //Initialse functions
            CalculatePlayerAttackDamage.Add(CalculateDamage);
            CalculateCost.Add((t, d, cost) => t.Cost);

            //Start work to update information in GUI
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, continentAlpha: 0);

            //GUI Work
            console.Text = "Starting game ... \n";
            enemyMovement.Checked = mission != null ? mission.EnemyMoveTogether : false;
            UpdateStatusList();

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

            //Initialise trees
            trees.ForEach(t => t.Initialise(this));

            //Initialise info about player trees
            UpdateTreeView();

            //Activate players tress
            humanPlayer.trees.ForEach(t => t.Activate());

            //Activate all players
            players.ForEach(p => p.Initialise(this));

            //As it is first turn - set action button to start the game
            nextAction.Text = "Start game!";
            changeWeapon.Enabled = false;

            ShowPositionStats();
            dumpWeapon.Enabled = false;

            //Set level up button correctly
            levelUpButton.Enabled = humanPlayer.storedLevelUps != 0;
        }

        #region Game Logic

        #region Game Event Handler

        public class PlayerMovementData : EventArgs
        {
            public Player player;
            public MapTile start;
            public MapTile goal;
            public int distance;
        }

        public event EventHandler<PlayerMovementData> PlayerMoved = delegate { };

        public class CombatData : EventArgs
        {
            public Player attacked;
            public Player attacker;
            public Weapon weapon;
            public int damage;
            public bool doged;
            public bool killed;
            public int range;
        }

        public event EventHandler<CombatData> Combat = delegate { };

        public class TurnData : EventArgs
        {
            public Player active;
        }

        public event EventHandler<TurnData> Turn = delegate { };

        #endregion Game Event Handler

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

            Turn(this, new TurnData() { active = activePlayer });

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
            activePlayer.PlayTurn(this, enemyMovement.Checked);

            if (DoChecks()) return;
            if (humanPlayer is null) return;
            //If end of mutli turn
            if (activePlayer.Name == humanPlayer.Name || !enemyMovement.Checked)
            {
                if (playerDamage != 0 || playerDoged != 0)
                {
                    map.overlayObjects.Add(new OverlayText(humanPlayer.troop.Position.X * fieldSize, humanPlayer.troop.Position.Y * fieldSize, Color.Red,
                        $"{(playerDamage != 0 ? $"-{playerDamage}" : "")} {(playerDoged != 0 && playerDamage != 0 ? "and" : "")}" +
                        $" {(playerDoged != 0 ? $"Doged {playerDoged} {(playerDoged != 1 ? "times" : "time")}" : "")}"));
                    playerDamage = 0;
                    playerDoged = 0;
                }
            }

            UpdateOverlay();
            ShowPlayerStats();

            //Goto next turn
            if (enemyMovement.Checked && activePlayer.Name != humanPlayer.Name)
            {
                NextTurn();
                return;
            }

            //GUI Updates

            if (humanPlayer.active)
            {
                UpdateStatusInfo();
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

        #endregion Game Logic

        #region GUI Updates

        public void UpdateTreeView()
        {
            List<string> playerTreeNames = humanPlayer.trees.ConvertAll(t => t.name);
            List<string> diff = treeList.Items.Cast<string>().Except(playerTreeNames).ToList();
            foreach (string dif in diff)
            {
                treeList.Items.Remove(dif);
            }

            diff = playerTreeNames.Except(treeList.Items.Cast<string>()).ToList();
            foreach (string dif in diff)
            {
                treeList.Items.Add(dif);
            }

            UpdateSelectedTreeInformation();
        }

        public void TreeGained(Tree tree)
        {
            MessageBox.Show($"You have gained the {tree.GetType().BaseType.Name} {tree.name}! \n {tree.description} \n Unlocked by: {tree.reason}", "Tree gained!");
        }

        private void UpdateSelectedTreeInformation()
        {
            int selected = treeList.SelectedIndex;
            if (selected == -1)
            {
                treeName.Text = "";
                treeInformation.Text = "";
            }
            else
            {
                treeName.Text = humanPlayer.trees[selected].name;
                treeInformation.Text = humanPlayer.trees[selected].description + " \n " + "Found by " + humanPlayer.trees[selected].reason;
            }
        }

        public void WriteConsole(string text)
        {
            console.AppendText(text);
            console.AppendText(Environment.NewLine);
        }

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
            //Easy: Don't readd everything, only the things which change
            troopList.Items.Clear();
            for (int i = 0; i < players.Count; i++)
            {
                troopList.Items.Add(players[i].troop.name);
            }
            troopList.SelectedIndex = 0;
        }

        private void UpdateStatusInfo()
        {
            if (statusList.SelectedIndex != -1)
            {
                statusTitle.Text = humanPlayer.troop.statuses[statusList.SelectedIndex].name;
                statusDescription.Text = humanPlayer.troop.statuses[statusList.SelectedIndex].Description();
            }
            else
            {
                statusTitle.Text = "";
                statusDescription.Text = "";
            }
        }

        public void UpdateStatusList()
        {
            List<string> playerStatusNames = humanPlayer.troop.statuses.ConvertAll(t => t.name);
            List<string> diff = statusList.Items.Cast<string>().Except(playerStatusNames).ToList();
            foreach (string dif in diff)
            {
                statusList.Items.Remove(dif);
            }

            diff = playerStatusNames.Except(statusList.Items.Cast<string>()).ToList();
            foreach (string dif in diff)
            {
                statusList.Items.Add(dif);
            }

            UpdateStatusInfo();
        }

        public void ShowPlayerStats()
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
                playerHeight.Text = $"{map.map[humanPlayer.troop.Position.X, humanPlayer.troop.Position.Y].height.ToString("0.##")}";
                playerDefense.Text = $"Defense: {humanPlayer.troop.defense}";
                playerStrength.Text = $"Strength: {humanPlayer.strength}";
                playerAgility.Text = $"Agility: {humanPlayer.agility}";
                playerEndurance.Text = $"Endurance: {humanPlayer.endurance}";
                playerVitatlity.Text = $"Vitality: {humanPlayer.vitality}";
                playerLevel.Text = $"Level: {humanPlayer.level} + ({humanPlayer.storedLevelUps})";
                playerXP.Text = $"XP: {humanPlayer.xp} / {humanPlayer.levelXP}";
            }
        }

        public void UpdateGameBoard()
        {
            gameBoard.Image = map.DrawMapBackground(gameBoard.Width, gameBoard.Height, continentAlpha: 0);
        }

        private void UpdateOverlay(bool keepAll = false)
        {
            keepAll = enemyMovement.Checked && activePlayer.Name != humanPlayer.Name;
            Image image = new Bitmap(map.background);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.DrawImage(map.DrawOverlay(gameBoard.Width, gameBoard.Height, keepAll), 0, 0);
            }
            gameBoard.Image = image;
        }

        private void ShowEntityData(Entity entity)
        {
            troopList.SelectedIndex = -1;
            enemyName.Text = entity.name;
            enemyAttackDamage.Text = "";
            enemyAttackRange.Text = "";
            enemyAttackType.Text = "";
            enemyHealth.Text = "";
            enemyPosition.Text = "";
            enemyHeight.Text = "";
            enemyDefense.Text = "";
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
                enemyPosition.Text = $"{player.troop.Position.X} : {player.troop.Position.Y}";
                enemyHeight.Text = $"{map.map[player.troop.Position.X, player.troop.Position.Y].height.ToString("0.##")}";
                enemyDefense.Text = $"Defense: {player.troop.defense}";
            }
            if (clicked)
            {
                SelectField(player.troop.Position.X, player.troop.Position.Y);
            }
        }

        #endregion GUI Updates

        #region Event Handler

        private void StatusList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStatusInfo();
        }

        private void ShowBlockedFields_Click(object sender, EventArgs e)
        {
            Color fill = Color.FromArgb(120, Color.Brown);
            for (int x = 0; x <= map.map.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= map.map.GetUpperBound(1); y++)
                {
                    if (!map.map[x, y].free)
                        map.overlayObjects.Add(new OverlayRectangle(x * MapCreator.fieldSize, y * MapCreator.fieldSize, MapCreator.fieldSize, MapCreator.fieldSize, Color.Brown, true, FillColor: fill));
                }
            }
            UpdateOverlay();
        }

        private void LevelUpButton_Click(object sender, EventArgs e)
        {
            LevelUp();
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

        private void TreeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedTreeInformation();
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
            FocusField(X, Y);
        }

        private void FocusField(int X, int Y)
        {
            SelectField(X, Y);

            Troop p = map.troops.Find(t => t.Position.X == X && t.Position.Y == Y);
            if (p != null)
            {
                troopList.SelectedIndex = -1;
                Player selected = players.Find(P => P.troop == p);
                troopList.SelectedIndex = troopList.Items.IndexOf(selected.Name);
            }
            else
            {
                //Check if any entity has been clicked
                if (map.entites.Exists(t => t.Position.X == X && t.Position.Y == Y))
                {
                    Entity e = map.entites.Find(t => t.Position.X == X && t.Position.Y == Y);
                    ShowEntityData(e);
                }
            }

            if (humanPlayer.active)
            {
                //try to attack
                if (canAttack.Count != 0 && canAttack.Exists(m => m.position.X == X
                     && m.position.Y == Y))
                {
                    Player attacked = players.Find(t => t.troop.Position.X == X && t.troop.Position.Y == Y);
                    Attack(humanPlayer, attacked, show: true);
                    return;
                }
                canAttack.Clear();

                //move player
                if (canMoveTo.Count != 0)
                {
                    MapTile start = map.map[humanPlayer.troop.Position.X, humanPlayer.troop.Position.Y];
                    if (canMoveTo.Exists(f => f.position.X == X && Y == f.position.Y))
                    {
                        MapTile moveTo = canMoveTo.Find(f => f.position.X == X
                            && Y == f.position.Y);
                        humanPlayer.actionPoints = moveTo.leftValue;

                        Point humanPos = humanPlayer.troop.Position;
                        humanPos.X = moveTo.position.X;
                        humanPos.Y = moveTo.position.Y;
                        humanPlayer.troop.Position = humanPos;

                        MapTile end = map.map[humanPlayer.troop.Position.X, humanPlayer.troop.Position.Y];
                        PlayerMoved(this, new PlayerMovementData() { player = humanPlayer, start = start, distance = AIUtility.Distance(start.position, end.position), goal = end });

                        map.DrawEntities();
                        UpdateOverlay();
                        canMoveTo.Clear();
                        ShowPlayerStats();
                        FocusField(X, Y);
                        return;
                    }
                }
                canMoveTo.Clear();
                //Find fields the player can move to
                if (humanPlayer.troop.Position.X == X && humanPlayer.troop.Position.Y == Y)
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
                            sorroundingTiles = sorroundingTiles.Where(t => (t.leftValue == -1 || t.leftValue < checking.leftValue - t.MovementCost) && t.free).ToList();
                            sorroundingTiles.ForEach(t =>
                            {
                                double cost = 0;
                                foreach (var func in CalculateCost)
                                {
                                    cost = func(t, AIUtility.Distance(t.position, humanPlayer.troop.Position), cost);
                                }
                                t.leftValue = checking.leftValue - cost;
                            });
                            toCheck.AddRange(sorroundingTiles);
                        }
                    }
                    //Remove field where player is standing
                    possibleFields.RemoveAll(m => m.position.X == humanPlayer.troop.Position.X && m.position.Y == humanPlayer.troop.Position.Y);

                    //Add rectangles foreach to the overlay
                    possibleFields.ForEach(f =>
                        map.overlayObjects.Add(new OverlayRectangle(f.position.X * fieldSize, f.position.Y * fieldSize, fieldSize, fieldSize, Color.Green, false)));
                    //Add text with cost for each field to overlay
                    possibleFields.ForEach(f =>
                        map.overlayObjects.Add(new OverlayText(f.position.X * fieldSize, f.position.Y * fieldSize, Color.DarkGreen, $"{f.leftValue}")));
                    canMoveTo.AddRange(possibleFields);

                    //Show all enemies it might hit and damage dealt
                    Point center = humanPlayer.troop.Position;
                    if (humanPlayer.actionPoints >= 1 && humanPlayer.troop.activeWeapon.attacks > 0)
                    {
                        foreach (var troop in map.troops)
                        {
                            if (troop.Position != center)
                            {
                                int distance = AIUtility.Distance(troop.Position, center);
                                if (distance <= humanPlayer.troop.activeWeapon.range)
                                {
                                    map.overlayObjects.Add(new OverlayRectangle(troop.Position.X * fieldSize,
                                        troop.Position.Y * fieldSize, fieldSize, fieldSize,
                                        Color.Red, false));
                                    canAttack.Add(map.map[troop.Position.X, troop.Position.Y]);
                                }
                            }
                        }
                    }

                    UpdateOverlay();
                    return;
                }
            }
        }

        /// <summary>
        /// Function list used to calculate cost to move to certain field.
        /// Input: Maptile, Distance moved, Cost of movement
        /// Ouput: Cost
        /// </summary>
        public List<Func<MapTile, int, double, double>> CalculateCost = new List<Func<MapTile, int, double, double>>();

        #endregion Player Game Board Interaction

        #endregion Event Handler

        #region Player Events

        public void DamageAtField(int damage, Point point)
        {
            if (players.Exists(p => p.troop.Position == point))
            {
                DamagePlayer(damage, players.First(p => p.troop.Position == point));
            }
        }

        /// <summary>
        /// Damage player is used to damage players due to enviromental(statuses) causes
        /// </summary>
        /// <param name="damage"> Damage dealt </param>
        /// <param name="player"> Player damaged </param>
        public void DamagePlayer(int damage, Player player)
        {
            damage = Math.Min(player.troop.health, damage);
            player.troop.health -= damage;

            if (player.troop.health <= 0)
            {
                if (player.Name == humanPlayer.Name)
                {
                    PlayerDied();
                    return;
                }
                else
                {
                    //Enemy Died
                    player.troop.Die();
                    map.troops.Remove(player.troop);
                    map.entites.Remove(player.troop);
                    players.Remove(player);
                    UpdatePlayerList();

                    //Let player gain xp
                    humanPlayer.GainXP(player.XP);
                }
            }

            if (player.Name == humanPlayer.Name)
            {
                playerDamage += damage;
            }
            else
            {
                map.overlayObjects.Add(new OverlayText(player.troop.Position.X * fieldSize, player.troop.Position.Y * fieldSize, Color.Red, $"-{damage}"));
            }

            UpdateOverlay();
        }

        private void LevelUp()
        {
            //Level up
            LevelUp levelUp = new LevelUp(humanPlayer, humanPlayer.storedLevelUps);
            levelUp.ShowDialog();

            humanPlayer.level += humanPlayer.storedLevelUps;
            humanPlayer.storedLevelUps = 0;

            ShowPlayerStats();
            levelUpButton.Enabled = false;
        }

        public void PlayerDied(string message = "You have died!")
        {
            if (humanPlayer is null) return;
            players.Remove(humanPlayer);
            map.troops.Remove(humanPlayer.troop);
            map.entites.Remove(humanPlayer.troop);
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
                var (reward, xp) = mission.Reward();
                humanPlayer.GainXP(xp);
                DialogResult result = MessageBox.Show($"You have gained {xp} xp. {(humanPlayer.storedLevelUps != 0 ? $"You have {humanPlayer.storedLevelUps} level ups stored. Would you like to level up?" : "")}", "XP gained", humanPlayer.storedLevelUps != 0 ? MessageBoxButtons.YesNo : MessageBoxButtons.OK);
                if (humanPlayer.storedLevelUps != 0 && result == DialogResult.Yes)
                {
                    LevelUp();
                }
                Weapon received = campaign.CalculateReward(reward);
                WeaponView weaponView = new WeaponView(received, true);
                weaponView.ShowDialog();

                if (weaponView.decision)
                {
                    humanPlayer.troop.weapons.Add(received);
                }
            }

            Close();
        }

        public int CalculateDamage(CombatData data)
        {
            return CalculateDamage(data.attacker.troop.Position, data.attacker, data.attacked, data.weapon);
        }

        public int CalculateDamage(Player attacking, Player defending, Weapon weapon)
        {
            return CalculateDamage(attacking.troop.Position, attacking, defending, weapon);
        }

        public int CalculateDamage(Point attackingPosition, Player attacking, Player defending, Weapon weapon)
        {
            int damage = weapon.attackDamage - defending.troop.defense;
            damage += attacking is HumanPlayer ? (attacking as HumanPlayer).strength : 0;

            //if melee code check for height difference
            if (weapon.type == AttackType.melee)
            {
                double attackingHeight = map.map[attackingPosition.X, attackingPosition.Y].height;
                double defendingHeight = map.map[defending.troop.Position.X, defending.troop.Position.Y].height;

                double difference = attackingHeight - defendingHeight;

                damage = damage + (int)Math.Ceiling(difference * damage);
            }

            damage = defending.troop.health - damage < 0 ? defending.troop.health : damage;
            return damage;
        }

        /// <summary>
        /// List of all functions used to calculate player damage
        /// </summary>
        public List<Func<CombatData, int>> CalculatePlayerAttackDamage = new List<Func<CombatData, int>>();

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

            int damage = 0;
            if (attacking.Name == humanPlayer.Name)
            {
                CombatData combatData = new CombatData()
                {
                    attacked = attacked,
                    attacker = attacking,
                    damage = 0,
                    doged = false,
                    killed = false,
                    range = AIUtility.Distance(attacked.troop.Position, attacking.troop.Position),
                    weapon = attacking.troop.activeWeapon
                };

                foreach (var func in CalculatePlayerAttackDamage)
                {
                    combatData.damage = func(combatData);
                }
                damage = combatData.damage;
            }
            else
            {
                damage = CalculateDamage(attacking, attacked, attacking.troop.activeWeapon);
            }

            attacking.troop.activeWeapon.attacks--;
            attacking.actionPoints -= attacking.troop.activeWeapon.attackCost;
            //Check if hit
            if (random.Next(100) < attacked.troop.dodge)
            {
                //dodged
                if (show)
                    map.overlayObjects.Add(new OverlayText(attacked.troop.Position.X * fieldSize, attacked.troop.Position.Y * fieldSize, Color.Red, "Dodged!"));
                UpdateOverlay();
                Combat(this, new CombatData() { attacked = attacked, attacker = attacking, doged = true, damage = damage, killed = false, range = AIUtility.Distance(attacking.troop.Position, attacked.troop.Position), weapon = attacking.troop.activeWeapon });
                return (0, false, false);
            } //Long term: Add scraping hit

            attacked.troop.health -= damage;

            if (attacked.troop.health <= 0)
            {
                killed = true;
                if (attacked.Name == humanPlayer.Name)
                {
                    //Do nothing allow player AI code to handle it
                }
                else
                {
                    //Enemy Died
                    attacked.troop.Die();
                    map.troops.Remove(attacked.troop);
                    map.entites.Remove(attacked.troop);
                    players.Remove(attacked);
                    UpdatePlayerList();

                    Combat(this, new CombatData() { attacked = attacked, attacker = attacking, doged = false, damage = damage, killed = true, range = AIUtility.Distance(attacking.troop.Position, attacked.troop.Position), weapon = attacking.troop.activeWeapon });

                    //Let player gain xp
                    humanPlayer.GainXP(attacked.XP);
                }
            }
            else
            {
                Combat(this, new CombatData() { attacked = attacked, attacker = attacking, doged = false, damage = damage, killed = false, range = AIUtility.Distance(attacking.troop.Position, attacked.troop.Position), weapon = attacking.troop.activeWeapon });
            }

            if (show)
            {
                map.overlayObjects.Add(new OverlayText(attacked.troop.Position.X * fieldSize, attacked.troop.Position.Y * fieldSize, Color.Red, $"-{damage}"));
                map.DrawEntities();
                UpdateOverlay();
                if (attacking == humanPlayer)
                {
                    WriteConsole($"You have attacked {attacked.Name} for {damage}");
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
            map.entites.Add(player.troop);
            UpdatePlayerList();
        }

        public void MovePlayer(Point end, Point start, Player player, bool CostActionPoints = true)
        {
            Point troopP = player.troop.Position;
            troopP.X = end.X;
            troopP.Y = end.Y;
            player.troop.Position = troopP;
            int distance = AIUtility.Distance(start, end);
            if (CostActionPoints)
                player.actionPoints -= distance; //Bug: Non human players should still move correctly

            PlayerMoved(this, new PlayerMovementData() { player = player, start = map.map[start.X, start.Y], goal = map.map[end.X, end.Y], distance = distance });

            map.DrawEntities();
        }

        #endregion Program Interaction
    }
}