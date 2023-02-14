using MineAuth.Launcher;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineAuth
{
    public class GameClient
    {



        public void Start(/*string username, string authUUID, string versionName, int width, int height, string versionType*/)
        {

            string launcher_name = "Minecraft Launcher";
            string natives_directory = "";
            string launcher_version = "1.0";
            string assets_index_name = "1.0";



            ProcessStartInfo mc_game_process = new ProcessStartInfo()
            {
                FileName = "Minecraft",
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = "java -jar -XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Dos.name=Windows 10 -Dos.version=10.0"
            };

            mc_game_process.Arguments += $"-Dminecraft.launcher.brand={launcher_name} -Dminecraft.launcher.version={launcher_version} -cp {LauncherBuilder.classPath}";


            Process.Start(mc_game_process);


            //JObject? manifest = JObject.Parse(LauncherBuilder.json_manifest);

            //Platform user_os = LauncherBuilder.GetUserPlatform();

            //var jvm_args = (JArray)manifest["arguments"]["jvm"];


            //foreach (JToken args in jvm_args)
            //{

            //    var osObj = args["rules"][0]["os"];

            //    if (osObj["name"] != null)
            //    {

            //    }
            //    else if (osObj["arch"] != null)
            //    {
            //        if ((string?)osObj["arch"] == LauncherBuilder.osArchitecture)
            //        {

            //        }


            //    }

            //    if (osObj["name"] == user_os.ToString())
            //    {
            //        if(args["value"].Type is JArray)
            //        {
            //            foreach (var item in args["value"])
            //            {
            //            }
            //        }

            //        else
            //            Console.WriteLine("1: "+args["value"]);


            //    }
            //}


        }



    }


}
