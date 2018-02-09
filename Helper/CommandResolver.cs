using System.Collections.Generic;
using System.Linq;
using MyAdb.Commands;
using MyAdb.Entity;

namespace MyAdb.Helper
{
    public class CommandResolver
    {
        public bool Execute(ExecParams p)
        {
            CommandType type = p.Cmd;

            if (type == CommandType.Help)
            {
                Help();
                return true;
            }

            if (type == CommandType.Unknown)
            {
                Unknown(p);
                return false;
            }

            if (type == CommandType.Devices)
            {
                return Devices(false /* isSubCommand */) != null;
            }

            // Main commands.

            if (p.App == null && type != CommandType.Install)
            {
                CW.Error("<Not set or unknown application>");
                return false;
            }

            List<Device> devices = Devices(true /* isSubCommand */);

            if (devices == null || devices.Count == 0)
            {
                CW.Error("<Have no any active device>");
                return false;
            }

            if (devices.Count > 1 && p.Aliases.Count == 0 && !Config.Instance.AllDevicesByDefault)
            {
                CW.Error(
                    "<More than one device connected. Try use {0} as alias or set option AllDevicesByDefault to TRUE at xml file>",
                    Utils.ALL);
                return false;
            }

            List<Device> targetDevices = GetTargetDeviceNames(devices, p.Aliases);

            if (targetDevices.Count == 0)
            {
                CW.Error("<Have no target devices (filtered by alias)>");
                return false;
            }

            if (type == CommandType.Install && string.IsNullOrEmpty(p.Apk))
            {
                CW.Error("<Missing APK file>");
                return false;
            }

            bool success = true;
            bool multiThreading = (targetDevices.Count > 1) && p.MultiThreading;

            if (multiThreading)
                MultiThreadObject.Instance.Start(targetDevices.Count);

            foreach (Device device in targetDevices)
            {
                CW.Info("Device: {0}...", device.Model);
                string deviceName = multiThreading ? device.Model : null;

                Command cmd = Command.Create(type, deviceName);
                p.DeviceId = device.Id;

                int result = cmd.Execute(p);

                if (result == Command.OK)
                    continue;

                Utils.BeepIfCan();
                success = false;
            }

            if (multiThreading)
            {
                success = (MultiThreadObject.Instance.Wait() == Command.OK);

                if (!success)
                    Utils.BeepIfCan();
            }

            return success;
        }

        private List<Device> GetTargetDeviceNames(List<Device> devices, List<string> aliases)
        {
            List<string> loAliases = aliases.Select(a => a.ToLower()).ToList();

            if (loAliases.Contains(Utils.ALL) || (Config.Instance.AllDevicesByDefault && loAliases.Count == 0))
                return new List<Device>(devices);

            List<string> availDevicesLo = devices.Select(d => d.Model.ToLower()).ToList();

            List<Device> result = new List<Device>();

            for (int i = 0; i < aliases.Count; i++)
            {
                string alias = loAliases[i];

                int devIndex = availDevicesLo.FindIndex(name => name.Contains(alias));

                if (devIndex == -1)
                    CW.Hint("<Device {0} offline>", aliases[i]);
                else
                    result.Add(devices[devIndex]);
            }

            return result;
        }

        private List<Device> Devices(bool isSubCommand)
        {
            Devices devCmd = new Devices();
            List<Device> devices = devCmd.GetList();

            if (devices == null)
                return null;

            if (!isSubCommand)
                devCmd.Log();


            return devices;
        }

        private void Unknown(ExecParams p)
        {
            CW.Error("<Unknown command>");
            p.Cmd = CommandType.Help;
            Execute(p);
        }

        private void Help()
        {
            new Help().Execute();
        }
    }
}
