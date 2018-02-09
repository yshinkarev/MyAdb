using MyAdb.Entity;

namespace MyAdb.Commands
{
    public class Install : Command
    {
        public Install(string device) : base(device)
        {
        }

        protected override string GetArguments(ExecParams p)
        {
            return string.Format("install -r {0}", p.Apk);
        }
    }
}