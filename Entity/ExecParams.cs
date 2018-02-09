using System.Collections.Generic;

namespace MyAdb.Entity
{
    public class ExecParams
    {
        public string DeviceId;

        public CommandType Cmd;
        public List<string> Aliases;
        public AppConfig App;
        public string Apk;
        public bool MultiThreading;

        public ExecParams(CommandType cmd)
        {
            Cmd = cmd;
        }

        public override string ToString()
        {
            return string.Format("Command: {0}. App: {1}. Apk: {2}. Aliases: ({3}). MultiThreading: {4}",
                Cmd, App, Apk, string.Join(", ", Aliases.ToArray()), MultiThreading);
        }
    }
}