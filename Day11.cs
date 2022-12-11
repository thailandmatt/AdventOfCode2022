using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day11
    {      
        public class Day11Monkey
        {
            public List<int> items = new List<int>();
            public List<long> longItems = new List<long>();
            public bool isAddition = false;
            public bool isMultiplication = false;
            public bool isSquared = false;
            public int operand = 0;

            public int divisibleTest = 0;

            public int monkeyNumberTrue = 0;
            public int monkeyNumberFalse = 0;

            public int totalVisited = 0;
        }

        public static void Day11Part1()
        {
            var fileText = System.IO.File.ReadAllText("Day11.txt");
            var monkeyTexts = fileText.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<Day11Monkey> monkeys = new List<Day11Monkey>();
            foreach (var monkeyText in monkeyTexts)
            {
                Day11Monkey monkey = new Day11Monkey();
                monkeys.Add(monkey);

                string[] split = monkeyText.Split('\n');

                monkey.items = split[1].Trim().Replace("Starting items: ", "").Split(',').Select(x => int.Parse(x.Trim())).ToList();
                monkey.isAddition = split[2].Contains("+");
                monkey.isMultiplication = split[2].Contains("*") && !split[2].Contains("old * old");
                monkey.isSquared = split[2].Contains("old * old");
                monkey.operand = monkey.isSquared ? 0 : int.Parse(split[2].Split(' ').Last());
                monkey.divisibleTest = int.Parse(split[3].Split(' ').Last());
                monkey.monkeyNumberTrue = int.Parse(split[4].Split(' ').Last());
                monkey.monkeyNumberFalse = int.Parse(split[5].Split(' ').Last());
            }

            for (int round = 0; round < 20; round++)
            {
                foreach (var monkey in monkeys)
                {
                    monkey.totalVisited += monkey.items.Count;

                    for (int i = 0; i < monkey.items.Count; i++)
                    {
                        //pull it out
                        int item = monkey.items[i];
                        monkey.items.RemoveAt(0);
                        i--;

                        //do the operation
                        if (monkey.isAddition) item += monkey.operand;
                        if (monkey.isMultiplication) item *= monkey.operand;
                        if (monkey.isSquared) item *= item;

                        //divide by 3
                        item = item / 3;

                        //check
                        bool check = item % monkey.divisibleTest == 0;

                        if (check)
                            monkeys[monkey.monkeyNumberTrue].items.Add(item);
                        else
                            monkeys[monkey.monkeyNumberFalse].items.Add(item);
                    }
                }
            }

            var totals = monkeys.Select(one => one.totalVisited).ToList();
            totals.Sort();
            totals.Reverse();            

            Console.WriteLine("Part 1 - " + (totals[0] * totals[1]));
        }

        public static void Day11Part2()
        {
            //two things for part 2 - first the numbers get big enough they need to be long instead of int
            //second, the overall gets too big for even a long, so we have to mod by something
            //and we mod by all the "divisibles" for the monkeys multiplied together
            //looking at the puzzle input they're all primes, so I'm sure this is what is expected

            var fileText = System.IO.File.ReadAllText("Day11.txt");
            var monkeyTexts = fileText.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //gonna have to mod by all the divisible tests put together
            int productOfAllDivisble = 1;

            List<Day11Monkey> monkeys = new List<Day11Monkey>();
            foreach (var monkeyText in monkeyTexts)
            {
                Day11Monkey monkey = new Day11Monkey();
                monkeys.Add(monkey);

                string[] split = monkeyText.Split('\n');

                monkey.longItems = split[1].Trim().Replace("Starting items: ", "").Split(',').Select(x => long.Parse(x.Trim())).ToList();
                monkey.isAddition = split[2].Contains("+");
                monkey.isMultiplication = split[2].Contains("*") && !split[2].Contains("old * old");
                monkey.isSquared = split[2].Contains("old * old");
                monkey.operand = monkey.isSquared ? 0 : int.Parse(split[2].Split(' ').Last());
                monkey.divisibleTest = int.Parse(split[3].Split(' ').Last());
                monkey.monkeyNumberTrue = int.Parse(split[4].Split(' ').Last());
                monkey.monkeyNumberFalse = int.Parse(split[5].Split(' ').Last());

                productOfAllDivisble *= monkey.divisibleTest;
            }

            for (int round = 1; round <= 10000; round++)
            {
                foreach (var monkey in monkeys)
                {
                    monkey.totalVisited += monkey.longItems.Count;

                    for (int i = 0; i < monkey.longItems.Count; i++)
                    {
                        //pull it out
                        var item = monkey.longItems[i];
                        monkey.longItems.RemoveAt(0);
                        i--;

                        //do the operation
                        if (monkey.isAddition) item += (long)monkey.operand;
                        if (monkey.isMultiplication) item *= (long)monkey.operand;
                        if (monkey.isSquared) item *= item;

                        //divide by 3 - not for part 2
                        //item = item / 3;

                        item %= productOfAllDivisble;

                        //check
                        bool check = item % monkey.divisibleTest == 0;

                        if (check)
                            monkeys[monkey.monkeyNumberTrue].longItems.Add(item);
                        else
                            monkeys[monkey.monkeyNumberFalse].longItems.Add(item);
                    }
                }
            }

            var totals = monkeys.Select(one => one.totalVisited).ToList();
            totals.Sort();
            totals.Reverse();

            Console.WriteLine("Part 2 - " + ((long)totals[0] * (long)totals[1]));
            Console.ReadLine();
        }
    }
}
