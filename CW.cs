using System;
using System.Collections.Generic;
using MyAdb.Entity;

namespace MyAdb
{
    public static class CW
    {
        private static readonly Dictionary<ConsoleType, ConsoleColor> _colors = new Dictionary<ConsoleType, ConsoleColor>();
        private static ConsoleColor _color;
        private static readonly object _lockFlag = new object();

        static CW()
        {
            _colors.Add(ConsoleType.Normal, Console.ForegroundColor);
            _colors.Add(ConsoleType.Adb, ConsoleColor.DarkGray);
            _colors.Add(ConsoleType.Error, ConsoleColor.Red);
            _colors.Add(ConsoleType.Hint, ConsoleColor.DarkYellow);
        }

        public static void Reset()
        {
            _colors.TryGetValue(ConsoleType.Normal, out _color);
            Console.ForegroundColor = _color;
        }

        public static void Line()
        {
            Log(ConsoleType.Normal, "");
        }

        public static void Hint(String format, params Object[] args)
        {
            Log(ConsoleType.Hint, format, args);
        }

        public static void Info(String format, params Object[] args)
        {
            Log(ConsoleType.Normal, format, args);
        }

        public static void Adb(String format, params Object[] args)
        {
            Log(ConsoleType.Adb, format, args);
        }

        public static void Error(String format, params Object[] args)
        {
            Log(ConsoleType.Error, format, args);
        }

        private static void Log(ConsoleType type, String format, params Object[] args)
        {
            lock (_lockFlag)
            {
                _colors.TryGetValue(type, out _color);
                Console.ForegroundColor = _color;

                if (args == null || args.Length == 0)
                    Console.WriteLine(format);
                else
                    Console.WriteLine(format, args);
            }
        }
    }
}