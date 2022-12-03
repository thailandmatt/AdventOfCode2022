using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day3
    {
        public static void Day3Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day3.txt");
            int score = 0;

            foreach (var line in fileLines)
            {
                string part1 = line.Substring(0, line.Length / 2);
                string part2 = line.Substring(line.Length / 2);
                char shared = part1.ToCharArray().Intersect(part2.ToCharArray()).First();
                if (char.IsUpper(shared))
                    score += (shared - 'A' + 27);
                else
                    score += (shared - 'a' + 1);
            }

            Console.WriteLine(score);
            Console.ReadLine();
        }

        public static void Day3Part2()
        {
            var fileLines = System.IO.File.ReadAllLines("Day3.txt");
            int score = 0;

            for (int i = 0; i < fileLines.Length; i = i + 3)
            {             
                char shared = fileLines[i].ToCharArray()
                    .Intersect(fileLines[i + 1].ToCharArray())
                    .Intersect(fileLines[i + 2].ToCharArray())
                    .First();

                if (char.IsUpper(shared))
                    score += (shared - 'A' + 27);
                else
                    score += (shared - 'a' + 1);
            }

            Console.WriteLine(score);
            Console.ReadLine();
        }
    }
}
