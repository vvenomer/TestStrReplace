using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace GenerateData
{
    class Program
    {
        static void Main(string[] args)
        {
            DataGenerator dataGenerator = new DataGenerator();
            var result = dataGenerator.GenerateAllData();
            SaveData(result.subject, result.replace);
            int i = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();

            foreach (var (r1, r2) in result.replace)
            {
                if(result.subject.Replace(r1, r2) != result.subject)
                {
                    i++;
                }
            }
            stopwatch.Stop();
            Console.WriteLine(Stopwatch.Frequency/1000 + " per ms");
            Console.WriteLine(stopwatch.ElapsedMilliseconds + " " + nameof(stopwatch.ElapsedMilliseconds));
            Console.WriteLine(i + " times replaced of " + result.replace.Count + " times");
        }

        static void SaveData(string subject, Dictionary<string, string> data)
        {
            var str = subject + "\n" + data
                .Select(entry => entry.Key + "," + entry.Value)
                .Aggregate((a, b) => a + "\n" + b);

            File.WriteAllText("C:/Users/pblajer/source/repos/JSiZ/GenerateData/data.txt", str, Encoding.UTF8);
        }
    }
}
