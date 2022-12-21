using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day21
    {  
        public static void Part1Part2()
        {
            //parse            
            var fileLines = System.IO.File.ReadAllLines("Day21.txt");

            Dictionary<string, string> monkeys = new Dictionary<string, string>();
            foreach (var line in fileLines)
            {
                var s = line.Split(':');
                monkeys[s[0]] = s[1];
            }

            long part1 = Eval(monkeys["root"], monkeys);

            Console.WriteLine("Part 1 - " + part1);
            monkeys.Remove("humn");
            string curKey = "root";
            monkeys["root"] = monkeys["root"].Replace("+", "-");
            long wantResult = 0;
            long humnAnswer = 0;

            while (curKey != "")
            {
                if (curKey == "humn")
                {
                    humnAnswer = wantResult;
                    break;
                }

                var parts = monkeys[curKey].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                long left = 0;
                long right = 0;
                bool hasLeft = false;

                try
                {
                    left = Eval(monkeys[parts[0]], monkeys);
                    hasLeft = true;
                }
                catch { }


                try
                {
                    right = Eval(monkeys[parts[2]], monkeys);
                }
                catch { }

                

                if (hasLeft)
                {
                    //set to go down the right path
                    curKey = parts[2];

                    //do the opposite match to figure out what we want
                    switch (parts[1])
                    {
                        case "+":
                            //X + left == want, so want - left = x
                            wantResult = wantResult - left;
                            break;
                        case "-":
                            //left - X == want, so left - want = x
                            wantResult = left - wantResult;
                            break;
                        case "*":
                            //X * left == want, so want / left = x
                            wantResult = wantResult / left;
                            break;
                        case "/":
                            //left / X == want, so left / want = x
                            wantResult =  left / wantResult;
                            break;
                    }
                }
                else
                {
                    //set to go down the right path
                    curKey = parts[0];

                    //do the opposite match to figure out what we want
                    switch (parts[1])
                    {
                        case "+":
                            //X + right == want, so want - right = x
                            wantResult = wantResult - right;
                            break;
                        case "-":
                            //X - right == want, so want + right = x
                            wantResult = wantResult + right;
                            break;
                        case "*":
                            //X * right == want, so want / left = x
                            wantResult = wantResult / right;
                            break;
                        case "/":
                            //X / right  == want, so want * right = x
                            wantResult = wantResult * right;
                            break;
                    }
                }
            }

            Console.WriteLine("Part 2 - " + humnAnswer);

            Console.ReadLine();
        }

        static string Simplify(string s)
        {
            bool changed = true;

            while (changed)
            {
                changed = false;
                for (int i = 0; i < s.Length; i++)
                {
                    if (s[i] == '(')
                    {
                        string sub = s.Substring(i + 1, s.IndexOf(")", i) - i - 1);
                        if (sub.Contains("(") || sub.Contains("=") || sub.Contains("humn"))
                            continue;
                        else
                        {
                            changed = true;

                            var parts = sub.Split('+', '-', '*', '/');
                            long answer = -1;
                            if (s.Contains("+"))
                                answer = long.Parse(parts[0]) + long.Parse(parts[1]);                            
                            else if (s.Contains("*")) 
                                answer = long.Parse(parts[0]) * long.Parse(parts[1]);
                            else if (s.Contains("/"))
                                answer = long.Parse(parts[0]) / long.Parse(parts[1]);
                            else if (s.Contains("-"))
                                answer = long.Parse(parts[0]) - long.Parse(parts[1]);


                            s = s.Substring(0, i) + answer.ToString() + s.Substring(i + sub.Length + 2);
                        }
                    }
                }
            }

            return s;
        }
        
        static List<string> GetTree(string s, Dictionary<string, string> monkeys, int leftIfZero)
        {
            var parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> returnValue = new List<string>(parts);
            if (parts.Length == 1) return returnValue;
            returnValue.AddRange(GetTree(monkeys[parts[0]], monkeys, leftIfZero));            
            returnValue.AddRange(GetTree(monkeys[parts[2]], monkeys, leftIfZero));            

            return returnValue;
        }

        static string GetEquationForHumn(string s, Dictionary<string, string> monkeys)
        {
            var parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0];

            string left = parts[0];
            string right = parts[2];

            if (left != "humn") left = GetEquationForHumn(monkeys[left], monkeys);
            if (right != "humn") right = GetEquationForHumn(monkeys[right], monkeys);

            return "(" + left + parts[1] + right + ")";
        }

        static long Eval(string s, Dictionary<string, string> monkeys)
        {
            var parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return long.Parse(parts[0]);

            switch (parts[1])
            {
                case "+":
                    return Eval(monkeys[parts[0]], monkeys) + Eval(monkeys[parts[2]], monkeys);
                case "-":
                    return Eval(monkeys[parts[0]], monkeys) - Eval(monkeys[parts[2]], monkeys);
                case "*":
                    return Eval(monkeys[parts[0]], monkeys) * Eval(monkeys[parts[2]], monkeys);
                case "/":
                    return Eval(monkeys[parts[0]], monkeys) / Eval(monkeys[parts[2]], monkeys);
            }

            return -1;
        }
    }
}
