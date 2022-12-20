using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day20
    {       
        public class LinkedListInt
        {
            public int value;
            public long multipliedValue;
            public LinkedListInt prev;
            public LinkedListInt next;            

            public void Mix()
            {
                for (int i = 0; i != value; i += value < 0 ? -1 : 1)
                {
                    if (value > 0)
                    {
                        //move A->B**->C->D to A->C->B**->D                        
                        var A = this.prev;
                        var B = this;
                        var C = this.next;
                        var D = this.next.next;

                        A.next = C;
                        C.next = B;
                        B.next = D;
                        C.prev = A;
                        B.prev = C;
                        D.prev = B;
                    }
                    else
                    {
                        //move Z->A->B**->C to Z->B**->A->C                       
                        var Z = this.prev.prev;
                        var A = this.prev;
                        var B = this;
                        var C = this.next;

                        Z.next = B;
                        B.next = A;
                        A.next = C;
                        B.prev = Z;
                        A.prev = B;
                        C.prev = A;
                    }
                }
            }

            public override string ToString()
            {
                return value.ToString();
            }

            public List<LinkedListInt> EnumerateFromHere()
            {
                List<LinkedListInt> newOrder = new List<LinkedListInt>();
                var cur = this;
                do
                {
                    newOrder.Add(cur);
                    cur = cur.next;
                } while (cur != this);

                return newOrder;
            }

            public string EnumerateFromHereAsString()
            {
                var newOrder = EnumerateFromHere();
                return string.Join(" ", newOrder);
            }
        }
        public static void Part1()
        {
            //parse            
            var fileLines = System.IO.File.ReadAllLines("Day20.txt");
            var originalOrder = fileLines.Select(one => new LinkedListInt() { value = int.Parse(one) }).ToList();

            //hook up the list
            for (int i = 0; i < originalOrder.Count(); i++)
            {
                originalOrder[i].prev = (i == 0) ? originalOrder[originalOrder.Count - 1] : originalOrder[i - 1];
                originalOrder[i].next = (i == originalOrder.Count - 1) ? originalOrder[0] : originalOrder[i + 1];
            }

            //mix
            foreach (var one in originalOrder)            
                one.Mix();

            var newOrder = originalOrder[0].EnumerateFromHere();

            int zero = newOrder.FindIndex(one => one.value == 0);

            var x1000 = newOrder[(1000 + zero) % (newOrder.Count)].value;
            var x2000 = newOrder[(2000 + zero) % (newOrder.Count)].value;
            var x3000 = newOrder[(3000 + zero) % (newOrder.Count)].value;
            Console.WriteLine("Part 1 - " + x1000 + ", " + x2000 + ", " + x3000 + " = " + (x1000 + x2000 + x3000));            
        }

        public static void Part2()
        {
            //parse            
            var fileLines = System.IO.File.ReadAllLines("Day20.txt");
            var originalOrder = fileLines.Select(one => new LinkedListInt() { value = int.Parse(one) }).ToList();

            //hook up the list
            for (int i = 0; i < originalOrder.Count(); i++)
            {
                originalOrder[i].prev = (i == 0) ? originalOrder[originalOrder.Count - 1] : originalOrder[i - 1];
                originalOrder[i].next = (i == originalOrder.Count - 1) ? originalOrder[0] : originalOrder[i + 1];
            }

            //multiply and mod by the size of the list
            foreach (var one in originalOrder)
            {
                long x = (long)one.value;
                x *= 811589153;
                one.multipliedValue = x;
                x %= (originalOrder.Count - 1);
                one.value = (int)x;
            }

            //mix 10 times
            for (int i = 0; i < 10; i++)
            {
                foreach (var one in originalOrder)
                    one.Mix();
            }

            var newOrder = originalOrder[0].EnumerateFromHere();

            int zero = newOrder.FindIndex(one => one.value == 0);

            var x1000 = newOrder[(1000 + zero) % (newOrder.Count)].multipliedValue;
            var x2000 = newOrder[(2000 + zero) % (newOrder.Count)].multipliedValue;
            var x3000 = newOrder[(3000 + zero) % (newOrder.Count)].multipliedValue;
            Console.WriteLine("Part 2 - " + x1000 + ", " + x2000 + ", " + x3000 + " = " + (x1000 + x2000 + x3000));
            

            Console.ReadLine();
        }
    }
}
