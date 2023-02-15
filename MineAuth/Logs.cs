using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineAuth
{
    public enum MessageType
    {
        Success,
        Error,
        Warning,
        Normal,
        Information
    }

    public static class Logs
    {
 
        public static void Add(string message, MessageType type = MessageType.Normal, bool timeCode = true)
        {
            switch (type)
            {
                case MessageType.Success:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case MessageType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case MessageType.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case MessageType.Information:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }

            DateTime time = DateTime.Now;

            if(timeCode)
                Console.WriteLine($"[{time:HH:mm:ss}] {message}");
            else
                Console.WriteLine(message);


            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }


    


}
