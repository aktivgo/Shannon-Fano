using System;
using System.Collections.Generic;
using System.Linq;

namespace Shannon_Fano
{
    public static class CoderShannonFano
    {
        private static Dictionary<char, string> codeDict = new Dictionary<char, string>();

        private static Dictionary<char, double> getProbabilities(string str)
        {
            Dictionary<char, double> result = new Dictionary<char, double>();

            for (int i = 0; i < str.Length; i++)
            {
                if (result.ContainsKey(str[i]))
                {
                    result[str[i]]++;
                    continue;
                }

                result.Add(str[i], 1);
                codeDict.Add(str[i], "");
            }

            foreach (char key in result.Keys.ToArray())
            {
                result[key] /= str.Length;
            }

            return result;
        }

        private static int GetMedian(List<double> values, int startIndex, int endIndex)
        {
            double sumLeft = 0;
            double sumRight = 0;
            double min = 1;

            int resIndex = 0;

            for (int i = startIndex; i < endIndex; i++)
            {
                for (int j = 0; j <= startIndex; j++)
                {
                    sumLeft += values[j];
                }

                for (int j = startIndex + 1; j <= endIndex; j++)
                {
                    sumRight += values[j];
                }

                if (Math.Abs(sumLeft - sumRight) < min)
                {
                    min = Math.Abs(sumLeft - sumRight);
                    sumLeft = 0;
                    sumRight = 0;
                    continue;
                }

                return i;
            }

            return resIndex;
        }

        public static string Encode(string inputStr)
        {
            Dictionary<char, double> probabilities = getProbabilities(inputStr);
            foreach (var item in probabilities)
            {
                Console.WriteLine(item.Key + " " + item.Value);
            }

            Console.WriteLine();

            foreach (var item in probabilities.OrderByDescending(item => item.Value))
            {
                Console.WriteLine(item.Key + " " + item.Value);
            }

            Console.WriteLine();

            List<string> codeShanonFano = new List<string>();

            int median = GetMedian(probabilities.Values.ToList(), 0, probabilities.Count - 1);
            Console.WriteLine(median);

            for (int i = 0; i < probabilities.Count; i++)
            {
                if (i <= median)
                {
                    codeShanonFano.Add("0");
                    continue;
                }

                codeShanonFano.Add("1");
            }

            Console.WriteLine();

            foreach (var item in codeShanonFano)
            {
                Console.WriteLine(item + " ");
            }


            return "";
        }
    }
}