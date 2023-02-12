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
    }

    public static class Logs
    {
 
        public static void Add(string message, MessageType type = MessageType.Normal)
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
                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }

            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }


    


}
