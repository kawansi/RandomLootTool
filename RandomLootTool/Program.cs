using RandomVisualizerWPF.Statistics;
using RandomVisualizerWPF.Types;
using RandomVisualizerWPF.UIControls;
using RandomVisualizerWPF.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomVisualizerWPF
{
    public class Program
    {
        private MainWindow? mainWindow;
        private StatisticsReader statisticsReader = new StatisticsReader();

        private List<StatItem> unfilteredStatList = new List<StatItem>();
        private string oldSaveDir = "";

        public Program()
        {
        }

        public void SetMainWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void Run()
        {
            //Refresh stats file
            string? currentSaveDir = mainWindow?.GetSaveDirectory();
            //Is not empty/null
            bool refreshed = false;
            if(!string.IsNullOrEmpty(currentSaveDir))
            {
                //Update save directory if new found
                if (currentSaveDir != oldSaveDir)
                {
                    oldSaveDir = currentSaveDir;
                    statisticsReader.SetSavePath(currentSaveDir);
                }

                //Attempt to refresh save
                refreshed = statisticsReader.Refresh();
            }

            if (refreshed)
            {
                RefreshUnfilteredList();
            }

            if (refreshed || (mainWindow != null && mainWindow.FilterModified)) { 
                List<StatItem> filteredStatList = FilterItems();
                List<StatItem> sortedFilteredStatList = SortItems(filteredStatList);
                ApplyFilteredList(sortedFilteredStatList);
                if(mainWindow != null)
                {
                    mainWindow.FilterModified = false;
                }
            }
        }

        private void AddToUnfilteredList(Dictionary<string, int>? statDict, StatType type)
        {
            if (statDict != null)
            {
                foreach(KeyValuePair<string,int> kv in statDict)
                {
                    unfilteredStatList.Add(new StatItem(kv.Key, kv.Value, type));
                }
            }
        }

        private void RefreshUnfilteredList()
        {
            //Update list with new/same data
            unfilteredStatList.Clear();
            AddToUnfilteredList(statisticsReader.GetMinedBlocksDict(), StatType.Mined);
            AddToUnfilteredList(statisticsReader.GetPickedUpItemsDict(), StatType.Picked);
            AddToUnfilteredList(statisticsReader.GetKilledMobsDict(), StatType.Killed);
            
            foreach (KeyValuePair<string,string> tagName in DataManager.Instance.TagToNameDict)
            {
                //If tag not already added, do it now
                if(!unfilteredStatList.Any(item => item.Tag.Equals(tagName.Key)))
                {
                    StatType? type = GetTagType(tagName.Key);
                    if(type != null)
                    {
                        unfilteredStatList.Add(new StatItem(tagName.Key, 0, type.Value));
                    }
                }
            }
        }

        private List<StatItem> FilterItems()
        {
            List<StatItem> filteredStatList = new List<StatItem>(unfilteredStatList
                                                                 .Where(FilterType)
                                                                 .Where(FilterCount)
                                                                 .Where(FilterStatView)
                                                                 .Where(FilterSearch));
            return filteredStatList;
        }

        private void ApplyFilteredList(List<StatItem> filteredStatList)
        {
            //Replace visual list with filtered list, works fine by clearing and making new instances for now
            bool colorBackground = true;
            if (mainWindow != null)
            {
                mainWindow.ItemPanel.Children.Clear();
                colorBackground = mainWindow.ColoredBackground.IsChecked.GetValueOrDefault(true);
            }
            
            foreach (StatItem item in filteredStatList)
            {
                mainWindow?.ItemPanel.Children.Add(new IconListItem(item.Tag, item.Text, item.Count, colorBackground));
            }
        }

        private bool FilterType(StatItem item)
        {
            return ((mainWindow.TypeFilterBlocks.IsChecked.GetValueOrDefault(true) &&
                    (item.Type == StatType.Mined) || item.Type == StatType.Picked)) ||

                   (mainWindow.TypeFilterMonsters.IsChecked.GetValueOrDefault(true) &&
                    (item.Type == StatType.Killed));
        }

        private bool FilterCount(StatItem item)
        {
            return (mainWindow.CountFilterAll.IsChecked.GetValueOrDefault(true) ||

                   (mainWindow.CountFilter0.IsChecked.GetValueOrDefault(false) &&
                    (item.Count == 0)) ||

                   (mainWindow.CountFilter01.IsChecked.GetValueOrDefault(false) &&
                    (item.Count == 0 || item.Count == 1)) ||

                   (mainWindow.CountFilter1.IsChecked.GetValueOrDefault(false) &&
                    (item.Count == 1)) ||

                   (mainWindow.CountFilter1more.IsChecked.GetValueOrDefault(false) &&
                    (item.Count >= 1)));
        }

        private bool FilterStatView(StatItem item)
        {
            return ((mainWindow.StatViewMined.IsChecked.GetValueOrDefault(false) &&
                    (item.Type == StatType.Mined || item.Type == StatType.Killed)) ||

                   (mainWindow.StatViewPicked.IsChecked.GetValueOrDefault(false) &&
                    (item.Type == StatType.Picked)));
        }

        private bool FilterSearch(StatItem item)
        {
            return item.Text.IndexOf(mainWindow.WordFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private List<StatItem> SortItems(List<StatItem> filteredList)
        {
            if(mainWindow.CurrentSorting == SortType.ItemNameAscending) {
                return SortAlphabetically(filteredList, false);
            } else if(mainWindow.CurrentSorting == SortType.ItemNameDescending)
            {
                return SortAlphabetically(filteredList, true);
            }
            else if (mainWindow.CurrentSorting == SortType.CountAscending)
            {
                return SortCount(filteredList, true);
            }
            else if (mainWindow.CurrentSorting == SortType.CountDescending)
            {
                return SortCount(filteredList, false);
            }
            return filteredList;
        }

        private List<StatItem> SortAlphabetically(List<StatItem> filteredList, bool reversed)
        {
            filteredList.Sort((lhs, rhs) => lhs.Text.CompareTo(rhs.Text));
            if(reversed)
            {
                filteredList.Reverse();
            }
            return filteredList;            
        }

        private List<StatItem> SortCount(List<StatItem> filteredList, bool reversed)
        {
            filteredList.Sort((lhs, rhs) => lhs.Count.CompareTo(rhs.Count));
            if (reversed)
            {
                filteredList.Reverse();
            }
            return filteredList;
        }


        private StatType? GetTagType(string tag)
        {
            if(tag.StartsWith("block"))
            {
                return StatType.Mined;
            } else if(tag.StartsWith("item"))
            {
                return StatType.Picked;
            } else if(tag.StartsWith("entity"))
            {
                return StatType.Killed;
            }
            return null;
        }
    }
}
