using StartGame.Entities;
using StartGame.Extra.Loading;
using StartGame.PlayerData;
using StartGame.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartGame.Dungeons
{
    public class Room
    {
        public string name;
        private int width;
        private int height;
        public Map map;
        public List<Door> doors = new List<Door>();
        private List<Entity> entities = new List<Entity>();
        public List<EntityPlaceHolder> entityPlaceHolders = new List<EntityPlaceHolder>();
        public List<Player> players = new List<Player>();
        private Player player;
        public int DoorID = 0;

        public Room(int Width, int Height, string Name)
        {
            width = Width;
            height = Height;
            name = Name;
            if (map is null)
            {
                map = new Map(this.Width, this.Height);
                map.SetupMap(MapTileTypeEnum.wall, false);
            }
        }

        public Room(int Width, int Height, string Name, int doorid, Map map) : this(Width, Height, Name)
        {
            this.map = map;
            DoorID = doorid;
        }

        public int Width
        {
            get => width; set
            {
                if (value != width)
                {
                    Map newMap = new Map(value, height);
                    lock (map.RenderController)
                    {
                        newMap.renderObjects.AddRange(map.renderObjects);
                        newMap.entities.AddRange(map.entities);
                        newMap.troops.AddRange(map.troops);
                    }
                    newMap.SetupMap(MapTileTypeEnum.wall);
                    for (int x = 0; x < Math.Min(value, width); x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            newMap.map[x, y] = map.map[x, y];
                        }
                    }
                    newMap.UpdateMapTileData();
                    map = newMap;
                }
                width = value;
            }
        }

        public int Height
        {
            get => height; set
            {
                if (value != height)
                {
                    Map newMap = new Map(width, value);
                    lock (map.RenderController)
                    {
                        newMap.renderObjects.AddRange(map.renderObjects);
                        newMap.entities.AddRange(map.entities);
                        newMap.troops.AddRange(map.troops);
                    }
                    newMap.SetupMap(MapTileTypeEnum.wall);
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < Math.Min(value, height); y++)
                        {
                            newMap.map[x, y] = map.map[x, y];
                        }
                    }
                    newMap.UpdateMapTileData();
                    map = newMap;
                }
                height = value;
            }
        }

        public (bool, string) Valid()
        {
            string message = "";
            List<Func<bool>> tests = new List<Func<bool>> {
                () => {
                    message = "The room needs a door!";
                    return doors.Count != 0;
                    },
                () => {
                    for (int x = 0; x < map.map.GetUpperBound(0); x++)
                    {
                        if(map.map[x, 0].free && !map.GetEntities(x, 0).Exists(e => e is Door))
                        {
                            message = $"The tile at {x},0 is neither blocked nor a door!";
                            return false;
                        }
                    }
                    return true;
                },
                () => {
                    for (int x = 0; x < map.map.GetUpperBound(0); x++)
                   {
                        if(map.map[x, map.map.GetUpperBound(1)].free && !map.GetEntities(x, map.map.GetUpperBound(1)).Exists(e => e is Door))
                        {
                            message = $"The tile at {x},{map.map.GetUpperBound(0)} is neither blocked nor a door!";
                            return false;
                        }
                    }
                    return true;
                },
                () => {
                    for (int y = 0; y < map.map.GetUpperBound(1); y++)
                        {
                        if(map.map[0, y].free && !map.GetEntities(0, y).Exists(e => e is Door))
                        {
                            message = $"The tile at 0,{y} is neither blocked nor a door!";

                            return false;
                        }
                    }
                    return true;
                },
                () => {
                    for (int y = 0; y < map.map.GetUpperBound(1); y++)
                    {
                        if(map.map[map.map.GetUpperBound(0), y].free && !map.GetEntities(map.map.GetUpperBound(0), y).Exists(e => e is Door))
                        {
                            message = $"The tile at {map.map.GetUpperBound(0)},{y} is neither blocked nor a door!";
                            return false;
                        }
                    }
                    return true;
                }
            };
            return (tests.All(t => t.Invoke()), message);
        }

        public void PlayerEnter(Room from, Player player)
        {
            this.player = player ?? throw new ArgumentNullException(nameof(player));
            AddEntity(player.troop);

            var door = doors.Find(d => d.from == from);

            //Spawn player at this position
            player.troop.Position = door.Position;
        }

        public void AddEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            entities.Add(entity);
            lock (map.RenderController)
            {
                map.entities.Add(entity);
                map.renderObjects.Add(new EntityRenderObject(entity, new TeleportPointAnimation(new System.Drawing.Point(-1, -1), entity.Position)));
            }

            switch (entity)
            {
                case Door d:
                    doors.Add(d);
                    break;

                case Troop t:
                    players.Add(t.player);
                    map.troops.Add(t.player.troop);
                    break;

                default:
                    break;
            }
        }

        public void RemoveEntity(Entity entity)
        {
        }

        public override string ToString()
        {
            return $"{name} ({width}x{height})";
        }

        public bool Save(string path)
        {
            List<string> lines = new List<string> {
                //name
                E.WriteAttribute(name, "name"),
                //Door id
                E.WriteAttribute(DoorID, "doorid"),
                //Width
                E.WriteAttribute(width, "width"),
                //height
                E.WriteAttribute(height, "height")
            };
            //now write all tiles
            for (int x = 0; x <= map.map.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= map.map.GetUpperBound(1); y++)
                {
                    lines.Add(E.WriteAttribute(map.map[x, y].RawData(), "tile"));
                }
            }
            //now we write all entities
            foreach (var e in entities)
            {
                lines.Add(E.WriteAttribute(e.RawValue(), "entity"));
            }
            File.WriteAllLines(path, lines.ToArray());
            return true;
        }

        public static Room Load(string name, string path, List<CustomPlayer> customPlayers)
        {
            string[] lines = File.ReadAllLines(path);
            string _name = lines[0].GetString();
            if (_name != name) throw new Exception();
            int doorid = lines[1].GetInt();
            int width = lines[2].GetInt();
            int height = lines[3].GetInt();
            Map map = new Map(width, height);
            map.SetupMap();
            int counter = 1;
            for (int x = 0; x <= map.map.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= map.map.GetUpperBound(1); y++)
                {
                    map.map[x, y] = MapTile.Load(lines[3 + counter++]);
                }
            }

            Room room = new Room(width, height, name, doorid, map);
            map.UpdateMapTileData();

            //Now we pre-load the entities
            while (3 + counter < lines.Length)
            {
                if (lines[3 + counter].Trim().Length != 0)
                    room.entityPlaceHolders.Add(lines[3 + counter].GetEntity(room, customPlayers));
                counter++;
            }
            return room;
        }
    }
}