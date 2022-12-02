using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day1
    {
        public static void Day1Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day1.txt");

            int cur = 0;
            List<int> totals = new List<int>();

            foreach (var line in fileLines)
            {
                if (line == "")
                {
                    totals.Add(cur);
                    cur = 0;
                }
                else
                {
                    cur += int.Parse(line);
                }
            }

            totals.Add(cur);

            totals.Sort();
            totals.Reverse();

            Console.WriteLine(totals[0]);
            Console.WriteLine(totals[1]);
            Console.WriteLine(totals[2]);
            Console.WriteLine(totals[0] + totals[1] + totals[2]);
            Console.ReadLine();
        }
    }
}
