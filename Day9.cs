using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day9
    {      

        public static void Day9Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day9.txt");
            List<(int, int)> visitedPoints = new List<(int, int)>();

            (int, int) curHead = (0, 0);
            (int, int) curTail = (0, 0);

            visitedPoints.Add(curTail);

            foreach (var fileLine in fileLines)
            {
                string[] split = fileLine.Split(' ');
                for (int i = 0; i < int.Parse(split[1]); i++)
                {
                    //move head
                    if (split[0] == "L")
                    {
                        curHead = (curHead.Item1 - 1, curHead.Item2);
                        //see if we need to move the tail
                        if (curTail.Item1 - curHead.Item1 >= 2)
                        {
                            curTail = (curTail.Item1 - 1, curHead.Item2);
                            visitedPoints.Add(curTail);
                        }
                    }
                    else if (split[0] == "R")
                    {
                        curHead = (curHead.Item1 + 1, curHead.Item2);
                        if (curHead.Item1 - curTail.Item1 >= 2)
                        {
                            curTail = (curTail.Item1 + 1, curHead.Item2);
                            visitedPoints.Add(curTail);
                        }
                    }
                    else if (split[0] == "U")
                    {
                        curHead = (curHead.Item1, curHead.Item2 - 1);
                        if (curTail.Item2 - curHead.Item2 >= 2)
                        {
                            curTail = (curHead.Item1, curTail.Item2 - 1);
                            visitedPoints.Add(curTail);
                        }
                    }
                    else if (split[0] == "D")
                    {
                        curHead = (curHead.Item1, curHead.Item2 + 1);
                        if (curHead.Item2 - curTail.Item2 >= 2)
                        {
                            curTail = (curHead.Item1, curTail.Item2 + 1);
                            visitedPoints.Add(curTail);
                        }
                    }
                }
            }

            visitedPoints = visitedPoints.Distinct().ToList();

            Console.WriteLine("Part 1 - " + visitedPoints.Count);
        }

        public static void Day9Part2()
        {
            var fileLines = System.IO.File.ReadAllLines("Day9.txt");
            List<(int, int)> visitedPoints = new List<(int, int)>();

            List<(int, int)> snake = new List<(int, int)>();
            for (int i = 0; i < 10; i++)
            {
                snake.Add((0, 0));
            }            

            visitedPoints.Add(snake[9]);

            foreach (var fileLine in fileLines)
            {
                string[] split = fileLine.Split(' ');
                for (int i = 0; i < int.Parse(split[1]); i++)
                {
                    //move head
                    if (split[0] == "L")
                    {
                        snake[0] = (snake[0].Item1 - 1, snake[0].Item2);
                    }
                    else if (split[0] == "R")
                    {
                        snake[0] = (snake[0].Item1 + 1, snake[0].Item2);
                    }
                    else if (split[0] == "U")
                    {
                        snake[0] = (snake[0].Item1, snake[0].Item2 - 1);
                    }
                    else if (split[0] == "D")
                    {
                        snake[0] = (snake[0].Item1, snake[0].Item2 + 1);
                    }

                    //see if we need to move the tails
                    for (int j = 1; j < 10; j++)
                    {
                        int leftRight = snake[j - 1].Item1 - snake[j].Item1;
                        int upDown = snake[j - 1].Item2 - snake[j].Item2;

                        //no need if within 1 each way
                        if (Math.Abs(leftRight) <= 1 && Math.Abs(upDown) <= 1) continue;

                        snake[j] = (
                            snake[j].Item1 + (leftRight == 0 ? 0 : leftRight > 0 ? 1 : -1), 
                            snake[j].Item2 + (upDown == 0 ? 0 : upDown > 0 ? 1 : -1)
                            );
                    }

                    visitedPoints.Add(snake[9]);
                }
            }

            visitedPoints = visitedPoints.Distinct().ToList();

            Console.WriteLine("Part 2 - " + visitedPoints.Count);
            Console.ReadLine();
        }
    }
}
