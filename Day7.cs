using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
    internal class Day7
    {
        class File
        {
            public string FileName { get; set; }
            public int Size { get; set; }
        }

        class Dir
        {
            public Dir()
            {
                Files = new List<File>();
                Dirs = new List<Dir>();
            }

            public Dir Parent { get; set; }
            public string Name { get; set; }
            public List<File> Files { get; set; }
            public List<Dir> Dirs { get; set; }

            public int Size()
            {
                return Files.Sum(a => a.Size) + Dirs.Sum(a => a.Size());
            }
        }

        public static void Day7Part1()
        {
            var fileLines = System.IO.File.ReadAllLines("Day7.txt");
                        
            Dir root = new Dir();
            var curDir = root;
                        
            foreach (var line in fileLines)
            {
                if (line == "$ cd /")
                {
                    curDir = root;
                }
                else if (line.StartsWith("$ cd"))
                {
                    //change cur dir
                    if (line == "$ cd ..")
                    {
                        curDir = curDir.Parent;
                    }
                    else
                    {
                        curDir = curDir.Dirs.Find(one => one.Name == line.Substring(5));                        
                    }
                }

                if (!line.StartsWith("$"))
                {
                    if (line.StartsWith("dir"))
                    {
                        curDir.Dirs.Add(new Dir() { Parent = curDir, Name = line.Substring(4) });
                    }
                    else
                    {
                        string[] split = line.Split(' ');
                        curDir.Files.Add(new File() { FileName = split[1], Size = int.Parse(split[0]) });
                    }
                }
            }

            var list = FindDirsOfSizeLessThan100000(root);
                        
            Console.WriteLine("Part 1 - " + list.Sum(a => a.Size()));

            int freeNow = 70000000 - root.Size();
            int needToDelete = 30000000 - freeNow;

            var list2 = EnumerateDirs(root);
            var answer = list2.FindAll(one => one.Size() >= needToDelete).Min(one => one.Size());

            Console.WriteLine("Part 2 - " + answer);
            Console.ReadLine();
        }

        static List<Dir> FindDirsOfSizeLessThan100000(Dir curDir)
        {
            List<Dir> list = new List<Dir>();
            if (curDir.Size() <= 100000) list.Add(curDir);            

            foreach (var dir in curDir.Dirs)
            {
                list.AddRange(FindDirsOfSizeLessThan100000(dir));
            }

            return list;
        }

        static List<Dir> EnumerateDirs(Dir curDir)
        {
            List<Dir> list = new List<Dir>();
            list.Add(curDir);
            
            foreach (var dir in curDir.Dirs)
            {
                list.AddRange(EnumerateDirs(dir));
            }

            return list;
        }
    }
}
