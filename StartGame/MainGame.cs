using StartGame.Dungeons;
using StartGame.Entities;
using StartGame.Items;
using StartGame.PlayerData;
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
    public partial class MainGameWindow : Form
    {
        //Long term: Add dialog
        //Long term: Add save feature
        //Bug: Leveling up endurance to 10 to increase action points will not set value to max, instantly

        private const int fieldSize = MapCreator.fieldSize;

        private Random random;

        public Map map;

        private Point selected;

        public List<Player> players;
        public Player activePlayer;
        public HumanPlayer humanPlayer;
        private readonly Mission mission;
        private int activePlayerCounter = 0;
        public bool gameStarted = false;

        private List<WinCheck> winConditions;
        private List<WinCheck> deathConditions;
        private bool useWinChecks = false;

        public bool dead = false;

        private string description;

        public int playerDamage;
        public int playerDoged;

        private Spell activeSpell;
        private List<Point> spellPoints = new List<Point>();
        private bool castingspell;

        private DebugEditor debug;
        public bool forceWin = false;

        public List<Player> killedPlayers = new List<Player>();

        public MainGameWindow(Map Map, HumanPlayer player, Mission mission,
            List<Tree> trees, int difficulty, int round)
        {
            player.main = this;
            map = Map;
            if (map.map is null)
                throw new Exception("Map of maptiles can not be undefined when starting the game. Please initialise before calling this function");

            selected = new Point(-1, 0);
            random = new Random();

            //Setup mission
            bool first = true;
            do
            {
                if (!first)
                {
                    map = new Map();
                }
                else
                    first = false;
                (players, winConditions, deathConditions, description) = mission.GenerateMission(difficulty, round, ref map, player);
            } while (players is null);

            useWinChecks = mission.useWinChecks;
            //Setup game
            humanPlayer = player;
            this.mission = mission;
            activePlayer = players[0];

            players.ForEach(p => { if (!map.troops.Contains(p.troop)) map.troops.Add(p.troop); }); //Sometimes may already have been added in a map
            players.ForEach(p => { if (!map.entities.Contains(p.troop)) map.entities.Add(p.troop); });
            players.ForEach(p => { if (!map.renderObjects.Exists(e => e.Name == p.troop.Name)) map.renderObjects.Add(
                  new EntityRenderObject(p.troop, new TeleportPointAnimation(new Point(0, 0), p.troop.Position)));
            });

            InitializeComponent();

            exitMission.Visible = !useWinChecks;
            //Initialse functions
            CalculatePlayerAttackDamage.Add(CalculateDamage);

            //Start work to update information in GUI
            RenderMap();
            //GUI Work
            console.Text = "Starting game ... \n";
            enemyMovement.Checked = mission != null ? mission.EnemyMoveTogether : false;

            //Add players to list
            UpdatePlayerList();

            //Initialise information about player
            playerView.Activate(humanPlayer, this, true);
            ShowPlayerStats();

            //Initialise trees
            trees.ForEach(t => t.Initialise(this));

            //Activate players tress
            humanPlayer.trees.ForEach(t => t.Activate());

            //Activate all players
            players.ForEach(p => p.Initialise(this));

            //As it is first turn - set action button to start the game
            nextAction.Text = "Start game!";

            ShowPositionStats();

            //Set level up button correctly
            levelUpButton.Enabled = humanPlayer.storedLevelUps != 0;

            //DEBUG
            debug = new DebugEditor(this);

            humanPlayer.spells.ForEach(s => s.Initialise(this, map));

            UpdateSpellList();

            if (mission is Dungeon d)
                d.EnterDungeon(player, this);
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
        public event EventHandler<PlayerMovementData> PlayerMoved = (sender, data) => {
            EntityRenderObject entity = data.map.EntityRenderObjects.FirstOrDefault(e => e.Name == data.player.troop.Name);

            Trace.TraceInformation($"PLayer Moved: Player Name {data.player.troop.Name} Render Object {entity.Name} Start {data.start} End {data.goal}");

            if (data.player.troop.health.Value == 0)
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
            //Trigger the first building
            for (int i = 0; i < data.map.entities.Count; i++)
            {
                Entity e = data.map.entities[i];
                if (e.Position == data.player.troop.Position && e is Building b)
                {
                    b.PlayerEnter(data.player);
                    break;
                }
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
            public BodyPart hit;
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
                if (weapon.type != BaseAttackType.range)
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
                nextAction.Text = "End turn";
                nextAction.Enabled = true;
            }
            else
            {
                nextAction.Enabled = false;
                nextAction.Text = "Next turn!";
                nextAction.Enabled = true;
            }
            nextAction.Focus();
        }

        #endregion Game Loop

        #endregion Game Logic

        #region GUI Updates

        private void UpdateSpellList()
        {
            UpdateSpellInfo();
        }

        public void UpdateSpellInfo()
        {
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

        public void RenderMap(bool forceEntityRedrawing = false, bool forceDrawBackground = false)
        {
            if (!animating && !actionOccuring)
            {
                StackTrace stackTrace = new StackTrace();
                Trace.TraceInformation($"Rendering - Called from {stackTrace.GetFrame(1).GetMethod().Name}");
                animating = true;
                frames = new List<Bitmap>();
                animator = new Thread(() => map.Render(gameBoard, frames, forceEntityRedrawing, frameTime: 100, debug: false,
                    colorAlpha: showHeightDifference.Checked ? 50 : 255,
                    showInverseHeight: showHeightDifference.Checked, forceDrawBackground: forceDrawBackground)) {
                    Name = "Animator Thread"
                };
                animator.Start();
                System.Timers.Timer timer = new System.Timers.Timer(100) {
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

        public void UpdateTroopList()
        {
            troopList.Items.Clear();
            troopList.Items.AddRange(map.troops.Select(t => t.Name).ToArray());
        }

        public void UpdatePlayerView()
        {
            playerView.Render();
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
                if (position.X >= map.width || position.Y >= map.height)
                    fieldHeight.Text = "";
                else
                    fieldHeight.Text = "Height: " + map.map[position.X, position.Y].Height.ToString("0.##");
            }
        }

        private void UpdatePlayerList()
        {
            List<string> playerNames = players.ConvertAll(t => t.troop.Name);
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

        public void ShowPlayerStats()
        {
            playerView.Render();
        }

        private void ShowEntityData(Entity entity)
        {
            troopList.SelectedIndex = -1;
            enemyName.Text = entity.Name;
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
                enemyHealth.Text = $"{player.troop.health}";
                enemyPosition.Text = $"{player.troop.Position.X} : {player.troop.Position.Y}";
                enemyHeight.Text = $"{map.map[player.troop.Position.X, player.troop.Position.Y].Height.ToString("0.##")}";
                enemyDefense.Text = $"Defense: {player.troop.defense}";
            }
            if (clicked)
            {
                SelectField(player.troop.Position.X, player.troop.Position.Y);
            }
        }

        #endregion GUI Updates

        #region Event Handler

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

        private void PlayerView1_Load(object sender, EventArgs e)
        {
        }

        private void SpellList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSpellInfo();
        }

        private void GameBoard_Click(object sender, EventArgs e)
        {
        }

        private void NextAction_Click(object sender, EventArgs e)
        {
            if (humanPlayer is null) Close();
            activePlayer.ActionButtonPressed(this);
        }

        private void CastSpell_Click(Spell activeSpell, Button castSpell)
        {
            if (activeSpell.Ready && humanPlayer.mana.Value >= activeSpell.manaCost)
            {
                humanPlayer.mana.rawValue -= activeSpell.manaCost;
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

        private void ExitMission_Click(object sender, EventArgs e)
        {
            PlayerWins("You exit the mission!");
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
                    playerView.Render();
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
                if (map.entities.Exists(t => t.Position.X == X && t.Position.Y == Y))
                {
                    Entity e = map.entities.Find(t => t.Position.X == X && t.Position.Y == Y);
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

                        List<Point> movement = movementGraph.GeneratePath(moveTo.position, humanPlayer.troop.Position, map).ToList().ConvertAll(m => m.position);
                        //calculate the actual movements not the real fields
                        for (int i = 0; i < movement.Count; i++)
                        {
                            if (movement.Count - i - 1 != 0)
                                movement[movement.Count - i - 1] = movement[movement.Count - i - 2].Sub(movement[movement.Count - i - 1]);
                            else
                                movement[movement.Count - i - 1] = moveTo.position.Sub(movement[movement.Count - i - 1]);
                        }
                        movement.Remove(movement[0]);
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
                            sorroundingTiles.ForEach(t => {
                                t.leftValue = distanceGraph.graph.Get(t.position);
                            });
                            sorroundingTiles = sorroundingTiles.Where(t => t.leftValue <= humanPlayer.movementPoints.Value && t.free && !possibleFields.Exists(f => f.position == t.position)).ToList();
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
                        map.overlayObjects.Add(new OverlayText(f.position.X * fieldSize, f.position.Y * fieldSize, Color.DarkGreen, $"{humanPlayer.movementPoints.Value - f.leftValue}")));
                    canMoveTo.AddRange(possibleFields);

                    //Show all enemies it might hit and damage dealt
                    Point center = humanPlayer.troop.Position;
                    if (humanPlayer.actionPoints.Value >= 1 && humanPlayer.troop.activeWeapon.attacks > 0)
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

            damage = Math.Min(player.troop.health.Value, (int)(damage * player.troop.GetVurneability(damageType)));
            player.troop.health.rawValue -= damage;

            if (player.troop.health.Value <= 0)
            {
                if (player.Name == humanPlayer.Name)
                {
                    PlayerDied();
                    return;
                }
                else
                {
                    EnemyKilled(player);
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
            map.entities.Remove(humanPlayer.troop);
            map.RemoveEntityRenderObject(humanPlayer.troop.Name);
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

            Close();
        }

        public int CalculateDamage(CombatData data)
        {
            return CalculateDamage(data.attacker.troop.Position, data.attacker, data.attacked, data.weapon, data.hit);
        }

        public int CalculateDamage(Player attacking, Player defending, Weapon weapon, BodyPart hit)
        {
            return CalculateDamage(attacking.troop.Position, attacking, defending, weapon, hit);
        }

        /// <summary>
        /// This function simualates combat and returns the damage dealt, armour is simulated here and changes in armour are directly applied
        /// </summary>
        /// <param name="attackingPosition"></param>
        /// <param name="attacking"></param>
        /// <param name="defending"></param>
        /// <param name="weapon"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public int CalculateDamage(Point attackingPosition, Player attacking, Player defending, Weapon weapon, BodyPart hit)
        {
            string text = $"{defending.Name} is attacked by {attacking.Name} at the {hit.name}";
            int damage = weapon.attackDamage - defending.troop.defense.Value;
            damage += attacking.strength.Value;

            //if melee code check for height difference
            if (weapon.type == BaseAttackType.melee)
            {
                double attackingHeight = map.map[attackingPosition.X, attackingPosition.Y].Height;
                double defendingHeight = map.map[defending.troop.Position.X, defending.troop.Position.Y].Height;

                double difference = attackingHeight - defendingHeight;

                damage = damage + (int)Math.Ceiling(difference * damage);
            }
            text += $"Raw damage: {damage}";

            //Find damage protected by armour
            List<Armour> protecting = defending.troop.armours.Where(a => a.active && a.affected.Exists(b => b == hit.part)).ToList();
            protecting = protecting.OrderByDescending(p => (int)p.layer).ToList();

            //Armour must be penetrated from the outside inwards, if sharp must be stronger than armour to continue full strength or it is like blunt, portion of damage continues, magic just ignore all but magic resistance
            BaseDamageType damageType = weapon.damageType;
            foreach (var layer in protecting)
            {
                int durLost;
                switch (damageType)
                {
                    case BaseDamageType.sharp:
                        if (layer.sharpDefense >= damage)
                        {
                            //Weapon does not penetrate
                            damage = layer.sharpDefense < damage * 2 ? damage / 10 : 0;
                            durLost = damage * 2 + 1;
                            text += $"The weapon does not penetrate {layer.name}. {damage} continues as blunt damage. {layer.name} looses {durLost} durability.";
                            layer.durability -= durLost;
                            damageType = BaseDamageType.blunt;
                        }
                        else
                        {
                            damage -= layer.sharpDefense;
                            durLost = (int)(damage * (double)layer.durability / layer.maxDurability) + 1;
                            text += $"The weapon penetrates {layer.name}. {damage} continues on as sharp damage. {layer.name} looses {durLost} durability.";
                            layer.durability -= durLost;
                        }
                        break;

                    case BaseDamageType.blunt:
                        int redDamage = (int)(damage * (100 - (double)layer.bluntDefense) / 100d);
                        durLost = damage - redDamage;
                        text += $"The weapon does a blunt attack on {layer.name}. {redDamage} from {damage} is not blocked. {layer.name} looses {durLost} durability.";
                        layer.durability -= durLost;
                        damage = redDamage;
                        break;

                    case BaseDamageType.magic: //Longterm: Find better way to handle magic defense
                        redDamage = (int)(damage * (100 - (double)layer.magicDefense) / 100d);
                        durLost = damage - redDamage;
                        text += $"The weapon does a magic attack on {layer.name}. {redDamage} from {damage} is not blocked. {layer.name} looses {durLost} durability.";
                        layer.durability -= durLost;
                        damage = redDamage;
                        break;

                    default:
                        break;
                }
                if (layer.durability <= 0)
                {
                    //Armor broken
                    text += $"The attack destroys {layer.name}";
                    defending.troop.armours.Remove(layer);
                }
                if (damage == 0) break;
            }
            console.Text += text + "\n";
            damage = defending.troop.health.Value - damage < 0 ? defending.troop.health.Value : damage;
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

            //Check if hit
            if (random.Next(100) < attacked.troop.dodge.Value)
            {
                attacking.actionPoints.rawValue -= attacking.troop.activeWeapon.attackCost;
                attacking.troop.activeWeapon.attacks--;
                //dodged
                if (show)
                    map.overlayObjects.Add(new OverlayText(attacked.troop.Position.X * fieldSize, attacked.troop.Position.Y * fieldSize, Color.Red, "Dodged!"));
                RenderMap();
                Combat(this, new CombatData() { attacked = attacked, attacker = attacking, doged = true, damage = damage, killed = false, range = AIUtility.Distance(attacking.troop.Position, attacked.troop.Position), weapon = attacking.troop.activeWeapon });
                return (0, false, false);
            } //Long term: Add scraping hit

            //Determine where teh attacker hits the attacked;
            BodyPart hit = DetermineHitBodyPart(attacking, attacked);

            if (attacking.Name == humanPlayer.Name)
            {
                CombatData combatData = new CombatData() {
                    attacked = attacked,
                    attacker = attacking,
                    damage = 0,
                    doged = false,
                    killed = false,
                    range = AIUtility.Distance(attacked.troop.Position, attacking.troop.Position),
                    weapon = attacking.troop.activeWeapon,
                    hit = hit
                };

                foreach (var func in CalculatePlayerAttackDamage)
                {
                    combatData.damage = func(combatData);
                }
                damage = combatData.damage;
            }
            else
            {
                damage = CalculateDamage(attacking, attacked, attacking.troop.activeWeapon, hit);
            }

            attacking.troop.activeWeapon.attacks--;
            attacking.actionPoints.rawValue -= attacking.troop.activeWeapon.attackCost;

            attacked.troop.health.rawValue -= damage;

            if (attacked.troop.health.Value <= 0)
            {
                killed = true;
                if (attacked.Name == humanPlayer.Name)
                {
                    //Do nothing allow player AI code to handle it
                }
                else
                {
                    EnemyKilled(attacked);

                    Combat(this, new CombatData() { attacked = attacked, attacker = attacking, doged = false, damage = damage, killed = true, range = AIUtility.Distance(attacking.troop.Position, attacked.troop.Position), weapon = attacking.troop.activeWeapon, hit = hit });
                }
            }
            else
            {
                Combat(this, new CombatData() { attacked = attacked, attacker = attacking, doged = false, damage = damage, killed = false, range = AIUtility.Distance(attacking.troop.Position, attacked.troop.Position), weapon = attacking.troop.activeWeapon, hit = hit });
            }

            if (show)
            {
                map.overlayObjects.Add(new OverlayText(attacked.troop.Position.X * fieldSize, attacked.troop.Position.Y * fieldSize, Color.Red, $"-{damage}"));
                RenderMap();
                if (attacking == humanPlayer)
                {
                    WriteConsole($"You have attacked {attacked.Name} for {damage}");
                }
            }

            canAttack.Clear();
            canMoveTo.Clear();

            ShowPlayerStats();
            ShowEnemyStats();

            return (damage, killed, true);
        }

        private void EnemyKilled(Player killed)
        {
            //Enemy Died
            killedPlayers.Add(killed);
            killed.troop.Die();
            map.troops.Remove(killed.troop);
            map.entities.Remove(killed.troop);
            map.RemoveEntityRenderObject(killed.troop.Name);
            players.Remove(killed);
            UpdatePlayerList();

            //Let player gain xp
            humanPlayer.GainXP(killed.XP);
        }

        public void TreeGained(Tree tree)
        {
            MessageBox.Show($"You have gained the {tree.GetType().BaseType.Name} {tree.name}! \n {tree.description} \n Unlocked by: {tree.reason}", "Tree gained!");
        }

        public void SkillLevelUp(Skill skill)
        {
            MessageBox.Show($"The skill {skill.name} has leveled up to level {skill.level}!");
        }

        public BodyPart DetermineHitBodyPart(Player attacker, Player defender)
        {
            return defender.troop.body.bodyParts[random.Next(defender.troop.body.bodyParts.Count)];
        }

        #endregion Player Events

        #region Program Interaction

        public void AddPlayer(Player player)
        {
            players.Add(player);
            map.troops.Add(player.troop);
            player.troop.Map = map;
            map.entities.Add(player.troop);
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
                            player.movementPoints.MoveUnit(player.CalculateStep(map.map.Get(start), map.map.Get(pointer), map.map.Get(next), i + 1, MovementType.walk));
                            pointer = next;
                        }
                    }
                    break;

                case MovementType.teleport:
                    distance = AIUtility.Distance(start, end);
                    if (CostActionPoints)
                        player.actionPoints.rawValue -= 1;
                    break;

                default:
                    break;
            }
            //If end position is blocked, take damage and go to a close tile
            if (!map.map[end.X, end.Y].free)
            {
                actionOccuring = true;
                DamagePlayer(10, DamageType.earth, player);
                WriteConsole($"{player.troop.Name} takes 10 damage as he is in an object!");
                if (player.troop.health.Value == 0)
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
                    WriteConsole($"{receiving.Name} has been daamaged as {player.troop.Name} has teleported into it!");
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
                        DamagePlayer(player.troop.health.Value + 2, DamageType.unblockable, player); //Should kill him
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

            PlayerMoved(this, new PlayerMovementData() {
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

        public Player GetPlayer(Point point)
        {
            return players.FirstOrDefault(p => p.troop.Position == point);
        }

        #endregion Helper Functions
    }
}