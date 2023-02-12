using MineAuth.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
        private static string json_manifest = "";
        

        public static void CreateLauncherFolders(string path, string name)
        {
            json_manifest = MinecraftManifest.GetVersionManifest();
            string _path = Path.Combine(path, name);


            if (Directory.Exists(_path))
            {
                Logs.Add($"The folder {_path} already exist", MessageType.Warning);
            }
 
            Directory.CreateDirectory(_path);
            DownloadClient(_path);
            BuildLibraries(_path);

        }

        private static void DownloadClient(string path)
        {
            JObject? manifest = JObject.Parse(json_manifest);
            string client_url =  (string?)manifest["downloads"]["client"]["url"];
            long? exceptedSize =  (long?)manifest["downloads"]["client"]["size"];
            string version = (string?)manifest["id"];


            string clientPath = Path.Combine( new string[]{path, "versions", version } );
            Directory.CreateDirectory(clientPath);

            WebQuery.DownloadFile(client_url, clientPath, $"{version}.jar", exceptedSize);
            Thread.Sleep(1000);
        }

        private static void DownloadAssets(string path)
        {
            JObject? manifest = JObject.Parse(json_manifest);
            string client_url = (string?)manifest["downloads"]["client"]["url"];
            long? exceptedSize = (long?)manifest["downloads"]["client"]["size"];
            string version = (string?)manifest["id"];


            string assetsPath = Path.Combine(new string[] { path, "assets", "indexes", version });
            Directory.CreateDirectory(assetsPath);

            WebQuery.DownloadFile(client_url, assetsPath, $"{version}.jar", exceptedSize);
            Thread.Sleep(1000);
        }


        private static void BuildLibraries(string path)
        {

            JObject? manifest = JObject.Parse(json_manifest);

            string libraries_folder = Path.Combine(path, "libraries");

            Directory.CreateDirectory(libraries_folder);

            JArray? librairies = (JArray?)manifest["libraries"];

            foreach (var lib in librairies)
            {
                if (lib["rules"] != null)
                {
                    if ( (string?)lib["rules"][0]["os"]["name"] == platform.ToString())
                    {
                        GetLibraryData(lib, libraries_folder);
                    }
                }
                else
                {
                    GetLibraryData(lib, libraries_folder);
                }
            }
        }

        private static void GetLibraryData(JToken? library, string parentFolder)
        {
            string? lib_raw_path = (string?)library["downloads"]["artifact"]["path"];
            string? lib_dl_url = (string?)library["downloads"]["artifact"]["url"];
            long? exceptedSize = (long?)library["downloads"]["artifact"]["size"];

            DownloadLibrary(parentFolder, lib_raw_path, lib_dl_url, exceptedSize);
        }


        private static async void DownloadLibrary(string parent_folder, string lib_raw_path, string lib_dl_url,long? exceptedSize)
        {
            string[] folders = lib_raw_path.Split('/');
            string fileName = folders[folders.Length - 1];
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < folders.Length - 1; i++)
                sb.Append(folders[i] + '\\');

            string dl_folder = Path.Combine(parent_folder, sb.ToString());    
            Directory.CreateDirectory(dl_folder);
            WebQuery.DownloadFile(lib_dl_url, dl_folder,fileName, exceptedSize);
            Thread.Sleep(1000);

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
