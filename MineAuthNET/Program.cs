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
      
            LauncherBuilder.CreateLauncherFolders(@"C:\Users\jojok\AppData\Roaming",".launcher");


            //GameClient client = new GameClient();
            //client.Start();

       
            //Console.WriteLine(JToken.Parse(json).ToString(Formatting.Indented));

        }

    }
}