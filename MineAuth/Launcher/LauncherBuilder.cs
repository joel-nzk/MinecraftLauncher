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

 

    public static class LauncherBuilder
    {

        public static string version_manifest = "";

        private static string classPath = "";
        private static string nativeLibrariesFolder = "";
        private static string gameDir = "";
        private static string clientName = "";
        private static string assetsPath = "";
        private static string gameVersion = "";
        private static string assetIndexes = "";
        private static string configurationFilePath = "";

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


        public static void CreateNewLauncher(string path, string name, string _gameVersion, string _clientName)
        {
            Task task = Task.Run(() => CreateLauncherFolders(path, name, _gameVersion, _clientName));
            task.Wait();
        }


        private static void CreateLauncherFolders(string path, string name, string _gameVersion, string _clientName)
        {
            gameDir = Path.Combine(path, name);

            if (Directory.Exists(gameDir))
            {
                Logs.Add($"The folder {gameDir} already exist", MessageType.Warning);
                return;
            }

            gameVersion = _gameVersion;
            version_manifest = MinecraftManifest.GetVersionManifest(_gameVersion).Replace("${arch}", OsArchitecture);
            clientName = _clientName;
       
            //Create the equivalent of the .minecraft
            Directory.CreateDirectory(gameDir);


            //Create the folder for the native libraries
            //TODO : use client name instead of version number
            nativeLibrariesFolder = Path.Combine(gameDir, "bin" , Utils.getHash(clientName));
            Directory.CreateDirectory(nativeLibrariesFolder);


            BuildLibraries(gameDir);
            DownloadClient(gameDir);

            //Need to build assets before config file
            DownloadAssets(gameDir);           
            DownloadLogConfigFile(gameDir);

      

        }



        private static void DownloadLogConfigFile(string path)
        {
            JObject? manifest = JObject.Parse(version_manifest);

            if (manifest["logging"] != null)
            {
                //Create log configs folder
                string savePath = Path.Combine(path, "assets", "log_configs");
                Directory.CreateDirectory(savePath);


                string fileUrl = (string?)manifest["logging"]["client"]["file"]["url"];

                //TODO: Check file size
                long? exceptedSize = (long?)manifest["logging"]["client"]["file"]["size"];
                string? fileName = (string?)manifest["logging"]["client"]["file"]["id"];


                configurationFilePath = Path.Combine(savePath, fileName);
                var jsonFile = WebQuery.GetStringAsync(fileUrl);
                CreateNewFile(jsonFile, configurationFilePath);
            }
           
        }

        private static void DownloadClient(string path)
        {
            JObject? manifest = JObject.Parse(version_manifest);
            string client_url =  (string?)manifest["downloads"]["client"]["url"];
            long? exceptedSize =  (long?)manifest["downloads"]["client"]["size"];


            string clientPath = Path.Combine( new string[]{path, "versions", clientName } );

            Directory.CreateDirectory(clientPath);

            var dlTask = WebQuery.DownloadFile(client_url, clientPath, $"{clientName}.jar", exceptedSize);
            dlTask.Wait();

            CreateNewFile(version_manifest, Path.Combine(new string[] { clientPath, $"{clientName}.json" }));


            //Don't add ';' at the end, it's the last element of the classPath
            classPath += Path.Combine(new string[] { clientPath, $"{clientName}.jar" });
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
                //TODO: Save this file
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


                    //Only needed for version below 1.7
                    if (minecraftVersion(gameVersion) <= 170)
                    {
                        string hashedRssourcePath = Path.Combine(assetFolderpath, hash);
                        string rawResourceName = ((JProperty)asset).Name.Replace('/', '\\');
                        string resourceName = rawResourceName.Split('\\').Last();
                        string resourceFolder = rawResourceName.Replace(resourceName, "");
                        string ressourcePath = Path.Combine(new string[] { gameDir, "resources", resourceFolder });
                        Directory.CreateDirectory(ressourcePath);
                        File.Copy(hashedRssourcePath, Path.Combine(ressourcePath, resourceName), true);
                    }
                       

                    


                }
            }
            catch (Exception ex)
            {
                Logs.Add($"Error when downloading {version}.json, {ex}", MessageType.Error);

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
                                if ((string?)rule["os"]["name"] != Utils.GetUserPlatform().ToString())              
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
                string classifier = (string?)library["natives"][Utils.GetUserPlatform().ToString()];




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

        private static int minecraftVersion(string version)
        {
            return int.Parse(string.Concat(version.Split('.')));
        }







    }
}
