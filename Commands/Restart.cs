using MyAdb.Entity;

namespace MyAdb.Commands
{
    public class Restart : Command
    {
        public Restart(string device) : base(device)
        {
        }

        public override int Execute(ExecParams prms)
        {
            Command kill = Create(CommandType.Kill, null);

            int result = kill.Execute(prms);
            int result2 = base.Execute(prms);

            if (result != OK)
                return result;

            return result2;
        }

        protected override string GetArguments(ExecParams p)
        {
            // adb shell am start -n com.flipdog.penguins.turbocharged/com.flipdog.penguins.turbocharged.PenguinFlyersMainActivity
            return string.Format("shell am start -n {0}/{1}", p.App.PackageName, p.App.StartActivity);
        }
    }
}