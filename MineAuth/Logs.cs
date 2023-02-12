using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineAuth
{
    public static class Logs
    {

        private static Queue<string> logs = new Queue<string>();



        public static void Add(string log_msg)
        {
            logs.Enqueue(log_msg);
        }

        public static string GetLog()
        {
            return logs.Dequeue();
        }
    }
}
