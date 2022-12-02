using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Program
    {
        static void Main(string[] args)
        {            
            Day2Part2();
        }

        #region Day1

        static void Day1Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day1.txt");

            int max = 0;
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

        #endregion

        #region Day2

        static void Day2Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day2.txt");
            int score = 0;
            string theirs = "ABC";
            string mine = "XYZ";
            
            foreach (var line in fileLines)
            {
                var parts = line.Split(' ');
                int theirChoice = theirs.IndexOf(parts[0][0]);
                int myChoice = mine.IndexOf(parts[1][0]);

                score += myChoice + 1;

                if (theirChoice == myChoice)
                    score += 3;
                if (theirChoice == myChoice - 1 || theirChoice == 2 && myChoice == 0)
                    score += 6;
            }

            Console.WriteLine(score);
            Console.ReadLine();
        }

        static void Day2Part2()
        {
            var fileLines = System.IO.File.ReadAllLines("Day2.txt");
            int score = 0;
            string theirs = "ABC";            

            //X = lose
            //Y = draw
            //Z = win

            foreach (var line in fileLines)
            {
                var parts = line.Split(' ');
                int theirChoice = theirs.IndexOf(parts[0][0]);
                int myChoice = -1;

                if (parts[1][0] == 'X')
                {
                    //lose
                    myChoice = theirChoice - 1;
                    if (myChoice == -1) myChoice = 2;
                }
                else if (parts[1][0] == 'Y')
                {
                    //draw
                    myChoice = theirChoice;
                }
                else if (parts[1][0] == 'Z')
                {
                    //win
                    myChoice = theirChoice + 1;
                    if (myChoice == 3) myChoice = 0;
                }

                score += myChoice + 1;

                if (theirChoice == myChoice)
                    score += 3;
                if (theirChoice == myChoice - 1 || theirChoice == 2 && myChoice == 0)
                    score += 6;
            }

            Console.WriteLine(score);
            Console.ReadLine();
        }

        #endregion
    }
}
