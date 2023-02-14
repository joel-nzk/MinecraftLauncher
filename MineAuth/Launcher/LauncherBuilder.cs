using MineAuth.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        public static string json_manifest = "";

        public static string classPath = "";
        public static string nativeLibrariesFolder = "";

        public static string osArchitecture
        {
            get
            {
                if (Environment.Is64BitProcess)
                    return "x64";
                else
                    return "x86";
            }
        }
        

        public static void CreateLauncherFolders(string path, string name)
        {
            //TODO: SAUVEGARDER CE FICHIER
            json_manifest = MinecraftManifest.GetVersionManifest("1.8.1");
            string _path = Path.Combine(path, name);


            if (Directory.Exists(_path))
            {
                Logs.Add($"The folder {_path} already exist", MessageType.Warning);
            }
 
            //Create the equivalent of the .minecraft
            Directory.CreateDirectory(_path);


            //Create the folder for the native libraries
            nativeLibrariesFolder = Path.Combine(_path, "bin");
            Directory.CreateDirectory(nativeLibrariesFolder);


            //DownloadClient(_path);
            //DownloadAssets(_path);
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
            classPath += Path.Combine(new string[] { clientPath, $"{version}.jar" }) + ";";

        }

        private static void DownloadAssets(string path)
        {
            JObject? manifest = JObject.Parse(json_manifest);
            string assetsUrl = (string?)manifest["assetIndex"]["url"];
            long? exceptedSize = (long?)manifest["assetIndex"]["size"];
            string version = (string?)manifest["assetIndex"]["id"];


            string assetsPath = Path.Combine(new string[] { path, "assets", "indexes" });
            Directory.CreateDirectory(assetsPath);
            

            try
            {
                //TODO: SAUVEGARDER CE FICHIER
                var jsonFile = WebQuery.GetStringAsync(assetsUrl);
                JObject? assetsList = JObject.Parse(jsonFile);

                foreach (JToken asset in assetsList["objects"])
                {
                    string hash = (string?)asset.First["hash"];
                    exceptedSize = (long?)asset.First["size"];
                    string assetUrl = $@"https://resources.download.minecraft.net/{hash.Substring(0, 2)}/{hash}";

                    string assetFolderpath = Path.Combine(new string[] { path, "assets", "objects", hash.Substring(0, 2) });
                    Directory.CreateDirectory(assetFolderpath);
                    WebQuery.DownloadFile(assetUrl, assetFolderpath, hash, exceptedSize);


                    //TODO : Store a copy to ".minecraft/assets/virtual/legacy/"
                    //in the old format for versions that don't support the new system (1.7 and below)
                    string assetpath = Path.Combine(new string[] { assetFolderpath, hash, });

                   

                }
            }
            catch (Exception ex)
            {
                Logs.Add($"Error when downloading {version}.json", MessageType.Error);

            }

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
                    if(lib["rules"][0]["os"] != null)
                    {
                        if ((string?)lib["rules"][0]["os"]["name"] == platform.ToString())
                        {
                            GetLibraryData(lib, libraries_folder);
                        }
                    }
                    else if (lib["rules"][1]["os"] != null)
                    {
                        if ((string?)lib["rules"][1]["os"]["name"] == platform.ToString())
                        {
                            GetLibraryData(lib, libraries_folder);
                        }
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
            string? lib_raw_path = "";
            string? lib_dl_url = "";
            long? exceptedSize = 0;

            bool isNative = false;

            //TODO: Need optimization
            if (library["natives"] != null)
            {
                string classifier = (string?)library["natives"][platform.ToString()];


                if (classifier.Contains("${arch}"))
                {
                    string arch = osArchitecture.Remove(0, 1);
                    classifier = classifier.Replace("${arch}", arch);
                }

                lib_raw_path = (string?)library["downloads"]["classifiers"][classifier]["path"];
                lib_dl_url = (string?)library["downloads"]["classifiers"][classifier]["url"];
                exceptedSize = (long?)library["downloads"]["classifiers"][classifier]["size"];

                isNative = true;



            }
            else
            {
                lib_raw_path = (string?)library["downloads"]["artifact"]["path"];
                lib_dl_url = (string?)library["downloads"]["artifact"]["url"];
                exceptedSize = (long?)library["downloads"]["artifact"]["size"];




            }

            DownloadLibrary(parentFolder, lib_raw_path, lib_dl_url, exceptedSize, isNative);



        }


        private static  void DownloadLibrary(string parent_folder, string lib_raw_path, string lib_dl_url,long? exceptedSize, bool isNative = false)
        {
            string[] folders = lib_raw_path.Split('/');
            string fileName = folders[folders.Length - 1];
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < folders.Length - 1; i++)
                sb.Append(folders[i] + '\\');

            string dl_folder = Path.Combine(parent_folder, sb.ToString());    
            Directory.CreateDirectory(dl_folder);
            WebQuery.DownloadFile(lib_dl_url, dl_folder,fileName, exceptedSize);

            string lib_Path = Path.Combine(new string[] { dl_folder, fileName });
            classPath += lib_Path + ";";

            if (isNative)
            {
                try
                {
                    ZipFile.ExtractToDirectory(lib_Path, nativeLibrariesFolder);

                }catch(Exception ex)
                {

                }
            }



        }


        private static async Task<string> GetJsonFile(string path)
        {
            if(File.Exists(path))
            {
                using (StreamReader sr = File.OpenText(path))
                {
                    return await sr.ReadToEndAsync();
                }
            }

            return "";
        }

        private static void GetZipFileContent(string path)
        {
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
