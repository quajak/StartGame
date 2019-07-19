using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Accord.Video.FFMPEG;
using StartGame.Rendering;

namespace StartGame
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainGameMenu());
            //Application.Run(new World.WorldGenerationViewer());
            //MakeVideo();
            //FindOptimalThreads();
        }

        /// <summary>
        /// Call to make video of world with time
        /// </summary>
#pragma warning disable IDE0051 // Remove unused private members
        static void MakeVideo()
#pragma warning restore IDE0051 // Remove unused private members
        {
            int frames = 6000;
            WorldRenderer worldRenderer = new WorldRenderer();
            using (VideoFileWriter vFWriter = new VideoFileWriter())
            {
                const int RENDERSIZE = World.World.WORLD_SIZE * 4;
                int framRate = 15;
                // create new video file
                vFWriter.Open("output.mp4", RENDERSIZE, RENDERSIZE, framRate, VideoCodec.MPEG4, 163840000);
                try
                {
                    for (int i = 0; i < frames; i++)
                    {
                        Bitmap imageFrame = worldRenderer.Render(RENDERSIZE, RENDERSIZE, 4, false);
                        Thread thread = new Thread(() => { vFWriter.WriteVideoFrame(imageFrame); imageFrame.Dispose(); });
                        thread.Start();
                        World.World.Instance.ProgressTime();
                        thread.Join();
                    }

                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                    vFWriter.Close();
                }
            }
        }
        /// <summary>
        /// Run function to find optimal number of threads for atmosphere calculation
        /// </summary>
#pragma warning disable IDE0051 // Remove unused private members
        static void FindOptimalThreads()
#pragma warning restore IDE0051 // Remove unused private members
        {
            List<int> AverageTime = new List<int>();
            for (int i = 1; i < 20; i++)
            {
                Trace.TraceInformation($"Threads: {i}");
                World.World.THREADS = i;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                const int tests = 100;
                for (int j = 0; j < tests; j++)
                {
                    World.World.Instance.ProgressTime();
                }
                AverageTime.Add((int)stopwatch.ElapsedMilliseconds / tests);
            }
            Trace.TraceInformation(" --- Results --- ");
            for (int i = 0; i < AverageTime.Count; i++)
            {
                Trace.TraceInformation($"{i}: {AverageTime[i]} milliseconds");
            }
        }
    }
}