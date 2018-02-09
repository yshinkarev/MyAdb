using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using MyAdb.Entity;

namespace MyAdb.Commands
{
    public class Help : Command
    {
        public Help() : base(null)
        {
        }

        public new void Execute()
        {
            // Header.
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(Utils.Location());
            string name = info.ProductName;
            CW.Info("{0} {1}", name, Regex.Match(info.FileVersion, @"^\d+.\d+").Value);

            CW.Info("{0} [COMMAND] [OPTIONS]", name);
            CW.Line();

            // Commands.
            CW.Info("Available commands:");

            foreach (string cmd in GetSupportedCommands())
                CW.Info("   {0}", cmd);

            // Options.
            CW.Line();
            CW.Info("Available options:");

            WriteOption(Utils.APP, "STRING", "Set associated application (xml)");
            WriteOption(Utils.ALIAS, "LIST", "Device names (may be substring) separated comma or " + Utils.ALL);

            WriteOption(Utils.MULTI_THREAD, "", "Using several threads, if operation processed on several connected devices");


            // Applications.
            CW.Line();
            List<AppConfig> apps = Config.Instance.Apps;
            CW.Info("Associated applications: {0}", apps.Count);

            foreach (AppConfig ac in apps)
            {
                CW.Line();
                CW.Info(ac.ToString());
            }
        }

        protected override string GetArguments(ExecParams app)
        {
            throw new NotImplementedException();
        }

        private static string[] GetSupportedCommands()
        {
            List<CommandType> values = ((CommandType[])Enum.GetValues(typeof(CommandType))).ToList();
            values.Remove(CommandType.Unknown);

            return values.Select(s => "[" + s.ToString().ToLower() + "]").ToArray();
        }

        private void WriteOption(string option, string type, string description)
        {
            CW.Info("   " + option + type + new String(' ', 20 - option.Length - type.Length) + description);
        }
    }
}