using Microsoft.VisualStudio.TestTools.UnitTesting;
using StartGame.Rendering;
using StartGame.World;
using System.Drawing;

namespace Tests
{
    [TestClass]
    public class WordMapTest
    {
        [TestMethod]
        public void GenerateMap()
        {
            Assert.IsNotNull(World.Instance);

            Assert.IsNotNull(World.Instance.MovementCost());

            WorldRenderer worldRenderer = new WorldRenderer();
            Assert.IsNotNull(worldRenderer);
            Bitmap b = worldRenderer.Render(200, 200);
            Assert.IsNotNull(b);
            Assert.IsTrue(b.Width == 200 && b.Height == 200);
            Assert.IsNotNull(worldRenderer.DrawRainfallMap());
            Assert.IsNotNull(worldRenderer.DrawAgriculturalMap());
            Assert.IsNotNull(worldRenderer.DrawBackground());
            Assert.IsNotNull(worldRenderer.DrawHeightMap());
            Assert.IsNotNull(worldRenderer.DrawIslands());
            Assert.IsNotNull(worldRenderer.DrawMineralMap());
            Assert.IsNotNull(worldRenderer.DrawNationMap());
            Assert.IsNotNull(worldRenderer.DrawRawHeightMap());
            Assert.IsNotNull(worldRenderer.DrawTemperatureMap());
            Assert.IsNotNull(worldRenderer.DrawValueMap());
        }
        [TestMethod]

        public void CheckZoomLevels()
        {
            WorldRenderer renderer = new WorldRenderer();
            for (int i = 0; i < 38; i++)
            {
                int zoom = 40 - i;
                Assert.IsNotNull(renderer.Render(100, 100, zoom));
            }
        }
    }
}