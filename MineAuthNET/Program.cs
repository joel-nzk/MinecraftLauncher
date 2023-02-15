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

            //LauncherBuilder.CreateLauncherFolders(@"C:\Users\jojok\AppData\Roaming",".launcher");


            GameClient client = new GameClient();
            if (client.CheckIfJavaInstalled())
                Console.WriteLine("dd");

            client.StartAsync("zendenn", "1.0", 550, 310);




            string my_launcher_classPath = @"C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\mojang\netty\1.8.8\netty-1.8.8.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\net\sf\jopt-simple\jopt-simple\4.6\jopt-simple-4.6.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\paulscode\codecjorbis\20101023\codecjorbis-20101023.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\paulscode\codecwav\20101023\codecwav-20101023.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\paulscode\libraryjavasound\20101123\libraryjavasound-20101123.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\paulscode\librarylwjglopenal\20100824\librarylwjglopenal-20100824.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\paulscode\soundsystem\20120107\soundsystem-20120107.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\io\netty\netty-all\4.0.23.Final\netty-all-4.0.23.Final.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\google\guava\guava\17.0\guava-17.0.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\apache\commons\commons-lang3\3.3.2\commons-lang3-3.3.2.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\commons-io\commons-io\2.4\commons-io-2.4.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\commons-codec\commons-codec\1.9\commons-codec-1.9.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\google\code\gson\gson\2.2.4\gson-2.2.4.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\mojang\authlib\1.5.21\authlib-1.5.21.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\com\mojang\realms\1.7.8\realms-1.7.8.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\apache\logging\log4j\log4j-api\2.0-beta9\log4j-api-2.0-beta9.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\apache\logging\log4j\log4j-core\2.0-beta9\log4j-core-2.0-beta9.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\lwjgl\lwjgl\lwjgl\2.9.1\lwjgl-2.9.1.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\org\lwjgl\lwjgl\lwjgl_util\2.9.1\lwjgl_util-2.9.1.jar;C:\Users\jojok\AppData\Roaming\.launcher\libraries\tv\twitch\twitch\6.5\twitch-6.5.jar;C:\Users\jojok\AppData\Roaming\.launcher\versions\1.8.1\1.8.1.jar";
            string mc_classPat = @"C:\Users\jojok\AppData\Roaming\.minecraft\libraries\com\mojang\netty\1.8.8\netty-1.8.8.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\net\sf\jopt-simple\jopt-simple\4.6\jopt-simple-4.6.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\com\paulscode\codecjorbis\20101023\codecjorbis-20101023.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\com\paulscode\codecwav\20101023\codecwav-20101023.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\com\paulscode\libraryjavasound\20101123\libraryjavasound-20101123.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\com\paulscode\librarylwjglopenal\20100824\librarylwjglopenal-20100824.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\com\paulscode\soundsystem\20120107\soundsystem-20120107.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\io\netty\netty-all\4.0.23.Final\netty-all-4.0.23.Final.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\com\google\guava\guava\17.0\guava-17.0.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\org\apache\commons\commons-lang3\3.3.2\commons-lang3-3.3.2.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\commons-io\commons-io\2.4\commons-io-2.4.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\commons-codec\commons-codec\1.9\commons-codec-1.9.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\com\google\code\gson\gson\2.2.4\gson-2.2.4.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\com\mojang\authlib\1.5.21\authlib-1.5.21.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\com\mojang\realms\1.7.8\realms-1.7.8.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-api\2.0-beta9\log4j-api-2.0-beta9.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-core\2.0-beta9\log4j-core-2.0-beta9.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl\lwjgl\2.9.1\lwjgl-2.9.1.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl\lwjgl_util\2.9.1\lwjgl_util-2.9.1.jar;C:\Users\jojok\AppData\Roaming\.minecraft\libraries\tv\twitch\twitch\6.5\twitch-6.5.jar;C:\Users\jojok\AppData\Roaming\.minecraft\versions\1.8.1\1.8.1.jar";


            //foreach (string mc_cls in mc_classPat.Split(';'))
            //{
            //    bool existent = false;

            //    foreach (var my_cls in my_launcher_classPath.Split(';'))
            //    {
            //        if (my_cls.Split('\\').Last() == mc_cls.Split('\\').Last())
            //        {
            //            existent = true;
            //            break;
            //        }
            //    }

            //    if (!existent)
            //        Console.WriteLine(mc_cls.Split('\\').Last());

            //}



            //Console.WriteLine(JToken.Parse(json).ToString(Formatting.Indented));

        }

    }
}