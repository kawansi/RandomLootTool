using RandomVisualizerWPF.Utility;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RandomVisualizerWPF.UIControls
{
    public class IconListItem : FrameworkElement
    {
        private static readonly Pen BOTTOM_LINE_PEN = new Pen(Brushes.Black, 1);

        private static readonly double COUNT_SPLITTER_RIGHT_OFFSET = 100.0;
        private static readonly int ITEM_HEIGHT = 54;

        private static readonly SolidColorBrush CUSTOM_LIGHT_GRAY = new SolidColorBrush(Color.FromRgb(240, 240, 240));
        private static readonly SolidColorBrush CUSTOM_LIGHT_GREEN = new SolidColorBrush(Color.FromRgb(202, 235, 202));
        private static readonly SolidColorBrush CUSTOM_LIGHT_RED = new SolidColorBrush(Color.FromRgb(235, 197, 197));

        private readonly string tag;
        private readonly string text;
        private readonly int count;
        private readonly bool colorBackground;

        public IconListItem(string tag, string text, int count, bool colorBackground)
        {
            this.tag = tag;
            this.text = text;
            this.count = count;
            this.colorBackground = colorBackground;

            Height = ITEM_HEIGHT;
        }

        protected override void OnRender(DrawingContext dc)
        {

            Rect backgroundRect = new Rect(0, 0, ActualWidth, ActualHeight);
            if(colorBackground)
            {
                if(count == 0)
                {
                    dc.DrawRectangle(CUSTOM_LIGHT_RED, null, backgroundRect);
                } else
                {
                    dc.DrawRectangle(CUSTOM_LIGHT_GREEN, null, backgroundRect);
                }
            } else
            {
                dc.DrawRectangle(CUSTOM_LIGHT_GRAY, null, backgroundRect);
            }

            double desiredWidth = 48;
            double desiredHeight = 48;

            BitmapImage? iconImage = ImageManager.Instance.GetImageByTag(tag);
            if (iconImage != null)
            {

                Rect imgRect = CalculateIconRect(desiredWidth - 6, desiredHeight - 6, iconImage.Width, iconImage.Height);
                //Rect imgRect = new Rect(0, 0, iconWidth - 6, iconHeight - 6);
                imgRect.Offset(12, 4);
                dc.DrawImage(iconImage, imgRect);
            }

            double textXOffset = 10 + 48 + 30; //Icon offset + Icon width + 30 spacing
            dc.DrawText(MakeFormattedItemText(text,
                                              ActualWidth - COUNT_SPLITTER_RIGHT_OFFSET - textXOffset),
                        new Point(textXOffset, Height / 2 - 12));

            dc.DrawText(MakeFormattedCountText(count, COUNT_SPLITTER_RIGHT_OFFSET),
                        new Point(ActualWidth - COUNT_SPLITTER_RIGHT_OFFSET + 16, Height / 2 - 14));

            Point bottomLeft = new Point(0, Height - 1);
            Point bottomRight = new Point(ActualWidth, Height - 1);
            dc.DrawLine(BOTTOM_LINE_PEN, bottomLeft, bottomRight);

            Point countSplitterTop = new Point(ActualWidth - COUNT_SPLITTER_RIGHT_OFFSET, 0);
            Point countSplitterBottom = new Point(ActualWidth - COUNT_SPLITTER_RIGHT_OFFSET, Height);
            dc.DrawLine(BOTTOM_LINE_PEN, countSplitterTop, countSplitterBottom);
        }

        private FormattedText MakeFormattedItemText(string text, double maxWidth)
        {
            FormattedText formattedText = new FormattedText(text,
                                                            CultureInfo.GetCultureInfo("en-us"),
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Miriam Libre"),
                                                            16,
                                                            Brushes.Black,
                                                            1.25);

            formattedText.MaxTextWidth = maxWidth;

            formattedText.SetFontWeight(FontWeights.Bold);
            formattedText.MaxLineCount = 1;


            return formattedText;
        }

        private FormattedText MakeFormattedCountText(int count, double maxWidth)
        {

            FormattedText formattedText = new FormattedText(count.ToString(),
                                                            CultureInfo.GetCultureInfo("en-us"),
                                                            FlowDirection.LeftToRight,
                                                            new Typeface("Miriam Libre"),
                                                            22,
                                                            Brushes.Black,
                                                            1.25);

            formattedText.MaxTextWidth = maxWidth;

            formattedText.SetFontWeight(FontWeights.Bold);
            formattedText.MaxLineCount = 1;

            return formattedText;
        }

        private Rect CalculateIconRect(double desiredWidth, double desiredHeight, double iconWidth, double iconHeight)
        {
            if(iconWidth < iconHeight)
            {
                double scale = iconHeight / desiredHeight;
                return new Rect(0, 0, iconWidth/scale, iconHeight/scale);
            } else
            {
                double scale = iconWidth / desiredWidth;
                return new Rect(0, 0, iconWidth / scale, iconHeight / scale);
            }
        }
    }
}
