using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day23
    {        
        public static void Part1Part2(bool part1)
        {
            var fileLines = System.IO.File.ReadAllLines("Day23.txt");
            
            //parse
            HashSet<(int, int)> elfs = new HashSet<(int, int)>();

            for (int i = 0; i < fileLines.Count(); i++)
            {
                var line = fileLines[i];

                for (int j = 0; j < line.Length; j++)
                    if (line[j] == '#') elfs.Add((i, j));                
            }

            //directions
            (int, int)[] directions = new (int, int)[]
            {
                (-1, 0), //North
                (1, 0),  //South
                (0, -1), //West
                (0, 1)   //East
            };

            int curDirection = 0;

            //Print(elfs, 0);

            //do 10 rounds
            for (int round = 0; round < (part1 ? 10 : 1000000); round++)
            {
                //first half - propose
                                
                Dictionary<(int, int), (int, int)> proposals = new Dictionary<(int, int), (int, int)>();
                Dictionary<(int, int), int> proposalCount = new Dictionary<(int, int), int>();
                HashSet<(int, int)> onesNotToDoBecauseTheCollided = new HashSet<(int, int)>();

                foreach (var elf in elfs)
                {
                    //see if we're alone
                    if (!(elfs.Contains((elf.Item1 + 1, elf.Item2 - 1)) ||
                        elfs.Contains((elf.Item1 + 1, elf.Item2)) ||
                        elfs.Contains((elf.Item1 + 1, elf.Item2 + 1)) ||
                        elfs.Contains((elf.Item1 - 1, elf.Item2 - 1)) ||
                        elfs.Contains((elf.Item1 - 1, elf.Item2)) ||
                        elfs.Contains((elf.Item1 - 1, elf.Item2 + 1)) ||
                        elfs.Contains((elf.Item1 - 1, elf.Item2 - 1)) ||
                        elfs.Contains((elf.Item1, elf.Item2 - 1)) ||
                        elfs.Contains((elf.Item1, elf.Item2 + 1))))              
                    {
                        proposals[elf] = elf;
                        continue;
                    }

                    //get the three to check                    
                    bool proposed = false;

                    for (int dir = 0; dir < 4; dir++)
                    {
                        var testDirection = directions[(curDirection + dir) % 4];

                        if (CanGoDirection(testDirection, elf, elfs))
                        {
                            var target = (elf.Item1 + testDirection.Item1, elf.Item2 + testDirection.Item2);
                            
                            proposals[elf] = target;
                            if (!proposalCount.ContainsKey(target)) proposalCount[target] = 0;
                            proposalCount[target]++;
                            proposed = true;
                            break;
                        }
                    }

                    if (!proposed)
                    {
                        proposals[elf] = elf;
                    }
                }

                //second half - move
                int executeCount = 0;               
                foreach (var elf in proposals.Keys)
                {
                    if (proposals[elf] != elf && proposalCount[proposals[elf]] == 1)
                    {
                        //execute
                        executeCount++;
                        elfs.Remove(elf);
                        elfs.Add(proposals[elf]);
                    }
                }

                if (executeCount == 0 && !part1)
                {
                    Console.WriteLine("Part 2 - " + (round + 1));
                    Console.ReadLine();
                    return;
                }

                //change preferred direction
                curDirection++;
                if (curDirection == 4) curDirection = 0;

                //Print(elfs, round + 1);
            }

            int minRow = elfs.Min(one => one.Item1);
            int minCol = elfs.Min(one => one.Item2);
            int maxRow = elfs.Max(one => one.Item1);
            int maxCol = elfs.Max(one => one.Item2);

            int totalSpaces = (maxRow - minRow + 1) * (maxCol - minCol + 1);
            int answer = totalSpaces - elfs.Count;

            Console.WriteLine("Part 1 - " + answer);
            Console.ReadLine();
        }

        static void Print(HashSet<(int, int)> elfs, int round)
        {
            //print it out for debugging
            int minR = elfs.Min(one => one.Item1);
            int minC = elfs.Min(one => one.Item2);
            int maxR = elfs.Max(one => one.Item1);
            int maxC = elfs.Max(one => one.Item2);

            Console.WriteLine("Round " + round);
            for (int r = minR; r <= maxR; r++)
            {
                for (int c = minC; c <= maxC; c++)
                {
                    if (elfs.Contains((r, c)))
                        Console.Write("#");
                    else
                        Console.Write(".");
                }
                Console.Write("\n");
            }
        }

        public static (int, int)[] GetStepsInDirection((int, int) direction, (int, int) elf)
        {
            (int, int) proposedDirection1 = direction;
            (int, int) proposedDirection2 = direction;
            (int, int) proposedDirection3 = direction;

            if (proposedDirection2.Item1 == 0)
            {
                proposedDirection2.Item1++;
                proposedDirection3.Item1--;
            }
            else
            {
                proposedDirection2.Item2++;
                proposedDirection3.Item2--;
            }

            var check1 = (elf.Item1 + proposedDirection1.Item1, elf.Item2 + proposedDirection1.Item2);
            var check2 = (elf.Item1 + proposedDirection2.Item1, elf.Item2 + proposedDirection2.Item2);
            var check3 = (elf.Item1 + proposedDirection3.Item1, elf.Item2 + proposedDirection3.Item2);

            return new (int, int)[] { check1, check2, check3 }; 
        }

        public static bool CanGoDirection((int, int) direction, (int, int) elf, HashSet<(int, int)> elfs) 
        {
            var toCheck = GetStepsInDirection(direction, elf);            
            foreach (var check in toCheck)            
                if (elfs.Contains(check)) return false;

            return true;
        }
    }
}
