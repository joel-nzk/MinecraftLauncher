using MineAuth.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        public static string version_manifest = "";

        public static string classPath = "";
        public static string nativeLibrariesFolder = "";
        public static string gameDir = "";
        public static string assetsPath = "";
        public static string gameVersion = "1.8.1";
        public static string assetIndexes = "";
        public static string configurationFilePath = "";

        public static string OsArchitecture
        {
            get
            {
                if (Environment.Is64BitProcess)
                    return "64";
                else
                    return "32";
            }
        }
        

        public static void CreateLauncherFolders(string path, string name)
        {
            //TODO: SAUVEGARDER CE FICHIER
            version_manifest = MinecraftManifest.GetVersionManifest(gameVersion).Replace("${arch}", OsArchitecture);
            gameDir = Path.Combine(path, name);


            if (Directory.Exists(gameDir))
            {
                Logs.Add($"The folder {gameDir} already exist", MessageType.Warning);
            }
 
            //Create the equivalent of the .minecraft
            Directory.CreateDirectory(gameDir);


            //Create the folder for the native libraries
            nativeLibrariesFolder = Path.Combine(gameDir, "bin" , GetRandomHash(16).ToLower().Replace("-",""));
            Directory.CreateDirectory(nativeLibrariesFolder);





            BuildLibraries(gameDir);
            DownloadClient(gameDir);

            //Need to build assets before config file
            DownloadAssets(gameDir);           
            DownloadLogConfFile(gameDir);

            //Debug
            CreateNewFile(classPath, Path.Combine(gameDir, "classPath.txt"));

        }

        private static void DownloadLogConfFile(string path)
        {
            //Create log configs folder
            string savePath = Path.Combine(path, "assets", "log_configs");
            Directory.CreateDirectory(savePath);


            JObject? manifest = JObject.Parse(version_manifest);
            string fileUrl = (string?)manifest["logging"]["client"]["file"]["url"];

            //TODO: Check file size
            long? exceptedSize = (long?)manifest["logging"]["client"]["file"]["size"];
            string? fileName = (string?)manifest["logging"]["client"]["file"]["id"];


            configurationFilePath = Path.Combine(savePath, fileName);
            var jsonFile = WebQuery.GetStringAsync(fileUrl);
            CreateNewFile(jsonFile, configurationFilePath);
        }

        private static void DownloadClient(string path)
        {
            JObject? manifest = JObject.Parse(version_manifest);
            string client_url =  (string?)manifest["downloads"]["client"]["url"];
            long? exceptedSize =  (long?)manifest["downloads"]["client"]["size"];
            string version = (string?)manifest["id"];


            string clientPath = Path.Combine( new string[]{path, "versions", version } );

            Directory.CreateDirectory(clientPath);

            var dlTask = WebQuery.DownloadFile(client_url, clientPath, $"{version}.jar", exceptedSize);
            dlTask.Wait();

            CreateNewFile(version_manifest, Path.Combine(new string[] { clientPath, $"{version}.json" }));


            //Don't add ';' at the end, it's the last element of the classPath
            classPath += Path.Combine(new string[] { clientPath, $"{version}.jar" });
        }

        private static void DownloadAssets(string path)
        {
            JObject? manifest = JObject.Parse(version_manifest);
            string assetsUrl = (string?)manifest["assetIndex"]["url"];
            long? exceptedSize = (long?)manifest["assetIndex"]["size"];
            string version = (string?)manifest["assetIndex"]["id"];


            assetIndexes = version;
            assetsPath = Path.Combine(new string[] { path, "assets", "indexes" });
            Directory.CreateDirectory(assetsPath);
            

            try
            {
                //TODO: SAUVEGARDER CE FICHIER
                var jsonFile = WebQuery.GetStringAsync(assetsUrl);
                CreateNewFile(jsonFile, Path.Combine(new string[] { assetsPath, $"{version}.json" }));


                JObject? assetsList = JObject.Parse(jsonFile);

                foreach (JToken asset in assetsList["objects"])
                {
                    string hash = (string?)asset.First["hash"];
                    exceptedSize = (long?)asset.First["size"];
                    string assetUrl = $@"https://resources.download.minecraft.net/{hash.Substring(0, 2)}/{hash}";

                    string assetFolderpath = Path.Combine(new string[] { path, "assets", "objects", hash.Substring(0, 2) });
                    Directory.CreateDirectory(assetFolderpath);
                    var dlTask = WebQuery.DownloadFile(assetUrl, assetFolderpath, hash, exceptedSize);
                    dlTask.Wait();


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

            JObject? manifest = JObject.Parse(version_manifest);

            string libraries_folder = Path.Combine(path, "libraries");

            Directory.CreateDirectory(libraries_folder);

            JArray? librairies = (JArray?)manifest["libraries"];

            foreach (var lib in librairies)
            {

                if (lib["rules"] != null)
                {
                    bool allowDl = false;

                    foreach (JToken? rule in (JArray)lib["rules"])
                    {
                        if ((string?)rule["action"] == "allow")
                        {
                            allowDl = true;

                            if (rule["os"] != null){
                                if ((string?)rule["os"]["name"] != platform.ToString())              
                                    allowDl = false;
                            }

                        }
                    }

                    if(allowDl)
                        GetLibraryData(lib, libraries_folder);

  

                }
                else
                {
                    GetLibraryData(lib, libraries_folder);
                }
            }
        }

        private static void GetLibraryData(JToken? library, string parentFolder)
        {
            bool isNative = false;
            long? exceptedSize;
            string? lib_dl_url;
            string? lib_raw_path;

            //TODO: Need optimization
            if (library["natives"] != null)
            {
                string classifier = (string?)library["natives"][platform.ToString()];




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

        private static async void CreateNewFile(string srcData, string destPath)
        {
            using(FileStream fs = File.Create(destPath))
            {
                byte[] data = new UTF8Encoding(true).GetBytes(srcData);
                await fs.WriteAsync(data);
            }
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
            var dlTask = WebQuery.DownloadFile(lib_dl_url, dl_folder,fileName, exceptedSize);
            dlTask.Wait();

            string lib_Path = Path.Combine(new string[] { dl_folder, fileName });


            if (isNative)
            {
                try
                {
                    ZipFile.ExtractToDirectory(lib_Path, nativeLibrariesFolder,true);

                }catch(Exception ex)
                {
                    Logs.Add(ex.ToString(), MessageType.Error);
                }
            }
            else
            {
                classPath += lib_Path + ";";

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

        private static string GetRandomHash(int length)
        {
            return BitConverter.ToString(RandomNumberGenerator.GetBytes(length));
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
