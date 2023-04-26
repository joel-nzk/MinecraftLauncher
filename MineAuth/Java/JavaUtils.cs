using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineAuth.Java
{
    public static class JavaUtils
    {
        public static bool IsJavaInstalled()
        {
            //TODO: Add java detection support for Linux and OSX
            try
            {
                Process process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.Arguments = "/c \"" + "java -version " + "\"";

                process.Start();
                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch{}


            return false;
        }

        public static void InstallJava(string requiredVersion)
        {
            //TODO: Install the correct version of java
        }

        public static string GetJavaPath()
        {
            if (!IsJavaInstalled())
                return "";

            //TODO: Need to use a a command (for example 'where') instead of a path
            return @"C:\Program Files\Java\jre1.8.0_351\bin\java.exe";
        }


    }
}
