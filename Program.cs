using System;
using MyAdb.Entity;
using MyAdb.Helper;

namespace MyAdb
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //            new ConfigXmlFileCreator().Execute();

                ExecParams p = new CmdLineParser().Parse(args);
                bool success = new CommandResolver().Execute(p);

                if (!success)
                {
                    Utils.BeepIfCan();

                    if (Config.Instance.PressAnyKeyAfterFailedRun)
                        Console.ReadKey();
                }

#if DEBUG
                Console.WriteLine("Debug pause before exit...");
                Console.ReadKey();
#endif
            }
            finally
            {
                CW.Reset();
            }
        }
    }
}