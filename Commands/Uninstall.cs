using MyAdb.Entity;

namespace MyAdb.Commands
{
    public class Uninstall : Command
    {
        public Uninstall(string device) : base(device)
        {
        }

        protected override string GetArguments(ExecParams p)
        {
            return string.Format("uninstall {0}", p.App.PackageName);
        }
    }
}
