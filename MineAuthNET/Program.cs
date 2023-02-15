using MineAuth;
using MineAuth.Launcher;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MineAuthNET
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //LauncherBuilder.CreateLauncherFolders(@"C:\Users\jojok\AppData\Roaming",".minecreative","1.4.7","mc147");

            GameClient client = new GameClient();

            int allocatedMemory = 2048;
            string launcherArgs = $"-Xmx{allocatedMemory}M -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M ";
            client.Start("zenden300", "mc147", "1.4.7", launcherArgs);
        }



    }
}