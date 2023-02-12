using MineAuth.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Diagnostics;
using System.Linq;

namespace MineAuth
{
    public static class MinecraftManifest
    {
        private static readonly string all_versions_manifest_uri = @"https://launchermeta.mojang.com/mc/game/version_manifest.json";



        public static string GetVersionsManifestList()
        {
            return WebQuery.GetStringAsync(all_versions_manifest_uri);
        }

        public static string GetVersionManifest(string version_id = "") 
        {
            JObject? manifest = JObject.Parse(GetVersionsManifestList());

            if (manifest != null)
            {
               if(version_id == "")
                    version_id = (string)manifest["latest"]["release"];

                JArray versions_manifest = (JArray)manifest["versions"];

                string version_manifest_uri = "";

                foreach (JObject obj in versions_manifest)
                {
                    if ((string)obj["id"] == version_id)
                    {
                        version_manifest_uri = @$"{obj["url"]}";
                        break;
                    }
                         
                }

                if(version_manifest_uri != "")
                {
                    return WebQuery.GetStringAsync(version_manifest_uri);
                }
            }


            return "";
        }

    }
}