using Microsoft.WindowsAPICodePack.Dialogs;
using RandomVisualizerWPF.Types;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace RandomVisualizerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool FilterModified { get; set; }

        private static readonly TimeSpan MAIN_LOOP_DELAY = new TimeSpan(10 * 1000 * 100); //100ms

        private static readonly RotateTransform ROTATE_IMAGE_TRANSFORM = new RotateTransform(180);
        private static readonly SolidColorBrush BORDER_COLOR_BRUSH = new SolidColorBrush(Colors.Black);

        private Program program = new Program();
        private DispatcherTimer timer = new DispatcherTimer();

        private string? lastSavePath;

        public SortType CurrentSorting { get; private set; } = SortType.ItemNameAscending;

        public MainWindow()
        {
            InitializeComponent();

            FilterModified = true;

            //Default settings of filters
            TypeFilterBlocks.IsChecked = true;
            TypeFilterMonsters.IsChecked = true;
            CountFilterAll.IsChecked = true;
            CountFilter0.IsChecked = false;
            CountFilter01.IsChecked = false;
            CountFilter1.IsChecked = false;
            CountFilter1more.IsChecked = false;
            StatViewMined.IsChecked = true;
            StatViewPicked.IsChecked = false;
            MiscShowHidden.IsChecked = false;
            ColoredBackground.IsChecked = true;

            //Setup click events
            BrowseSaveFileButton.Click += BrowseSaveFilePressed;

            TypeFilterBlocks.Click += FilterButtonClicked;
            TypeFilterMonsters.Click += FilterButtonClicked;
            CountFilterAll.Click += FilterButtonClicked;
            CountFilter0.Click += FilterButtonClicked;
            CountFilter01.Click += FilterButtonClicked;
            CountFilter1.Click += FilterButtonClicked;
            CountFilter1more.Click += FilterButtonClicked;
            StatViewMined.Click += FilterButtonClicked;
            StatViewPicked.Click += FilterButtonClicked;
            MiscShowHidden.Click += FilterButtonClicked;
            ColoredBackground.Click += FilterButtonClicked;

            ItemSortButton.Click += ItemSortButtonClicked;
            CountSortButton.Click += CountSortButtonClicked;

            WordFilter.TextChanged += SearchFilterChanged;

            ItemScrollPanel.PreviewMouseWheel += ScrollItems;

            this.Loaded += MainWindowLoaded;

            UpdateSortButtons();
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            program.SetMainWindow(this);

            timer.Interval = MAIN_LOOP_DELAY;
            timer.Tick += MainLoop;
            timer.Start();
        }

        public string? GetSaveDirectory()
        {
            return lastSavePath;
        }

        private void MainLoop(object? sender, EventArgs e)
        {
            program.Run();
        }

        private void ScrollItems(object sender, MouseWheelEventArgs e)
        {
            if (Control.ModifierKeys.HasFlag(Keys.Control)) { 
                if(e.Delta > 0)
                {
                    ZoomOutItems();
                }
                else
                {
                    ZoomInItems();
                }
            }
        }

        private void BrowseSaveFilePressed(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog browseSaveDirectoryDialog = new CommonOpenFileDialog();
            browseSaveDirectoryDialog.IsFolderPicker = true;

            CommonFileDialogResult result = browseSaveDirectoryDialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string filename = browseSaveDirectoryDialog.FileName;
                lastSavePath = filename;
                string dirName = new DirectoryInfo(filename).Name;
                SaveFileText.Text = dirName;
            }
        }

        private void FilterButtonClicked(object sender, RoutedEventArgs e)
        {
            FilterModified = true;
        }

        private void SearchFilterChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FilterModified = true;
        }

        private void ItemSortButtonClicked(object sender, RoutedEventArgs e)
        {
            if(CurrentSorting == SortType.ItemNameAscending)
            {
                CurrentSorting = SortType.ItemNameDescending;
            } else
            {
                CurrentSorting = SortType.ItemNameAscending;
            }
            UpdateSortButtons();
            FilterModified = true;
        }

        private void CountSortButtonClicked(object sender, RoutedEventArgs e)
        {
            if (CurrentSorting == SortType.CountAscending)
            {
                CurrentSorting = SortType.CountDescending;
            }
            else
            {
                CurrentSorting = SortType.CountAscending;
            }
            UpdateSortButtons();
            FilterModified = true;
        }


        private void ZoomInItems()
        {
            if (ItemScale.ScaleX > 0.2)
            {
                ItemScale.ScaleX -= 0.1;
            }
            if (ItemScale.ScaleY > 0.2)
            {
                ItemScale.ScaleY -= 0.1;
            }
        }

        private void ZoomOutItems()
        {
            if (ItemScale.ScaleX < 5)
            {
                ItemScale.ScaleX += 0.1;
            }
            if (ItemScale.ScaleY < 5)
            {
                ItemScale.ScaleY += 0.1;
            }
        }

        private void UpdateSortButtons()
        {
            switch (CurrentSorting)
            {
                case SortType.ItemNameAscending:
                    ItemButtonBorder.BorderBrush = BORDER_COLOR_BRUSH;
                    CountButtonBorder.BorderBrush = null;
                    ItemSortImage.RenderTransform = null;
                    break;
                case SortType.ItemNameDescending:
                    ItemButtonBorder.BorderBrush = BORDER_COLOR_BRUSH;
                    CountButtonBorder.BorderBrush = null;
                    ItemSortImage.RenderTransform = ROTATE_IMAGE_TRANSFORM;
                    break;
                case SortType.CountAscending:
                    ItemButtonBorder.BorderBrush = null;
                    CountButtonBorder.BorderBrush = BORDER_COLOR_BRUSH;
                    CountSortImage.RenderTransform = null;
                    break;
                case SortType.CountDescending:
                    ItemButtonBorder.BorderBrush = null;
                    CountButtonBorder.BorderBrush = BORDER_COLOR_BRUSH;
                    CountSortImage.RenderTransform = ROTATE_IMAGE_TRANSFORM;
                    break;
                default:
                    break;
            }
        }
    }
}
