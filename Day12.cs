using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day12
    {      
        public static void Day12Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day12.txt");

            int[][] grid = fileLines.Select(x => x.ToCharArray().Select(y => y - 'a').ToArray()).ToArray();

            //find the start
            var startNum = 'S' - 'a';
            var endNum = 'E' - 'a';
            (int, int) start = (-1, -1);
            (int, int) end  = (-1, -1);

            List<(int, int)> part2List = new List<(int, int)>();

            for (int x = 0; x < grid.Length; x++)
            {
                for (int y = 0; y < grid[x].Length; y++)
                {
                    if (grid[x][y] == startNum)
                    {
                        start = (x, y);
                        grid[x][y] = 0;
                    }

                    if (grid[x][y] == endNum)
                    {
                        end = (x, y);
                        grid[x][y] = 'z' - 'a';
                    }

                    if (grid[x][y] == 0) part2List.Add((x, y));
                }
            }

            //Breadth first search
            List<(int, int)> steps = BreadthFirstSearchShortestPath(start, end, grid);

            Console.WriteLine("Part 1 - " + (steps.Count - 1));

            int min = int.MaxValue;
            foreach (var possibleStart in part2List)
            {
                List<(int, int)> possibleSteps = BreadthFirstSearchShortestPath(possibleStart, end, grid);
                if (possibleSteps.Count > 0 && possibleSteps.Count < min)
                {
                    min = possibleSteps.Count;
                }
            }

            Console.WriteLine("Part 2 - " + (min - 1));
            Console.ReadLine();
        }

        static List<(int, int)> BreadthFirstSearchShortestPath((int, int) start, (int, int) end, int[][] grid)
        {
            Queue<(int, int)> toCheck = new Queue<(int, int)>();
            HashSet<(int, int)> visited = new HashSet<(int, int)>();
            Dictionary<(int, int), (int, int)> childParentMap = new Dictionary<(int, int), (int, int)>();

            toCheck.Enqueue(start);
            visited.Add(start);

            while (toCheck.Count > 0)
            {
                (int, int) cur = toCheck.Dequeue();

                if (cur == end)
                {
                    //end - return the map
                    List<(int, int)> answer = new List<(int, int)>();
                    answer.Add(cur);

                    while (childParentMap.ContainsKey(cur))
                    {
                        answer.Insert(0, childParentMap[cur]);
                        cur = childParentMap[cur];
                    }

                    return answer;
                }

                List<(int, int)> adjacencyList = new List<(int, int)>();
                int height = grid[cur.Item1][cur.Item2];
                                
                adjacencyList.Add((cur.Item1 - 1, cur.Item2));
                adjacencyList.Add((cur.Item1 + 1, cur.Item2));
                adjacencyList.Add((cur.Item1, cur.Item2 - 1));
                adjacencyList.Add((cur.Item1, cur.Item2 + 1));

                foreach (var adjacent in adjacencyList)
                {
                    if (adjacent.Item1 > -1 && adjacent.Item1 < grid.Length &&
                        adjacent.Item2 > -1 && adjacent.Item2 < grid[0].Length &&
                        grid[adjacent.Item1][adjacent.Item2] <= height + 1
                        )
                    {
                        if (!visited.Contains(adjacent))
                        {
                            toCheck.Enqueue(adjacent);
                            visited.Add(adjacent);
                            childParentMap.Add(adjacent, cur);
                        }
                    }
                }
            }

            return new List<(int, int)>();
        }

    }
}
