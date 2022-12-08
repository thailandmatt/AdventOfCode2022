using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day8
    {      

        public static void Day8Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day8.txt");

            bool[,] visible = new bool[fileLines.Length, fileLines[0].Length];
            int[,] arr = new int[fileLines.Length, fileLines[0].Length];
            


            for (int lineIndex = 0; lineIndex < fileLines.Length; lineIndex++)
            {
                var line = fileLines[lineIndex];
                for (int i = 0; i < line.Length; i++)
                {
                    arr[lineIndex, i] = int.Parse(line[i].ToString());
                }
            }

            int totalCount = 0;

            //left
            for (int lineIndex = 0; lineIndex < fileLines.Length; lineIndex++)
            {
                int max = -1;
                for (int i = 0; i < fileLines[0].Length; i++)
                {
                    if (arr[lineIndex, i] > max)
                    {
                        max = arr[lineIndex, i];
                        if (!visible[lineIndex, i]) totalCount++;
                        visible[lineIndex, i] = true;                        
                    }
                }
            }

            //right
            for (int lineIndex = 0; lineIndex < fileLines.Length; lineIndex++)
            {
                int max = -1;
                for (int i = fileLines[0].Length - 1; i >= 0; i--)
                {
                    if (arr[lineIndex, i] > max)
                    {
                        max = arr[lineIndex, i];
                        if (!visible[lineIndex, i]) totalCount++;
                        visible[lineIndex, i] = true;
                    }
                }
            }

            //up
            for (int colIndex = 0; colIndex < fileLines[0].Length; colIndex++)
            {
                int max = -1;
                for (int i = 0; i < fileLines.Length; i++)
                {
                    if (arr[i, colIndex] > max)
                    {
                        max = arr[i, colIndex];
                        if (!visible[i, colIndex]) totalCount++;
                        visible[i, colIndex] = true;
                    }
                }
            }

            //down
            for (int colIndex = 0; colIndex < fileLines[0].Length; colIndex++)
            {
                int max = -1;
                for (int i =  fileLines.Length - 1; i >= 0; i--)
                {
                    if (arr[i, colIndex] > max)
                    {
                        max = arr[i, colIndex];
                        if (!visible[i, colIndex]) totalCount++;
                        visible[i, colIndex] = true;
                    }
                }
            }

            Console.WriteLine("Part 1 - " + totalCount);

            int maxScenic = 0;
            for (int i = 0; i < fileLines.Length; i++)
            {                
                for (int j = 0; j < fileLines[0].Length; j++)
                {
                    //check all around for this one
                    int up = 0;                    
                    for (int x = i - 1; x >= 0; x--)
                    {                        
                        up++;
                        if (arr[x, j] >= arr[i, j]) break;                        
                    }
                                        
                    int down = 0;
                    for (int x = i + 1; x < fileLines.Length; x++)
                    {
                        down++;
                        if (arr[x, j] >= arr[i, j]) break;
                    }
                   
                    int left = 0;
                    for (int x = j - 1; x >= 0; x--)
                    {
                        left++;
                        if (arr[i, x] >= arr[i, j]) break;
                    }
                   
                    int right = 0;
                    for (int x = j + 1; x < fileLines[0].Length; x++)
                    {
                        right++;
                        if (arr[i, x] >= arr[i, j]) break;
                    }

                    int scenic = up * down * left * right;
                    if (scenic > maxScenic)
                    {
                        maxScenic = scenic;
                    }
                }
            }

            Console.WriteLine("Part 2 - " + maxScenic);
            Console.ReadLine();
        }
    }
}
