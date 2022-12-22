using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day22
    {
        public static void Part1()
        {
            //parse            
            var fileText = System.IO.File.ReadAllText("Day22.txt").Replace("\r", "").Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            var map = fileText[0].Split('\n');
            var instructions = fileText[1];

            //pad the map so they're all the same length
            int maxLength = map.Max(one => one.Length);
            for (int i = 0; i < map.Length; i++)
                if (map[i].Length < maxLength) map[i] = map[i] + new string(' ', maxLength - map[i].Length);

            //coordinates are row, col = y, x
            (int, int) pos = (0, map[0].IndexOf("."));
            (int, int) direction = (0, 1);

            //precalc all the mins and maxes to make it easy
            List<(int, int)> minMaxForEachRow = new List<(int, int)>();
            List<(int, int)> minMaxForEachCol = new List<(int, int)>();

            for (int i = 0; i < map.Length; i++)
                minMaxForEachRow.Add((int.MaxValue, int.MinValue));

            for (int i = 0; i < map[0].Length; i++)
                minMaxForEachCol.Add((int.MaxValue, int.MinValue));

            for (int row = 0; row < map.Length; row++)
            {
                for (int col = 0; col < map[0].Length; col++)
                {
                    if (map[row][col] != ' ')
                    {
                        if (minMaxForEachRow[row].Item1 > col) minMaxForEachRow[row] = (col, minMaxForEachRow[row].Item2);
                        if (minMaxForEachRow[row].Item2 < col) minMaxForEachRow[row] = (minMaxForEachRow[row].Item1, col);

                        if (minMaxForEachCol[col].Item1 > row) minMaxForEachCol[col] = (row, minMaxForEachCol[col].Item2);
                        if (minMaxForEachCol[col].Item2 < row) minMaxForEachCol[col] = (minMaxForEachCol[col].Item1, row);
                    }
                }
            }

            List<(int, int)> directions = new List<(int, int)>(new (int, int)[]
            {
                (0, 1),
                (1, 0),
                (0, -1),
                (-1, 0)
            });

            while (!string.IsNullOrEmpty(instructions))
            {
                //get how many to move forward
                int RLIndex = instructions.IndexOfAny(new char[] { 'R', 'L' });
                int forward = RLIndex > 0 ? int.Parse(instructions.Substring(0, RLIndex)) : int.Parse(instructions);
                string LR = RLIndex > 0 ? instructions.Substring(instructions.IndexOfAny(new char[] { 'R', 'L' }), 1) : "";
                instructions = RLIndex > 0 ? instructions.Substring(instructions.IndexOfAny(new char[] { 'R', 'L' }) + 1): "";
                //move forward until we hit a wall
                for (int i = 0; i < forward; i++)
                {
                    (int, int) next = (pos.Item1 + direction.Item1, pos.Item2 + direction.Item2);

                    //see if we went off the right
                    if (next.Item2 > minMaxForEachRow[pos.Item1].Item2)
                        next.Item2 = minMaxForEachRow[pos.Item1].Item1;

                    //off the left
                    if (next.Item2 < minMaxForEachRow[pos.Item1].Item1)
                        next.Item2 = minMaxForEachRow[pos.Item1].Item2;

                    //off the top
                    if (next.Item1 < minMaxForEachCol[pos.Item2].Item1)
                        next.Item1 = minMaxForEachCol[pos.Item2].Item2;

                    //off the bottom
                    if (next.Item1 > minMaxForEachCol[pos.Item2].Item2)
                        next.Item1 = minMaxForEachCol[pos.Item2].Item1;


                    char testNext = map[next.Item1][next.Item2];

                    if (testNext == '#')
                        break;

                    pos = next;
                }

                //turn
                if (LR == "L")                
                { 
                    int newDirIndex = directions.IndexOf(direction) - 1;
                    if (newDirIndex < 0) newDirIndex = directions.Count - 1;
                    direction = directions[newDirIndex];
                }

                if (LR == "R")
                {
                    int newDirIndex = directions.IndexOf(direction) + 1;
                    if (newDirIndex == directions.Count) newDirIndex = 0;
                    direction = directions[newDirIndex];
                }
            }

            int answer = (1000 * (pos.Item1 + 1)) + (4 * (pos.Item2 + 1)) + directions.IndexOf(direction);
            Console.WriteLine("Part 1 - " + answer);
        }

        public enum Dir
        {
            East = 0,
            South = 1,
            West = 2,
            North = 3
        }
        
        public static void Part2()
        {
            //parse            
            var fileText = System.IO.File.ReadAllText("Day22.txt").Replace("\r", "").Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            var map = fileText[0].Split('\n');
            var instructions = fileText[1];

            //pad the map so they're all the same length
            int maxLength = map.Max(one => one.Length);
            for (int i = 0; i < map.Length; i++)
                if (map[i].Length < maxLength) map[i] = map[i] + new string(' ', maxLength - map[i].Length);

            //get cube size
            int cubeSize = Math.Abs(maxLength - map.Length);

            //directions index == point value
            //add 1 = clockwise (R)
            //sub 1 = counterclockwise (L)
            List<(int, int)> directions = new List<(int, int)>(new (int, int)[]
            {
                (0, 1),
                (1, 0),
                (0, -1),
                (-1, 0)
            });

            //now get all the faces
            List<string[]> faces = new List<string[]>();
            List<(int, int)> faceStartingPoint = new List<(int, int)>();
            for (int row = 0; row < map.Length; row += cubeSize)
            {
                for (int col = 0; col < maxLength; col += cubeSize)
                {
                    if (map[row][col] == ' ') continue;
                    List<string> thisFace = new List<string>();
                    //(row, col) is the starting point
                    for (int i = 0; i < cubeSize; i++)                    
                        thisFace.Add(map[row + i].Substring(col, cubeSize));

                    faces.Add(thisFace.ToArray());
                    faceStartingPoint.Add((row, col));
                }
            }

            //figure out the direction translation matrix for
            //each cube combo
            List<(int, int)> faceQuadrants = faceStartingPoint.Select(one => (one.Item1 / cubeSize, one.Item2 / cubeSize)).ToList();

            //so faceDirections[0][Dir.North] would be (face to the north, Direction north translates to on that face)
            var faceDirections = new List<Dictionary<Dir, (int, Dir)>>();

            //figure out each face connections - what we can observe without traversing
            for (int i = 0; i < faceQuadrants.Count; i++)
            {
                Dictionary<Dir, (int, Dir)> thisDictionary = new Dictionary<Dir, (int, Dir)>();
                faceDirections.Add(thisDictionary);

                var thisFace = faceQuadrants[i];

                //straight
                for (int dir = 0; dir < directions.Count; dir++)
                {
                    var thatFace = (thisFace.Item1 + directions[dir].Item1, thisFace.Item2 + directions[dir].Item2);
                    var thatFaceIndex = faceQuadrants.IndexOf(thatFace);
                    if (thatFaceIndex > -1)
                    {
                        //we have one we are connected to in the same direction
                        thisDictionary.Add((Dir)dir, (thatFaceIndex, (Dir)dir));
                    }
                }

                //now check diagonals
                for (int dir1 = 0; dir1 < directions.Count; dir1++)
                {
                    for (int dir2 = 0; dir2 < directions.Count; dir2++)
                    {
                        if (dir1 == dir2) continue;

                        var thatFace1 = (thisFace.Item1 + directions[dir1].Item1, thisFace.Item2 + directions[dir1].Item2);
                        var thatFace1Index = faceQuadrants.IndexOf(thatFace1);

                        var thatFace2 = (thatFace1.Item1 + directions[dir2].Item1, thatFace1.Item2 + directions[dir2].Item2);
                        var thatFace2Index = faceQuadrants.IndexOf(thatFace2);

                        if (thatFace2 == thisFace) continue;

                        if (thatFace1Index == -1 && thatFace2Index != -1)
                        {
                            //so we went blank -> something which means a 90 degree turn
                            //example, south (dir1) and west (dir2)
                            //would mean that we are adjacent to thatFace2 but going west (dir2)
                            thisDictionary.Add((Dir)dir1, (thatFace2Index, (Dir)dir2));
                        }                        
                    }
                }

                //now check 1-over-2-down L shapes
                for (int dir1 = 0; dir1 < directions.Count; dir1++)
                {
                    for (int dir2 = 0; dir2 < directions.Count; dir2++)
                    {
                        if (dir1 == dir2) continue;

                        var thatFace1 = (thisFace.Item1 + directions[dir1].Item1, thisFace.Item2 + directions[dir1].Item2);
                        var thatFace1Index = faceQuadrants.IndexOf(thatFace1);

                        var thatFace2 = (thatFace1.Item1 + directions[dir2].Item1, thatFace1.Item2 + directions[dir2].Item2);
                        var thatFace2Index = faceQuadrants.IndexOf(thatFace2);

                        var thatFace3 = (thatFace2.Item1 + directions[dir2].Item1, thatFace2.Item2 + directions[dir2].Item2);
                        var thatFace3Index = faceQuadrants.IndexOf(thatFace3);

                        if (thatFace1 == thisFace || thatFace2 == thisFace || thatFace3 == thisFace) continue;

                        if (thatFace1Index != -1 && thatFace2Index != -1 && thatFace3Index != -1)
                        {
                            var oppDir1 = ((int)dir1 + 2);
                            if (oppDir1 > 3) oppDir1 -= 4;

                            //so we went in a connected L shape which means                            
                            //example west-south-south means I go east and end up west on target
                            thisDictionary.Add((Dir)oppDir1, (thatFace3Index, (Dir)dir1));
                        }
                    }
                }

                //now check 2-over-1-down L shapes
                for (int dir1 = 0; dir1 < directions.Count; dir1++)
                {
                    for (int dir2 = 0; dir2 < directions.Count; dir2++)
                    {
                        if (dir1 == dir2) continue;

                        var thatFace1 = (thisFace.Item1 + directions[dir1].Item1, thisFace.Item2 + directions[dir1].Item2);
                        var thatFace1Index = faceQuadrants.IndexOf(thatFace1);

                        var thatFace2 = (thatFace1.Item1 + directions[dir1].Item1, thatFace1.Item2 + directions[dir1].Item2);
                        var thatFace2Index = faceQuadrants.IndexOf(thatFace2);

                        var thatFace3 = (thatFace2.Item1 + directions[dir2].Item1, thatFace2.Item2 + directions[dir2].Item2);
                        var thatFace3Index = faceQuadrants.IndexOf(thatFace3);

                        if (thatFace1 == thisFace || thatFace2 == thisFace || thatFace3 == thisFace || thatFace3 == thatFace1) continue;

                        if (thatFace1Index != -1 && thatFace2Index != -1 && thatFace3Index != -1)
                        {
                            var oppDir2 = ((int)dir2 + 2);
                            if (oppDir2 > 3) oppDir2 -= 4;

                            //so we went in a connected L shape which means                            
                            //example south-south-west means I go west and end up east on target
                            thisDictionary.Add((Dir)dir2, (thatFace3Index, (Dir)oppDir2));
                        }
                    }
                }
            }

            //now link the ends by traversing - should be 24 chains - starting from everywhere going everywhere
            //and some of them will be complete (e.g. 4 in length) so we can then link the circles
            //and then they should all be complete
            List<List<(int, Dir)>> chains = new List<List<(int, Dir)>>();

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    chains.Add(Traverse(i, (Dir)j, faceDirections));
                }
            }

            foreach (var chain in chains)
            {
                var firstEntry = chain.First();
                var lastEntry = chain.Last();

                if (chain.Count == 4)
                {
                    //connect the last to the first
                    if (!faceDirections[lastEntry.Item1].ContainsKey(lastEntry.Item2))
                        faceDirections[lastEntry.Item1].Add(lastEntry.Item2, firstEntry);

                    //connect the first to the last
                    var oppositeFirstDir = ((int)firstEntry.Item2 + 2);
                    if (oppositeFirstDir > 3) oppositeFirstDir -= 4;

                    var oppositeLastDir = ((int)lastEntry.Item2 + 2);
                    if (oppositeLastDir > 3) oppositeLastDir -= 4;

                    if (!faceDirections[firstEntry.Item1].ContainsKey((Dir)oppositeFirstDir))
                        faceDirections[firstEntry.Item1].Add((Dir)oppositeFirstDir, (lastEntry.Item1, (Dir)oppositeLastDir));
                }
                else
                {
                    int a = 1;
                }
            }

            //just check to make sure
            chains.Clear();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    chains.Add(Traverse(i, (Dir)j, faceDirections));
                }
            }

            if (chains.Min(one => one.Count) < 4)
                throw new Exception("Didn't fully enumerate the cube directions");                        

            //coordinates are row, col = y, x
            (int, int) pos = (0, 0);
            (int, int) direction = (0, 1);
            int curFaceIndex = 0;

            while (!string.IsNullOrEmpty(instructions))
            {
                //get how many to move forward
                int RLIndex = instructions.IndexOfAny(new char[] { 'R', 'L' });
                int forward = RLIndex > 0 ? int.Parse(instructions.Substring(0, RLIndex)) : int.Parse(instructions);
                string LR = RLIndex > 0 ? instructions.Substring(instructions.IndexOfAny(new char[] { 'R', 'L' }), 1) : "";
                instructions = RLIndex > 0 ? instructions.Substring(instructions.IndexOfAny(new char[] { 'R', 'L' }) + 1) : "";

                //move forward until we hit a wall
                for (int i = 0; i < forward; i++)
                {
                    (int, int) next = (pos.Item1 + direction.Item1, pos.Item2 + direction.Item2);
                    (int, int) nextDirection = direction;
                    int nextFaceIndex = curFaceIndex;

                    (int, int) posQuadrant = (pos.Item1 / cubeSize, pos.Item2 / cubeSize);
                    (int, int) nextQuadrant = (next.Item1 / cubeSize, next.Item2 / cubeSize);
                    if (next.Item1 < 0) nextQuadrant.Item1--;
                    if (next.Item2 < 0) nextQuadrant.Item2--;

                    if (posQuadrant != nextQuadrant)
                    {
                        //translate

                        //figure out next direction and face
                        Dir thisDir = (Dir)directions.IndexOf(direction);
                        var nextFaceAndDirection = faceDirections[curFaceIndex][thisDir];
                        nextFaceIndex = nextFaceAndDirection.Item1;
                        nextDirection = directions[(int)nextFaceAndDirection.Item2];
                        Dir nextDir = nextFaceAndDirection.Item2;

                        if (thisDir == Dir.North && nextDir == Dir.East)
                        {
                            //(0, y) -> (y, 0)
                            next = (pos.Item2, 0);
                        }
                        else if (thisDir == Dir.East && nextDir == Dir.South)
                        {
                            //(x, max) => (0, max - x)
                            next = (0, cubeSize - 1 - pos.Item1);
                        }
                        else if (thisDir == Dir.South && nextDir == Dir.West)
                        {
                            //(max, y) => (y, max)
                            next = (pos.Item2, cubeSize - 1);
                        }
                        else if (thisDir == Dir.West && nextDir == Dir.North)
                        {
                            //(x, 0) -> (max, max-x)
                            next = (cubeSize - 1, cubeSize - 1 - pos.Item2);
                        }                        
                        else if ((thisDir == Dir.North && nextDir == Dir.South) ||
                                (thisDir == Dir.South && nextDir == Dir.North))
                        {
                            //(max, y) => (max, max - y)
                            next = (pos.Item1, cubeSize - pos.Item2 - 1);
                        }
                        else if ((thisDir == Dir.East && nextDir == Dir.West) ||
                                (thisDir == Dir.West && nextDir == Dir.East))
                        {
                            //(x, max) => (max - x, max)
                            next = (cubeSize - pos.Item1 - 1, pos.Item2);
                        }
                        else if (thisDir == Dir.North && nextDir == Dir.East)
                        {
                            //(0, y) -> (y, 0)
                            next = (pos.Item2, 0);
                        }
                        else if (thisDir == Dir.East && nextDir == Dir.North)
                        {
                            //(x, max) => (max, x)
                            next = (cubeSize - 1, pos.Item1);
                        }
                        else if (thisDir == Dir.South && nextDir == Dir.East)
                        {
                            //(max, y) => (max - y, 0)
                            next = (cubeSize - 1 - pos.Item2, 0);
                        }
                        else if (thisDir == Dir.West && nextDir == Dir.South)
                        {
                            //(x, 0) => (0, x)
                            next = (0, pos.Item1);
                        }
                        else if (thisDir == Dir.North && nextDir == Dir.North)
                        {
                            //(0, y) => (max, y)
                            next = (cubeSize - 1, pos.Item2);
                        }
                        else if (thisDir == Dir.South && nextDir == Dir.South)
                        {
                            //(max, y) => (0, y)
                            next = (0, pos.Item2);
                        }
                        else if (thisDir == Dir.West && nextDir == Dir.West)
                        {
                            //(x, 0) => (x, max)
                            next = (pos.Item1, cubeSize - 1);
                        }
                        else if (thisDir == Dir.East && nextDir == Dir.East)
                        {
                            //(x, max) => (x, 0)
                            next = (pos.Item1, 0);
                        }
                    }

                    char testNext = faces[nextFaceIndex][next.Item1][next.Item2];

                    if (testNext == '#')
                        break;

                    pos = next;
                    direction = nextDirection;
                    curFaceIndex = nextFaceIndex;
                }

                //turn
                if (LR == "L")
                {
                    int newDirIndex = directions.IndexOf(direction) - 1;
                    if (newDirIndex < 0) newDirIndex = directions.Count - 1;
                    direction = directions[newDirIndex];
                }

                if (LR == "R")
                {
                    int newDirIndex = directions.IndexOf(direction) + 1;
                    if (newDirIndex == directions.Count) newDirIndex = 0;
                    direction = directions[newDirIndex];
                }
            }

            //translate to absolute
            (int, int) absolutePos = (faceStartingPoint[curFaceIndex].Item1 + pos.Item1, faceStartingPoint[curFaceIndex].Item2 + pos.Item2);
            int answer = (1000 * (absolutePos.Item1 + 1)) + (4 * (absolutePos.Item2 + 1)) + directions.IndexOf(direction);
            Console.WriteLine("Part 2 - " + answer);

            Console.ReadLine();
        }

        public static List<(int, Dir)> Traverse(int startingFaceIndex, Dir direction, List<Dictionary<Dir, (int, Dir)>> faceDirections, int firstParent = -1)
        {
            if (startingFaceIndex == firstParent) return new List<(int, Dir)>();

            List<(int, Dir)> ret = new List<(int, Dir)>();            
            ret.Add((startingFaceIndex, direction));

            if (firstParent == -1) firstParent = startingFaceIndex;

            if (faceDirections[startingFaceIndex].ContainsKey(direction))
            {
                var destination = faceDirections[startingFaceIndex][direction];                
                ret.AddRange(Traverse(destination.Item1, destination.Item2, faceDirections, firstParent));
            }

            return ret;
        }

        public static void Part2Attemp1WorksForSampleButNotMineCry()
        {
            //parse            
            var fileText = System.IO.File.ReadAllText("Day22.txt").Replace("\r", "").Split(new string[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            var map = fileText[0].Split('\n');
            var instructions = fileText[1];

            //pad the map so they're all the same length
            int maxLength = map.Max(one => one.Length);
            for (int i = 0; i < map.Length; i++)
                if (map[i].Length < maxLength) map[i] = map[i] + new string(' ', maxLength - map[i].Length);

            int cubeSize = maxLength / 4;

            //coordinates are row, col = y, x
            (int, int) pos = (0, map[0].IndexOf("."));
            (int, int) direction = (0, 1);            

            List<(int, int)> directions = new List<(int, int)>(new (int, int)[]
            {
                (0, 1),
                (1, 0),
                (0, -1),
                (-1, 0)
            });

            //the three loops on the faces
            //2 right => 3 right => 4 right => 6 down => 2 right
            //1 down => 4 down => 5 down => 2 up => 1 down
            //6 left => 5 left => 3 up => 1 right => 6 left

            List<(int, int)> quadrants = new List<(int, int)>(new (int, int)[]
            {
                (0, 0), //no 0 quadrant
                (0, 2),
                (1, 0),
                (1, 1),
                (1, 2),
                (2, 2),
                (2, 3)
            });

            while (!string.IsNullOrEmpty(instructions))
            {
                //get how many to move forward
                int RLIndex = instructions.IndexOfAny(new char[] { 'R', 'L' });
                int forward = RLIndex > 0 ? int.Parse(instructions.Substring(0, RLIndex)) : int.Parse(instructions);
                string LR = RLIndex > 0 ? instructions.Substring(instructions.IndexOfAny(new char[] { 'R', 'L' }), 1) : "";
                instructions = RLIndex > 0 ? instructions.Substring(instructions.IndexOfAny(new char[] { 'R', 'L' }) + 1) : "";
                //move forward until we hit a wall
                for (int i = 0; i < forward; i++)
                {
                    (int, int) next = (pos.Item1 + direction.Item1, pos.Item2 + direction.Item2);
                    (int, int) posQuadrant = (pos.Item1 / cubeSize, pos.Item2 / cubeSize);
                    (int, int) nextQuadrant = (next.Item1 / cubeSize, next.Item2 / cubeSize);
                    (int, int) posQuadrantStart = (posQuadrant.Item1 * cubeSize, posQuadrant.Item2 * cubeSize);

                    (int, int) newDirection = direction;

                    if (!quadrants.Contains(nextQuadrant))
                    {
                        //translate the blank quadrants
                        if (nextQuadrant == (1, 3) && direction == (0, 1))
                        {
                            //this is right of 4 or up of 6
                            //(5, 11) going (0, 1) becomes (8, 14) going (1, 0)                          

                            //right of 4 - target 6 going down
                            newDirection = (direction.Item2, direction.Item1);
                            //get relative coordinates, switch and go negative                            
                            var targetQuadrant = quadrants[6];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (cubeSize - 1 - posRelative.Item2, cubeSize - 1 - posRelative.Item1);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                        else if (nextQuadrant == (1, 3) && direction == (-1, 0))
                        {
                            //up of 6 - target 4 going down
                            //(8, 14) going (1, 0) becomes (5, 12) going (0, 1)
                            //8-8=0, 14-12=2 (0, 2)                            
                            //5-4=1, 12-9=3 (1, 3)  or (3-2, 3-0)

                            newDirection = (direction.Item2, direction.Item1);
                            //get relative coordinates, switch and go negative                            
                            var targetQuadrant = quadrants[4];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (cubeSize - 1 - posRelative.Item2, cubeSize - 1 - posRelative.Item1);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                        else if (nextQuadrant == (0, 3))
                        {
                            //only way this happens is 1 going right
                            //target 6 going left
                            //relative (0, 3) going (0, 1) becomes relative (3, 3) going (0, -1)
                            //which is just (3 - 0, 3) so keep the col, reverse the row
                            newDirection = (0 - direction.Item1, 0 - direction.Item2);
                            var targetQuadrant = quadrants[6];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (cubeSize - 1 - posRelative.Item1, posRelative.Item2);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                        else if (nextQuadrant == (0, 0))
                        {
                            //only way this happens is 2 going up
                            //target 1 going down
                            //relative (0, 1) going (-1, 0) becomes relative (0, 2) going (1, 0)
                            //which is just (0, 3-1) so keep the row, reverse the col
                            newDirection = (0 - direction.Item1, 0 - direction.Item2);
                            var targetQuadrant = quadrants[1];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (posRelative.Item1, cubeSize - 1 - posRelative.Item2);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                        else if (nextQuadrant == (2, 0))
                        {
                            //only way this happens is 2 going down
                            //target 5 going up
                            //relative (3, 1) going (1, 0) becomes relative (3, 2) going (-1, 0)
                            //which is just (3, 3-1) so keep the row, reverse the col
                            newDirection = (0 - direction.Item1, 0 - direction.Item2);
                            var targetQuadrant = quadrants[5];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (posRelative.Item1, cubeSize - 1 - posRelative.Item2);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                        else if (nextQuadrant == (0, 1) && direction == (-1, 0))
                        {
                            //up of 3 - target 1 going right (-1, 0) becomes (0, 1) for direction
                            //relative (0, 1) becomes (1, 0) - so just switch

                            newDirection = (0 - direction.Item2, 0 - direction.Item1);
                            //get relative coordinates, switch and go negative                            
                            var targetQuadrant = quadrants[1];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (posRelative.Item2, posRelative.Item1);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                        else if (nextQuadrant == (0, 1) && direction == (0, -1))
                        {
                            //left of 1 - target 3 going down (0, -1) becomes (1, 0) for direction
                            //relative (1, 0) becomes (0, 1) - so just switch

                            newDirection = (0 - direction.Item2, 0 - direction.Item1);
                            //get relative coordinates, switch and go negative                            
                            var targetQuadrant = quadrants[3];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (posRelative.Item2, posRelative.Item1);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                        else if (nextQuadrant == (2, 1) && direction == (1, 0))
                        {
                            //down of 3 - target 5 going right (1, 0) becomes (0, 1) for direction
                            //relative (3, 1) becomes (0, 2) - so just reverse

                            newDirection = (direction.Item2, direction.Item1);
                            //get relative coordinates, switch and go negative                            
                            var targetQuadrant = quadrants[5];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (cubeSize - 1 - posRelative.Item1, cubeSize - 1 - posRelative.Item2);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                        else if (nextQuadrant == (2, 3))
                        {
                            //down of 6 - target 2 going right (1, 0) becomes (0, 1) for direction
                            //relative (3, 1) becomes (0, 2) - so just reverse

                            newDirection = (direction.Item2, direction.Item1);
                            //get relative coordinates, switch and go negative                            
                            var targetQuadrant = quadrants[2];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (cubeSize - 1 - posRelative.Item1, cubeSize - 1 - posRelative.Item2);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                        else if (nextQuadrant == (1, -1))
                        {
                            //left of 2 - target 6 going up (0, 1) becomes (1, 0) for direction
                            //relative (0, 2) becomes (3, 1) - so just reverse

                            newDirection = (direction.Item2, direction.Item1);
                            //get relative coordinates, switch and go negative                            
                            var targetQuadrant = quadrants[6];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (cubeSize - 1 - posRelative.Item1, cubeSize - 1 - posRelative.Item2);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                        else if (nextQuadrant == (-1, 2))
                        {
                            //up of 1 - target 5 going up no change in direction
                            //relative (0, 2) becomes (3, 2) - so just reverse the row
                                                        
                            //get relative coordinates, switch and go negative                            
                            var targetQuadrant = quadrants[5];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (cubeSize - 1 - posRelative.Item1, posRelative.Item2);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                        else if (nextQuadrant == (3, 2))
                        {
                            //down of 5 - target 2 going up (1, 0) becomes (-1, 0)
                            //relative (3, 2) becomes (3, 1) - so just reverse the col

                            //get relative coordinates, switch and go negative                            
                            newDirection = (0 - direction.Item1, 0 - direction.Item2);
                            var targetQuadrant = quadrants[2];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (posRelative.Item1, cubeSize - 1 - posRelative.Item2);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                        else if (nextQuadrant == (2, 4))
                        {
                            //right of 6 - target 1 going left (0, 1) becomes (0, -1)
                            //relative (2, 3) becomes (1, 3) - so just reverse the row

                            //get relative coordinates, switch and go negative                            
                            newDirection = (0 - direction.Item1, 0 - direction.Item2);
                            var targetQuadrant = quadrants[2];
                            var targetQuadrantStart = (targetQuadrant.Item1 * cubeSize, targetQuadrant.Item2 * cubeSize);
                            var posRelative = (pos.Item1 - posQuadrantStart.Item1, pos.Item2 - posQuadrantStart.Item2);
                            var nextRelative = (cubeSize - 1 - posRelative.Item1, posRelative.Item2);
                            next = (targetQuadrantStart.Item1 + nextRelative.Item1, targetQuadrantStart.Item2 + nextRelative.Item2);
                        }
                    }

                    char testNext = map[next.Item1][next.Item2];

                    if (testNext == '#')
                        break;

                    pos = next;
                    direction = newDirection;
                }

                //turn
                if (LR == "L")
                {
                    int newDirIndex = directions.IndexOf(direction) - 1;
                    if (newDirIndex < 0) newDirIndex = directions.Count - 1;
                    direction = directions[newDirIndex];
                }

                if (LR == "R")
                {
                    int newDirIndex = directions.IndexOf(direction) + 1;
                    if (newDirIndex == directions.Count) newDirIndex = 0;
                    direction = directions[newDirIndex];
                }
            }

            int answer = (1000 * (pos.Item1 + 1)) + (4 * (pos.Item2 + 1)) + directions.IndexOf(direction);
            Console.WriteLine("Part 2 - " + answer);

            Console.ReadLine();
        }
    }
}
