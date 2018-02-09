using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyAdb.Entity;

namespace MyAdb.Helper
{
    public class CmdLineParser
    {
        public ExecParams Parse(string[] args)
        {
            //            if (args != null)
            //                foreach (string s in args)
            //                    Console.WriteLine(s);

            List<string> cmdArgs = args.ToList();
            cmdArgs.RemoveAll(String.IsNullOrEmpty);
            List<string> loCmdArgs = cmdArgs.Select(s => s.ToLower()).ToList();

            if (cmdArgs.Count == 0)
                return Help();

            CommandType cmd = ParseCommand(cmdArgs[0]);

            if (NotAvailCommand(cmd))
                return Unknown();

            if (cmd == CommandType.Help)
                return Help();

            if (cmd == CommandType.Devices)
                return new ExecParams(CommandType.Devices);

            ExecParams result = new ExecParams(cmd);
            result.App = ParseApp(loCmdArgs);
            result.Aliases = ParseAliases(loCmdArgs, cmdArgs);
            result.MultiThreading = Config.Instance.MultiThreadingByDefault || (FindParam(Utils.MULTI_THREAD, cmdArgs) != null);

            if (cmd == CommandType.Install)
                result.Apk = ParseApk(loCmdArgs, cmdArgs);

            return result;
        }

        private string ParseApk(List<string> loCmdArgs, List<string> cmdArgs)
        {
            string apk = loCmdArgs.Find(s => s.EndsWith(Utils.APK));

            if (apk == null)
                return null;

            return cmdArgs[loCmdArgs.IndexOf(apk)];
        }

        private bool NotAvailCommand(CommandType cmd)
        {
            return cmd == CommandType.Unknown || !((CommandType[])Enum.GetValues(typeof(CommandType))).ToList().Contains(cmd);
        }

        private List<string> ParseAliases(List<string> loCmdArgs, List<string> cmdArgs)
        {
            string alias = loCmdArgs.Find(s => s.StartsWith(Utils.ALIAS));

            List<string> result = new List<string>();

            if (alias == null)
                return result;

            alias = cmdArgs[loCmdArgs.IndexOf(alias)].Substring(Utils.ALIAS.Length);

            return string.IsNullOrEmpty(alias) ? result : alias.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        private AppConfig ParseApp(List<string> loCmdArgs)
        {
            string app = FindParam(Utils.APP, loCmdArgs);

            if (app == null)
                return null;

            int index = Config.Instance.Apps.FindIndex(ap => ap.Alias.ToLower() == app);
            return Config.Instance.Apps[index];
        }

        private ExecParams Unknown()
        {
            return new ExecParams(CommandType.Unknown);
        }

        private ExecParams Help()
        {
            return new ExecParams(CommandType.Help);
        }

        private CommandType ParseCommand(string s)
        {
            if (string.IsNullOrEmpty(s))
                return CommandType.Unknown;

            if (s.Length < 2 || s[0] != '[' || s[s.Length - 1] != ']')
                return CommandType.Unknown;

            StringBuilder sb = new StringBuilder(s.ToLower().Substring(1, s.Length - 2));
            sb[0] = char.ToUpper(sb[0]);
            s = sb.ToString();

            try
            {
                return (CommandType)Enum.Parse(typeof(CommandType), s);
            }
            catch (Exception)
            {
                return CommandType.Unknown;
            }
        }

        private string FindParam(string name, List<string> loCmdArgs)
        {
            string app = loCmdArgs.Find(s => s.StartsWith(name));
            return app == null ? null : app.Substring(Utils.APP.Length);
        }
    }
}