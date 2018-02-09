using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MyAdb.Entity;
using MyAdb.Helper;

namespace MyAdb.Commands
{
    public abstract class Command
    {
        public const int OK = 0;

        private readonly List<string> _buffer = new List<string>();
        private List<string> _lastInBufferSplitted;

        protected readonly string Device;

        private Process _p;

        public Command(string device)
        {
            Device = device;
        }

        public int Execute()
        {
            return Execute(null);
        }

        public static Command Create(CommandType type, string deviceName)
        {
            switch (type)
            {
                case CommandType.Help:
                    return new Help();
                case CommandType.Kill:
                    return new Kill(deviceName);
                case CommandType.Install:
                    return new Install(deviceName);
                case CommandType.Uninstall:
                    return new Uninstall(deviceName);
                case CommandType.Restart:
                    return new Restart(deviceName);
                default:
                    throw new Exception(type.ToString());
            }
        }

        public virtual int Execute(ExecParams prms)
        {
            _p = new Process();

            try
            {
                bool multiThread = (Device != null);

                _p.StartInfo = new ProcessStartInfo
                {
                    FileName = Config.Instance.AdbFullPath,
                    Arguments = BuildArguments(prms),
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                LogCommandLine(_p.StartInfo);

                _p.OutputDataReceived += OnOutputDataReceivedHalder;
                _p.EnableRaisingEvents = true;

                if (multiThread)
                {
                    _p.Exited += (o, a) => MultiThreadObject.Instance.CommandFinish(CloseProcess());
                    StartProcess();
                    return OK;
                }

                using (ManualResetEvent mre = new ManualResetEvent(false))
                {
                    _p.Exited += (o, a) => mre.Set();

                    StartProcess();
                    mre.WaitOne();
                    return CloseProcess();
                }
            }
            catch (Exception ex)
            {
                string msg = new ErrorLogger().FormatErrorMessage(_p, ex);
                Error(msg);
                _p.Dispose();
                return -1;
            }
        }

        protected virtual void LogCommandLine(ProcessStartInfo psi)
        {
            Adb("{0} {1}", Path.GetFileName(psi.FileName), psi.Arguments);
        }

        protected abstract string GetArguments(ExecParams app);

        protected void LogBuffer()
        {
            foreach (string s in _buffer)
                Adb(s);
        }

        protected virtual void OnOutputDataReceived(string line)
        {
            Adb(line);
        }

        protected void Error(string format, params Object[] args)
        {
            CW.Error(DeviceName() + format, args);
        }

        protected void Adb(string format, params Object[] args)
        {
            CW.Adb(DeviceName() + format, args);
        }

        // Private.

        private string DeviceName()
        {
            return (Device == null) ? "" : "[" + Device + "] ";
        }

        private void OnOutputDataReceivedHalder(object sendingProcess, DataReceivedEventArgs outLine)
        {
            string text = outLine.Data;

            if (string.IsNullOrEmpty(text))
                return;

            _buffer.Add(text);

            // On progress (install apk).
            if (_lastInBufferSplitted == null)
                _lastInBufferSplitted = Utils.DistinctList(text);
            else
            {
                List<string> currSplitted = string.IsNullOrEmpty(text) ? new List<string>() : Utils.DistinctList(text);
                List<string> diff = _lastInBufferSplitted.Count > currSplitted.Count ?
                    _lastInBufferSplitted.Except(currSplitted).ToList() :
                    currSplitted.Except(_lastInBufferSplitted).ToList();

                int len = diff.Where(s => !string.IsNullOrEmpty(s)).Sum(s => s.Length);

                if (len < 5)
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
            }

            OnOutputDataReceived(text);
        }

        private string BuildArguments(ExecParams p)
        {
            StringBuilder sb = new StringBuilder();

            if (p != null)
                sb.Append("-s ").Append(p.DeviceId).Append(" ");

            sb.Append(GetArguments(p));
            return sb.ToString();
        }

        private void StartProcess()
        {
            _p.Start();
            _p.BeginOutputReadLine();
        }

        private int CloseProcess()
        {
            _p.WaitForExit();
            int code = _p.ExitCode;
            _p.Dispose();
            _p = null;
            return code;
        }
    }
}