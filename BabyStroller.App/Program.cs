using BabyStroller.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    //当插件开发商，不小心将Voice写成了voice，那么就会过滤掉方法，
                    //为了避免这类错误，主体商就要开发一个SDK给到第三方使用，思路：
                    //就是新建一个SDK类库，里面定义一个IAnimal接口，同时主体商和第三方都要引入
                    //这个类库SDK文件。UnfinishedAttribute:Attribute这个是为了第三方未完成时
                    //可以用[Unfinished]确保代码还能正常运行
                    if (t.GetInterfaces().Contains(typeof(IAnimal)))
                    {
                        var isUnfinished = t.GetCustomAttributes(false).Any(a => a.GetType() == typeof(UnfinishedAttribute));
                        if (isUnfinished) continue;
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
                var a = o as IAnimal;
                a.Voice(times);
            }
        }
    }
}
