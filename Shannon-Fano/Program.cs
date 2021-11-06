using System;
using System.Collections.Generic;
using System.IO;

namespace Shannon_Fano
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            int ch = -1;
            while (ch != 0)
            {
                Console.WriteLine("1. Закодировать сообщение");
                Console.WriteLine("2. Раскодировать сообщение");
                Console.WriteLine("0. Выход\n");
                Console.Write("Выберите пункт меню: ");
                ch = int.Parse(Console.ReadLine());
                switch (ch)
                {
                    case 0:
                        return;
                    case 1:
                    {
                        Console.Write("Введите название файла: ");
                        string fileName = Console.ReadLine();
                        if (!File.Exists("encode/" + fileName))
                        {
                            Console.WriteLine("Попробуйте ещё раз\n");
                            break;
                        }
                        Console.WriteLine();
                        
                        FileStream stream = new FileStream("encode/" + fileName, FileMode.Open);
                        StreamReader reader = new StreamReader(stream);
                        string textFromFile = reader.ReadToEnd();
                        stream.Close();
                        
                        Console.WriteLine("Входные данные:\n" + textFromFile + "\n");
                        Console.WriteLine("Результат:");
                        Console.WriteLine(EncoderShannonFano.Encode(textFromFile));
                        Dictionary<char, string> result = EncoderShannonFano.GetResultTable();
                        foreach (var item in result)
                        {
                            Console.WriteLine(item.Key + " " + item.Value);
                        }
                        Console.WriteLine();
                    } break;
                    case 2:
                    {
                        Console.Write("Введите название файла: ");
                        string fileName = Console.ReadLine();
                        if (!File.Exists("decode/" + fileName))
                        {
                            Console.WriteLine("Попробуйте ещё раз\n");
                            break;
                        }
                        Console.WriteLine();

                        FileStream stream = new FileStream("decode/" + fileName, FileMode.Open);
                        StreamReader reader = new StreamReader(stream);
                        string textFromFile = reader.ReadToEnd();
                        stream.Close();
                        Console.WriteLine("Входные данные:\n" + textFromFile + "\n");
                        char[] separators = { '\n', '\r' };
                        string[] textAr = textFromFile.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        Dictionary<string, char> table = new Dictionary<string, char>();
                        for (int i = 1; i < textAr.Length; i++)
                        {
                            string code = textAr[i].Substring(textAr[i].IndexOf(" ", StringComparison.Ordinal) + 1);
                            char character = textAr[i][0];
                            table.Add(code, character);
                        }
                        Console.WriteLine("Результат:");
                        Console.WriteLine(DecoderShannonFano.Decode(textAr[0], table));
                        Console.WriteLine();
                    } break;
                    default:
                        Console.WriteLine("Попробуйте ещё раз\n");
                        break;
                }
            }
        }
    }
}