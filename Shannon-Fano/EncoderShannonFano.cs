using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Shannon_Fano
{
    public static class EncoderShannonFano
    {
        private static Dictionary<char, string> _resultTable; // Словарь символ-двоичная строка
        private static List<string> _codes; // Двоичные коды символов
        private static double _codingPrice; // Цена кодирования

        /// <summary>
        /// Возвращает таблицу с закодированными символами
        /// </summary>
        /// <returns></returns>
        public static Dictionary<char, string> GetResultTable()
        {
            return _resultTable;
        }

        /// <summary>
        /// Инициализирует лист двоичных кодов
        /// </summary>
        /// <param name="str"></param>
        private static void InitCodes(string str)
        {
            _codes = new List<string>();
            for (int i = 0; i < str.Length; i++)
            {
                _codes.Add("");
            }
        }

        /// <summary>
        /// Возвращает цену кодирования
        /// </summary>
        /// <returns></returns>
        public static double GetCodingPrice()
        {
            return _codingPrice;
        }

        /// <summary>
        /// Считает цену кодирования
        /// </summary>
        /// <param name="probabilities">Словарь символ-вероятность</param>
        private static void CalculateCodingPrice(Dictionary<char, double> probabilities)
        {
            _codingPrice = 0;
            foreach (var item in probabilities)
            {
                _codingPrice += item.Value * _resultTable[item.Key].Length;
            }
        }

        /// <summary>
        /// Считает вероятности символов
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static Dictionary<char, double> GetCharsProbability(string str)
        {
            Dictionary<char, double> result = new Dictionary<char, double>();

            foreach (var character in str)
            {
                if (char.IsWhiteSpace(character))
                {
                    if (!result.ContainsKey('&'))
                    {
                        result.Add('&', 1);
                    }
                    else
                    {
                        result['&']++;
                    }

                    continue;
                }

                if (result.ContainsKey(character))
                {
                    result[character]++;
                }
                else
                {
                    result.Add(character, 1);
                }
            }

            foreach (char key in result.Keys.ToArray())
            {
                result[key] /= str.Length;
            }

            return result;
        }

        /// <summary>
        /// Рекурсивно создает результативную таблицу
        /// </summary>
        /// <param name="p">Лист вероятностей символов</param>
        /// <param name="start">Индекс начала</param>
        /// <param name="end">Индекс конца</param>
        private static void RecursiveCreateTable(List<double> p, int start, int end)
        {
            // Если осталась 1 строка, то конец рекурсии
            if (Math.Abs(start - end) <= 1)
            {
                return;
            }

            // Считаем медиану
            int index = GetMedian(p, start, end);

            // Слева от медианы добавляем 1
            for (int i = start; i < index; i++)
            {
                _codes[i] += "1";
            }

            // Справа 0
            for (int i = index; i < end; i++)
            {
                _codes[i] += "0";
            }

            // Вызываем рекурсию от левого отрезка медианы и от правого
            RecursiveCreateTable(p, start, index);
            RecursiveCreateTable(p, index, end);
        }

        /// <summary>
        /// Считает медиану вероятностей, начиная с индекса start до индекса end
        /// </summary>
        /// <param name="map">Лист вероятностей символов</param>
        /// <param name="start">Индекс начала</param>
        /// <param name="end">Индекс конца</param>
        /// <returns></returns>
        private static int GetMedian(List<double> map, int start, int end)
        {
            double min = Int32.MaxValue, sumLeft = 0, sumRight = 0;
            int res = 0;

            // Проходим по каждому индексу
            for (int i = 1; i < end - start; i++)
            {
                // Берем вероятности слева и считаем сумму
                for (int j = start; j < start + i; j++)
                {
                    sumLeft += map[j];
                }

                // Берем вероятности справа и считаем сумму
                for (int j = start + i; j < end; j++)
                {
                    sumRight += map[j];
                }

                // Ищем минимум разницы между этими суммами
                if (min > Math.Abs(sumLeft - sumRight))
                {
                    min = Math.Abs(sumLeft - sumRight);
                    // Запоминаем индекс, при котором разность минимальна
                    res = start + i;
                }

                sumLeft = 0;
                sumRight = 0;
            }

            return res;
        }

        /// <summary>
        /// Записывает результат в файл
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encode"></param>
        private static void WriteResultToFile(string str, string encode)
        {
            string path = "decode/";
            string fileName = "test_" + str.Substring(0, 5) + ".txt";

            using (FileStream fstream = new FileStream(path + fileName, FileMode.OpenOrCreate))
            {
                byte[] array = Encoding.Default.GetBytes(encode + "\r\n");
                fstream.Write(array, 0, array.Length);
                string table = "";
                foreach (var item in _resultTable)
                {
                    table += item.Key + " " + item.Value + "\r\n";
                }

                table = table.Remove(table.Length - 2);
                array = Encoding.Default.GetBytes(table);
                fstream.Write(array, 0, array.Length);
            }

            Console.WriteLine("Результат сохранен в файл " + fileName);
        }

        /// <summary>
        /// Кодирует входную строку
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string Encode(string inputStr)
        {
            // Инициализируем таблицу с результатом
            _resultTable = new Dictionary<char, string>();
            InitCodes(inputStr);

            // Считаем словарь с вероятностями и сортируем по убыванию вероятностей
            Dictionary<char, double> probabilities = GetCharsProbability(inputStr);
            probabilities = probabilities.OrderByDescending(pair => pair.Value)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            // Рекурсивно создаем результативную таблицу
            RecursiveCreateTable(probabilities.Values.ToList(), 0, probabilities.Count);

            // Записываем коды символов в результат
            for (int i = 0; i < probabilities.Count; i++)
            {
                char character = probabilities.Keys.ToArray()[i];

                if (char.IsWhiteSpace(character) && !_resultTable.ContainsKey('&'))
                {
                    _resultTable.Add('&', _codes[i]);
                    continue;
                }

                if (!_resultTable.ContainsKey(character))
                {
                    _resultTable.Add(character, _codes[i]);
                }
            }

            // Считаем цену кодирования
            CalculateCodingPrice(probabilities);

            // С помощью получившегося словаря кодов кодирует входное сообщение
            string encode = "";
            foreach (var character in inputStr)
            {
                if (char.IsWhiteSpace(character))
                {
                    encode += _codes[probabilities.Keys.ToList().IndexOf('&')];
                    continue;
                }

                encode += _codes[probabilities.Keys.ToList().IndexOf(character)];
            }

            // Записываем результат в файл
            WriteResultToFile(inputStr, encode);
            return encode;
        }
    }
}