using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day2
    {
        public static void Day2Part1()
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

        public static void Day2Part2()
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
    }
}
