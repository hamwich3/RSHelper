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
     * LENGTH is total length of movement in points (each point is ~1ms)
     * DIRECTION is left/right (x) then up/down (y): RD, RU, LD, LU
     * eg:
     * "123_456_UL"
     * "1234_2345_DR"
     * 
     */

    static class MouseSample
    {
        public static bool Recording { get { return recording; } }
        public static bool FilesLoaded { get { return filesLoaded; } }
        public static int TotalFilesLoaded { get { return totalFilesLoaded; } }
        public static int FileCount { get { return fileCount; } }
        public static event EventHandler LoadFinished = delegate { };
        public static event EventHandler MoveFinished = delegate { };

        static List<Sample> Samples = new List<Sample>();

        static bool nextSample = false;
        static bool recording = false;
        static bool startSampling = false;
        static bool filesLoaded = false;
        static int totalFilesLoaded = 0;
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
            List<Point> samplePoints = new List<Point>();
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
                    samplePoints.Clear();
                    while (millis <= 30000 && !nextSample && recording)
                    {
                        millis = sw.ElapsedMilliseconds;
                        if (millis != lastMillis)
                        {
                            lastMillis = millis;
                            samplePoints.Add(Cursor.Position);
                        }
                        Thread.Sleep(0);
                    }
                    if (millis < 30000 && samplePoints.Count > 0 && recording)
                    {
                        Save(TrimStart(Normalize(samplePoints)));
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
        /// Moves sample's points starting point to {0,0}
        /// </summary>
        /// <param name="samplePoints"></param>
        /// <returns></returns>
        public static List<Point> Normalize(List<Point> samplePoints)
        {
            List<Point> normalizedSample = new List<Point>();
            int xAdjust = samplePoints[0].X;
            int yAdjust = samplePoints[0].Y;
            for (int i = 0; i < samplePoints.Count; i++)
            {
                normalizedSample.Add(new Point(samplePoints[i].X - xAdjust, samplePoints[i].Y - yAdjust));
            }
            return normalizedSample;
        }

        /// <summary>
        /// Trims all but one leading {0,0} points
        /// </summary>
        /// <param name="samplePoints"></param>
        /// <returns></returns>
        public static List<Point> TrimStart(List<Point> samplePoints)
        {
            List<Point> trimmedSample = new List<Point>();
            int startIndex = 0;
            for (int i = 0; i < samplePoints.Count; i++)
            {
                if (!samplePoints[0].Equals(samplePoints[i]))
                {
                    startIndex = i - 1;
                    break;
                }
            }
            for (int i = startIndex; i < samplePoints.Count; i++)
            {
                trimmedSample.Add(samplePoints[i]);
            }
            return trimmedSample;
        }

        /// <summary>
        /// Shifts sample's points to end at desired location
        /// </summary>
        /// <param name="samplePoints"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <returns></returns>
        public static List<Point> Transform(List<Point> samplePoints, Point start, Point end)
        {
            List<Point> tSample = new List<Point>();
            //Normalize(sample);
            //Normalize x and y endpoint
            double normX = end.X - start.X;
            double normY = end.Y - start.Y;
            double xFactor = normX / samplePoints.Last().X; // lol X Factor
            double yFactor = normY / samplePoints.Last().Y;
            //Scales x,y factors from 0-1 over movement
            double tFactor = 1.0000 / samplePoints.Count;

            for (int i = 0; i < samplePoints.Count; i++)
            {
                double x = Math.Round(samplePoints[i].X * (xFactor * (tFactor * i)));
                double y = Math.Round(samplePoints[i].Y * (yFactor * (tFactor * i)));
                tSample.Add(new Point((int)x, (int)y));
            }
            tSample = Transpose(tSample, start);
            tSample[tSample.Count - 1] = end;
            return tSample;
        }

        /// <summary>
        /// Moves sample's points to start at desired location
        /// </summary>
        /// <param name="samplePoints"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <returns></returns>
        public static List<Point> Transpose(List<Point> samplePoints, Point start)
        {
            List<Point> tSample = new List<Point>();

            for (int i = 0; i < samplePoints.Count; i++)
            {
                tSample.Add(new Point(samplePoints[i].X + start.X, samplePoints[i].Y + start.Y));
            }
            return tSample;
        }

        /// <summary>
        /// Move mouse from current position to new position using closest sample match
        /// </summary>
        public static void MoveTo(Point endP)
        {
            if (!filesLoaded)
            {
                throw new Exception("Mouse sample files not done loading");
            }
            Point startP = Cursor.Position;

            Sample.SampleInfo.Directions dir;
            if (endP.X >= startP.X && endP.Y >= startP.Y) dir = Sample.SampleInfo.Directions.RightDown;
            else if (endP.X < startP.X && endP.Y >= startP.Y) dir = Sample.SampleInfo.Directions.LeftDown;
            else if (endP.X >= startP.X && endP.Y < startP.Y) dir = Sample.SampleInfo.Directions.RightUp;
            else dir = Sample.SampleInfo.Directions.LeftUp;

            float dist = ((int)Math.Sqrt(Math.Pow(endP.X - startP.X, 2) + Math.Pow(endP.Y - startP.Y, 2)));

            //Same sample direction, same or greater distance
            var samples = from s in Samples
                          where (s.Info.Direction == dir && s.Info.Distance > dist * 1.1 && s.Info.Distance < dist*1.3 && s.Info.Length < dist*3.5)
                          select s;
            //Get any direction if no match found
            if (samples.Count() == 0)
            {
                samples = from s in Samples
                          where (s.Info.Distance > dist * 1.1 && s.Info.Distance < dist * 1.3 && s.Info.Length < dist * 3.5) select s;
            }
            if (samples.Count() == 0) throw new Exception("Not enough samples! couldn't find anything for length: " + dist);

            // shortest length first
            samples = samples.OrderBy((s) => { return s.Info.Length; });
            Random r = new Random();
            int n = r.Next(0, Math.Max((int)(samples.Count()*.7), 1));
            Sample sample = samples.ElementAt(n);
            var movement = Transform(sample.Points, startP, endP);
            //Remove sample if it "jumps" too much and try again
            if (!SampleOK(movement))
            {
                Samples.Remove(sample);
                Console.WriteLine("Removed sample from list: " + sample.Info.Distance + "_" + sample.Info.Length + "_" + sample.Info.Direction);
                MoveTo(endP);
                return;
            }
            Move(movement);
        }

        /// <summary>
        /// Returns false if sample "jumps" too much
        /// </summary>
        /// <param name="samplePoints"></param>
        /// <returns></returns>
        static bool SampleOK(List<Point> samplePoints)
        {
            for (int i = 1; i < samplePoints.Count; i++)
            {
                int deltaX = Math.Abs(samplePoints[i].X - samplePoints[i - 1].X);
                int deltaY = Math.Abs(samplePoints[i].Y - samplePoints[i - 1].Y);
                int dist = (int)Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
                if (dist > 50)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Starts movement thread; fires MoveFinished event when finished.
        /// Checks for and prints to console length of movements that skip too many pixels
        /// </summary>
        /// <param name="samplePoints"></param>
        public static void Move(List<Point> samplePoints)
        {
            Thread moveThread = new Thread(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                long millis = 0;
                long lastMillis = -1;
                while (millis < samplePoints.Count)
                {
                    if (millis != lastMillis)
                    {
                        lastMillis = millis;
                        Cursor.Position = samplePoints[(int)millis];
                    }
                    millis = sw.ElapsedMilliseconds;
                }
                MoveFinished(null, new EventArgs());
            });
            moveThread.Start();
        }


        static void Save(List<Point> samplePoints)
        {
            string filename = "";
            string filepath = "";
            string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RS\samples";

            if (!Directory.Exists(initialDirectory))
            {
                Directory.CreateDirectory(initialDirectory);
            }

            int xDist = samplePoints.Last().X - samplePoints[0].X;
            int yDist = samplePoints.Last().Y - samplePoints[0].Y;
            string DIST = ((int)Math.Sqrt(Math.Pow(xDist, 2) + Math.Pow(yDist, 2))).ToString();
            string LENGTH = samplePoints.Count.ToString();
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
                    foreach (var p in samplePoints)
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
            filesLoaded = false;
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
                        totalFilesLoaded++;
                        Console.WriteLine(totalFilesLoaded);
                    }
                    else faillist.Add(Path.GetFileName(file));
                }
                Console.WriteLine(fileCount);
                if (totalFilesLoaded == fileCount)
                {
                    //MessageBox.Show("Mouse movement samples loaded.");
                    filesLoaded = true;
                }
                else
                {
                    MessageBox.Show("Some files failed to load!");
                }
                LoadFinished(null, new EventArgs());
            });
            loadThread.Start();
        }

        static bool TryOpen(string filename, out List<Point> samplePoints)
        {
            try
            {
                samplePoints = new List<Point>();
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
                        samplePoints.Add(new Point(x, y));
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            samplePoints = null;
            return false;
        }
    }

    class Sample
    {
        public List<Point> Points;
        public SampleInfo Info;

        public Sample(List<Point> data)
        {
            Points = data;
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
