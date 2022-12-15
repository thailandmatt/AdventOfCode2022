using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day15
    {   
        public static void Day15Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day15.txt");

            List<(int, int)> Sensors = new List<(int, int)>();
            List<(int, int)> Beacons = new List<(int, int)>();

            fileLines = fileLines.Select(line => line.Replace("Sensor at x=", "").Replace("y=", "").Replace(": closest beacon is at x=", ",")).ToArray();
            List<int[]> pointSetList = fileLines.Select(line => 
                line.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(num => int.Parse(num)
                ).ToArray()).ToList();

            foreach (var pointSet in pointSetList)
            {
                Sensors.Add((pointSet[0], pointSet[1]));
                Beacons.Add((pointSet[2], pointSet[3]));
            }
                        
            //int xMax = Sensors.Union(Beacons).Max(one => one.Item1);
            //int yMax = Sensors.Union(Beacons).Max(one => one.Item2);

            //char[][] grid = new char[yMax+1][];
            //for (int i = 0; i < grid.Length; i++)
            //{
            //    grid[i] = new char[xMax + 1];
            //    for (int j = 0; j < grid[i].Length; j++)
            //    {
            //        grid[i][j] = '.';
            //    }
            //}
                        
            //string numString = "abcdefghijklmnopqrstuvwxyz";
            //string numString2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //for (int i = 0; i < Sensors.Count; i++)
            //{
            //    if (Sensors[i].Item1 >= 0 && Sensors[i].Item2 >= 0)
            //        grid[Sensors[i].Item2][Sensors[i].Item1] = numString2[i];

            //    if (Beacons[i].Item1 >= 0 && Beacons[i].Item2 >= 0)
            //        grid[Beacons[i].Item2][Beacons[i].Item1] = numString[i];
            //}

            //for (int i = 0; i < grid.Length; i++)
            //{
            //    Console.WriteLine(new string(grid[i]));
            //}

            //we want to look at y = 2000000
            int targetY = 2000000;

            //the number of items on a row that it will fill up
            //is the manhattan distance from sensor to beacon
            //less the number of rows away it is from the sensor
            //so if a sensor is on 2000000, then it will be 2 * manhattan + 1 (for the sensor)
            //if a sensor is on 1999999, then it will be 2 * manhattan + 1 (for the sensor) - 2 etc
            //but we have to count overlapping ranges so keep track of ranges
            List<(int, int)> ranges = new List<(int, int)>();
                        
            for (int i = 0; i < Sensors.Count; i++)
            {
                int manhattan = Math.Abs(Sensors[i].Item1 - Beacons[i].Item1) + Math.Abs(Sensors[i].Item2 - Beacons[i].Item2);                
                int distanceToTarget = Math.Abs(Sensors[i].Item2 - targetY);                
                int onRow = 2 * manhattan + 1 - (2 * distanceToTarget);
                if (onRow > 0)
                {                    
                    var distFromCenter = ((onRow - 1) / 2);
                    ranges.Add((Sensors[i].Item1 - distFromCenter, Sensors[i].Item1 + distFromCenter));
                }
            }

            ranges.Sort();

            //merge
            for (int i = 0; i < ranges.Count - 1; i++)
            {
                (int, int) cur = ranges[i];
                (int, int) next = ranges[i + 1];
                                
                if (cur.Item1 <= next.Item1 && cur.Item2 >= next.Item2)
                {
                    //full overlap
                    ranges.RemoveAt(i + 1);
                    i--;
                }
                else if (cur.Item1 <= next.Item1 && cur.Item2 < next.Item2 && cur.Item2 >= next.Item1 - 1)
                {
                    //partial overlap or adjacent
                    ranges[i] = (ranges[i].Item1, ranges[i + 1].Item2);
                    ranges.RemoveAt(i + 1);
                    i--;
                }               
            }


            Console.WriteLine("Part 1 - " + ranges.Sum(one => one.Item2 - one.Item1));            
        }

        public static void Day15Part2()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var fileLines = System.IO.File.ReadAllLines("Day15.txt");

            List<(int, int)> Sensors = new List<(int, int)>();
            List<(int, int)> Beacons = new List<(int, int)>();
            List<int> Manhattans = new List<int>();
            fileLines = fileLines.Select(line => line.Replace("Sensor at x=", "").Replace("y=", "").Replace(": closest beacon is at x=", ",")).ToArray();
            List<int[]> pointSetList = fileLines.Select(line =>
                line.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(num => int.Parse(num)
                ).ToArray()).ToList();

            foreach (var pointSet in pointSetList)
            {
                Sensors.Add((pointSet[0], pointSet[1]));
                Beacons.Add((pointSet[2], pointSet[3]));
                Manhattans.Add(Math.Abs(pointSet[0] - pointSet[2]) + Math.Abs(pointSet[1] - pointSet[3]));
            }

            //the number of items on a row that it will fill up
            //is the manhattan distance from sensor to beacon            
            //but we have to count overlapping ranges so keep track of ranges            
            Dictionary<int, List<(int, int)>> exclusionRangesOnEachRow = new Dictionary<int, List<(int, int)>>();

            for (int i = 0; i < Sensors.Count; i++)
            {
                int manhattan = Math.Abs(Sensors[i].Item1 - Beacons[i].Item1) + Math.Abs(Sensors[i].Item2 - Beacons[i].Item2);
                for (int x = Sensors[i].Item2 - manhattan; x <= Sensors[i].Item2 + manhattan; x++)
                {
                    int distFromCenter = manhattan - Math.Abs(Sensors[i].Item2 - x);
                    if (!exclusionRangesOnEachRow.ContainsKey(x)) exclusionRangesOnEachRow[x] = new List<(int, int)>();
                    exclusionRangesOnEachRow[x].Add((Sensors[i].Item1 - distFromCenter, Sensors[i].Item1 + distFromCenter));
                }
            }

            foreach (var entry in exclusionRangesOnEachRow)
            {
                var ranges = entry.Value;
                ranges.Sort();

                //merge
                for (int i = 0; i < ranges.Count - 1; i++)
                {
                    (int, int) cur = ranges[i];
                    (int, int) next = ranges[i + 1];

                    if (cur.Item1 <= next.Item1 && cur.Item2 >= next.Item2)
                    {
                        //full overlap
                        ranges.RemoveAt(i + 1);
                        i--;
                    }
                    else if (cur.Item1 <= next.Item1 && cur.Item2 < next.Item2 && cur.Item2 >= next.Item1 - 1)
                    {
                        //partial overlap or adjacent
                        ranges[i] = (ranges[i].Item1, ranges[i + 1].Item2);
                        ranges.RemoveAt(i + 1);
                        i--;
                    }
                }

                if (entry.Key >= 0 && entry.Key <= 4000000 && (ranges[0].Item1 >= 0 || ranges[0].Item2 <= 4000000))
                {
                    int yAnswer = entry.Key;
                    int xAnswer = ranges[0].Item2 + 1;
                    long tuning = ((long)xAnswer * (long)4000000) + (long)yAnswer;

                    stopwatch.Stop();

                    Console.WriteLine("Part 2 - " + entry.Key + " has ranges " + ranges[0] + " and " + ranges[1]);
                    Console.WriteLine("Answer = " + "(" + xAnswer + ", " + yAnswer + ") with tuning frequency " + tuning);
                    Console.WriteLine("Part 2 took " + stopwatch.ElapsedMilliseconds.ToString() + " ms to run ");
                }
            }

            Console.ReadLine();
        }
    }
}


