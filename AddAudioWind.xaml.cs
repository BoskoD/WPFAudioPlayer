using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Audio_Player
{
    /// <summary>
    /// Interaction logic for AddAudioWind.xaml
    /// </summary>
    public partial class AddAudioWind : Window
    {
        private readonly List<Audio> SelectedAudioList = new List<Audio>();
        public int Index;

        public AddAudioWind()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) //if checkbox songs are checked
        {
            SelectedAudioList.Add((sender as CheckBox).DataContext as Audio);  //add a song to the list
            if (SelectedAudioList.Count == 1)
            {
                OkB.IsEnabled = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)// ec if unchecked
        {
            SelectedAudioList.Remove((sender as CheckBox).DataContext as Audio); //delete a song from the list
            if (SelectedAudioList.Count == 0)
            {
                OkB.IsEnabled = false;
            }
        }

        private void OkB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            RefreshMainPlayLists(ref main);
            main.Opacity = 1;
            this.Close();
        }

        private void CancB_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = this.Owner as MainWindow;
            main.Opacity = 1;
            this.Close();
        }

        private void RefreshMainPlayLists(ref MainWindow main)
        {
            main.playLists[Index].AudioList.AddRange(SelectedAudioList); //add songs to the playlist
            main.PL_ListBox.ItemsSource = new List<PlayList>(main.playLists); //updating the list of playlists
            main.playLists[Index].GetTime();//recount the duration for the playlist
            main.PListInfo.DataContext = null; 
            main.PListInfo.DataContext = main.playLists[Index]; // refresh playlist page
        }
    }
}
