using MineAuth;
using MineAuth.Launcher;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static MineAuth.GameClient;

namespace MineAuthNET
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string gameDir = @"C:\Users\jojok\AppData\Roaming";
            string gameFolderName = ".minespace";

            LauncherBuilder.CreateNewLauncher(gameDir, gameFolderName, "1.12.2", "minespace");


            GameClient client = new GameClient();
            string path = Path.Combine(gameDir, gameFolderName);
            int allocatedMemory = 2048;
            string email = "email_user@gmail.com";
            string password = "dummy_password";
            string launcherArgs = $"-Xmx{allocatedMemory}M -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M ";
            client.Start("user3", "minespace", "1.12.2", path, AccountType.Offline, email, password, launcherArgs);




        }



    }
}