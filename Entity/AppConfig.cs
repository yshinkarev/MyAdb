using System.Xml.Serialization;

namespace MyAdb.Entity
{
    public class AppConfig
    {
        public string Alias;
        public string PackageName;
        public string StartActivity;

        [XmlIgnore]
        public string FileName;

        public AppConfig()
        {

        }

        public AppConfig(string alias, string packageName, string startActivity)
        {
            Alias = alias;
            PackageName = packageName;
            StartActivity = startActivity;
        }

        public override string ToString()
        {
            return string.Format("Alias: {0}.\nPackageName: {1}.\nStartActivity: {2}.\nFilename: {3}", Alias, PackageName, StartActivity, FileName);
        }
    }
}