
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using LibVLCSharp.Shared;
using Vision_Media_Player.Services;
using Media = Vision_Media_Player.models.Media;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace Vision_Media_Player.UI;

public partial class MainWindow : Window
{
    private readonly MediaManager _mediaManager;
    private readonly PlaylistManager _playlistManager;
    private readonly LibVLC _libVLC;
    private readonly MediaPlayer _mediaPlayer;
    private readonly DispatcherTimer _processingTimer;

    private LibVLCSharp.Shared.Media? _currentVlcMedia;
    private int _previousVolume = 50;
    private bool _isMuted;

    public MainWindow()
    {
        InitializeComponent();

        var app = (App)Application.Current;

        _mediaManager = app.MediaManager;
        _playlistManager = app.PlaylistManager;
        _libVLC = app.LibVlc;

        _mediaPlayer = new MediaPlayer(_libVLC);
        VideoView.MediaPlayer = _mediaPlayer;

        _mediaPlayer.Volume = 50;

        _processingTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };

        _processingTimer.Tick += ProgressTimer;
        _processingTimer.Start();

        LoadMedia();
        LoadPlaylists();

        Closed += (_, _) =>
        {
            _processingTimer.Stop();
            _mediaPlayer.Stop();

            VideoView.MediaPlayer = null;

            _mediaPlayer.Dispose();
            _currentVlcMedia?.Dispose();
            _currentVlcMedia = null;
        };
    }
    
    // Load media list
    private void LoadMedia()
    {
        MediaList.ItemsSource = _mediaManager.DisplayAllMedia();
    }
    
    // Loads all playlists
    private void LoadPlaylists()  
    {
        var playlists = _playlistManager.DisplayAllPlaylists();

        PlaylistsListBox.ItemsSource = playlists;
    }
    
    // Add media 
    private void AddMedia_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select audio or video",
            Filter =
                "Media Files|*.mp3;*.wav;*.flac;*.aac;*.m4a;*.wma;*.ogg;*.opus;*.aiff;*.aif;*.mp4;*.mkv;*.avi;*.mov;*.wmv;*.webm;*.m4v",
            Multiselect = true
        };

        if (dialog.ShowDialog() != true)
            return;

        var result = _mediaManager.AddMediaThroughFilePath(
            dialog.FileNames.ToList());

        LoadMedia();

        MessageBox.Show(
            string.Join(Environment.NewLine, result),
            "Import Results",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    // Create playlist
    private void CreatePlaylist_Click(object sender, RoutedEventArgs e)
    {
        string playlistName = Interaction.InputBox(
            "Enter playlist name:",
            "Create New Playlist",
            "");

        if (string.IsNullOrWhiteSpace(playlistName))
            return;

        bool result = _playlistManager.CreateNewPlaylist(playlistName);

        if (result)
        {
            LoadPlaylists();

            MessageBox.Show(
                "Playlist created successfully!",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(
                "Could not create playlist.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    
    // Loads the media items in the selected playlist
    private void LoadPlaylistMedia()
    {
        if (PlaylistsListBox.SelectedItem is not Vision_Media_Player.models.Playlist selectedPlaylist)
        {
            PlaylistMediaListBox.ItemsSource = null;
            return;
        }

        var playlist = _playlistManager.DisplayPlaylist(selectedPlaylist.Id);

        PlaylistMediaListBox.ItemsSource = playlist?.PlaylistMedias
            .Select(pm => pm.Media)
            .ToList();
    }
    
    
    private void PlaylistsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadPlaylistMedia();
    }
    
    // Add media to a playlist
    private void AddMediaToPlaylist_Click(object sender, RoutedEventArgs e)
    {
        if (PlaylistsListBox.SelectedItem is not Vision_Media_Player.models.Playlist selectedPlaylist)
        {
            MessageBox.Show("Please select a playlist first.");
            return;
        }

        if (MediaList.SelectedItem is not Media selectedMedia)
        {
            MessageBox.Show("Please select a media file first.");
            return;
        }

        bool result = _playlistManager.AddMediaToPlaylist(
            selectedPlaylist.Id,
            selectedMedia.Id);

        if (!result)
        {
            MessageBox.Show(
                "Could not add media. It may already exist in this playlist.",
                "Add Media",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        LoadPlaylistMedia();

        MessageBox.Show(
            "Media added to playlist successfully!",
            "Success",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
    
    // Remove media from a playlist
    private void RemoveMediaFromPlaylist_Click(object sender, RoutedEventArgs e)
    {
        if (PlaylistsListBox.SelectedItem is not Vision_Media_Player.models.Playlist selectedPlaylist)
        {
            MessageBox.Show("Please select a playlist first.");
            return;
        }

        if (PlaylistMediaListBox.SelectedItem is not Media selectedMedia)
        {
            MessageBox.Show("Please select a media file from the playlist.");
            return;
        }

        var confirmation = MessageBox.Show(
            $"Remove '{selectedMedia.Name}' from this playlist?",
            "Confirm Removal",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmation != MessageBoxResult.Yes)
            return;

        bool result = _playlistManager.DeleteMediaFromPlaylist(
            selectedPlaylist.Id,
            selectedMedia.Id);

        if (!result)
        {
            MessageBox.Show("Could not remove media from playlist.");
            return;
        }

        LoadPlaylistMedia();

        MessageBox.Show(
            "Media removed from playlist successfully!",
            "Success",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
    
    // Show all media 
    private void AllMedia_Click(object sender, RoutedEventArgs e)
    {
        MediaList.ItemsSource = _mediaManager.DisplayAllMedia();
    }
    
    // Show all audio 
    private void Audio_Click(object sender, RoutedEventArgs e)
    {
        MediaList.ItemsSource = _mediaManager.DisplayAllAudio();
    }
    
    // Show all videos 
    private void Video_Click(object sender, RoutedEventArgs e)
    {
        MediaList.ItemsSource = _mediaManager.DisplayAllVideos();
    }
    
    // Plays the selected media item
    private void Play_Click(object sender, RoutedEventArgs e)
    {
        if (MediaList.SelectedItem is not Media selectedMedia)
        {
            MessageBox.Show("Please select a media file first.");
            return;
        }

        PlayMedia(selectedMedia);
    }

    private void MediaList_MouseDoubleClick(
        object sender, MouseButtonEventArgs e)
    {
        if (MediaList.SelectedItem is Media selectedMedia)
        {
            PlayMedia(selectedMedia);
        }
    }

    private void PlayMedia(Media selectedMedia)
    {
        var app = (App)Application.Current;

        string fullPath = app.StorageManager.GetFullPath(
            selectedMedia.FilePath);

        if (!File.Exists(fullPath))
        {
            MessageBox.Show(
                $"Media file not found:\n{fullPath}",
                "Playback Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        _mediaPlayer.Stop();

        _currentVlcMedia?.Dispose();

        _currentVlcMedia = new LibVLCSharp.Shared.Media(
            _libVLC,
            fullPath,
            FromType.FromPath);

        bool playbackStarted = _mediaPlayer.Play(_currentVlcMedia);

        if (!playbackStarted)
        {
            MessageBox.Show(
                "Playback Failed",
                "Playback Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // Stop 
    private void stop_Click(object sender, RoutedEventArgs e)
    {
        _mediaPlayer.Stop();

        ProgressSlider.Value = 0;
        CurrentTimeText.Text = "00:00";
    }

    // Updates playback progress and time labels
    private void ProgressTimer(object? sender, EventArgs e)
    {
        long duration = _mediaPlayer.Length;
        long currentTime = _mediaPlayer.Time;

        if (duration <= 0)
            return;

        ProgressSlider.Maximum = duration;

        if (!ProgressSlider.IsMouseCaptureWithin)
        {
            ProgressSlider.Value = Math.Clamp(
                (double)currentTime,
                ProgressSlider.Minimum,
                ProgressSlider.Maximum);
        }

        CurrentTimeText.Text = FormatTime(currentTime);
        DurationText.Text = FormatTime(duration);
    }

    private void ProgressSlider_PreviewMouseLeftButtonUp(
        object sender, MouseButtonEventArgs e)
    {
        SeekToSliderPosition();
    }

    private void ProgressSlider_KeyUp(
        object sender, KeyEventArgs e)
    {
        SeekToSliderPosition();
    }
    
    // Seeks playback to the selected position
    private void SeekToSliderPosition()
    {
        if (_mediaPlayer.Length <= 0)
            return;

        _mediaPlayer.Time = (long)ProgressSlider.Value;
    }

    private void VolumeSlider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        if (_mediaPlayer is null)
            return;

        int volume = (int)VolumeSlider.Value;

        _mediaPlayer.Volume = volume;

        if (volume > 0)
        {
            _previousVolume = volume;
            _isMuted = false;
            MuteButton.Content = "Mute";
        }
        else
        {
            _isMuted = true;
            MuteButton.Content = "Unmute";
        }
    }

    // Toggles audio mute
    private void Mute_Click(object sender, RoutedEventArgs e)
    {
        if (_isMuted)
        {
            int volume = Math.Max(1, _previousVolume);

            VolumeSlider.Value = volume;
            _mediaPlayer.Volume = volume;

            _isMuted = false;
            MuteButton.Content = "Mute";
        }
        else
        {
            if (VolumeSlider.Value > 0)
            {
                _previousVolume = (int)VolumeSlider.Value;
            }

            VolumeSlider.Value = 0;
            _mediaPlayer.Volume = 0;

            _isMuted = true;
            MuteButton.Content = "Unmute";
        }
    }

    // Formats milliseconds into a readable time string
    private static string FormatTime(long milliseconds)
    {
        if (milliseconds < 0)
            milliseconds = 0;

        var time = TimeSpan.FromMilliseconds(milliseconds);

        return time.TotalHours >= 1
            ? time.ToString(@"h\:mm\:ss")
            : time.ToString(@"mm\:ss");
    }
    
    // Search Box
    private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        string title = SearchTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            LoadMedia();
            return;
        }

        MediaList.ItemsSource = _mediaManager.SearchMediaByTitle(title);
    }
    
    // Delete Media

    private void DeleteMedia_Click(object sender, RoutedEventArgs e)
    {
        if (MediaList.SelectedItem is not Media selectedMedia)
        {
            MessageBox.Show("Please select a media file first.");
            return;
        }

        var result = MessageBox.Show(
            $"Are you sure you want to delete '{selectedMedia.Name}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;

        string message = _mediaManager.RemoveMedia(selectedMedia.Id);

        LoadMedia();

        MessageBox.Show(
            message,
            "Delete Result",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }
    

}
