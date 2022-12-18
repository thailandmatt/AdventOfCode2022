using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day18
    {   public static void Part1Part2()
        {
            var fileLines = System.IO.File.ReadAllLines("Day18.txt");
                        
            List<(int, int, int)> cubes = fileLines.Select(one => {
                var p = one.Split(',');
                return (int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]));
                }).ToList();

            var xMax = cubes.Max(one => one.Item1);
            var yMax = cubes.Max(one => one.Item2);
            var zMax = cubes.Max(one => one.Item3);

            int[,,] grid = new int[xMax + 1, yMax + 1, zMax + 1];

            foreach (var cube in cubes)            
                grid[cube.Item1, cube.Item2, cube.Item3] = 1;

            //just count how many non-adjacent directions there are from the 6 directions in 3D space for each cube
            int part1Faces = 0;
            foreach (var cube in cubes)
            {
                int x = cube.Item1;
                int y = cube.Item2;
                int z = cube.Item3;

                if (x + 1 > xMax || grid[x + 1, y, z] == 0) part1Faces++;
                if (x == 0 || grid[x - 1, y, z] == 0) part1Faces++;
                if (y + 1 > yMax || grid[x, y + 1, z] == 0) part1Faces++;
                if (y == 0 || grid[x, y - 1, z] == 0) part1Faces++;
                if (z + 1 > zMax || grid[x, y, z + 1] == 0) part1Faces++;
                if (z == 0 || grid[x, y, z - 1] == 0) part1Faces++;
            }

            Console.WriteLine("Part 1 - " + part1Faces);

            //Part 2 - gonna flood fill from 0,0,0
            //converting to a "2" to mean empty
            //then do part 1 again only touching 2s this time
            HashSet<(int, int, int)> q = new HashSet<(int, int, int)>();
            q.Add((0, 0, 0));

            while (q.Count > 0)
            {
                (int, int, int) cube = q.First();
                q.Remove(cube);
                int x = cube.Item1;
                int y = cube.Item2;
                int z = cube.Item3;
                grid[x, y, z] = 2;
                if (x + 1 <= xMax && grid[x + 1, y, z] == 0 && !q.Contains((x + 1, y, z))) q.Add((x + 1, y, z));
                if (x > 0 && grid[x - 1, y, z] == 0 && !q.Contains((x - 1, y, z))) q.Add((x - 1, y, z));
                if (y + 1 <= yMax && grid[x, y + 1, z] == 0 && !q.Contains((x, y + 1, z))) q.Add((x, y + 1, z));
                if (y > 0 && grid[x, y - 1, z] == 0 && !q.Contains((x, y - 1, z))) q.Add((x, y - 1, z));
                if (z + 1 <= zMax && grid[x, y, z + 1] == 0 && !q.Contains((x, y, z + 1))) q.Add((x, y, z + 1));
                if (z > 0 && grid[x, y, z - 1] == 0 && !q.Contains((x, y, z - 1))) q.Add((x, y, z - 1));
            }

            int part2Faces = 0;
            foreach (var cube in cubes)
            {
                int x = cube.Item1;
                int y = cube.Item2;
                int z = cube.Item3;

                if (x + 1 > xMax || grid[x + 1, y, z] == 2) part2Faces++;
                if (x == 0 || grid[x - 1, y, z] == 2) part2Faces++;
                if (y + 1 > yMax || grid[x, y + 1, z] == 2) part2Faces++;
                if (y == 0 || grid[x, y - 1, z] == 2) part2Faces++;
                if (z + 1 > zMax || grid[x, y, z + 1] == 2) part2Faces++;
                if (z == 0 || grid[x, y, z - 1] == 2) part2Faces++;
            }
            Console.WriteLine("Part 2 - " + part2Faces);

            Console.ReadLine();
        }
    }
}