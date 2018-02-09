using System;
using System.Collections.Generic;
using System.Linq;
using MyAdb.Entity;

namespace MyAdb
{
    public static class Utils
    {
        public const string ALL = "[all]";
        public const string APP = "-app=";
        public const string ALIAS = "-alias=";
        public const string APK = ".apk";
        public const string MULTI_THREAD = "-multithread";

        public static string Location()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().Location;
        }

        public static void BeepIfCan()
        {
            if (Config.Instance.BeepOnError)
                Console.Beep();
        }

        public static List<string> DistinctList(string str)
        {
            return str.Split(' ').Distinct().ToList();
        }
    }
}