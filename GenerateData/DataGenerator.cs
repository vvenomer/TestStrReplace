using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateData
{
    class DataGenerator
    {
        int lines = 1_000;
        int lineMaxLength = 00_100_000;
        int replaceMaxLength = 5;
        string allChars;
        Encoding encoding = Encoding.UTF8;

        Random random = new Random();

        public DataGenerator()
        {
            string unicodeCharSetFile = "unicode.txt";
            if (!File.Exists(unicodeCharSetFile))
            {
                allChars = AllChars();
                File.WriteAllText(unicodeCharSetFile, allChars, encoding);
            }
            else
            {
                allChars = File.ReadAllText(unicodeCharSetFile, encoding);
            }
        }
        char RandomChar()
        {
            return allChars[random.Next(0, allChars.Length)];
        }

        int ReplaceLength()
        {
            return random.Next(1, replaceMaxLength);
        }

        string RandomString(int length)
        {
            char[] tab = new char[length];
            for (int i = 0; i < length; i++)
            {
                tab[i] = RandomChar();
            }
            return new string(tab);
        }

        (string replace1, string replace2) GenerateLine()
        {
            string replace1 = RandomString(ReplaceLength());
            string replace2;
            do
            {
                replace2 = RandomString(ReplaceLength());
            } while (replace2.Contains(replace1));

            return (replace1, replace2);
        }

        public (string subject, Dictionary<string, string> replace) GenerateAllData()
        {
            var replace = new Dictionary<string, string>();

            string subject = RandomString(lineMaxLength);
            for (int i = 0; i < lines; i++)
            {
                var line = GenerateLine();
                if (replace.ContainsKey(line.replace1))
                {
                    i--;
                    continue;
                }
                replace[line.replace1] = line.replace2;
            }
            return (subject, replace);
        }

        string AllChars()
        {
            var results = new ConcurrentBag<int>();
            var min = 0;
            var max = 0x10ffff;
            var procs = 8;
            Parallel.For(0, procs, set =>
            {
                var c = encoding.GetEncoder();
                var start = min + set * ((max - min) / procs);
                var end = start + ((max - min) / procs);

                char[] input = new char[1];
                byte[] output = new byte[5];
                for (int i = start; i < end; i++)
                {
                    input[0] = (char)i;
                    int success = c.GetBytes(input, 0, 1, output, 0, true);
                    if (success != 0)
                        results.Add(i);
                }
            });
            var hashSet = new HashSet<int>(results);
            hashSet.Remove('\r');
            hashSet.Remove('\n');
            hashSet.Remove(',');
            var sorted = hashSet.ToArray();
            return new string(sorted.Select(i => (char)i).ToArray());
        }
    }
}
