using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day19
    {
        public enum R
        {
            ore = 0,
            clay = 1,
            obsidian = 2,
            geode = 3
        }

        public class SimulationState
        {
            public int[] Robots = new int[4];
            public int[] Resources = new int[4];
            public int Minute;
            public List<string> History = new List<string>();

            public SimulationState Clone()
            {
                return new SimulationState()
                {
                    Robots = new List<int>(Robots).ToArray(),
                    Resources = new List<int>(Resources).ToArray(),
                    Minute = Minute,
                    History = new List<string>(History)
                };
            }

            public override string ToString()
            {
                return this.Minute + " - Robots: " + string.Join(" ", Robots);
            }
        }

        public static void Part1Part2()
        {
            //parse
            List<int[,]> robotCosts = new List<int[,]>();
            var fileLines = System.IO.File.ReadAllLines("Day19.txt");
            foreach (var line in fileLines)
            {
                int[,] blueprint = new int[4, 4];
                var numbers = Regex.Matches(line, "\\d+");
                blueprint[(int)R.ore, (int)R.ore] = int.Parse(numbers[1].Value); //cost of ore machine in ore
                blueprint[(int)R.clay, (int)R.ore] = int.Parse(numbers[2].Value); //cost of clay machine in ore
                blueprint[(int)R.obsidian, (int)R.ore] = int.Parse(numbers[3].Value); //cost of obsidian machine in ore
                blueprint[(int)R.obsidian, (int)R.clay] = int.Parse(numbers[4].Value); //cost of obsidian machine in clay
                blueprint[(int)R.geode, (int)R.ore] = int.Parse(numbers[5].Value); //cost of geode machine in ore
                blueprint[(int)R.geode, (int)R.obsidian] = int.Parse(numbers[6].Value); //cost of geode machine in obsidian
                robotCosts.Add(blueprint);
            }

            List<int> maxForEachBluePrint = new List<int>();

            foreach (var bluePrintCosts in robotCosts)
            {
                Queue<SimulationState> queue = new Queue<SimulationState>();
                queue.Enqueue(new SimulationState()
                {
                    Robots = new int[] { 1, 0, 0, 0 },
                    Resources = new int[] { 0, 0, 0, 0 },
                    Minute = 1
                });

                int maxTime = 24;
                int maxGeodes = 0;

                HashSet<string> alreadyTested = new HashSet<string>();

                while (queue.Count > 0)
                {
                    var state = queue.Dequeue();

                    string code = state.Minute + "~" + string.Join(" ", state.Resources) + "~" + string.Join(" ", state.Robots);
                    if (alreadyTested.Contains(code)) continue;
                    alreadyTested.Add(code);

                    //this state max is the max we could do if we choose to do nothing from
                    //here on out
                    var thisStateGeodes = state.Resources[3] + (state.Robots[3] * (maxTime - state.Minute));
                    maxGeodes = Math.Max(thisStateGeodes, maxGeodes);

                    // for every bot we could build, jump to that
                    // point in simulation and add to queue
                    for (int i = 3; i >= 0; i--)
                    {
                        if (state.Robots[i] > 20) continue;

                        //see how long to build this one
                        int maxTurns = 0;
                        for (int j = 0; j < 4; j++)
                        {
                            int neededToBuildThis = Math.Max(0, bluePrintCosts[i, j] - state.Resources[j]);
                            int turnsForThisMineral = neededToBuildThis == 0 ? 0 : 
                                state.Robots[j] == 0 ? int.MaxValue :
                                neededToBuildThis / state.Robots[j];
                            maxTurns = Math.Max(turnsForThisMineral, maxTurns);
                        }

                        if (maxTurns == int.MaxValue || maxTurns + state.Minute >= maxTime)
                        {
                            //can't build it in time
                            continue;
                        }

                        //how many resources will we have then
                        int[] newResources = new int[4];
                        for (var j = 0; j < 4; j++)
                            newResources[j] = state.Resources[j] + (state.Robots[j] * (maxTurns));

                        //buy the robot
                        for (var j = 0; j < 4; j++)
                            newResources[j] -= bluePrintCosts[i, j];

                        //add the amount for that turn
                        for (var j = 0; j < 4; j++)
                            newResources[j] += state.Robots[j];

                        //add the new robot
                        int[] newRobots = { state.Robots[0], state.Robots[1], state.Robots[2], state.Robots[3] };
                        newRobots[i]++;

                        //queue up the beginning of the next turn
                        if (state.Minute + maxTurns + 1 == maxTime)
                        {
                            var testGeodes = newResources[3];
                            maxGeodes = Math.Max(testGeodes, maxGeodes);
                        }
                        else
                        {
                            queue.Enqueue(new SimulationState()
                            {
                                Robots = newRobots,
                                Resources = newResources,
                                Minute = state.Minute + maxTurns + 1
                            });
                        }
                    }
                }

                maxForEachBluePrint.Add(maxGeodes);
            }

            int quality = 0;
            for (int i = 0; i < maxForEachBluePrint.Count; i++)
            {
                quality += (i + 1) * maxForEachBluePrint[i];
            }

            Console.WriteLine("Part 1 - " + quality);
        }


        public static void Part1Part2Attempt2()
        {
            //parse
            List<int[,]> robotCosts = new List<int[,]>();
            var fileLines = System.IO.File.ReadAllLines("Day19.txt");
            foreach (var line in fileLines)
            {
                int[,] blueprint = new int[4, 4];
                var numbers = Regex.Matches(line, "\\d+");
                blueprint[(int)R.ore, (int)R.ore] = int.Parse(numbers[1].Value); //cost of ore machine in ore
                blueprint[(int)R.clay, (int)R.ore] = int.Parse(numbers[2].Value); //cost of clay machine in ore
                blueprint[(int)R.obsidian, (int)R.ore] = int.Parse(numbers[3].Value); //cost of obsidian machine in ore
                blueprint[(int)R.obsidian, (int)R.clay] = int.Parse(numbers[4].Value); //cost of obsidian machine in clay
                blueprint[(int)R.geode, (int)R.ore] = int.Parse(numbers[5].Value); //cost of geode machine in ore
                blueprint[(int)R.geode, (int)R.obsidian] = int.Parse(numbers[6].Value); //cost of geode machine in obsidian
                robotCosts.Add(blueprint);
            }

            List<int> maxForEachBluePrint = new List<int>();
                        
            foreach (var bluePrintCosts in robotCosts)
            {
                int[] robots = new int[] { 1, 0, 0, 0 };
                int[] minerals = new int[] { 0, 0, 0, 0 };
                List<string> history = new List<string>();

                //there is a deterministic algorithm to get the best I believe
                for (int minute = 1; minute <= 24; minute++)
                {
                    //always need more geode machines
                    List<R> needs = new List<R>();
                    needs.Add(R.geode);

                    //see if we can afford anything
                    bool canAffordAnything = false;                    
                    for (int i = 0; i < 4; i++)
                    {
                        bool canAffordThis = true;
                        for (int j = 0; j < 4; j++)
                        {
                            if (bluePrintCosts[i, j] > minerals[j])
                            {
                                canAffordThis = false;
                                break;
                            }
                        }

                        if (canAffordThis)
                        {
                            canAffordAnything = true;
                            break;
                        }
                    }

                    if (!canAffordAnything)
                    {
                        history.Add("M" + minute + ": Can't afford anything");
                    }

                    int newRobotIndex = -1;

                    while (canAffordAnything && true)
                    {                        
                        R robotNeeded = needs.Last();

                        history.Add("M" + minute + ": Need " + robotNeeded.ToString());

                        //see if we can afford the next need
                        bool canAfford = true;
                        for (int i = 0; i < 4; i++)
                        {
                            if (bluePrintCosts[(int)robotNeeded, i] > minerals[i])
                            {
                                canAfford = false;
                                break;
                            }
                        }

                        if (canAfford)
                        {
                            //if so, check for adverse affects on previous needs
                            //unless its a geode and then just buy it
                            bool buyIt = true;
                            if (robotNeeded != R.geode)
                            {
                                foreach (R previousNeed in needs)
                                {
                                    if (previousNeed == robotNeeded) break;
                                    history.Add("M" + minute + ": Check Adverse effect of buying " + robotNeeded.ToString() + " on " + previousNeed);

                                    //ok so calculate the ratios for the previous need (I know we already did this but I'm lazy)
                                    //ratio = (mineral cost - minerals we have) / (mineral robots) = how many turns until we'll be able to build
                                    List<double> ratios = new List<double>();
                                    for (int i = 0; i < 4; i++)
                                    {
                                        //account for dividing by zero (infinite turns)
                                        if (robots[i] == 0)
                                            ratios.Add(bluePrintCosts[(int)previousNeed, i] == 0 ? 0 : int.MaxValue);
                                        else
                                            ratios.Add(
                                                (double)(bluePrintCosts[(int)previousNeed, i] - minerals[i]) 
                                                / 
                                                (double)robots[i]);
                                    }

                                    //now do it again to see what it would be if we spent the money on this one
                                    //if the max is higher, then we choose to opt out
                                    //ratio = (mineral cost - (minerals we have - cost to build this robot)) / (mineral robots)
                                    //= how many turns until we'll be able to build if we build this robot
                                    List<double> ratiosIfWeBuy = new List<double>();
                                    for (int i = 0; i < 4; i++)
                                    {
                                        //account for dividing by zero (infinite turns)
                                        if (robots[i] == 0)
                                            ratiosIfWeBuy.Add(bluePrintCosts[(int)previousNeed, i] == 0 ? 0 : int.MaxValue);
                                        else
                                            ratiosIfWeBuy.Add(
                                                (double)(bluePrintCosts[(int)previousNeed, i] - (minerals[i] - bluePrintCosts[(int)robotNeeded, i])) 
                                                / 
                                                (double)robots[i]);
                                    }

                                    history.Add("M" + minute + ": Previous turns until buy = " + ratios.Max() + ", if purchase = " + ratiosIfWeBuy.Max());
                                    buyIt = (ratiosIfWeBuy.Max() <= ratios.Max());
                                }
                            }

                            if (buyIt)
                            {
                                history.Add("M" + minute + ": buying " + robotNeeded.ToString());

                                //deduct cost
                                for (int i = 0; i < 4; i++)
                                    minerals[i] -= bluePrintCosts[(int)robotNeeded, i];

                                //add robot
                                newRobotIndex = (int)robotNeeded;
                            }
                            else
                            {
                                history.Add("M" + minute + ": opted to not buy " + robotNeeded.ToString());
                            }


                            break;
                        }
                        else
                        {
                            //if not, see what we need to get er done

                            //unless this is ore, and then we're done
                            if (robotNeeded == R.ore) break;

                            //ratio = (mineral cost - minerals we have) / (mineral robots) = how many turns until we'll be able to build
                            List<double> ratios = new List<double>();
                            for (int i = 0; i < 4; i++)
                            {
                                //account for dividing by zero (infinite turns)
                                if (robots[i] == 0)                                    
                                    ratios.Add(bluePrintCosts[(int)robotNeeded, i] == 0 ? 0 : int.MaxValue);
                                else
                                    ratios.Add((double)(bluePrintCosts[(int)robotNeeded, i] - minerals[i]) / (double)robots[i]);
                            }

                            //which mineral do we need the most - put it on our stack and check if we can/should buy it 
                            var nextNeed = ratios.IndexOf(ratios.Max());
                            needs.Add((R)nextNeed);
                        }
                    }

                    //add the resources we collect this turn
                    for (int i = 0; i < 4; i++)
                    {
                        minerals[i] += robots[i];
                        history.Add("M" + minute + ": Generated " + robots[i] + " " + ((R)i).ToString() + " for " + minerals[i] + " total");
                    }

                    //add the robot if it is cooking
                    if (newRobotIndex > -1)
                        robots[newRobotIndex]++;
                }

                maxForEachBluePrint.Add(minerals[3]);
            }

            int quality = 0;
            for (int i = 0; i < maxForEachBluePrint.Count; i++)
            {
                quality += (i + 1) * maxForEachBluePrint[i];
            }

            Console.WriteLine("Part 1 - " + quality);

            Console.WriteLine("Part 2 - ");

            Console.ReadLine();
        }

        public static void Part1Part2Attempt1()
        {
            //parse
            List<int[,]> robotCosts = new List<int[,]>();
            var fileLines = System.IO.File.ReadAllLines("Day19.txt");
            foreach (var line in fileLines)
            {
                int[,] blueprint = new int[4,4];
                var numbers = Regex.Matches(line, "\\d+");
                blueprint[(int)R.ore, (int)R.ore] = int.Parse(numbers[1].Value); //cost of ore machine in ore
                blueprint[(int)R.clay, (int)R.ore] = int.Parse(numbers[2].Value); //cost of clay machine in ore
                blueprint[(int)R.obsidian, (int)R.ore] = int.Parse(numbers[3].Value); //cost of obsidian machine in ore
                blueprint[(int)R.obsidian, (int)R.clay] = int.Parse(numbers[4].Value); //cost of obsidian machine in clay
                blueprint[(int)R.geode, (int)R.ore] = int.Parse(numbers[5].Value); //cost of geode machine in ore
                blueprint[(int)R.geode, (int)R.obsidian] = int.Parse(numbers[6].Value); //cost of geode machine in obsidian
                robotCosts.Add(blueprint);
            }


            //ok now I guess we'll try just going down simulation and branching each time and seeing where we get?
            List<SimulationState> maxForEachBluePrint = new List<SimulationState>();

            foreach (var bluePrintCosts in robotCosts)
            {
                List<int> maxCosts = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    int maxCost = 0;
                    for (int j = 0; j < 4; j++)
                        if (maxCost < bluePrintCosts[j, i]) maxCost = bluePrintCosts[j, i];

                    maxCosts.Add(maxCost);
                }

                SimulationState s = new SimulationState();
                s.Robots[(int)R.ore] = 1;
                s.Minute = 0;
                SimulationState sReturn = DoSimulationReturnMaxGeodes(s, bluePrintCosts, maxCosts.ToArray());
                maxForEachBluePrint.Add(sReturn);
            }

            int quality = 0;
            for (int i = 0; i < maxForEachBluePrint.Count; i++)
            {
                quality += (i + 1) * maxForEachBluePrint[i].Resources[(int)R.geode];
            }

            Console.WriteLine("Part 1 - " + quality);

            Console.WriteLine("Part 2 - " );

            Console.ReadLine();
        }

        public static SimulationState DoSimulationReturnMaxGeodes(SimulationState s, int[,] robotCosts, int[] maxCosts)
        {
            if (s.Minute == 24)
                return s;        

            //get a list of robots we could afford at this point
            List<int> robotsWeCanAfford = new List<int>();
            for (int i = 3; i >= 0 ; i--)
            {
                //first optimization - don't build more than we can spend per minute
                if (s.Robots[i] == maxCosts[i] && i < 3) continue;

                bool canAffordThisRobot = true;
                for (int j = 0; j < 4; j++)
                {
                    if (robotCosts[i,j] > s.Resources[j])
                    {
                        canAffordThisRobot = false;
                        break;
                    }
                }

                if (canAffordThisRobot) robotsWeCanAfford.Add(i);
            }

            //add resources we get
            for (int i = 0; i < 4; i++)
                s.Resources[i] += s.Robots[i];

            s.Minute++;

            if (robotsWeCanAfford.Count == 0)
            {
                s.History.Add("Can't buy anything");
                return DoSimulationReturnMaxGeodes(s, robotCosts, maxCosts);
            }
            else if (robotsWeCanAfford.Contains((int)R.geode))
            {
                //always buy the geode robot               
                s.Robots[(int)R.geode]++;
                for (int i = 0; i < 4; i++)
                    s.Resources[i] -= robotCosts[(int)R.geode, i];

                s.History.Add("Bought geode robot");
                return DoSimulationReturnMaxGeodes(s, robotCosts, maxCosts);
            }
            //else if (robotsWeCanAfford.Contains((int)R.obsidian))
            //{
            //    //always buy the obsidian robot               
            //    s.Robots[(int)R.obsidian]++;
            //    for (int i = 0; i < 4; i++)
            //        s.Resources[i] -= robotCosts[(int)R.obsidian, i];

            //    s.History.Add("Bought obsidian robot");
            //    return DoSimulationReturnMaxGeodes(s, robotCosts, maxCosts);
            //}
            else
            {
                SimulationState max = null;

                //try doing nothing
                var newSim = s.Clone();
                newSim.History.Add("Chose to sit tight");
                var test = DoSimulationReturnMaxGeodes(newSim, robotCosts, maxCosts);
                if (max == null || max.Resources[(int)R.geode] < test.Resources[(int)R.geode])
                {
                    max = test;
                }

                //this is where we branch - we can do nothing or we can buy one of the robots we can afford
                foreach (var robot in robotsWeCanAfford)
                {
                    //buy the robot
                    newSim = s.Clone();
                    newSim.History.Add("Chose to buy " + ((R)robot).ToString());
                    newSim.Robots[robot]++;
                    for (int i = 0; i < 4; i++)
                        newSim.Resources[i] -= robotCosts[robot, i];

                    test = DoSimulationReturnMaxGeodes(newSim, robotCosts, maxCosts);
                    if (max == null || max.Resources[(int)R.geode] < test.Resources[(int)R.geode])
                    {
                        max = test;
                    }
                }

                return max;
            }
           
        }
    }
}