using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace RandomVisualizerWPF.Utility
{
    public sealed class ImageManager
    {
        public static ImageManager Instance { get { return Nested.instance; } }

        public BitmapImage? ItemBackgroundImage { get; private set; }

        private static Dictionary<string, BitmapImage> tagToImageDict = new Dictionary<string, BitmapImage>();

        private ImageManager() {}

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly ImageManager instance = new ImageManager();
        }

        public void Initialize()
        {

            foreach(KeyValuePair<string,string> tagImage in DataManager.Instance.TagToImagePathDict)
            {
                BitmapImage? img = AttemptImageLoad(tagImage.Value);
                if (img != null)
                {
                    tagToImageDict.Add(tagImage.Key, img);
                }
            }
        }

        public BitmapImage? GetImageByTag(string tag)
        {
            return tagToImageDict.GetValueOrDefault(tag);
        }

        private BitmapImage? AttemptImageLoad(string relativePath)
        {
            Uri imgUri = new Uri(relativePath, UriKind.RelativeOrAbsolute);

            BitmapImage? img = null;
            //Quick way to test if image exists
            try
            {
                img = new BitmapImage(imgUri);
                var w = img.Width;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                img = null;
            }

            return img;
        }

    }
}
