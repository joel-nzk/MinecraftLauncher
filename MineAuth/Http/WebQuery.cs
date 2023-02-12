using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MineAuth.Http
{
    public static class WebQuery
    {
        private static HttpClient client = new HttpClient();

        public static string GetStringAsync(string url)
        {
            try
            {
                return client.GetStringAsync(url).Result;
            }
            catch (Exception e)
            {
                return "";

            }
        }

        public static async void DownloadFile(string url, string savePath, string fileName, long? exceptedSize = 0)
        {
            string path = Path.Combine(savePath, fileName);

            var response = await client.GetAsync(url);
            using (var fs = new FileStream(path, FileMode.Create))
            {
                await response.Content.CopyToAsync(fs);

                if (!CheckFileintegrity(path, exceptedSize))
                {
                    Logs.Add($"The file {fileName} doesn't match the excepted size");   
                    File.Delete(path);
                }
                else
                {
                    Logs.Add($"{fileName} have been succefully downloaded");

                }
            }
           


        }

        private static bool CheckFileintegrity(string path, long? exceptedSize)
        {
            long file_length = new FileInfo(path).Length;

            if(file_length != exceptedSize)
                return false;


            return true; ;
        }

        
    }
}
