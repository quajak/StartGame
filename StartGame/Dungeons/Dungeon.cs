using StartGame.Entities;
using StartGame.Extra.Loading;
using StartGame.Mission;
using StartGame.PlayerData;
using StartGame.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace StartGame.Dungeons
{
    public class Dungeon : Mission.Mission
    {
        public Room active;
        public List<Room> dungeonRooms = new List<Room>();
        private Player player;
        public string name;
        public (Room room, Point position) start = (null, new Point(-1, -1));
        public MainGameWindow mainGame;
        public List<CustomPlayer> customEntities = new List<CustomPlayer>();

        public Dungeon(string name, int FirstRoomWidth = 10, int FirstRoomHeight = 10) : base(name, false)
        {
            active = new Room(FirstRoomWidth, FirstRoomHeight, "Entrance");
            dungeonRooms.Add(active);
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public Dungeon(string name, List<Room> rooms, string startRoom, Point startPosition) : base(name, false)
        {
            this.name = name;
            dungeonRooms = rooms;
            if (startRoom != "null")
            {
                start = (dungeonRooms.First(r => r.name == startRoom), startPosition);
                active = start.room;
            }
            else
            {
                start = (null, new Point(-1, -1));
                active = dungeonRooms[0];
            }
        }

        public override (List<Player> players, List<WinCheck> winConditions, List<WinCheck> lossConditions, string description)
            GenerateMission(int difficulty, int Round, ref Map map, Player player)
        {
            if (!IsValid().Item1 || start.room is null) throw new Exception();
            map = start.room.map;
            active = start.room;
            player.troop.Map = map;
            player.troop.Position = start.position;
            player.map = map;
            List<Player> list = new List<Player> { player };
            list.AddRange(active.players);
            foreach (var room in dungeonRooms)
            {
                room.players.ForEach(p => p.enemies = new[] { player });
            }
            return (list, null, new List<WinCheck> { deathCheck },
                $"Welcome to the {name} dungeon!");
        }

        public override bool MissionAllowed(int Round)
        {
            //Easy: Add difficulty to dungeon
            return true;
        }

        public override bool MapValidity(Map map)
        {
            return true;
        }

        public override Reward Reward()
        {
            throw new NotImplementedException();
        }

        public void EnterDungeon(Player player, MainGameWindow gameWindow)
        {
            this.player = player;
            mainGame = gameWindow;
        }

        public void MoveTo(Room from, (Room, Door) to)
        {
            var (room, door) = to;
            active.map.troops.Remove(player.troop);
            active.players = mainGame.players;
            active.players.Remove(player);
            active.map.entities.Remove(player.troop);
            active.map.renderObjects.RemoveAll(r => r.Name == player.troop.Name);
            active = room;
            player.map = active.map;
            player.troop.Map = active.map;
            player.troop.Position = door.Position;
            if (mainGame != null)
            {
                mainGame.map = active.map;
                mainGame.map.troops.Add(player.troop);
                mainGame.map.entities.Add(player.troop);
                mainGame.map.renderObjects.Add(
                    new EntityRenderObject(player.troop, new TeleportPointAnimation(new Point(0, 0), player.troop.Position)));
                mainGame.players = active.players;
                mainGame.players.Add(player);
                mainGame.UpdateTroopList();
                mainGame.RenderMap(true, true);
            }
        }

        public (bool, string) IsValid()
        {
            List<Func<(bool, string)>> tests = new List<Func<(bool, string)>> {
                () => (dungeonRooms.All(r => r.Valid().Item1), "Room is invalid!"),
                () => (start.room != null, "Player must have a spawnpoint"),
                () => (!dungeonRooms.Exists(r => r.doors.Exists(d => d.unlinked)), "All doors must be linked!")
            };
            foreach (var item in tests)
            {
                (bool valid, string error) = item.Invoke();
                if (!valid) return (false, error);
            }
            return (true, "");
        }

        public static Dungeon Load(string name)
        {
            string curDir = Directory.GetCurrentDirectory();
            string dungeonPath = curDir + @"\" + name;
            return LoadPath(dungeonPath);
        }

        public static Dungeon LoadPath(string dungeonPath)
        {
            if (string.IsNullOrWhiteSpace(dungeonPath))
                throw new ArgumentException("message", nameof(dungeonPath));
            string name = Path.GetFileName(dungeonPath);
            if (!Directory.Exists(dungeonPath)) throw new Exception();
            DirectoryInfo dungeon = new DirectoryInfo(dungeonPath);

            //Load main.txt
            string[] lines = File.ReadAllLines(dungeonPath + @"\main.txt");
            string _name = lines[0].GetString();
            if (name != _name) throw new Exception($"The name found in the file {_name} is not equal {name}");
            int roomNumber = lines[1].GetInt();
            List<string> roomNames = lines[2].GetStringList();
            string startRoom = lines[3].GetString();
            Point startPosition = lines[4].GetPoint();
            List<string> customEntityTypes = lines[5].GetStringList();

            //Now load custom entities
            var customPlayers = customEntityTypes.ConvertAll(c => CustomPlayer.Load(dungeonPath + "\\" + c + ".txt"));

            //Now load dungeon rooms
            List<Room> rooms = roomNames.ConvertAll(r => Room.Load(r, dungeonPath + "\\" + r + ".txt", customPlayers));

            //Prepare to create deungon from individual parts
            Dungeon dungeon1 = new Dungeon(name, rooms, startRoom, startPosition) {
                customEntities = customPlayers
            };

            //Now we finish intialising the entities
            foreach (var room in dungeon1.dungeonRooms)
            {
                foreach (var entity in room.entityPlaceHolders)
                {
                    switch (entity)
                    {
                        case DoorPlaceHolder d:
                            room.AddEntity(d.Initialise(dungeon1));
                            break;

                        case PlayerPlaceHolder p:
                            p.player.map = room.map;
                            p.player.troop.Map = room.map;
                            room.AddEntity(p.player.troop);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                room.entityPlaceHolders.Clear();
            }

            return dungeon1;
        }

        public bool Save(bool autoOverride = false)
        {
            string curDir = Directory.GetCurrentDirectory();
            string dungeonPath = curDir + @"\" + name;
            DirectoryInfo dungeon = null;
            if (curDir == dungeonPath) throw new Exception();
            if (!autoOverride && Directory.Exists(dungeonPath))
            {
                if (MessageBox.Show($"A dungeon with the name {name} already exists. Do you want to override it?", "Overwrite safe", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    dungeon = new DirectoryInfo(dungeonPath);
                    dungeon.EnumerateDirectories().ToList().ForEach(d => d.Delete());
                    dungeon.EnumerateFiles().ToList().ForEach(f => f.Delete());
                }
                else
                {
                    return false;
                }
            }
            else
            {
                dungeon = Directory.CreateDirectory(dungeonPath);
            }

            //Save main data always value name
            List<string> lines = new List<string> {
                //Save name
                E.WriteAttribute(name, "name"),
                //Room number
                E.WriteAttribute(dungeonRooms.Count.ToString(), "roomnumber")
            };
            //Room names
            string names = "";
            foreach (Room room in dungeonRooms)
            {
                names += room.name + ' ';
            }
            lines.Add(E.WriteAttribute(names, "roomname"));
            string startroom = "";
            if (start.room is null)
            {
                startroom = "null";
            }
            else
            {
                startroom = start.room.name;
            }
            //Start room
            lines.Add(E.WriteAttribute(startroom, "startroom"));
            //Start position
            lines.Add(E.WriteAttribute($"{start.position.X} {start.position.Y}", "startposition"));
            //now save all the custom entity types
            lines.Add(E.WriteAttribute(string.Join(" ", customEntities.Select(c => c.Name)), "customEntityTypes"));
            //write to file
            File.WriteAllLines(dungeonPath + @"\main.txt", lines.ToArray());

            //save the custom entities
            customEntities.ForEach(c => c.Save(dungeonPath + @"\" + c.Name + ".txt"));
            //now save all the rooms
            dungeonRooms.ForEach(d => d.Save(dungeonPath + @"\" + d.name + ".txt"));
            return true;
        }

        public static List<string> GetDungeons()
        {
            string dir = Directory.GetCurrentDirectory();
            return Directory.EnumerateDirectories(dir).Select(d => new DirectoryInfo(d).Name).ToList();
        }
    }
}