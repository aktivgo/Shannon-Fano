using System;

namespace Shannon_Fano
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Введите строку для кодирования: ");
            string inputStr = Console.ReadLine();
            CoderShannonFano.Encode(inputStr);
        }
    }
}