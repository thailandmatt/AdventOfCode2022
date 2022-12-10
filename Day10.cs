using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day10
    {      
        public static void Day10Part1Attemp1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day10.txt");

            int cycleNumber = 0;            
            int curInstruction = 0;
            bool onAddX = false;
            List<int> registerValues = new List<int>();
            int X = 1;

            while (curInstruction < fileLines.Length)
            {
                cycleNumber++;

                if (fileLines[curInstruction].StartsWith("addx"))
                {
                    if (onAddX)
                    {
                        //execute this one and reset
                        X += int.Parse(fileLines[curInstruction].Replace("addx ", ""));
                        onAddX = false;
                        curInstruction++;
                    }
                    else
                    {
                        onAddX = true;
                    }
                }
                else if (fileLines[curInstruction] == "noop")
                {
                    curInstruction++;
                }

                registerValues.Add(X);
            }

            int score = 0;
            for (int i = 20; i <= 220; i = i + 40)
            {
                int q = (i * registerValues[i - 1]);
                score += q;
            }

            Console.WriteLine("Part 1 - " + score);
            Console.ReadLine();
        }

        public static void Day10Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day10.txt");

            //simplify input
            List<string> newLines = new List<string>();
            foreach (var line in fileLines)
            {
                if (line.StartsWith("addx"))                
                    newLines.Add("addx 0");
                    

                newLines.Add(line);
            }

            List<int> registerValues = new List<int>();
            int X = 1;
            foreach (var line in newLines)
            {
                registerValues.Add(X);

                if (line.StartsWith("addx"))
                    X += int.Parse(line.Replace("addx ", ""));
            }

            int score = 0;
            for (int i = 20; i <= 220; i = i + 40)
            {
                int q = (i * registerValues[i - 1]);
                score += q;
            }

            Console.WriteLine("Part 1 - " + score);

            Console.WriteLine("Part 2");

            int cycle = 0;
            for (int row = 0; row < 6; row++)
            {
                string thisRow = "";
                for (int col = 0; col < 40; col++)
                {
                    int spriteMiddle = registerValues[cycle];
                    if (col == spriteMiddle - 1 || col == spriteMiddle || col == spriteMiddle + 1)
                        thisRow += "#";
                    else
                        thisRow += ".";

                    cycle++;
                }
                Console.WriteLine(thisRow);
            }

            Console.ReadLine();
        }
    }
}
