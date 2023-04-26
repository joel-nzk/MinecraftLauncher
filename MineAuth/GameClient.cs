using MineAuth.Http;
using MineAuth.Java;
using MineAuth.Launcher;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineAuth
{
    public class GameClient
    {
        public enum AccountType
        {
            Online,
            Offline,
        }


        public void Start(string username, string clientName, string gameVersion,string gameDir,AccountType accountType, string email = "", string password = "", string addArgs = "")
        {
            if(!JavaUtils.IsJavaInstalled())
            {
                Logs.Add("Java not installed", MessageType.Information);
                JavaUtils.InstallJava("");
            }


            if(accountType == AccountType.Online)
            {
                Logs.Add($"No account found for the email {email}", MessageType.Information);
                Logs.Add($"Launching in Offline mode", MessageType.Information);
            }



            var versionManifest =  GetVersionManifestAsync(gameDir, clientName);


            string clientPath = GetClientPath(gameDir, clientName);
            string nativeLibrariesFolder = GetNativelibrariesPath(gameDir, clientName);
            string assetsPath = GetAssetsPath(gameDir, gameVersion);
            string configurationFilePath = "";

            if (versionManifest["logging"] != null)
                configurationFilePath = GetConfigurationFilePath(gameDir, (string?)versionManifest["logging"]["client"]["file"]["id"]);


            string launcher_name = "minecraft-launcher";
            string launcher_version = "2.3.592";
            string userType = "msa";
            string accessToken = "no_token";
            string arguments = "";
            string versionType = "test";
            string uuid = "no_uuid";



            string assetIndexes = (string?)versionManifest["assetIndex"]["id"];      
            string classPath = GetClassPath(versionManifest, gameDir) + clientPath;


            ProcessStartInfo mc_game_process = new ProcessStartInfo()
            {
                FileName = JavaUtils.GetJavaPath(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
               
            };


            //JVM Arguments
            if (Utils.GetUserPlatform() == Platform.windows)
            {
                arguments += "\"-Dos.name=Windows 10\" -Dos.version=10.0 ";
                arguments += "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump ";
            }


            if (Utils.GetUserPlatform() == Platform.osx)
                arguments += "-XstartOnFirstThread ";


            arguments +=  $"{addArgs} ";
            mc_game_process.Arguments += $"-Djava.library.path={nativeLibrariesFolder} -Dminecraft.launcher.brand={launcher_name} -Dminecraft.launcher.version={launcher_version} -Dminecraft.client.jar={clientPath} -cp {classPath} ";

            //TODO: Verify from which version this parameter apply
            //TODO: Improve the verification system
            if (MinecraftVersion(gameVersion) <= 172)
            {
                mc_game_process.Arguments += $"-Dlog4j.configurationFile={configurationFilePath} ";
            }


            //Main Class
            //TODO: use net.minecraft.launchwrapper.Launch until 1.5.2, except for 13w16b who uses net.minecraft.client.main.Main
            //TODO: Improve the verification system

            if (MinecraftVersion(gameVersion) > 152)
            {
                mc_game_process.Arguments += "net.minecraft.client.main.Main ";
            }
            else
                mc_game_process.Arguments += "net.minecraft.launchwrapper.Launch ";


            //Game Arguments
            mc_game_process.Arguments += $"--username {username} --version {gameVersion} --gameDir {gameDir} --assetsDir {assetsPath} --assetIndex {assetIndexes} --accessToken {accessToken} --uuid {uuid} --userProperties {"{}"} --userType {userType}";

            //Args for below 1.7
            if (MinecraftVersion(gameVersion) <= 170)
                mc_game_process.Arguments += "--tweakClass net.minecraft.launchwrapper.AlphaVanillaTweaker";


            var process = new Process();
            process.StartInfo = mc_game_process;

            process.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                if (e.Data != null)
                {

                    Logs.Add(e.Data,MessageType.Normal,false);
                }
            });
            process.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                if (e.Data != null)
                {
                    Logs.Add(e.Data,MessageType.Error,false);
                }
            });

            
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }

        public JObject GetVersionManifestAsync(string gameDir, string clientName)
        {
            string path = Path.Combine(new string[] { gameDir, "versions", clientName, $"{clientName}.json" });
            var file = Utils.GetTextFileContent(path).Result;
            return JObject.Parse(file);
        }
        public string GetClientPath(string gameDir, string clientName)
        {
            return Path.Combine(new string[] { gameDir , "versions", clientName , $"{clientName}.jar" });
        }
        public string GetAssetsPath(string gameDir,string gameVersion)
        {
            if(MinecraftVersion(gameVersion) <= 172)
                return Path.Combine(new string[] { gameDir, "assets", "virtual" });

            return Path.Combine(gameDir, "assets");
        }
        public string GetClassPath(JObject versionManifest,string gameDir)
        {

            JArray? librairies = (JArray?)versionManifest["libraries"];
            string classPath = "";

            foreach (var lib in librairies)
            {
                bool validlibrary = false;

                if (lib["rules"] != null)
                {

                    foreach (JToken? rule in (JArray)lib["rules"])
                    {
                        if ((string?)rule["action"] == "allow")
                        {
                            validlibrary = true;

                            if (rule["os"] != null)
                            {
                                if ((string?)rule["os"]["name"] != Utils.GetUserPlatform().ToString())
                                    validlibrary = false;
                            }
                        }
                    }                
                }
                else
                {
                    validlibrary = true;
                }



                if (validlibrary)
                {
                    if (lib["natives"] == null)
                    {
                        string libPath = ((string?)lib["downloads"]["artifact"]["path"]).Replace('/', '\\');
                        classPath += $"{Path.Combine(new string[] { gameDir, "libraries", libPath, })};";
                    }
                }
            }


            return classPath;
        }
        public string GetNativelibrariesPath(string gameDir, string clientName)
        {
            string clientNameHash = Utils.getHash(clientName);
            return Path.Combine(new string[] { gameDir, "bin" , clientNameHash, });
        }
        public string GetConfigurationFilePath(string gameDir, string configFileName)
        {
            return Path.Combine(new string[] { gameDir, "assets", "log_configs", configFileName });
        }
   
    

        public static int MinecraftVersion(string version)
        {
            return  int.Parse(string.Concat(version.Split('.')));
        }
    }


}
