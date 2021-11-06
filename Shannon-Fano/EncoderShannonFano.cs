using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Shannon_Fano
{
    public static class EncoderShannonFano
    {
        private static readonly Dictionary<char, string> ResultTable = new Dictionary<char, string>();
        private static readonly List<string> Codes = new List<string>();

        public static Dictionary<char, string> GetResultTable()
        {
            return ResultTable;
        }

        private static void InitResultTable(string str)
        {
            foreach (var character in str)
            {
                if (ResultTable.ContainsKey(character)) continue;
                ResultTable.Add(character, "");
            }
        }

        private static void InitCodes(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                Codes.Add("");

            }
        }
        
        private static Dictionary<char, double> GetCharsProbability(string str)
        {
            Dictionary<char, double> result = new Dictionary<char, double>();

            foreach (var character in str)
            {
                if (result.ContainsKey(character))
                {
                    result[character]++;
                    continue;
                }

                result.Add(character, 1);
            }

            foreach (char key in result.Keys.ToArray())
            {
                result[key] /= str.Length;
            }

            return result;
        }
        
        private static void RecursiveCreateTable(List<double> p, int start, int end)
        {
            if (Math.Abs(start - end) <= 1)
            {
                return;
            }

            int index = GetMedian(p, start, end);

            for (int i = start; i < index; i++)
            {
                Codes[i] += "1";
            }

            for (int i = index; i < end; i++)
            {
                Codes[i] += "0";
            }

            RecursiveCreateTable(p, start, index);
            RecursiveCreateTable(p, index, end);
        }

        private static int GetMedian(List<double> map, int start, int end)
        {
            double min = Int32.MaxValue, sumLeft = 0, sumRight = 0;
            int res = 0;

            for (int i = 1; i < end - start; i++)
            {
                for (int j = start; j < start + i; j++)
                {
                    sumLeft += map[j];
                }

                for (int j = start + i; j < end; j++)
                {
                    sumRight += map[j];
                }

                if (min > Math.Abs(sumLeft - sumRight))
                {
                    min = Math.Abs(sumLeft - sumRight);
                    res = start + i;
                }

                sumLeft = 0;
                sumRight = 0;
            }

            return res;
        }

        private static void WriteResultToFile(string str, string encode)
        {
            FileStream stream = new FileStream("decode/test_" + str.Substring(0, 5) + ".txt", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(encode);
            foreach (var item in ResultTable)
            {
                writer.WriteLine(item.Key + " " + item.Value);
            }
            stream.Close();
            
            Console.WriteLine("Результат сохранен в файл test_" + str.Substring(0, 5) + ".txt");
        }

        public static string Encode(string inputStr)
        {
            InitResultTable(inputStr);
            InitCodes(inputStr);
            
            Dictionary<char, double> probabilities = GetCharsProbability(inputStr);
            probabilities = probabilities.OrderByDescending(pair => pair.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            RecursiveCreateTable(probabilities.Values.ToList(), 0, probabilities.Count);

            for (int i = 0; i < probabilities.Count; i++)
            {
                ResultTable[probabilities.Keys.ToArray()[i]] = Codes[i];
            }

            string encode = "";
            foreach (var character in inputStr)
            {
                encode += Codes[probabilities.Keys.ToList().IndexOf(character)];
            }

            WriteResultToFile(inputStr, encode);
            return encode;
        }
    }
}