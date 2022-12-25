using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day25
    {
        public static void Part1Part2()
        {
            var fileLines = System.IO.File.ReadAllLines("Day25.txt");

            List<long> translated = new List<long>();
            foreach (var line in fileLines)
            {
                translated.Add(SnafuToDecimal(line.Trim()));
            }

            long l = SnafuToDecimal("1=00=001121=1-00-");
            string test = DecimalToSnafu(l);

            Console.WriteLine("Part 1 - " + DecimalToSnafu(translated.Sum()));
            Console.WriteLine("Part 2 - ");
            Console.ReadLine();
        }

        public static long SnafuToDecimal(string s)
        {
            long total = 0;
            int x = 0;
            string map = "=-012";
            for (int i = s.Length - 1; i >= 0; i--)
            {
                long snafuValue = map.IndexOf(s[i]) - 2;
                total += ((long)Math.Pow(5, x) * snafuValue);
                x++;
            }

            return total;
        }

        public static string DecimalToSnafu(long l)
        {
            var cur = l;
            var answer = "";

            while (true)
            {
                var modFive = cur % 5;
                if (cur == 0)
                {
                    break;
                }
                else if (modFive == 0)
                {
                    answer = "0" + answer;
                    cur /= 5;
                }
                else if (modFive == 1)
                {
                    answer = "1" + answer;
                    cur -= 1;
                    cur /= 5;
                }
                else if (modFive == 2)
                {
                    answer = "2" + answer;
                    cur -= 2;
                    cur /= 5;
                }
                else if (modFive == 3)
                {
                    answer = "=" + answer;
                    cur += 2;
                    cur /= 5;
                }
                else if (modFive == 4)
                {
                    answer = "-" + answer;
                    cur += 1;
                    cur /= 5;
                }                 
                else
                {
                    throw new Exception("Problem");
                }
            }

            return answer;
        }
    }
}
