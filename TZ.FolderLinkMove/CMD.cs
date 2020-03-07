using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TZ.FolderLinkMove
{
    public class CMD
    {

        public static string Execute(string command,
            Action<object ,DataReceivedEventArgs > errorDataReceivedAction,
            Action<object ,DataReceivedEventArgs> outputDataReceivedAction
            )
        {
            string response = "";
            Process process = new Process();
            process.ErrorDataReceived += (sender, e) =>
            {
                errorDataReceivedAction(sender, e);
            };
            process.OutputDataReceived += (sender, e) =>
            {
                outputDataReceivedAction(sender, e);
            };

            StreamWriter streamWriter;
            StreamReader streamReader;
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd");

            //初始化进程信息
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.CreateNoWindow = true;
            process.StartInfo = processStartInfo;

            //执行进程
            process.Start();
            streamWriter = process.StandardInput;
            streamReader = process.StandardOutput;
            streamWriter.AutoFlush = true;
            streamWriter.WriteLine(command);
            streamWriter.Close();
            response = streamReader.ReadToEnd();
            streamWriter.Close();
            streamWriter.Dispose();
            streamReader.Close();
            streamReader.Dispose();
            process.Close();
            process.Dispose();
            return response;
        }
    }
}
