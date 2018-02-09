using MyAdb.Entity;

namespace MyAdb.Commands
{
    public class Kill: Command
    {
        public Kill(string device) : base(device)
        {
        }

        protected override string GetArguments(ExecParams p)
        {
            return string.Format("shell \"su -c 'killall {0}'\"", p.App.PackageName);
        }
    }
}