using System.IO;
using MyAdb.Entity;

namespace MyAdb.Helper
{
    public class ConfigXmlFileCreator
    {
        public void Execute()
        {
            Config c = Config.Instance;
            c.AdbPath = @"c:\Soft\android-sdk\platform-tools";
            c.AppConfigPath = @".\Apps";
            c.BeepOnError = true;

            string fileName = Path.ChangeExtension(Utils.Location(), "xml");
            bool result = Common.IO.Serialization.FileSerializer.XmlWriteToFile(fileName, c);

            if (!result)
                CW.Error(Common.IO.Serialization.FileSerializer.LastException.ToString());

            Directory.CreateDirectory(c.AppConfigPath);
            AppConfig ac = new AppConfig("RiverRaid", "com.flipdog.riverraid", "com.unity3d.player.UnityPlayerActivity");

            result = Common.IO.Serialization.FileSerializer.XmlWriteToFile(Path.Combine(c.AppConfigPath, "RiverRaid.xml"), ac);

            if (!result)
                CW.Error(Common.IO.Serialization.FileSerializer.LastException.ToString());
        }
    }
}
