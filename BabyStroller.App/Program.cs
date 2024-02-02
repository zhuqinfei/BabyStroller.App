using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Loader;

namespace BabyStroller.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var folder = Path.Combine(Environment.CurrentDirectory, "Animals");
            var files = Directory.GetFiles(folder);
            var animalTypes = new List<Type>();
            foreach (var file in files)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                var types = assembly.GetTypes();
                foreach (var t in types)
                {
                    //这种情况当插件开发商，不小心将Voice写成了voice，那么就会过滤掉方法，
                    //为了避免插件开发商避免这类错误，主体商就要开发一个SDK给到第三方
                    if (t.GetMethod("Voice") != null)
                    {
                        animalTypes.Add(t);
                    }
                }
            }

            while (true)
            {
                for (int i = 0; i < animalTypes .Count; i++)
                {
                    Console.WriteLine($"{i + 1}.{animalTypes[i].Name}");
                }
                Console.WriteLine("=================");
                Console.WriteLine("Please choose animal:");
                int index = int.Parse(Console.ReadLine());
                if (index>animalTypes.Count || index<1)
                {
                    Console.WriteLine("No such an animal.Try again!");
                    continue;
                }

                Console.WriteLine("How many times?");
                int times = int.Parse(Console.ReadLine());
                var t = animalTypes[index - 1];
                var m = t.GetMethod("Voice");
                var o = Activator.CreateInstance(t);
                m.Invoke(o, new object[] { times });
            }
        }
    }
}
