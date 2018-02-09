using System;
using System.Diagnostics;
using System.Text;

namespace MyAdb.Helper
{
    public class ErrorLogger
    {
        public string FormatErrorMessage(Process process, Exception exception)
        {
            const string padding = "   ";
            ProcessStartInfo psi = process.StartInfo;

            StringBuilder sb = new StringBuilder();
            sb.Append(padding).Append("Process.FileName: ").Append(psi.FileName).AppendLine();
            sb.Append(padding).Append("Process.Arguments: ").Append(psi.Arguments).AppendLine();
            sb.Append(padding).Append("Exception: ").Append(exception).AppendLine();

            return sb.ToString();
        }
    }
}