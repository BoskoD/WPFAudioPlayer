﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Audio_Player
{

    public partial class MainWindow : Window
    {

        private void AddPlayList_Click(object sender, RoutedEventArgs e)
        {
            CreateListWind Wind = new CreateListWind
            {
                Owner = Application.Current.MainWindow
            };
            this.Opacity = 0.2;
            Wind.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Wind.Show();
        }

        private void PlayList_Click(object sender, RoutedEventArgs e)
        {
            SetVisibleMain();
            OpacityAnim(PlayGrid, sender);
        }

        private void ShowLists_Click(object sender, RoutedEventArgs e)
        {
            SetVisiblePlayListsControl();
            CenterAnim(PListControl);
        }

        private void AddToFolder_Click(object sender, RoutedEventArgs e)
        {
            SetVisibleFolderAdder();
            RideAnim(AddFolder, sender);
        }


        private void CenterAnim(Grid elem)
        {
            ThicknessAnimation Anim = new ThicknessAnimation
            {
                From = new Thickness(-this.ActualWidth + 50, 0, 0, 0),
                To = new Thickness(0)
            };
            elem.BeginAnimation(MarginProperty, Anim);
        }

        private void OpacityAnim(Grid elem, object sender)
        {
            DoubleAnimation Anim = new DoubleAnimation();
            (sender as Button).IsHitTestVisible = false; //disable the button switch to animation time
            Anim.Completed += delegate
            {
                (sender as Button).IsHitTestVisible = true; //turn on the button
            };
            Anim.From = 0;
            Anim.To = 1;
            Anim.Duration = new Duration(TimeSpan.FromSeconds(1));
            elem.BeginAnimation(OpacityProperty, Anim);
        }

        private void RideAnim(Grid elem, object sender)
        {
            ThicknessAnimation Anim = new ThicknessAnimation();
            (sender as Button).IsHitTestVisible = false;
            Anim.Completed += delegate
            {
                (sender as Button).IsHitTestVisible = true;
            };
            Anim.From = new Thickness(-400, 200, 0, 0);
            Anim.To = elem.Margin;
            Anim.Duration = new Duration(TimeSpan.FromSeconds(1));
            elem.BeginAnimation(MarginProperty, Anim);
        }

    }
    
}