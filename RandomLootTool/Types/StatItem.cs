using RandomVisualizerWPF.Utility;

namespace RandomVisualizerWPF.Types
{
    public enum StatType
    {
        Mined,
        Picked,
        Killed
    }

    public struct StatItem
    {
        public StatItem(string tag, int count, StatType type)
        {
            Tag = tag;
            Count = count;
            Type = type;
            Text = DataManager.Instance.TagToName(tag);
        }

        public string Tag { get; private set; }
        public int Count { get; private set; }
        public StatType Type { get; private set; }
        public string Text { get; private set; }

        public override string ToString()
        {
            return "Tag: " + Tag + ", Count: " + Count + ", Type: " + Type + ", Text: '" + Text + "'";
        }
    }
}
