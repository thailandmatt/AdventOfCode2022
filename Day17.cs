using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day17
    {   public static void Part1Part2()
        {
            Play(2022);
            Play(1000000000000);
            Console.ReadLine();
        }

        static void Play(long rounds)
        {
            var pattern = System.IO.File.ReadAllText("Day17.txt");

            HashSet<(long, long)> cavernRocks = new HashSet<(long, long)>();

            int curJet = 0;

            Dictionary<string, (long, long)> lastSeen = new Dictionary<string, (long, long)>();
            List<long> history = new List<long>();

            long towerHeight = 0;

            for (long round = 0; round < rounds; round++)
            {
                //initiate rock 3 above current ceiling
                var ceiling = cavernRocks.Count == 0 ? (long)-1 : cavernRocks.Max(one => one.Item2);
                List<(long, long)> rock = GetRock((int)(round % 5), ceiling + 4);

                while (true)
                {
                    //see if it will move left or right
                    (int, int) dir = pattern[curJet] == '<' ? (-1, 0) : (1, 0);
                    curJet++;
                    if (curJet >= pattern.Length) curJet = 0;

                    DoMoveIfCan(rock, dir, cavernRocks);
                    bool canGoDown = DoMoveIfCan(rock, (0, -1), cavernRocks);
                    if (!canGoDown) break;
                }

                foreach (var point in rock)
                    cavernRocks.Add(point);

                //we only need to keep like the last 50 or something
                var max = cavernRocks.Max(p => p.Item2);
                cavernRocks.RemoveWhere(one => one.Item2 < max - 10);

                //see if we've been here before
                string code = (int)(round % 5) + "-" + curJet;

                history.Add(max);

                if (lastSeen.ContainsKey(code))
                {
                    //something still not right here because I keep being off by one
                    var startOfCycle = lastSeen[code];
                    var startOfSecondCycle = (round, max);
                    var cycleHeight = startOfSecondCycle.Item2 - startOfCycle.Item2;
                    var cycleLength = startOfSecondCycle.Item1 - startOfCycle.Item1;
                    var numCyclesRemaining = (rounds - round) / cycleLength;

                    var residue = (rounds - round) % cycleLength;
                    var residueHeight = history[(int)startOfCycle.Item1 + (int)residue] - history[(int)startOfCycle.Item1 + 1];                    
                    towerHeight = startOfSecondCycle.Item2 + (numCyclesRemaining * cycleHeight) + residueHeight;

                    break;                    
                }                

                lastSeen[code] = (round, max);
            }

            //add one for 0 based
            Console.WriteLine(rounds + " - " + towerHeight);                        
        }

        static bool DoMoveIfCan(List<(long, long)> rock, (int, int) direction, HashSet<(long, long)> cavernRocks)
        {
            bool canMove = true;
            foreach ((int, int) p in rock)
            {
                if (cavernRocks.Contains((p.Item1 + direction.Item1, p.Item2 + direction.Item2)) ||
                    p.Item1 + direction.Item1 < 0 ||
                    p.Item1 + direction.Item1 > 6 ||
                    p.Item2 + direction.Item2 < 0)
                {
                    canMove = false;
                    break;
                }
            }

            if (canMove)
            {
                for (int i = 0; i < rock.Count; i++)
                    rock[i] = (rock[i].Item1 + direction.Item1, rock[i].Item2 + direction.Item2);
            }

            return canMove;
        }

        static List<(long, long)> GetRock(int rockIndex, long y)
        {
            if (rockIndex == 0)
                return new List<(long, long)>(new (long, long)[]{ (2, y), (3, y), (4, y), (5, y) });
            else if (rockIndex == 1)
                return new List<(long, long)>(new (long, long)[]{ (3, y + 2), (2, y + 1), (3, y + 1), (4, y + 1), (3, y) });
            else if (rockIndex == 2)
                return new List<(long, long)>(new (long, long)[] { (2, y), (3, y), (4, y), (4, y + 1), (4, y + 2) });
            else if (rockIndex == 3)
                return new List<(long, long)>(new (long, long)[] { (2, y), (2, y + 1), (2, y + 2), (2, y + 3) });
            else if (rockIndex == 4)
                return new List<(long, long)>(new (long, long)[] { (2, y + 1), (2, y), (3, y + 1), (3, y) });

            return null;
        }            
    }
}