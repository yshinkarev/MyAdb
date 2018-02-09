using System.Collections.Generic;
using System.Diagnostics;
using MyAdb.Entity;

namespace MyAdb.Commands
{
    public class Devices : Command
    {
        private readonly List<Device> _devices = new List<Device>();

        public Devices() : base(null)
        {
        }

        public List<Device> GetList()
        {
            if (Execute() != OK)
            {
                LogBuffer();
                return null;
            }

            return _devices;
        }

        public void Log()
        {
            int count = (_devices == null) ? 0 : _devices.Count;
            CW.Info("Devices: {0}", count);

            if (count == 0)
                return;

            foreach (Device device in _devices)
                CW.Adb(device.ToString());
        }

        protected override void OnOutputDataReceived(string line)
        {
            Device device = new Device(line);

            if (device.IsEmpty())
                return;

            _devices.Add(device);
        }

        protected override string GetArguments(ExecParams app)
        {
            return "devices -l";
        }

        protected override void LogCommandLine(ProcessStartInfo psi)
        {
            
        }
    }
}