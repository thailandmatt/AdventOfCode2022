using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day6
    {
        public static void Day6Part1()
        {
            var fileLine = System.IO.File.ReadAllText("Day6.txt");

            int[] count = new int[26];
            int x = 14;

            for (int i = 0; i < x; i++)
            {
                count[fileLine[i] - 'a']++;
            }

            for (int i = x; i < fileLine.Length; i++)
            {
                if (count.Max() == 1)
                {
                    Console.WriteLine(i);
                    Console.ReadLine();
                }
                else
                {
                    count[fileLine[i - x] - 'a']--;
                    count[fileLine[i] - 'a']++;
                }
            }
        }
    }
}
