using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day16
    {   
        class Valve
        {
            public string ValveName { get; set; }
            public int Rate { get; set; }
            public bool Opened { get; set; }
            public List<string> OtherValves { get; set; }

            public override string ToString()
            {
                return ValveName + " - " + Rate + ", " + string.Join(",", OtherValves);
            }

        }
        public static void Day16Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day16.txt");

            Dictionary<string, Valve> valves = new Dictionary<string, Valve>();
            foreach (var line in fileLines)
            {
                var parts = line.Split(';');
                var parts1 = parts[0].Split(' ');
                var parts2 = parts[1].Replace(" lead to ", "")
                    .Replace(" leads to ", "")
                    .Replace("tunnels", "")
                    .Replace("tunnel", "")
                    .Replace("valve ", "")
                    .Replace("valves ", "")
                    .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                valves.Add(parts1[1], new Valve()
                {
                    ValveName = parts1[1],
                    Rate = int.Parse(parts1[4].Replace("rate=", "").Replace(";", "")),                    
                    OtherValves = new List<string>(parts2)
                });
            }

            //Ok run the floyd-warshall to get the full distance matrix
            List<string> valveIndices = valves.Keys.ToList();
            int[,] distanceMatrix = GetDistanceMatrix(valveIndices, valves);
            int[] flows = new int[valveIndices.Count];
            for (int i = 0; i < valveIndices.Count; i++)            
                flows[i] = valves[valveIndices[i]].Rate;

            ////Do DFS and prune
            var (max, maxPath) = DFSFindMax(
                30,
                valveIndices.IndexOf("AA"),
                0,
                valves.Values.ToList(),
                distanceMatrix,
                valveIndices);


            Console.WriteLine("Part 1 - " + max);
            Console.WriteLine(string.Join(",", maxPath));

            //I ended up doing this manually - it was easier with only the 15 nodes to do greedy algorithms
            //on paper and adjust manually with intuition so that's what I did
        }
                
        static int[,] GetDistanceMatrix(List<string> valveIndices, Dictionary<string, Valve> valves)
        {
            //Ok run the floyd-warshall to get the full distance matrix
            int[,] distanceMatrix = new int[valveIndices.Count, valveIndices.Count];

            for (int i = 0; i < valveIndices.Count; i++)
            {
                for (int j = 0; j < valveIndices.Count; j++)
                {
                    distanceMatrix[i, j] = (i == j) ? 0 : -1;
                }
            }

            //now add all the edges of 1
            foreach (var valve in valves.Values)
            {
                var vIndex = valveIndices.IndexOf(valve.ValveName);
                foreach (var other in valve.OtherValves)
                {
                    var oIndex = valveIndices.IndexOf(other);
                    if (oIndex == vIndex) continue;
                    distanceMatrix[vIndex, oIndex] = 1;
                    distanceMatrix[oIndex, vIndex] = 1;
                }
            }

            for (int k = 0; k < valveIndices.Count; k++)
            {
                for (int i = 0; i < valveIndices.Count; i++)
                {
                    for (int j = 0; j < valveIndices.Count; j++)
                    {
                        if (distanceMatrix[i, k] != -1 && distanceMatrix[k, j] != -1)
                        {
                            if (distanceMatrix[i, j] == -1 || distanceMatrix[i, j] > (distanceMatrix[i, k] + distanceMatrix[k, j]))
                                distanceMatrix[i, j] = distanceMatrix[i, k] + distanceMatrix[k, j];
                        }
                    }
                }
            }

            return distanceMatrix;
        }

        //returns the max pressure released and the path it took
        //ok so I couldn't get this to work - gonna try DP instead
        static (int, List<string>) DFSFindMax(
            int curMinute,
            int curValveIndex,
            int sumSoFarFromParents,
            List<Valve> valves, 
            int[,] distanceMatrix, 
            List<string> valveIndices)
        {
            var possibleValves = valves.FindAll(one => one.Rate > 0 && !one.Opened).OrderByDescending(one => one.Rate).ToList();

            if (curMinute == 0 || possibleValves.Count == 0)
            {
                //end of the road                
                return (sumSoFarFromParents, new List<string>(new string[] { valves[curValveIndex].ValveName + "(" + curMinute + ")" } ));
            }

            //look through each positive flow valve that isn't opened yet
            //see if we went there what the upper bound would be to see if its worth going
            //down that path

            //then go down each of those viable paths and take the max to return to
            //our parent            
            //List<int> distancesToPossibleValves = new List<int>();
            //foreach (var valve in possibleValves)
            //{
            //    int valveIndex = valveIndices.IndexOf(valve.ValveName);
            //    int distanceToThisValve = distanceMatrix[curValveIndex, valveIndex];
            //    distancesToPossibleValves.Add(distanceToThisValve);
            //}

            int maxSolution = 0;
            List<string> maxPath = new List<string>();
            
            for (int i = 0; i < possibleValves.Count; i++)
            {
                var valve = possibleValves[i];
                int valveIndex = valveIndices.IndexOf(valve.ValveName);                
                int thisPathSum = sumSoFarFromParents;
                //int distanceToThisValve = distancesToPossibleValves[i];
                int distanceToThisValve = distanceMatrix[curValveIndex, valveIndices.IndexOf(valve.ValveName)];
                int nextMinute = (curMinute - distanceToThisValve - 1);
                thisPathSum += (valve.Rate * nextMinute);
                 
                ////these checks for pruning
                ///
                //int sumValvesLeft = possibleValves.Where(one => one.ValveName != valve.ValveName).Sum(one => one.Rate);
                //int minDistance = distancesToPossibleValves.Min();
                //int upperBound = sumValvesLeft * (nextMinute - minDistance - 1);

                //if (thisPathSum + upperBound > maxSolution)
                //{
                    //recurse down the tree
                    valve.Opened = true;                    
                    (int, List<string>) answer = DFSFindMax(nextMinute, valveIndex, thisPathSum, valves, distanceMatrix, valveIndices);
                    if (answer.Item1 > maxSolution)
                    {
                        maxSolution = answer.Item1;
                        maxPath = answer.Item2;
                    }
                    valve.Opened = false;
                //}
            }

            maxPath.Insert(0, valves[curValveIndex].ValveName + "(" + curMinute + ")");
            return (maxSolution, maxPath);
        }

        public static void Day16Part2()
        {           
            var fileLines = System.IO.File.ReadAllLines("Day16.txt");

            Console.WriteLine("Part 2 - ");
            Console.ReadLine();
        }
    }
}