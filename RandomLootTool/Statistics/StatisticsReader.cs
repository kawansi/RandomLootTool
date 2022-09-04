using Newtonsoft.Json.Linq;
using RandomVisualizerWPF.Types;
using RandomVisualizerWPF.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Documents;

namespace RandomVisualizerWPF.Statistics
{
    internal class StatisticsReader
    {
        private JsonStream saveJsonStream;

        private Dictionary<string, int>? minedBlocksDict;
        private Dictionary<string, int>? pickedUpItemsDict;
        private Dictionary<string, int>? killedMobsDict;

        public StatisticsReader()
        {
            saveJsonStream = new JsonStream();
        }

        public void SetSavePath(string path)
        {
            //Find stats file path
            string statsDirPath = path + @"\stats";
            if (Directory.Exists(statsDirPath))
            {

                string[] statFilePath = Directory.GetFiles(statsDirPath);
                if (statFilePath.Length > 0)
                {
                    //Pick first one, fine for single player
                    saveJsonStream.SetPath(statFilePath[0]);
                }
            }
        }

        public bool Refresh()
        {
            bool modified = saveJsonStream.TryRefresh(false);

            minedBlocksDict = GetStatDict(StatType.Mined);
            pickedUpItemsDict = GetStatDict(StatType.Picked);
            killedMobsDict = GetStatDict(StatType.Killed);

            return modified;
        }

        public Dictionary<string, int>? GetMinedBlocksDict()
        {
            return minedBlocksDict;
        }
        public Dictionary<string, int>? GetPickedUpItemsDict()
        {
            return pickedUpItemsDict;
        }
        public Dictionary<string, int>? GetKilledMobsDict()
        {
            return killedMobsDict;
        }

        private Dictionary<string, int>? GetStatDict(StatType stat)
        {
            string statKey = StatToKeyTag(stat);

            JObject statsObject = saveJsonStream["stats"]?[statKey];
            if (statsObject != null)
            {
                Dictionary<string, int> statDict = new Dictionary<string, int>();
                foreach(JProperty prop in statsObject.Children<JProperty>())
                {
                    string tag = StatToNameTag(prop.Name, stat);
                    //Make special exception for wall mounted blocks
                    int wallIndex = tag.IndexOf("_wall_");
                    if (wallIndex >= 0)
                    {
                        tag = tag.Remove(wallIndex, 5);
                        Trace.WriteLine("removed tag: " + tag);
                    } else if (tag.Equals("block.minecraft.wall_torch"))
                    {
                        tag = "block.minecraft.torch";
                    }
                    //Increase count if the tag already exist, otherwise add
                    if (statDict.ContainsKey(tag))
                    {
                        statDict[tag] = statDict[tag] + prop.Value.ToObject<int>();
                    } else
                    {
                        statDict.Add(tag, prop.Value.ToObject<int>());
                    }
                }
                return statDict;
            }
            return null;
        }

        private string StatToNameTag(string statTag, StatType statType)
        {
            string[] statTagSplit = statTag.Split(":");
            string nameTag = "none";
            if(statTagSplit.Length > 1)
            {
                switch (statType)
                {
                    case StatType.Mined:
                        nameTag = "block.minecraft." + statTagSplit[1];
                        break;
                    case StatType.Picked:
                        nameTag = "item.minecraft." + statTagSplit[1];
                        break;
                    case StatType.Killed:
                        nameTag = "entity.minecraft." + statTagSplit[1];
                        break;
                    default:
                        break;
                }
            }
            return nameTag;
        }

        private string StatToKeyTag(StatType stat)
        {
            switch (stat)
            {
                case StatType.Mined:
                    return "minecraft:mined";
                case StatType.Picked:
                    return "minecraft:picked_up";
                case StatType.Killed:
                    return "minecraft:killed";
                default:
                    return "none";
            }
        }
    }
}
