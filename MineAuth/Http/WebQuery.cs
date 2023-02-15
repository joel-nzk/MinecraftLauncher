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
            catch (Exception)
            {
                return "";
            }
        }

        public static  async Task DownloadFile(string url, string savePath, string fileName, long? exceptedSize = 0)
        {

            string path = Path.Combine(savePath, fileName);

            try
            {
                using (var response = await client.GetAsync(url))
                {
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    {
                        using (Stream streamToWriteTo = File.Open(path, FileMode.Create))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);

                            long filesize = new FileInfo(path).Length;

                            if (!CheckFileSize(filesize, exceptedSize))
                            {
                                Logs.Add($"The file '{fileName}' ({filesize} bytes) doesn't match the excepted size ({exceptedSize} bytes), it may be corrupted", MessageType.Error);
                            }
                            else
                            {
                                Logs.Add($"'{fileName}' have been succefully downloaded to '{path}'", MessageType.Success);
                            }
                        }
                    }
                }
               
            }
            catch(Exception e)
            {
                Logs.Add(e.ToString(), MessageType.Error);
            }
        }

        private static bool CheckFileSize(long filesize, long? exceptedSize)
        {
            if(filesize != exceptedSize)
                return false;

            return true;
        }

        
    }
}
