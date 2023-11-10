using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Shapes;

namespace SubwaySurfers
{
    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class ToolWindow1Control : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public ToolWindow1Control()
        {
            this.InitializeComponent();
            this.SizeChanged += ToolWindow1Control_SizeChanged;

            CurrentWindowHeight = this.Height;
            CurrentWindowWidth = this.Width;

            mediaPlayer.Height = CurrentWindowHeight;
            mediaPlayer.Width = CurrentWindowWidth;
            mediaPlayer.MouseDown += MediaPlayer_MouseDown;
        }

        public class Test
        {
            public string FileName { get; set; }
        }

        private void ToolWindow1Control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CurrentWindowHeight = this.Height;
            CurrentWindowWidth = this.Width;

            mediaPlayer.Height = CurrentWindowHeight;
            mediaPlayer.Width = CurrentWindowWidth;
            
        }

        bool isPaused = false;

        private void MediaPlayer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isPaused == false)
            {
                mediaPlayer.Pause();
            } 
            else
            {
                mediaPlayer.Play();
            }

            isPaused = !isPaused;
        }

        public double CurrentWindowHeight;
        public double CurrentWindowWidth;
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.IsMuted)
            {
                mediaPlayer.IsMuted = false;
                btnMute.Content = "Mute";
            }
            else
            {
                mediaPlayer.IsMuted = true;
                btnMute.Content = "Unmute";
            }

        }

        static TimeSpan MediaPlayerCurrentPosition;

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is System.Windows.Controls.TabControl)
            {
                // do work when tab is changed

                //MediaPlayerCurrentPosition = mediaPlayer.Position;
                var newTab = (TabItem)tabControl.SelectedItem;
                if (newTab == null)
                    return;

                if (newTab.Header is string && newTab.Header.ToString() == "Media")
                {
                    mediaPlayer.Position = MediaPlayerCurrentPosition;
                }
                else
                {
                    //SaveTimeSpan(mediaPlayer.Position);
                    MediaPlayerCurrentPosition = mediaPlayer.Position;
                }
            }
        }

        private void btnRefreshGrid_Click(object sender, RoutedEventArgs e)
        {
            List<string> files = System.IO.Directory.EnumerateFiles(txtMediaPath.Text, "*.mp4")
                .Union(Directory.EnumerateFiles(txtMediaPath.Text, "*.wmv"))
                .Union(Directory.EnumerateFiles(txtMediaPath.Text, "*.wav"))
                .Union(Directory.EnumerateFiles(txtMediaPath.Text, "*.mp3")).ToList();

            var lst = new List<Test>();

            foreach (string file in files)
            {
                lst.Add(new Test() { FileName = file });
            }

            DG1.DataContext = lst;
        }

        private void chkAutoPlay_Checked(object sender, RoutedEventArgs e)
        {
            if (chkAutoPlay.IsChecked == true)
            {
                chkRandomPlay.Visibility = Visibility.Visible;
            }
            else
            {
                chkRandomPlay.Visibility = Visibility.Collapsed;
            }
        }

        System.Random rand = new System.Random();

        string selectedMediaPlaying = "";

        private void mediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaPlayerCurrentPosition = TimeSpan.Zero;
            if (chkAutoPlay.IsChecked.GetValueOrDefault())
            {
                List<Test> list = DG1.Items.OfType<Test>().ToList();

                var current = list.Where(x => x.FileName == selectedMediaPlaying).FirstOrDefault();

                if (chkRandomPlay.IsChecked.GetValueOrDefault())
                {
                    list.Remove(current);

                    var i = rand.Next(list.Count);

                    selectedMediaPlaying = list[i].FileName;
                    mediaPlayer.Source = new System.Uri(list[i].FileName);
                }
                else
                {
                    var index = list.IndexOf(current);

                    if (index >= list.Count - 1)
                    {
                        mediaPlayer.Source = new System.Uri(list[0].FileName);
                        selectedMediaPlaying = list[0].FileName;
                    }
                    else
                    {
                        var next = list[++index].FileName;
                        selectedMediaPlaying = next;
                        mediaPlayer.Source = new System.Uri(next);
                    }
                }
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //mediaPlayer.Play();
            //btnStart.Visibility = Visibility.Collapsed;
            //btnMute.Visibility = Visibility.Visible;
        }

        private void btnPlayFile_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayerCurrentPosition = TimeSpan.Zero;
            if (DG1.SelectedItem != null)
            {
                var item = DG1.SelectedItem as Test;
                selectedMediaPlaying = item.FileName;
                mediaPlayer.Source = new System.Uri(item.FileName);
                tabControl.SelectedIndex = 1;
                mediaPlayer.Play();
                isPaused = false;     
            }
        }

        private void DG1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (DG1.SelectedItem != null)
            {
                btnPlayFile.Visibility = Visibility.Visible;

                var item = DG1.SelectedItem as Test;

                if (item != null)
                {
                    lblPleaseLoadMedia.Visibility = Visibility.Collapsed;
                    btnMute.Visibility = Visibility.Visible;
                }
                else
                {
                    btnPlayFile.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                btnPlayFile.Visibility= Visibility.Collapsed;
            }
        }
    }
}