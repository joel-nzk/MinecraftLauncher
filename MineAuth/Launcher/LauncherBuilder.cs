using MineAuth.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineAuth.Launcher{

    public enum Platform
    {
        windows,
        linux,
        osx
    }

    public static class LauncherBuilder
    {
        private static Platform platform = GetUserPlatform();
        
        

        public static void CreateLauncherFolders(string path, string name = ".launcher")
        {
            string _path = Path.Combine(path, name);


            Directory.CreateDirectory(_path);
            BuildLibraries(_path);

            //if (!Directory.Exists(_path)) 
            //{
               
            //}
            //else
            //{
            //    throw new InvalidOperationException("This folder already exist");
            //}
               
        }


        private static void BuildLibraries(string path)
        {
            var json = MinecraftManifest.GetVersionManifest();
            JObject? manifest = JObject.Parse(json);

            string libraries_folder = Path.Combine(path, "libraries");

            Directory.CreateDirectory(libraries_folder);

            JArray? librairies = (JArray?)manifest["libraries"];

            foreach (var lib in librairies)
            {
                if (lib["rules"] != null)
                {
                    if ( (string?)lib["rules"][0]["os"]["name"] == platform.ToString())
                    {
                        string? lib_raw_path = (string?)lib["downloads"]["artifact"]["path"];
                        string? lib_dl_url = (string?)lib["downloads"]["artifact"]["url"];
                        long? exceptedSize = (long?)lib["downloads"]["artifact"]["size"];

                        BuildLibrary(libraries_folder,lib_raw_path, lib_dl_url, exceptedSize);
                    }
                }
            }
        }


        private static async void BuildLibrary(string parent_folder, string lib_raw_path, string lib_dl_url,long? exceptedSize)
        {
            string[] folders = lib_raw_path.Split('/');
            string fileName = folders[folders.Length - 1];
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < folders.Length - 1; i++)
                sb.Append(folders[i] + '\\');

            string dl_folder = Path.Combine(parent_folder, sb.ToString());    
            Directory.CreateDirectory(dl_folder);
            WebQuery.DownloadFile(lib_dl_url, dl_folder,fileName, exceptedSize);
            Thread.Sleep(500);
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
