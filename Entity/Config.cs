using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MyAdb.Entity
{
    public class Config
    {
        public string AdbPath;
        public string AppConfigPath;
        public bool BeepOnError;
        public bool PressAnyKeyAfterFailedRun;
        public bool AllDevicesByDefault;
        public bool MultiThreadingByDefault;

        private const string ADB_EXE = "adb.exe";

        private static Config _instance;
        private static readonly object _lockFlag = new object();

        [XmlIgnore]
        public List<AppConfig> Apps = new List<AppConfig>();

        [XmlIgnore]
        public string AdbFullPath
        {
            get { return Path.Combine(AdbPath, ADB_EXE); }
        }

        [XmlIgnore]
        public static Config Instance
        {
            get
            {
                lock (_lockFlag)
                {
                    if (_instance == null)
                    {
                        string fileName = Path.ChangeExtension(Utils.Location(), "xml");
                        _instance = (Config)Common.IO.Serialization.FileSerializer.XmlReadFromFile(fileName, typeof(Config));

                        if (_instance == null)
                            _instance = new Config();
                        else
                            _instance.LoadApps();
                    }
                }

                return _instance;
            }
        }

        private void LoadApps()
        {
            if (string.IsNullOrEmpty(AppConfigPath))
                return;

            string[] files = Directory.GetFiles(AppConfigPath, "*.xml");

            foreach (string fname in files)
            {
                AppConfig ac = (AppConfig)Common.IO.Serialization.FileSerializer.XmlReadFromFile(fname, typeof(AppConfig));

                if (ac == null)
                    CW.Error("Illegal format. Drop xml: {0}", Path.GetFileName(fname));
                else
                {
                    ac.FileName = Path.GetFileName(fname);
                    Apps.Add(ac);
                }
            }
        }
    }
}