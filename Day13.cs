using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day13
    {   
        public static List<object> Parse(string s)
        {
            List<object> list = new List<object>();

            Stack<List<object>> stack = new Stack<List<object>>();
            stack.Push(list);

            for (int i = 1; i < s.Length - 1; i++)
            {
                if (s[i] == '[')
                {
                    var newList = new List<object>();
                    stack.Peek().Add(newList);
                    stack.Push(newList);
                }
                else if (s[i] == ']')
                {
                    stack.Pop();
                }
                else if (s[i] == ',')
                {

                }
                else
                {
                    string toParse = s.Substring(i, s.IndexOfAny(new char[] { ',', ']' }, i) - i);
                    stack.Peek().Add(int.Parse(toParse));
                    i = s.IndexOfAny(new char[] { ',', ']' }, i);
                }
            }

            return list;
        }

        static string ComparePairs(List<object> pair1, List<object> pair2)
        {
            for (int i = 0; i < pair1.Count; i++)
            {
                //left runs out first false
                if (i >= pair2.Count) return "NOLeftListBigger";                

                if (pair1[i] is int && pair2[i] is int)
                {
                    //left is bigger between two numbers
                    if ((int)pair1[i] > (int)pair2[i]) return "NOLeftBigger";
                    if ((int)pair1[i] < (int)pair2[i]) return "OKRightBigger";
                }

                if (pair1[i] is List<object> && pair2[i] is List<object>)
                {
                    string test = ComparePairs(pair1[i] as List<object>, pair2[i] as List<object>);
                    if (!string.IsNullOrEmpty(test)) return test;
                }

                if (pair1[i] is int && pair2[i] is List<object>)
                {
                    List<object> converted = new List<object>();
                    converted.Add(pair1[i]);
                    string test = ComparePairs(converted, pair2[i] as List<object>);
                    if (!string.IsNullOrEmpty(test)) return test;
                }

                if (pair1[i] is List<object> && pair2[i] is int)
                {
                    List<object> converted = new List<object>();
                    converted.Add(pair2[i]);
                    string test = ComparePairs(pair1[i] as List<object>, converted);
                    if (!string.IsNullOrEmpty(test)) return test;
                }
            }

            if (pair1.Count == pair2.Count) return "";

            return "OKLeftListShorter";
        }

        public static void Day13Part1()
        {
            var fileText = System.IO.File.ReadAllText("Day13.txt").Replace("\r", "");
            string[] pairTexts = fileText.Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<int> pairIndicesInCorrectOrder = new List<int>();

            List<List<object>> part2List = new List<List<object>>();

            for (int i = 0; i < pairTexts.Length; i++)            
            {
                var pairText = pairTexts[i];
                var split = pairText.Split('\n');
                var pair1 = Parse(split[0]);
                var pair2 = Parse(split[1]);

                part2List.Add(pair1);
                part2List.Add(pair2);

                string test = ComparePairs(pair1, pair2);                
                if (test.StartsWith("OK") || string.IsNullOrEmpty(test))
                {
                    pairIndicesInCorrectOrder.Add(i + 1);
                }
            }
       
            Console.WriteLine("Part 1 - " + pairIndicesInCorrectOrder.Sum());

            var div1 = Parse("[[2]]");
            var div2 = Parse("[[6]]");
            part2List.Add(div1);
            part2List.Add(div2);

            part2List.Sort((a, b) => {
                string test = ComparePairs(a, b);
                return (test.StartsWith("OK") || string.IsNullOrEmpty(test)) ? -1 : 1;
                    });

            int div1Index = part2List.IndexOf(div1);
            int div2Index = part2List.IndexOf(div2);

            Console.WriteLine("Part 2 - " + ((div1Index + 1) * (div2Index + 1)));

            Console.ReadLine();
        }
    }
}
