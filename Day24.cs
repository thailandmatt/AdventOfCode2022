using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day24
    {
        public enum Dir
        {
            East = 0,
            South = 1,
            West = 2,
            North = 3
        }

        static (int, int)[] directions = new (int, int)[]
        {
            (0, 1),
            (1, 0),
            (0, -1),
            (-1, 0)
        };

        static string dirStrings = ">v<^";

        public static void Part1Part2()
        {
            var fileLines = System.IO.File.ReadAllLines("Day24.txt");

            //Ok this is gonna be a BFS - a list of how many places we *could* be on each minute
            //until we get to a miunte where we can be on the exit
            //I'm going to use HashSets of points

            //parse
            Dictionary<(int, int), List<Dir>> blizzards = new Dictionary<(int, int), List<Dir>>();

            for (int i = 0; i < fileLines.Length; i++)
            {
                var line = fileLines[i];
                for (int j = 0; j < line.Length; j++)
                {
                    if (line[j] == '^')
                        blizzards[(i, j)] = new List<Dir>() { Dir.North };
                    else if (line[j] == 'v')
                        blizzards[(i, j)] = new List<Dir>() { Dir.South };
                    else if (line[j] == '>')
                        blizzards[(i, j)] = new List<Dir>() { Dir.East };
                    else if (line[j] == '<')
                        blizzards[(i, j)] = new List<Dir>() { Dir.West };
                }
            }

            int maxRow = fileLines.Length - 2;
            int maxCol = fileLines[0].Length - 2;

            var start = (0, 1);
            var oneBeforeStart = (1, 1);
            var oneBeforeEnd = (fileLines.Length - 2, fileLines[0].Length - 2);
            var end = (fileLines.Length - 1, fileLines[0].Length - 2);
            int curMinute = 0;

            (curMinute, blizzards) = Go(start, oneBeforeEnd, curMinute, blizzards, maxRow, maxCol);
            Console.WriteLine("Part 1 - " + curMinute);
            (curMinute, blizzards) = Go(end, oneBeforeStart, curMinute, blizzards, maxRow, maxCol);
            (curMinute, blizzards) = Go(start, oneBeforeEnd, curMinute, blizzards, maxRow, maxCol);
            Console.WriteLine("Part 2 - " + curMinute);

            Console.ReadLine();
        }

        static (int, Dictionary<(int, int), List<Dir>>) Go((int, int) start, (int, int) end, int curMinute, Dictionary<(int, int), List<Dir>> blizzards, int maxRow, int maxCol)
        {
            HashSet<(int, int)> possiblePlacesAtStartOfThisMinute = new HashSet<(int, int)>();
            possiblePlacesAtStartOfThisMinute.Add(start);

            while (!possiblePlacesAtStartOfThisMinute.Contains(end))
            {
                //move the blizzards
                blizzards = MoveBlizzards(blizzards, maxRow, maxCol);

                //see where we can go
                HashSet<(int, int)> possibleDestinations = new HashSet<(int, int)>();
                foreach (var curSpot in possiblePlacesAtStartOfThisMinute)
                {
                    if (curSpot == start)
                    {
                        if (!blizzards.ContainsKey(curSpot)) possibleDestinations.Add(curSpot);
                        if (curSpot.Item1 == 0)
                        {
                            if (!blizzards.ContainsKey((1, 1))) possibleDestinations.Add((1, 1));
                        }
                        else
                        {
                            if (!blizzards.ContainsKey((maxRow, maxCol))) possibleDestinations.Add((maxRow, maxCol));
                        }
                    }
                    else
                    {
                        //see if we can wait
                        if (!blizzards.ContainsKey(curSpot)) possibleDestinations.Add(curSpot);

                        //see if we can go any of the 4 directions
                        for (int i = 0; i < 4; i++)
                        {
                            var test = AddDir(curSpot, (Dir)i, maxRow, maxCol, false);
                            if (test.Item1 < 1 || test.Item1 > maxRow || test.Item2 < 1 || test.Item2 > maxCol) continue;
                            if (!blizzards.ContainsKey(test)) possibleDestinations.Add(test);
                        }
                    }
                }

                //increment and move on
                //Console.WriteLine("Minute " + curMinute + ", " + possibleDestinations.Count);
                curMinute++;
                possiblePlacesAtStartOfThisMinute = possibleDestinations;


                //draw state for debugging
                //Console.WriteLine("State after minute " + curMinute);
                //DrawState(blizzards, possibleDestinations, maxRow, maxCol);
            }

            //move to end - one more move to do
            blizzards = MoveBlizzards(blizzards, maxRow, maxCol);
            curMinute++;

            //draw state for debugging
            //Console.WriteLine("State after minute " + curMinute);
            //DrawState(blizzards, new HashSet<(int, int)>(), maxRow, maxCol);

            return (curMinute, blizzards);
        }

        static void DrawState(Dictionary<(int, int), List<Dir>> blizzards, HashSet<(int, int)> possibleDestinations, int maxRow, int maxCol)
        {
            char[][] chars = new char[maxRow + 2][];
            chars[0] = new char[] {};
            chars[chars.Length - 1] = new char[] {};
            for (int i = 1; i <= maxRow; i++)
            {
                chars[i] = new char[maxCol + 2];

                for (int j = 1; j <= maxCol; j++)
                {
                    if (blizzards.ContainsKey((i, j)))
                    {
                        if (blizzards[(i, j)].Count == 1)
                        {
                            chars[i][j] = dirStrings[(int)blizzards[(i, j)][0]];
                        }
                        else
                        {
                            chars[i][j] = blizzards[(i, j)].Count.ToString()[0];
                        }
                    }
                    else if (possibleDestinations.Contains((i, j)))
                        chars[i][j] = 'E';
                    else
                        chars[i][j] = '.';
                }
            }

            Console.WriteLine("");
            foreach (var line in chars)            
                Console.WriteLine(string.Join("", line));
            Console.WriteLine("");
        }

        static Dictionary<(int, int), List<Dir>> MoveBlizzards(Dictionary<(int, int), List<Dir>> blizzards, int maxRow, int maxCol)
        {            
            Dictionary<(int, int), List<Dir>> newDict = new Dictionary<(int, int), List<Dir>>();

            var spots = blizzards.Keys.ToList();
            foreach (var spot in spots)
            {
                var list = blizzards[spot];                

                foreach (var blizzardDir in list)
                {                    
                    var newBlizzardSpot = AddDir(spot, blizzardDir, maxRow, maxCol, true);

                    if (newDict.ContainsKey(newBlizzardSpot))
                        newDict[newBlizzardSpot].Add(blizzardDir);
                    else
                        newDict[newBlizzardSpot] = new List<Dir>() { blizzardDir };
                }
            }

            return newDict;
        }

        static (int, int) AddDir((int, int) start, Dir direction, int maxRow, int maxCol, bool canWrap)
        {
            var dir = directions[(int)direction];
            var next = (start.Item1 + dir.Item1, start.Item2 + dir.Item2);

            if (next.Item1 < 1 && canWrap) next.Item1 = maxRow;
            if (next.Item2 < 1 && canWrap) next.Item2 = maxCol;

            if (next.Item1 > maxRow && canWrap) next.Item1 = 1;
            if (next.Item2 > maxCol && canWrap) next.Item2 = 1;

            return next;
        }
    }
}
