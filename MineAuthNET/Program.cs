﻿using MineAuth;
using MineAuth.Launcher;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MineAuthNET
{
    internal class Program
    {
        static void Main(string[] args)
        {

            LauncherBuilder.CreateLauncherFolders(@"C:\Users\jojok\AppData\Roaming");

       
            //Console.WriteLine(JToken.Parse(json).ToString(Formatting.Indented));

        }

    }
}