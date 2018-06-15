using PlayerCreator;
using StartGame.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private const int fieldSize = MapCreator.fieldSize;

        private Random random;

        public Map map;

        private Point selected;

        public List<Player> players;
        private Player activePlayer;
        public HumanPlayer humanPlayer;
        private readonly Mission mission;
        private int activePlayerCounter = 0;
        private bool gameStarted = false;

        private List<WinCheck> winConditions;
        private List<WinCheck> deathConditions;
        private bool useWinChecks = false;

        public bool dead = false;

        private string description;

        private Campaign campaign;

        public int playerDamage;
        public int playerDoged;

        private Spell activeSpell;
        private List<Point> spellPoints = new List<Point>();
        private bool castingspell;

        private DebugEditor debug;
        public bool forceWin = false;

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
            players.ForEach(p => map.renderObjects.Add(new EntityRenderObject(p.troop, new TeleportPointAnimation(new Point(0, 0), p.troop.Position))));

            InitializeComponent();

            //Initialse functions
            CalculatePlayerAttackDamage.Add(CalculateDamage);

            //Start work to update information in GUI
            RenderMap();
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

            //DEBUG
            debug = new DebugEditor(this);

            humanPlayer.spells.Add(new HealingSpell(5));
            humanPlayer.spells.Add(new EarthQuakeSpell(5, 5));
            humanPlayer.spells.Add(new LightningBoltSpell(15));
            humanPlayer.spells.Add(new DebuffSpell(2, 1, 5, 0));
            humanPlayer.spells.ForEach(s => s.Initialise(this, map));

            UpdateSpellList();
        }

        #region Game Logic

        #region Game Event Handler

        public class PlayerMovementData : EventArgs
        {
            public Player player;
            public MapTile start;
            public MapTile goal;
            public int distance;
            public MovementType movementType;
            public Map map;
            public Point[] path;
        }

        public enum MovementType { walk, teleport };

        //Initial handler is used to set animation
        public event EventHandler<PlayerMovementData> PlayerMoved = (sender, data) =>
        {
            EntityRenderObject entity = data.map.EntityRenderObjects.FirstOrDefault(e => e.Name == data.player.troop.name);

            if (data.player.troop.health == 0)
            {//Player has died
                return;
            }

            if (entity is null)
            {
                Trace.TraceError($"Player Move was called with no rendering entity attached to the player!");
                throw new Exception("Player must have an entity to be moved!");
            }

            if (data.path is null || data.path.Length == 0) //No path has been generated
            {
                switch (data.movementType)
                {
                    case MovementType.walk:
                        entity.Animation = new LinearPointAnimation(data.start.position, data.goal.position);
                        break;

                    case MovementType.teleport:
                        entity.Animation = new TeleportPointAnimation(data.start.position, data.goal.position);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                entity.Animation = new ListPointAnimation(data.path.ToList(), data.start.position);
            }
        };

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
                if (!lost)
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
            if (!gameStarted) StartGame();

            if (DoChecks()) return;
            canMoveTo.Clear();

            int activeIndex = players.FindIndex(p => p.Name == activePlayer.Name);
            activePlayerCounter = activeIndex == players.Count - 1 ?
                0 : activeIndex + 1;

            activePlayer.active = false;
            activePlayer = players[activePlayerCounter];
            activePlayer.active = true;
            activePlayer.spells.ForEach(s => s.coolDown = s.coolDown == 0 ? 0 : s.coolDown - 1);

            //Reset the status of the player
            activePlayer.NextTurn();
            Turn(this, new TurnData() { active = activePlayer });

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

            if (DoChecks()) return;
            activePlayer.PlayTurn(this, enemyMovement.Checked);

            if (DoChecks()) return;
            //If end of mutli turn
            if (humanPlayer != null && activePlayer.Name == humanPlayer.Name || !enemyMovement.Checked)
            {
                if (playerDamage != 0 || playerDoged != 0)
                {
                    map.overlayObjects.Add(new OverlayText(humanPlayer.troop.Position.X * fieldSize, humanPlayer.troop.Position.Y * fieldSize, Color.Red,
                        $"{(playerDamage != 0 ? $"-{playerDamage}" : "")} {(playerDoged != 0 && playerDamage != 0 ? "and" : "")}" +
                        $" {(playerDoged != 0 ? $"Doged {playerDoged} {(playerDoged != 1 ? "times" : "time")}" : "")}"));
                    playerDamage = 0;
                    playerDoged = 0;
                }
                actionOccuring = false;
                RenderMap();
                ShowPlayerStats();
                nextAction.Enabled = true;
            } //Goto next turn in multiturn
            else if (humanPlayer != null && enemyMovement.Checked && activePlayer.Name != humanPlayer.Name)
            {
                nextAction.Enabled = false;
                actionOccuring = true;
                NextTurn();
                return;
            }

            //GUI Updates

            if (humanPlayer != null && humanPlayer.active)
            {
                UpdateSpellInfo();
                UpdateStatusInfo();
                if (humanPlayer.troop.activeWeapon != humanPlayer.troop.weapons[playerWeaponList.SelectedIndex])
                    changeWeapon.Enabled = true;
                if (humanPlayer.troop.weapons[playerWeaponList.SelectedIndex].discardeable)
                    dumpWeapon.Enabled = true;
                nextAction.Enabled = false;
                nextAction.Text = "End turn";
                nextAction.Enabled = true;
            }
            else
            {
                changeWeapon.Enabled = false;
                dumpWeapon.Enabled = false;
                nextAction.Enabled = false;
                nextAction.Text = "Next turn!";
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

        private void UpdateSpellList()
        {
            List<string> spellNames = humanPlayer.spells.ConvertAll(t => t.name);
            List<string> diff = spellList.Items.Cast<string>().Except(spellNames).ToList();
            foreach (string dif in diff)
            {
                spellList.Items.Remove(dif);
            }

            diff = spellNames.Except(spellList.Items.Cast<string>()).ToList();
            foreach (string dif in diff)
            {
                spellList.Items.Add(dif);
            }

            UpdateSpellInfo();
        }

        public void UpdateSpellInfo()
        {
            int index = spellList.SelectedIndex;
            if (index != -1)
            {
                Spell spell = humanPlayer.spells[index];
                spellName.Text = spell.name;
                spellDescription.Text = $"Mana: {spell.manaCost} Cooldown: {spell.coolDown} / {spell.MaxCoolDown}";
                castSpell.Enabled = activePlayer.Name == humanPlayer.Name && gameStarted && spell.manaCost <= humanPlayer.mana && spell.Ready;
            }
            else
            {
                spellName.Text = "";
                spellDescription.Text = "";
                castSpell.Enabled = false;
            }
        }

        public void WriteConsole(string text)
        {
            console.AppendText(text);
            console.AppendText(Environment.NewLine);
        }

        public void StartGame()
        {
            gameStarted = true;
            UpdateSpellInfo();
        }

        private List<Bitmap> frames = new List<Bitmap>();
        private Thread animator;
        private bool animating = false;
        public bool actionOccuring = false;

        public void RenderMap(bool forceEntityRedrawing = false)
        {
            if (!animating && !actionOccuring)
            {
                StackTrace stackTrace = new StackTrace();
                Trace.TraceInformation($"Rendering - Called from {stackTrace.GetFrame(1).GetMethod().Name}");
                animating = true;
                frames = new List<Bitmap>();
                animator = new Thread(() => map.Render(gameBoard, frames, forceEntityRedrawing, frameTime: 100, debug: false, colorAlpha: showHeightDifference.Checked ? 50 : 255, showInverseHeight: showHeightDifference.Checked))
                {
                    Name = "Animator Thread"
                };
                animator.Start();
                System.Timers.Timer timer = new System.Timers.Timer(100)
                {
                    Enabled = true
                };
                timer.Elapsed += ChangeImage;
                timer.Start();
            }
            else
            {
                StackTrace stackTrace = new StackTrace();
                Trace.TraceInformation($"Skipped: {animating} Animating - {actionOccuring} Action Occuring - Called from {stackTrace.GetFrame(1).GetMethod().Name}");
            }
        }

        private void ChangeImage(object sender, System.Timers.ElapsedEventArgs e)
        {
            Bitmap newFrame = null;
            lock (frames)
            {
                Bitmap loc = frames.FirstOrDefault();
                if (loc is null) return;
                lock (loc)
                {
                    frames.Remove(loc);
                    if (loc is null)
                        newFrame = null;
                    else
                        newFrame = new Bitmap(loc);
                }
            }
            if (newFrame != null)
            {
                gameBoard.Image = newFrame;
            }
            if (!(animator.IsAlive || frames.Count != 0))
            {
                animating = false;
                (sender as System.Timers.Timer).Stop();
            }
        }

        public void SetUpdateState(bool active)
        {
            levelUpButton.Enabled = active;
        }

        private void SelectField(int X, int Y)
        {
            map.overlayObjects.Add(new OverlayRectangle(X * fieldSize, Y * fieldSize, fieldSize, fieldSize, Color.Orange, false));
            RenderMap(); //TODO: Check if this is necessary right now all calls are being skipped

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
            List<string> playerNames = players.ConvertAll(t => t.troop.name);
            List<string> diff = troopList.Items.Cast<string>().Except(playerNames).ToList();
            foreach (string dif in diff)
            {
                troopList.Items.Remove(dif);
            }

            diff = playerNames.Except(troopList.Items.Cast<string>()).ToList();
            foreach (string dif in diff)
            {
                troopList.Items.Add(dif);
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
            if (humanPlayer is null) return;
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
                playerMana.Text = $"Mana: {humanPlayer.mana} / {humanPlayer.maxMana}";
                playerWisdom.Text = $"Wisdom: {humanPlayer.wisdom}";
                playerIntelligence.Text = $"Intelligence: {humanPlayer.intelligence}";
            }
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
            RenderMap();
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
                RenderMap();
            }
        }

        private void TreeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedTreeInformation();
        }

        private void ShowHeightDifference_CheckedChanged(object sender, EventArgs e)
        {
            RenderMap();
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

        private void DebugButton_Click(object sender, EventArgs e)
        {
            try
            {
                debug.Show();
            }
            catch (Exception)
            {
                debug = new DebugEditor(this);
                debug.Show();
            }
        }

        private void SpellList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSpellInfo();
        }

        private void ChangeWeapon_Click(object sender, EventArgs e)
        {
            if (playerWeaponList.SelectedIndex >= 0)
            {
                humanPlayer.troop.activeWeapon = humanPlayer.troop.weapons[playerWeaponList.SelectedIndex];
            }
            ShowWeaponStats();
            ShowPlayerStats();
            RenderMap();
        }

        private void GameBoard_Click(object sender, EventArgs e)
        {
        }

        private void NextAction_Click(object sender, EventArgs e)
        {
            if (humanPlayer is null) Close();
            activePlayer.ActionButtonPressed(this);
        }

        private void CastSpell_Click(object sender, EventArgs e)
        {
            if (spellList.SelectedIndex != -1)
            {
                activeSpell = humanPlayer.spells[spellList.SelectedIndex];
                if (activeSpell.Ready && humanPlayer.mana >= activeSpell.manaCost)
                {
                    humanPlayer.mana -= activeSpell.manaCost;
                    if (activeSpell.format.Positions != 0)
                    {
                        castingspell = true;
                        castSpell.Text = "Select position on map";
                    }
                    else //Instantly cast
                    {
                        //TODO: Extract as method
                        //Now cast
                        WriteConsole(activeSpell.Activate(new SpellInformation() { positions = spellPoints, mage = humanPlayer }));

                        spellPoints.Clear();
                        castSpell.Text = "Cast spell";
                        activeSpell = null;
                        castingspell = false;
                        UpdateSpellInfo();
                        ShowPlayerStats();
                    }
                }
            }
        }

        private void MainGameWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            debug.Close();
            if (forceWin) return;
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
        private bool forceEntityRedraw = false;

        private void GameBoard_MouseClick(object sender, MouseEventArgs e)
        {
            int X = e.X / fieldSize;
            int Y = e.Y / fieldSize;

            if (map.map.GetUpperBound(0) < X && map.map.GetUpperBound(1) < Y) return;

            int x = e.X - e.X % fieldSize;
            int y = e.Y - e.Y % fieldSize;

            if (animating) return;

            if (castingspell)
            {
                spellPoints.Add(new Point(X, Y));
                map.overlayObjects.Add(new OverlayRectangle(x, y, 20, 20, Color.Orange));
                if (spellPoints.Count == activeSpell.format.Positions)
                {
                    //Now cast
                    WriteConsole(activeSpell.Activate(new SpellInformation() { positions = spellPoints, mage = humanPlayer }));

                    spellPoints.Clear();
                    castSpell.Text = "Cast spell";
                    activeSpell = null;
                    castingspell = false;
                    UpdateSpellInfo();
                    ShowPlayerStats();
                }
            }
            else
            {
                actionOccuring = true;
                FocusField(X, Y);
                actionOccuring = false;
                RenderMap(forceEntityRedraw);
                forceEntityRedraw = false;
            }
        }

        private void FocusField(int X, int Y)
        {
            SelectField(X, Y);

            Troop p = map.troops.Find(t => t.Position.X == X && t.Position.Y == Y);
            if (p != null)
            {
                troopList.SelectedIndex = -1;
                Player selected = players.Find(P => P.troop == p);
                if (troopList.SelectedIndex != troopList.Items.IndexOf(selected.Name))
                {
                    troopList.SelectedIndex = troopList.Items.IndexOf(selected.Name);
                }
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
                    forceEntityRedraw = true; //In case enemy is killed
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

                        //Generate path of movement
                        DistanceGraphCreator movementGraph = new DistanceGraphCreator(humanPlayer, humanPlayer.troop.Position.X, humanPlayer.troop.Position.Y, moveTo.position.X, moveTo.position.Y, map, true, true);
                        movementGraph.CreateGraph();

                        List<Point> movement = new List<Point>() { };
                        Point pointer = moveTo.position;
                        Point last = pointer;
                        //Easy: Use newer methods
                        while (pointer != humanPlayer.troop.Position)
                        {
                            pointer = AIUtility.GetFields(pointer, movementGraph).Aggregate((min, point) => movementGraph.graph.Get(point) < movementGraph.graph.Get(min) ? point : min);
                            movement.Add(last.Sub(pointer)); //Opposite order as we will reverse the array later
                            last = pointer;
                            if (movement.Count > 100) throw new Exception();
                        }
                        movement.Reverse();

                        MovePlayer(moveTo.position, humanPlayer.troop.Position, humanPlayer, MovementType.walk, true, movement);

                        canMoveTo.Clear();
                        ShowPlayerStats();
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
                    DistanceGraphCreator distanceGraph = new DistanceGraphCreator(humanPlayer, X, Y, X, Y, map, true); //Set start location at the same position so that is expands outwards
                    distanceGraph.CreateGraph();
                    while (toCheck.Count != 0)
                    {
                        MapTile checking = toCheck[0];
                        toCheck.Remove(checking);
                        if (distanceGraph.graph.Get(checking.position) >= 0)
                        {
                            possibleFields.Add(checking);
                            List<MapTile> sorroundingTiles = new SorroundingTiles<MapTile>(checking.position, map.map).rawMaptiles.ToList();
                            sorroundingTiles.ForEach(t =>
                            {
                                t.leftValue = distanceGraph.graph.Get(t.position);
                            });
                            sorroundingTiles = sorroundingTiles.Where(t => t.leftValue <= humanPlayer.actionPoints && t.free && !possibleFields.Exists(f => f.position == t.position)).ToList();
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
                    return;
                }
            }
        }

        #endregion Player Game Board Interaction

        #endregion Event Handler

        #region Player Events

        public bool DamageAtField(int damage, DamageType damageType, Point point)
        {
            if (players.Exists(p => p.troop.Position == point))
            {
                DamagePlayer(damage, damageType, players.First(p => p.troop.Position == point));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Damage player is used to damage players due to enviromental(statuses) causes
        /// </summary>
        /// <param name="damage"> Damage dealt </param>
        /// <param name="player"> Player damaged </param>
        public void DamagePlayer(int damage, DamageType damageType, Player player)
        {
            if (humanPlayer is null) return;

            damage = Math.Min(player.troop.health, (int)(damage * player.troop.GetVurneability(damageType)));
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
                    map.RemoveEntityRenderObject(player.troop.name);

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

            RenderMap();
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
            map.RemoveEntityRenderObject(humanPlayer.troop.name);
            UpdatePlayerList();
            RenderMap();
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
                Reward reward = mission.Reward();
                humanPlayer.GainXP(reward.XP);
                DialogResult result = MessageBox.Show($"You have gained {reward.XP} xp. {(humanPlayer.storedLevelUps != 0 ? $"You have {humanPlayer.storedLevelUps} level ups stored. Would you like to level up?" : "")}", "XP gained", humanPlayer.storedLevelUps != 0 ? MessageBoxButtons.YesNo : MessageBoxButtons.OK);
                if (humanPlayer.storedLevelUps != 0 && result == DialogResult.Yes)
                {
                    LevelUp();
                }
                if (reward.weaponReward != null)
                {
                    Weapon received = campaign.CalculateReward(((WeaponReward)reward.weaponReward));
                    WeaponView weaponView = new WeaponView(received, true);
                    weaponView.ShowDialog();
                    if (weaponView.decision)
                    {
                        humanPlayer.troop.weapons.Add(received);
                    }
                }

                if (reward.spellReward != null)
                {
                    Spell spell = ((SpellReward)reward.spellReward).spell;
                    if (!humanPlayer.spells.Exists(s => spell.name == s.name))
                    {
                        if (MessageBox.Show($"You have the ability to gain the spell {spell.name}. Do you want to gain it?", "Spell Gained", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            humanPlayer.spells.Add(spell);
                            UpdateSpellList();
                        }
                    }
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
                RenderMap();
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
                    map.RemoveEntityRenderObject(attacked.troop.name);
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
                RenderMap();
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
            map.renderObjects.Add(new EntityRenderObject(player.troop, new TeleportPointAnimation(new Point(0, 0), player.troop.Position)));
            UpdatePlayerList();
        }

        //TODO: HAndle all movement through move player, even when using graphs
        public void MovePlayer(Point end, Point start, Player player, MovementType movementType, bool CostActionPoints = true, List<Point> path = null)
        {
            bool render = false;
            if (path is null)
            {
                path = new List<Point>() { end.Sub(start) };
            }
            int distance = 0;

            switch (movementType)
            {
                case MovementType.walk:
                    distance = path.Count;
                    if (CostActionPoints)
                    {
                        Point pointer = new Point(start.X, start.Y);
                        for (int i = 0; i < path.Count; i++)
                        {
                            Point next = new Point(pointer.X + path[i].X, pointer.Y + path[i].Y);
                            player.actionPoints -= player.CalculateStep(map.map.Get(start), map.map.Get(pointer), map.map.Get(next), i + 1, MovementType.walk);
                        }
                    }
                    break;

                case MovementType.teleport:
                    distance = AIUtility.Distance(start, end);
                    if (CostActionPoints)
                        player.actionPoints -= 1;
                    break;

                default:
                    break;
            }
            //If end position is blocked, take damage and go to a close tile
            if (!map.map[end.X, end.Y].free)
            {
                actionOccuring = true;
                DamagePlayer(10, DamageType.earth, player);
                WriteConsole($"{player.troop.name} takes 10 damage as he is in an object!");
                if (player.troop.health == 0)
                {
                    actionOccuring = false;
                    RenderMap();
                    return;
                }
                Troop receiving = map.troops.FirstOrDefault(t => t.Position == end);
                bool free = false;
                if (receiving != null)
                {
                    Player recPlayer = GetPlayer(receiving);
                    DamagePlayer(10, DamageType.earth, recPlayer);
                    WriteConsole($"{receiving.name} has been daamaged as {player.troop.name} has teleported into it!");
                    if (recPlayer is null || recPlayer.Dead)
                    {
                        free = true;
                    }
                }
                if (!free)
                {
                    //Find neighbour field which is free else kill the entity
                    MapTile escape = map.map.Get(end).neighbours.rawMaptiles.FirstOrDefault(m => m.free);
                    if (escape is null)
                    {
                        DamagePlayer(player.troop.health + 2, DamageType.unblockable, player); //Should kill him
                        return;
                    }
                    else
                    {
                        end = escape.position;
                        path.Add(end);
                        render = true;
                    }
                    actionOccuring = false;
                }
            }

            Point troopP = player.troop.Position;
            troopP.X = end.X;
            troopP.Y = end.Y;
            player.troop.Position = troopP;

            PlayerMoved(this, new PlayerMovementData()
            {
                player = player,
                start = map.map[start.X, start.Y],
                goal = map.map[end.X, end.Y],
                distance = distance,
                movementType = movementType,
                map = map,
                path = path.ToArray()
            }); //Actually add path
            if (render)
            {
                RenderMap();
            }
        }

        #endregion Program Interaction

        #region Helper Functions

        /// <summary>
        /// Finds player for a specific troop
        /// </summary>
        /// <param name="troop"></param>
        /// <returns></returns>
        public Player GetPlayer(Troop troop)
        {
            return players.FirstOrDefault(p => p.troop == troop);
        }

        public Player GetPlayer(Point point) => players.FirstOrDefault(p => p.troop.Position == point);

        #endregion Helper Functions
    }
}