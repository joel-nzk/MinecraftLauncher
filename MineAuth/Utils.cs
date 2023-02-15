using MineAuth.Launcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MineAuth
{
    public enum Platform
    {
        windows,
        linux,
        osx
    }

    public static class Utils
    {
        static MD5 md5 = MD5.Create();


        public static string getHash(string input)
        {
            byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static async Task<string> GetTextFileContent(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    return await sr.ReadToEndAsync();
                }
            }

            return "";
        }

        public static Platform GetUserPlatform()
        {
            //source => https://stackoverflow.com/questions/10138040/how-to-detect-properly-windows-linux-mac-operating-systems
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    // Well, there are chances MacOSX is reported as Unix instead of MacOSX.
                    // Instead of platform check, we'll do a feature checks (Mac specific root folders)
                    if (Directory.Exists("/Applications")
                        & Directory.Exists("/System")
                        & Directory.Exists("/Users")
                        & Directory.Exists("/Volumes"))
                        return Platform.osx;
                    else
                        return Platform.linux;
                case PlatformID.MacOSX:
                    return Platform.osx;

                default:
                    return Platform.windows;
            }
        }
    }
}
