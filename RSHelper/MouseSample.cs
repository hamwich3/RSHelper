using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;


namespace MouseSampling
{

    /*TODO:
     * run sample in thread with callback function
     * 
     * File Info:
     * 
     * name format: [DIST]_[LENGTH]_[DIRECTION]
     * where DIST is the distance from start to finish,
     * LENGTH is total length of movement in frames (each frame is ~1ms)
     * DIRECTION is left/right (x) then up/down (y): RD, RU, LD, LU
     * eg:
     * "123_456_UL"
     * "1234_2345_DR"
     * 
     */

    static class MouseSample
    {
        public static bool Recording { get { return recording; } }
        public static int FilesLoaded { get { return filesLoaded; } }
        public static int FileCount { get { return fileCount; } }
        public static event EventHandler LoadFinished = delegate { };
        public static event EventHandler MoveFinished = delegate { };

        static List<Sample> Samples = new List<Sample>();

        static bool nextSample = false;
        static bool recording = false;
        static bool startSampling = false;
        static int filesLoaded = 0;
        static int fileCount = 0;

        /// <summary>
        /// Start recording and saving samples
        /// </summary>
        public static void StartRecord()
        {
            recording = true;
            startSampling = false;
            RecordSamples();
        }

        /// <summary>
        /// Stop recording and saving samples
        /// </summary>
        public static void StopRecord()
        {
            recording = false;
        }

        /// <summary>
        /// Start new sample, stops recording previous sample
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void StartNewSample(object sender, EventArgs e)
        {
            if (recording)
            {
                startSampling = true;
                nextSample = true;
            }
        }

        /// <summary>
        /// Starts thread that records and saves mouse movement samples
        /// </summary>
        static void RecordSamples()
        {
            Stopwatch sw = new Stopwatch();
            List<Point> MouseFrames = new List<Point>();
            long millis = 0;
            long lastMillis = -1;
            Thread recordThread = new Thread(() =>
            {
                while (recording)
                {
                    while (!startSampling) Thread.Sleep(0);
                    sw.Restart();
                    millis = 0;
                    lastMillis = -1;
                    MouseFrames.Clear();
                    while (millis <= 30000 && !nextSample && recording)
                    {
                        millis = sw.ElapsedMilliseconds;
                        if (millis != lastMillis)
                        {
                            lastMillis = millis;
                            MouseFrames.Add(Cursor.Position);
                        }
                        Thread.Sleep(0);
                    }
                    if (millis < 30000 && MouseFrames.Count > 0 && recording)
                    {
                        Save(TrimStart(Normalize(MouseFrames)));
                    }
                    if (millis > 30000)
                    {
                        startSampling = false;
                    }
                    if (nextSample) nextSample = false;
                }
            });
            recordThread.Start();
        }

        /// <summary>
        /// Moves sample starting point to {0,0}
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static List<Point> Normalize(List<Point> sample)
        {
            List<Point> normalizedSample = new List<Point>();
            int xAdjust = sample[0].X;
            int yAdjust = sample[0].Y;
            for (int i = 0; i < sample.Count; i++)
            {
                normalizedSample.Add(new Point(sample[i].X - xAdjust, sample[i].Y - yAdjust));
            }
            return normalizedSample;
        }

        /// <summary>
        /// Trims all but one leading {0,0} points
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        public static List<Point> TrimStart(List<Point> sample)
        {
            List<Point> trimmedSample = new List<Point>();
            int startIndex = 0;
            for (int i = 0; i < sample.Count; i++)
            {
                if (!sample[0].Equals(sample[i]))
                {
                    startIndex = i - 1;
                    break;
                }
            }
            for (int i = startIndex; i < sample.Count; i++)
            {
                trimmedSample.Add(sample[i]);
            }
            return trimmedSample;
        }

        /// <summary>
        /// Shifts sample to end at desired location
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <returns></returns>
        public static List<Point> Transform(List<Point> sample, Point start, Point end)
        {
            List<Point> tSample = new List<Point>();
            //Normalize(sample);
            //Normalize x and y endpoint
            double normX = end.X - start.X;
            double normY = end.Y - start.Y;
            double xFactor = normX / sample.Last().X; // lol X Factor
            double yFactor = normY / sample.Last().Y;
            //Scales x,y factors from 0-1 over movement
            double tFactor = 1.0000 / sample.Count;

            for (int i = 0; i < sample.Count; i++)
            {
                double x = Math.Round(sample[i].X * (xFactor * (tFactor * i)));
                double y = Math.Round(sample[i].Y * (yFactor * (tFactor * i)));
                tSample.Add(new Point((int)x, (int)y));
            }
            tSample = Transpose(tSample, start);
            tSample[tSample.Count - 1] = end;
            return tSample;
        }

        /// <summary>
        /// Moves sample to start at desired location
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <returns></returns>
        public static List<Point> Transpose(List<Point> sample, Point start)
        {
            List<Point> tSample = new List<Point>();

            for (int i = 0; i < sample.Count; i++)
            {
                tSample.Add(new Point(sample[i].X + start.X, sample[i].Y + start.Y));
            }
            return tSample;
        }

        /// <summary>
        /// Move mouse from current position to new position using closest sample match
        /// </summary>
        public static void MoveTo(Point endP)
        {
            Point startP = Cursor.Position;
            Sample.SampleInfo.Directions dir;
            if (endP.X >= startP.X && endP.Y >= startP.Y) dir = Sample.SampleInfo.Directions.RightDown;
            else if (endP.X < startP.X && endP.Y >= startP.Y) dir = Sample.SampleInfo.Directions.LeftDown;
            else if (endP.X >= startP.X && endP.Y < startP.Y) dir = Sample.SampleInfo.Directions.RightUp;
            else dir = Sample.SampleInfo.Directions.LeftUp;
            float dist = ((int)Math.Sqrt(Math.Pow(endP.X - startP.X, 2) + Math.Pow(endP.Y - startP.Y, 2)));

            //Same sample direction, same or greater distance
            var samples = from sample in Samples
                          where (sample.Info.Direction == dir && sample.Info.Distance > dist && sample.Info.Distance < dist*1.3)
                          select sample;
            samples = samples.OrderBy((s) => { return s.Info.Length; });
            Random r = new Random();
            int n = r.Next(0, Math.Max((int)(samples.Count()*.3), 1));
            var movement = Transform(samples.ElementAt(n).Frames, startP, endP);
            Move(movement);
        }

        // TDOD: make into thread or async with callback
        public static void Move(List<Point> sample)
        {
            Thread moveThread = new Thread(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                long millis = 0;
                long lastMillis = -1;
                while (millis < sample.Count)
                {
                    if (millis != lastMillis)
                    {
                        lastMillis = millis;
                        Cursor.Position = sample[(int)millis];
                    }
                    millis = sw.ElapsedMilliseconds;
                }
                MoveFinished(null, new EventArgs());
            });
            moveThread.Start();
        }


        static void Save(List<Point> list)
        {
            string filename = "";
            string filepath = "";
            string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RS\samples";

            if (!Directory.Exists(initialDirectory))
            {
                Directory.CreateDirectory(initialDirectory);
            }

            int xDist = list.Last().X - list[0].X;
            int yDist = list.Last().Y - list[0].Y;
            string DIST = ((int)Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2))).ToString();
            string LENGTH = list.Count.ToString();
            string DIRECTION = "";

            if (xDist >= 0) DIRECTION += "R";
            else DIRECTION += "L";

            if (yDist >= 0) DIRECTION += "D";
            else DIRECTION += "U";

            filename += DIST;
            filename += "_";
            filename += LENGTH;
            filename += "_";
            filename += DIRECTION;
            filename += ".bin";

            filepath += initialDirectory;
            filepath += @"\";
            filepath += filename;

            Console.WriteLine("save");
            try
            {
                using (Stream stream = File.Open(filepath, FileMode.OpenOrCreate))
                {
                    foreach (var p in list)
                    {
                        byte[] xbytes = BitConverter.GetBytes(p.X);
                        byte[] ybytes = BitConverter.GetBytes(p.Y);
                        byte[] buffer = new byte[xbytes.Length + ybytes.Length];
                        Buffer.BlockCopy(xbytes, 0, buffer, 0, xbytes.Length);
                        Buffer.BlockCopy(ybytes, 0, buffer, xbytes.Length, ybytes.Length);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                MessageBox.Show(e.ToString());
            }
        }

        public static void LoadSamples()
        {
            Thread loadThread = new Thread(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RS\samples";
                var files = Directory.EnumerateFiles(initialDirectory);
                fileCount = files.Count();
                List<string> faillist = new List<string>();
                foreach (var file in files)
                {
                    List<Point> data = new List<Point>();
                    bool success = TryOpen(file, out data);
                    if (success)
                    {
                        Samples.Add(new Sample(data));
                        filesLoaded++;
                        Console.WriteLine(FilesLoaded);
                    }
                    else faillist.Add(Path.GetFileName(file));
                }
                Console.WriteLine(fileCount);
                if (FilesLoaded == fileCount)
                {
                    //MessageBox.Show("Mouse movement samples loaded.");
                }
                else
                {
                    MessageBox.Show("Some files failed to load!");
                }
                LoadFinished(null, new EventArgs());
            });
            loadThread.Start();
        }

        static bool TryOpen(string filename, out List<Point> list)
        {
            try
            {
                list = new List<Point>();
                using (Stream stream = File.Open(filename, FileMode.Open))
                {
                    while (stream.Position < stream.Length)
                    {
                        byte[] xbytes = new byte[sizeof(int)];
                        byte[] ybytes = new byte[sizeof(int)];
                        stream.Read(xbytes, 0, sizeof(int));
                        stream.Read(ybytes, 0, sizeof(int));
                        int x = BitConverter.ToInt32(xbytes, 0);
                        int y = BitConverter.ToInt32(ybytes, 0);
                        list.Add(new Point(x, y));
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            list = null;
            return false;
        }
    }

    class Sample
    {
        public List<Point> Frames;
        public SampleInfo Info;

        public Sample(List<Point> data)
        {
            Frames = data;
            Info = new SampleInfo(data);
        }

        public class SampleInfo
        {
            public enum Directions
            {
                RightDown,
                RightUp,
                LeftDown,
                LeftUp
            }
            int length;
            int distance;
            Directions direction;

            public int Length { get { return length; } }
            public int Distance { get { return distance; } }
            public Directions Direction { get { return direction; } }

            public SampleInfo(List<Point> data)
            {
                length = data.Count;
                distance = GetDistance(data);
                direction = GetDirection(data);
            }

            int GetDistance(List<Point> data)
            {
                int xDist = data.Last().X - data[0].X;
                int yDist = data.Last().Y - data[0].Y;
                return ((int)Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2)));
            }

            Directions GetDirection(List<Point> data)
            {
                if (data.Last().X >= data[0].X && data.Last().Y >= data[0].Y) return Directions.RightDown;
                else if (data.Last().X >= data[0].X && data.Last().Y < data[0].Y) return Directions.RightUp;
                else if (data.Last().X < data[0].X && data.Last().Y >= data[0].Y) return Directions.LeftDown;
                else return Directions.LeftUp;
            }
        }
    }
}
