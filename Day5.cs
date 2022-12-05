using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day5
    {
        private static List<Stack<char>> ParseInput()
        {
            List<Stack<char>> stackList = new List<Stack<char>>();
            //1, 5, 9, 13...
            var fileLines = System.IO.File.ReadAllLines("Day5.txt");
            for (int i = 0; i <= fileLines[0].Length / 4; i++)
            {
                stackList.Add(new Stack<char>());
            }

            foreach (var line in fileLines)
            {
                if (line == "") break;
                for (int i = 1; i < line.Length; i = i + 4)
                {
                    if (line[i] != ' ')
                        stackList[i / 4].Push(line[i]);
                }
            }
            
            //pop the stack number
            foreach (var stack in stackList)
                stack.Pop();

            //reverse the stacks
            List<Stack<char>> revList = new List<Stack<char>>();            
            foreach (var stack in stackList)
            {
                var rev = new Stack<char>();
                revList.Add(rev);
                while (stack.Count > 0)
                    rev.Push(stack.Pop());
            }

            return revList;
        }

        public static void Day5Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day5.txt");            

            List<Stack<char>> stackList = ParseInput();

            bool loading = true;

            foreach (var line in fileLines)
            {
                if (line == "")
                {
                    loading = false;
                }
                else if (!loading)
                {                    
                    var instructions = line.Split(new string[] { "move", "from", "to" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < int.Parse(instructions[0]); i++)
                    {
                        var x = stackList[int.Parse(instructions[1]) - 1].Pop();
                        stackList[int.Parse(instructions[2]) - 1].Push(x);
                    }
                }
            }

            Console.WriteLine("Part 1:");           
            foreach (var stack in stackList)
            {
                Console.Write(stack.Peek());
            }
            Console.WriteLine("");
        }

        public static void Day5Part2()
        {
            var fileLines = System.IO.File.ReadAllLines("Day5.txt");

            List<Stack<char>> stackList = ParseInput();

            bool loading = true;

            foreach (var line in fileLines)
            {
                if (line == "")
                {
                    loading = false;
                }
                else if (!loading)
                {
                    var instructions = line.Split(new string[] { "move", "from", "to" }, StringSplitOptions.RemoveEmptyEntries);
                    Stack<char> temp = new Stack<char>();
                    for (int i = 0; i < int.Parse(instructions[0]); i++)
                    {                        
                        var x = stackList[int.Parse(instructions[1]) - 1].Pop();
                        temp.Push(x);                        
                    }
                    
                    while (temp.Count > 0)
                        stackList[int.Parse(instructions[2]) - 1].Push(temp.Pop());
                }
            }

            Console.WriteLine("Part 2:");
            foreach (var stack in stackList)
            {
                Console.Write(stack.Peek());
            }
            Console.WriteLine("");
            Console.ReadLine();
        }
    }
}
