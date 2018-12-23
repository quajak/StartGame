using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame;
using StartGame.Dungeons;
using StartGame.Entities;
using StartGame.Items;
using StartGame.PlayerData;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class DungeonTest
    {
        [TestMethod]
        public void SaLBasicDungeon()
        {
            Dungeon dungeon = new Dungeon("Test");
            Assert.IsNotNull(dungeon.active);
            Assert.IsTrue(dungeon.dungeonRooms.Count() == 1);
            Assert.IsTrue(dungeon.active.DoorID == 0);
            Assert.IsNull(dungeon.start.room);

            Assert.IsTrue(dungeon.Save(true));

            dungeon = Dungeon.Load("Test");
            Assert.IsNotNull(dungeon);
            Assert.IsNull(dungeon.start.room);
            Assert.IsNotNull(dungeon.active.map.map[0, 0].continent);
            Assert.IsTrue(dungeon.active.DoorID == 0);
            Assert.IsNotNull(dungeon.active.map.map[0, 0].neighbours);
            for (int x = 0; x <= dungeon.active.map.map.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= dungeon.active.map.map.GetUpperBound(1); y++)
                {
                    Assert.IsNotNull(dungeon.active.map.map[x, y]);
                }
            }
        }

        [TestMethod]
        public void SaLChangedDungeon()
        {
            Dungeon dungeon = new Dungeon("Test1");
            dungeon.active.map.map[0, 0].type = new MapTileType() { type = MapTileTypeEnum.land };
            dungeon.active.map.map[1, 0].color = Color.Red;
            dungeon.active.map.map[0, 0].free = true;
            dungeon.start = (dungeon.active, new Point(2, 2));
            dungeon.Save(true);

            dungeon = Dungeon.Load("Test1");
            Assert.IsNotNull(dungeon);
            Assert.IsNotNull(dungeon.start.room);

            Assert.IsTrue(dungeon.active.map.map[0, 0].type.type == MapTileTypeEnum.land);
            Assert.IsTrue(dungeon.active.map.map[1, 0].color.ToArgb() == Color.Red.ToArgb());
            Assert.IsTrue(dungeon.active.map.map[0, 0].free);
            Assert.IsTrue(dungeon.start.room.name == dungeon.active.name && dungeon.start.position == new Point(2, 2));
        }

        [TestMethod]
        public void SaLDungeonWithCustomEntities()
        {
            Dungeon dungeon = new Dungeon("SpiderTest");
            dungeon.active.map.map[0, 0].type = new MapTileType() { type = MapTileTypeEnum.land };
            dungeon.active.map.map[0, 0].free = true;
            CustomPlayer item = new CustomPlayer("spider", "spiderWarrior", Weapon.Fist, 0);
            item.troop.Position = new Point(0, 0);
            dungeon.customEntities.Add((CustomPlayer)item.Clone());
            dungeon.active.AddEntity(item.troop);

            Assert.IsTrue(dungeon.Save(true));

            dungeon = Dungeon.Load("SpiderTest");
            Assert.IsNotNull(dungeon);
            Assert.IsNotNull(dungeon.customEntities);
            Assert.IsTrue(dungeon.customEntities.Count == 1);
            Assert.IsTrue(dungeon.customEntities[0].Name == "spider");
            Assert.IsTrue(dungeon.active.map.entities.Count == 1);
        }

        [TestMethod]
        public void SaLDoorDungeon()
        {
            Dungeon dungeon = new Dungeon("EntityTest");
            Door door = new Door(dungeon, (null, null), dungeon.active, new Point(0, 0));
            dungeon.active.AddEntity(door);
            Assert.IsTrue(dungeon.active.DoorID == 1);
            Assert.IsTrue(door.Id == 0);
            dungeon.Save(true);

            dungeon = Dungeon.Load("EntityTest");
            Assert.IsNotNull(dungeon);
            Assert.IsNotNull(dungeon.active);
            Assert.IsNotNull(dungeon.active.map);
            Assert.IsTrue(dungeon.active.map.entities.Count == 1);
            Assert.IsTrue(dungeon.active.DoorID == 1);
        }

        [TestMethod]
        public void EntityRawValues()
        {
            Dungeon dungeon = new Dungeon("Test", 10, 10);
            Door door = new Door(dungeon, (null, null), dungeon.active, new Point(2, 2));
            Assert.AreEqual(door.RawValue(), "Door Door1 2 2 False 0 True null null");
        }

        [TestMethod]
        public void SaLDoorMultiRoom()
        {
            Dungeon dungeon = new Dungeon("DoorMultiRoom");
            Door entity = new Door(dungeon, (null, null), dungeon.active, new Point(3, 3));
            dungeon.active.AddEntity(entity);
            Room room = new Room(12, 12, "MultiRoom");
            dungeon.dungeonRooms.Add(room);
            room.AddEntity(new Door(dungeon, (dungeon.active, entity), room, new Point(2, 2)));

            Assert.IsTrue(dungeon.dungeonRooms.Count == 2);
            Room value = dungeon.dungeonRooms.FirstOrDefault(_d => _d.name == "MultiRoom");
            Assert.IsNotNull(value);
            Assert.IsTrue(value.doors.Count == 1);
            Assert.IsTrue(value.DoorID == 1);
            Door d = value.doors[0];
            Assert.IsNotNull(d);
            Assert.IsTrue(!d.unlinked);
            Assert.IsNotNull(d.Next.room);
            d = dungeon.active.doors[0];
            Assert.IsTrue(dungeon.active.DoorID == 1);
            Assert.IsTrue(!d.unlinked);
            Assert.IsNotNull(d.Next.room);

            dungeon.Save(true);

            dungeon = Dungeon.Load("DoorMultiRoom");
            Assert.IsTrue(dungeon.dungeonRooms.Count == 2);
            value = dungeon.dungeonRooms.FirstOrDefault(_d => _d.name == "MultiRoom");
            Assert.IsNotNull(value);
            Assert.IsTrue(value.doors.Count == 1);
            Assert.IsTrue(value.DoorID == 1);
            d = value.doors[0];
            Assert.IsNotNull(d);
            Assert.IsTrue(!d.unlinked);
            Assert.IsNotNull(d.Next.room);
            d = dungeon.active.doors[0];
            Assert.IsTrue(dungeon.active.DoorID == 1);
            Assert.IsTrue(!d.unlinked);
            Assert.IsNotNull(d.Next.room);
        }

        [TestMethod]
        public void PlayDungeon()
        {
            //Load the dungeon
            Dungeon dungeon = Dungeon.Load("new"); //This dungeon is first created and then manually copied over
            Assert.IsNotNull(dungeon);
            Assert.IsNotNull(dungeon.active);
            Assert.IsNotNull(dungeon.active.map);
            Assert.IsFalse(dungeon.useWinChecks);
            Assert.IsTrue(dungeon.IsValid().Item1);

            Map map = dungeon.active.map;
            HumanPlayer player = new HumanPlayer(PlayerType.localHuman, "Test", map, new Player[] { }, null, 0);
            player.troop = new Troop("Test", null, null, 0, map, player);
            (List<Player> players, List<WinCheck> winCondition, List<WinCheck> loss, string description) =
                dungeon.GenerateMission(1, 1, ref map, player);
            Assert.IsNotNull(map);
            Assert.IsTrue(players.Count == 1);
            Assert.IsNull(winCondition);
            Assert.IsTrue(description.Trim().Length != 0);
        }
    }
}