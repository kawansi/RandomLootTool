using Newtonsoft.Json.Linq;
using RandomVisualizerWPF.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace RandomVisualizerWPF.Utility
{
    public sealed class DataManager
    {
        public static DataManager Instance { get { return Nested.instance; } }

        public BitmapImage? ItemBackgroundImage { get; private set; }

        public Dictionary<string, string> TagToNameDict { get; private set; } = new Dictionary<string, string>();
        public Dictionary<string, string> TagToImagePathDict { get; private set; } = new Dictionary<string, string>();

        private JsonStream jsonStream = new JsonStream();

        private DataManager() {}

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly DataManager instance = new DataManager();
        }

        public void Initialize()
        {
            List<string> excludedTagsList = new List<string>();
            try
            {
                string[] excludedTags = File.ReadAllLines(DataPaths.ExcludeTagsList);
                excludedTagsList.AddRange(excludedTags);
            }
            catch (Exception)
            {
            }
            

            Dictionary<string,string?>? imageBlocksDict = LoadTagFile(DataPaths.TagImagesBlocks);
            if(imageBlocksDict != null)
            {
                foreach(KeyValuePair<string,string?> kv in imageBlocksDict)
                {
                    if(kv.Value != null && !excludedTagsList.Contains(kv.Key))
                    {
                        TagToImagePathDict.Add(kv.Key, @"Assets\Blocks\" + kv.Value);
                    }
                }
            }

            Dictionary<string,string?>? imageEntitiesDict = LoadTagFile(DataPaths.TagImagesEntities);
            if(imageEntitiesDict != null)
            {
                foreach(KeyValuePair<string,string?> kv in imageEntitiesDict)
                {
                    if(kv.Value != null && !excludedTagsList.Contains(kv.Key))
                    {
                        TagToImagePathDict.Add(kv.Key, @"Assets\Entities\" + kv.Value);
                    }
                }
            }

            Dictionary<string,string?>? nameBlocksDict = LoadTagFile(DataPaths.TagNamesBlocks);
            if(nameBlocksDict != null)
            {
                foreach(KeyValuePair<string,string?> kv in nameBlocksDict)
                {
                    if(kv.Value != null && !excludedTagsList.Contains(kv.Key))
                    {
                        TagToNameDict.Add(kv.Key, kv.Value);
                    }
                }
            }
            Dictionary<string,string?>? nameEntitiesDict = LoadTagFile(DataPaths.TagNamesEntities);
            if (nameEntitiesDict != null)
            {
                foreach (KeyValuePair<string, string?> kv in nameEntitiesDict)
                {
                    if (kv.Value != null && !excludedTagsList.Contains(kv.Key))
                    {
                        TagToNameDict.Add(kv.Key, kv.Value);
                    }
                }
            }
            Dictionary<string, string?>? nameItemsDict = LoadTagFile(DataPaths.TagNamesItems);
            if (nameItemsDict != null)
            {
                foreach (KeyValuePair<string, string?> kv in nameItemsDict)
                {
                    if (kv.Value != null && !excludedTagsList.Contains(kv.Key))
                    {
                        TagToNameDict.Add(kv.Key, kv.Value);
                    }
                }
            }
        }

        public string TagToName(string tag)
        {
            if (tag.StartsWith("item."))
            {
                if(TagToNameDict.ContainsKey(tag)) {
                    return TagToNameDict.GetValueOrDefault(tag, tag);
                } else
                {
                    return TagToNameDict.GetValueOrDefault("block." + tag.Substring(5), tag);
                }
            } else
            {
                return TagToNameDict.GetValueOrDefault(tag, tag);
            }
        }

        private Dictionary<string, string?>? LoadTagFile(string path)
        {
            jsonStream.SetPath(path);
            if (jsonStream.TryRefresh(false))
            {
                JObject data = jsonStream.JsonData;
                Dictionary<string, string?> dict = data.Children<JProperty>().ToDictionary(x => x.Name, x => x.Value.ToObject<string>());
                return dict;
            }
            else
            {
                Trace.WriteLine("Failed to load tags from " + path);
            }
            return null;
        }
    }
}
