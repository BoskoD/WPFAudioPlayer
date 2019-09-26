﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using TagLib;
using System.Windows.Media.Animation;

namespace Audio_Player
{
    public partial class MainWindow : Window
    {
        public List<string> Paths = new List<string>();
        public int CurrentIndex = 0;
        public bool Playing = false;
        private readonly LoadAudio load_audios = new LoadAudio();
        public PlayList mainPL = new PlayList() { Name = "Main Playlist" };
        public List<PlayList> playLists { get; set; } = new List<PlayList>();
        public List<Audio> CurrentList;
        private TimeSpan TotalTime;

        public MainWindow()
        {
            InitializeComponent();
            DeserializeData();
            CurrentList = mainPL.AudioList;
        }

        private void ms_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (RepeatButt.IsChecked == true)
            {
                ms.Position = TimeSpan.Zero;
                ms.Play();
            }
            else
            {
                if (CurrentIndex < CurrentList.Count - 1)
                    ChangeAudio(CurrentList[++CurrentIndex]);
            }
        }

        private void Audio_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var audioContext = ((sender as Grid).DataContext as Audio); // we get the data of the pressed grid
            ChangeAudio(audioContext);// reproduce
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(this.ActualWidth < 850)
            {
                TitleSizer.Visibility = Visibility.Hidden;
                AudioSlider.Visibility = Visibility.Hidden;
            }
            else
            {
                TitleSizer.Visibility = Visibility.Visible;
                AudioSlider.Visibility = Visibility.Visible;
                BotButtons.Children[3].Visibility = Visibility.Visible;
            }

            if(this.ActualWidth < 600)
            {
                BotButtons.Children[5].Visibility = Visibility.Collapsed;
                BotButtons.Children[4].Visibility = Visibility.Collapsed;
            }
            else
            {
                BotButtons.Children[5].Visibility = Visibility.Visible;
                BotButtons.Children[4].Visibility = Visibility.Visible;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MyDialogWindow dialog = new MyDialogWindow();
            dialog.TX.Text = "Save data?";
            dialog.Owner = this;
            dialog.ShowDialog();
            if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
            {
                SerializeData();
            }
        }

        public BitmapImage LoadImage(string text, bool decode)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(text, UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            if (decode)
            {
                bitmap.DecodePixelWidth = 200;
                bitmap.DecodePixelHeight = 200;
            }
            bitmap.EndInit();

            return bitmap;
        }

        private void ChangeAudio(Audio audioContext)
        {
            ms.Source = new Uri(audioContext.DirectoryName + "\\" + audioContext.Name);
            BottomPanel.Visibility = Visibility.Visible;
            TopPlayGrid.DataContext = audioContext; // update the top panel

            Playing = true;
            ms.Play();

            CurrentIndex = CurrentList.IndexOf(audioContext);

            (PlayButton.Child as Image).Source = LoadImage(@"pack://application:,,,/Resources/Pause.png", false);
            
            if(!System.IO.File.Exists(audioContext.DirectoryName + "\\" + audioContext.Name))  // checking file availability before playing
            {
                // if such a file does not exist, a dialog box is created 

                MyDialogWindow dialog = new MyDialogWindow();
                dialog.TX.Text = "This file has been deleted or moved ... Delete data about it?";
                dialog.Owner = this;
                dialog.ShowDialog();
                if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                {
                    playLists = playLists.Select(x => { x.AudioList = x.AudioList.Where
                        (s => s.Name != audioContext.Name || s.DirectoryName != audioContext.DirectoryName).ToList(); x.GetTime(); return x; }).ToList(); // с списка плейлистов, создаем новый список, в котором не будет такого файла
                    mainPL.AudioList = mainPL.AudioList.Where(x => x.Name != audioContext.Name || x.DirectoryName != audioContext.DirectoryName).ToList(); // то же делаем и для главного плейлиста
                    CurrentList = CurrentList.Where(x => x.Name != audioContext.Name || x.DirectoryName != audioContext.DirectoryName).ToList(); // то же и для текущего списка воспроизведения
                    Play.ItemsSource = CurrentList; // updating the main playlist
                    if (CurrentList.Count > 0)
                    {
                        ChangeAudio(CurrentList[--CurrentIndex]); // play the next song
                    }
                    return;
                }
                else
                {
                    ChangeAudio(CurrentList[++CurrentIndex]); // play the next song
                    return;
                }
            }

            TagLib.File Ds = TagLib.File.Create(audioContext.DirectoryName + "\\" + audioContext.Name);
            if (Ds.Tag.Pictures.Length > 0)
            {
                IPicture pic = Ds.Tag.Pictures[0];
                MemoryStream s = new MemoryStream(pic.Data.Data);
                s.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = s;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.DecodePixelWidth = 200;
                bitmap.DecodePixelHeight = 200;
                bitmap.EndInit();

                s.Close();

                Img_Audio.Source = bitmap;              
                GC.Collect();
            }
            else
            {
                Img_Audio.Source = LoadImage(@"pack://application:,,,/Resources/NoImg.jpg", true);
            }
            BottomInfo.DataContext = audioContext;
        }
        
        private void PL_Click(object sender, MouseButtonEventArgs e)
        {
            SetVisiblePlayListInfo();
            Bounce(PListInfo, this.ActualWidth, this.ActualHeight);
            PListInfo.DataContext = null;
            PListInfo.DataContext = (sender as Border).DataContext;
        }

        public void Bounce(Grid elem, double mainWin_ActW, double mainWin_ActH)
        {
            ThicknessAnimation Anim = new ThicknessAnimation
            {
                From = new Thickness(0, -elem.ActualHeight, 0, 0),
                To = new Thickness(0)
            };
            elem.BeginAnimation(MarginProperty, Anim);
        }

    }
}
