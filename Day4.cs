using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day4
    {
        public static void Day4Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day4.txt");
            int score = 0;

            foreach (var line in fileLines)
            {
                var elfAssignments = line.Split(',');
                var elf1 = elfAssignments[0].Split('-');
                var elf2 = elfAssignments[1].Split('-');

                var a = int.Parse(elf1[0]);
                var b = int.Parse(elf1[1]);
                var c = int.Parse(elf2[0]);
                var d = int.Parse(elf2[1]);

                if (
                    ((a <= c && b >= c) && (a <= d && b >= d)) ||
                    ((c <= a && d >= a) && (c <= b && d >= b))
                   )
                {
                    score++;
                }
            }

            Console.WriteLine("Part 1 - " + score);           
        }

        public static void Day4Part2()
        {
            var fileLines = System.IO.File.ReadAllLines("Day4.txt");
            int score = 0;

            foreach (var line in fileLines)
            {
                var elfAssignments = line.Split(',');
                var elf1 = elfAssignments[0].Split('-');
                var elf2 = elfAssignments[1].Split('-');

                var a = int.Parse(elf1[0]);
                var b = int.Parse(elf1[1]);
                var c = int.Parse(elf2[0]);
                var d = int.Parse(elf2[1]);

                if (
                    (a <= c && b >= c) ||
                    (a <= d && b >= d) ||
                    (c <= a && d >= a) ||
                    (c <= b && d >= b))
                {
                    score++;
                }
            }

            Console.WriteLine("Part 2 - " + score);
            Console.ReadLine();
        }
    }
}
