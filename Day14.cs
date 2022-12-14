using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day14
    {   
        public static void Day14Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day14.txt");

            //parse
            List<List<(int, int)>> rockPaths =
                fileLines
                .Select(x => x.Split(new string[] { " -> " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(y => (int.Parse(y.Split(',')[0]), int.Parse(y.Split(',')[1]))).ToList()).ToList();

            //how big grid?
            var xMax = rockPaths.Max(one => one.Max(two => two.Item1));
            var yMax = rockPaths.Max(one => one.Max(two => two.Item2));
            var xMin = rockPaths.Min(one => one.Min(two => two.Item1));
            var yMin = rockPaths.Min(one => one.Min(two => two.Item2));

            //adjusting for visibility
            for (int i = 0; i < rockPaths.Count; i++)
                {
                    for (int j = 0; j < rockPaths[i].Count; j++)
                    {
                        rockPaths[i][j] = (rockPaths[i][j].Item1 - xMin, rockPaths[i][j].Item2);
                    }
                }

            xMax = rockPaths.Max(one => one.Max(two => two.Item1));            
            var start = (500 - xMin, 0);

            //make grid
            List<string> grid = new List<string>();

            for (int i = 0; i < yMax + 1; i++)            
                grid.Add(new string('.', xMax + 1));

            grid[0] = ReplaceCharAt(grid[0], start.Item1, '+');            

            foreach (var rockPath in rockPaths)
            {
                for (int i = 0; i < rockPath.Count - 1; i++)
                {
                    var cur = rockPath[i];
                    var next = rockPath[i + 1];

                    if (cur.Item1 == next.Item1)
                    {
                        if (cur.Item2 < next.Item2)
                        {
                            for (int y = cur.Item2; y <= next.Item2; y++)
                                grid[y] = ReplaceCharAt(grid[y], cur.Item1, '#');
                        }
                        else
                        {
                            for (int y = cur.Item2; y >= next.Item2; y--)
                                grid[y] = ReplaceCharAt(grid[y], cur.Item1, '#');
                        }                                     
                    }
                    else
                    {
                        if (cur.Item1 < next.Item1)
                        {
                            for (int x = cur.Item1; x <= next.Item1; x++)
                                grid[cur.Item2] = ReplaceCharAt(grid[cur.Item2], x, '#');
                        }
                        else
                        {
                            for (int x = cur.Item1; x >= next.Item1; x--)
                                grid[cur.Item2] = ReplaceCharAt(grid[cur.Item2], x, '#');
                        }
                    }
                }
            }

            //part 1
            int sandCount = 0;            
            while (true)
            {
                (int, int) sandPos = (start.Item1, 0);               

                bool done = false;

                while (true)
                {
                    if (sandPos.Item2 + 1 == grid.Count)
                    {
                        done = true;
                        break;
                    }

                    if (grid[sandPos.Item2 + 1][sandPos.Item1] == '.')
                    {
                        sandPos = (sandPos.Item1, sandPos.Item2 + 1);
                    }
                    else if (sandPos.Item1 == 0)
                    {
                        done = true;
                        break;
                    }
                    else if (grid[sandPos.Item2 + 1][sandPos.Item1 - 1] == '.')
                    {
                        sandPos = (sandPos.Item1 - 1, sandPos.Item2 + 1);
                    }
                    else if (sandPos.Item1 == grid[0].Length)
                    {
                        done = true;
                        break;
                    }
                    else if (grid[sandPos.Item2 + 1][sandPos.Item1 + 1] == '.')
                    {
                        sandPos = (sandPos.Item1 + 1, sandPos.Item2 + 1);
                    }
                    else
                    {
                        grid[sandPos.Item2] = ReplaceCharAt(grid[sandPos.Item2], sandPos.Item1, 'o');
                        break;
                    }
                }       

                if (done) break;

                sandCount++;
            }

            for (int i = 0; i < grid.Count; i++)            
                Console.WriteLine(grid[i]);            

            Console.WriteLine("Part 1 - " + sandCount);            
        }

        public static void Day14Part2()
        {
            var fileLines = System.IO.File.ReadAllLines("Day14.txt");

            //parse
            List<List<(int, int)>> rockPaths =
                fileLines
                .Select(x => x.Split(new string[] { " -> " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(y => (int.Parse(y.Split(',')[0]), int.Parse(y.Split(',')[1]))).ToList()).ToList();

            var yMax = rockPaths.Max(one => one.Max(two => two.Item2));

            //just keep track of what is rock or sand
            HashSet<(int, int)> rocksOrSand = new HashSet<(int, int)>();
            
            foreach (var rockPath in rockPaths)
            {
                for (int i = 0; i < rockPath.Count - 1; i++)
                {
                    var cur = rockPath[i];
                    var next = rockPath[i + 1];

                    if (cur.Item1 == next.Item1)
                    {
                        if (cur.Item2 < next.Item2)
                        {
                            for (int y = cur.Item2; y <= next.Item2; y++)
                                rocksOrSand.Add((cur.Item1, y));                                
                        }
                        else
                        {
                            for (int y = cur.Item2; y >= next.Item2; y--)
                                rocksOrSand.Add((cur.Item1, y));
                        }
                    }
                    else
                    {
                        if (cur.Item1 < next.Item1)
                        {
                            for (int x = cur.Item1; x <= next.Item1; x++)
                                rocksOrSand.Add((x, cur.Item2));
                        }
                        else
                        {
                            for (int x = cur.Item1; x >= next.Item1; x--)
                                rocksOrSand.Add((x, cur.Item2));
                        }
                    }
                }
            }

            //part 2
            int sandCount = 0;
            while (true)
            {
                (int, int) sandPos = (500, 0);

                while (true)
                {
                    if (sandPos.Item2 == yMax + 1)
                    {
                        //one above floor - rest
                        rocksOrSand.Add(sandPos);
                        break;
                    }
                    else if (!rocksOrSand.Contains((sandPos.Item1, sandPos.Item2 + 1)))
                    {
                        //move down
                        sandPos = (sandPos.Item1, sandPos.Item2 + 1);
                    }
                    else if (!rocksOrSand.Contains((sandPos.Item1 - 1, sandPos.Item2 + 1)))
                    {
                        //move down - left
                        sandPos = (sandPos.Item1 - 1, sandPos.Item2 + 1);
                    }
                    else if (!rocksOrSand.Contains((sandPos.Item1 + 1, sandPos.Item2 + 1)))
                    {
                        //move down - right
                        sandPos = (sandPos.Item1 + 1, sandPos.Item2 + 1);
                    }
                    else
                    {
                        //rest
                        rocksOrSand.Add(sandPos);
                        break;
                    }
                }
                
                sandCount++;

                if (rocksOrSand.Contains((500, 0))) break;
            }
                        
            Console.WriteLine("Part 2 - " + sandCount);
            Console.ReadLine();
        }
        static string ReplaceCharAt(string s, int pos, char c)
        {            
            StringBuilder sb = new StringBuilder(s);
            sb[pos] = c;
            return sb.ToString();
        }
    }
}

