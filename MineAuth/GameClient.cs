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
        public enum AccountType
        {
            Online,
            Offline,
        }


        public  void StartAsync(string username, string versionName, int width, int height, string versionType = "test")
        {
            string launcher_name = "minecraft-launcher";
            string launcher_version = "2.3.592";
            string userType = "msa";
            string accessToken = "no_token";
            string dosVerison = "Windows 10";
            string clientJarPath = @"C:\Users\jojok\AppData\Roaming\.launcher\versions\1.8.1\1.8.1.jar";


            ProcessStartInfo mc_game_process = new ProcessStartInfo()
            {
                FileName = GetJavaPath(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = $"\"-Dos.name=Windows 10\" -Dos.version=10.0 -XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump "
            };

            LauncherBuilder.classPath = @"C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\mojang\netty\1.8.8\netty-1.8.8.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\net\sf\jopt-simple\jopt-simple\4.6\jopt-simple-4.6.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\paulscode\codecjorbis\20101023\codecjorbis-20101023.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\paulscode\codecwav\20101023\codecwav-20101023.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\paulscode\libraryjavasound\20101123\libraryjavasound-20101123.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\paulscode\librarylwjglopenal\20100824\librarylwjglopenal-20100824.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\paulscode\soundsystem\20120107\soundsystem-20120107.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\io\netty\netty-all\4.0.23.Final\netty-all-4.0.23.Final.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\google\guava\guava\17.0\guava-17.0.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\apache\commons\commons-lang3\3.3.2\commons-lang3-3.3.2.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\commons-io\commons-io\2.4\commons-io-2.4.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\commons-codec\commons-codec\1.9\commons-codec-1.9.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\google\code\gson\gson\2.2.4\gson-2.2.4.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\mojang\authlib\1.5.21\authlib-1.5.21.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\mojang\realms\1.7.8\realms-1.7.8.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\apache\logging\log4j\log4j-api\2.0-beta9\log4j-api-2.0-beta9.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\apache\logging\log4j\log4j-core\2.0-beta9\log4j-core-2.0-beta9.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\lwjgl\lwjgl\lwjgl\2.9.1\lwjgl-2.9.1.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\lwjgl\lwjgl\lwjgl_util\2.9.1\lwjgl_util-2.9.1.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\tv\twitch\twitch\6.5\twitch-6.5.jar;C:\Users\jojok\AppData\Roaming\.launcher\versions\1.8.1\1.8.1.jar";
            LauncherBuilder.nativeLibrariesFolder = @"C:\Users\jojok\AppData\Roaming\.launcher\bin\2c5b1bbf00289efee6ffb54d1ef4f736";
            LauncherBuilder.gameDir = @"C:\Users\jojok\AppData\Roaming\.launcher";
            LauncherBuilder.assetsPath = @"C:\Users\jojok\AppData\Roaming\.launcher\assets";
            LauncherBuilder.assetIndexes = "1.8";
            LauncherBuilder.gameVersion = "1.8.1";
            LauncherBuilder.configurationFilePath = @"C:\Users\jojok\AppData\Roaming\.launcher\assets\log_configs\client-1.7.xml";

            //JVM Arguments
            mc_game_process.Arguments += $"-Djava.library.path={LauncherBuilder.nativeLibrariesFolder} -Dminecraft.launcher.brand={launcher_name} -Dminecraft.launcher.version={launcher_version} -Dminecraft.client.jar={clientJarPath} -cp {LauncherBuilder.classPath} ";
            mc_game_process.Arguments += "-Xmx2G -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M ";
            mc_game_process.Arguments += $"-Dlog4j.configurationFile={LauncherBuilder.nativeLibrariesFolder} ";

            //Main Class
            mc_game_process.Arguments += "net.minecraft.client.main.Main ";

            //Game Arguments
            mc_game_process.Arguments += $"--username {username} --version {LauncherBuilder.gameVersion} --gameDir {LauncherBuilder.gameDir} --assetsDir {LauncherBuilder.assetsPath} --assetIndex {LauncherBuilder.assetIndexes} --accessToken {accessToken} --userProperties {"{}"} --userType {userType}";


            var process = new Process();
            process.StartInfo = mc_game_process;


            process.OutputDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                if (e.Data != null)
                {
                    Logs.Add(e.Data);
                }
            });
            process.ErrorDataReceived += new DataReceivedEventHandler((s, e) =>
            {
                if (e.Data != null)
                {
                    Logs.Add(e.Data,MessageType.Error);
                }
            });


            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }


        public string GetJavaPath()
        {
            //TODO: Utiliser la commande 'where' à la place de mettre en dur
            return @"C:\Program Files\Java\jre1.8.0_351\bin\javaw.exe";
        }


        public bool CheckIfJavaInstalled()
        {
            //TODO: Faire la même pour Linux et OSX
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

                return  process.ExitCode == 0;
            }
            catch
            {
            }


            return false;
        }



    }


}
