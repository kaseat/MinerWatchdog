using System;

namespace WDG.Extensions
{
    /// <summary>
    /// Represents extended console output API.
    /// </summary>
    public static class Output
    {
        public static void Write(String value) => Console.Write(value);

        public static void Write(String value, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(value);
            Console.ResetColor();
        }

        public static void WriteLine(String value) => Console.WriteLine(value);

        public static void WriteLine() => Console.WriteLine();

        public static void WriteLine(String value, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ResetColor();
        }
    }
}