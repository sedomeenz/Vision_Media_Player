using System;
using System.Collections.Generic;
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
    private List<Media> _currentPlaylist = new();
    private int _currentPlaylistIndex = -1;
    private int _previousVolume = 50;
    private bool _isMuted;
    private bool _isPaused;
    private bool _isClosing;

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
        _mediaPlayer.EndReached += MediaPlayer_EndReached;

        _processingTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };

        _processingTimer.Tick += ProgressTimer;
        _processingTimer.Start();

        LoadMedia();
        LoadPlaylists();

        Closed += MainWindow_Closed;
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        _isClosing = true;
        _processingTimer.Stop();
        _mediaPlayer.EndReached -= MediaPlayer_EndReached;
        _mediaPlayer.Stop();
        VideoView.MediaPlayer = null;
        _currentVlcMedia?.Dispose();
        _currentVlcMedia = null;
        _mediaPlayer.Dispose();
    }

    private void LoadMedia()
    {
        MediaList.ItemsSource = _mediaManager.DisplayAllMedia();
    }

    private void LoadPlaylists()
    {
        PlaylistsListBox.ItemsSource = _playlistManager.DisplayAllPlaylists();
    }

    private void AddMedia_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select audio or video",
            Filter = "Media Files|*.mp3;*.wav;*.flac;*.aac;*.m4a;*.wma;*.ogg;*.opus;*.aiff;*.aif;*.mp4;*.mkv;*.avi;*.mov;*.wmv;*.webm;*.m4v",
            Multiselect = true
        };

        if (dialog.ShowDialog() != true)
            return;

        var result = _mediaManager.AddMediaThroughFilePath(dialog.FileNames.ToList());
        LoadMedia();

        MessageBox.Show(
            string.Join(Environment.NewLine, result),
            "Import Results",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

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
            MessageBox.Show("Playlist created successfully!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show("Could not create playlist.", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadPlaylistMedia()
    {
        if (PlaylistsListBox.SelectedItem is not Vision_Media_Player.models.Playlist selectedPlaylist)
        {
            PlaylistMediaListBox.ItemsSource = null;
            return;
        }

        var playlist = _playlistManager.DisplayPlaylist(selectedPlaylist.Id);
        PlaylistMediaListBox.ItemsSource = playlist?.PlaylistMedias
            .Where(pm => pm.Media != null)
            .Select(pm => pm.Media)
            .ToList();
    }

    private void PlaylistsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadPlaylistMedia();
    }

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

        bool result = _playlistManager.AddMediaToPlaylist(selectedPlaylist.Id, selectedMedia.Id);

        if (!result)
        {
            MessageBox.Show(
                "Could not add media. It may already exist in this playlist.",
                "Add Media", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        LoadPlaylistMedia();
        MessageBox.Show("Media added to playlist successfully!", "Success",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

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

        bool result = _playlistManager.DeleteMediaFromPlaylist(selectedPlaylist.Id, selectedMedia.Id);

        if (!result)
        {
            MessageBox.Show("Could not remove media from playlist.");
            return;
        }

        LoadPlaylistMedia();
        MessageBox.Show("Media removed from playlist successfully!", "Success",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void PlayPlaylist_Click(object sender, RoutedEventArgs e)
    {
        if (PlaylistsListBox.SelectedItem is not Vision_Media_Player.models.Playlist selectedPlaylist)
        {
            MessageBox.Show("Please select a playlist first.");
            return;
        }

        var playlist = _playlistManager.DisplayPlaylist(selectedPlaylist.Id);
        var mediaItems = playlist?.PlaylistMedias
            .Where(pm => pm.Media != null)
            .Select(pm => pm.Media)
            .ToList() ?? new List<Media>();

        if (mediaItems.Count == 0)
        {
            MessageBox.Show("This playlist is empty.", "Playlist",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _currentPlaylist = mediaItems;
        _currentPlaylistIndex = 0;
        PlayCurrentPlaylistItem();
    }

    private void PlayCurrentPlaylistItem()
    {
        while (_currentPlaylistIndex >= 0 && _currentPlaylistIndex < _currentPlaylist.Count)
        {
            Media item = _currentPlaylist[_currentPlaylistIndex];

            if (PlayMedia(item))
                return;

            _currentPlaylistIndex++;
        }

        _currentPlaylist.Clear();
        _currentPlaylistIndex = -1;
        MessageBox.Show("The playlist has finished or no playable files remain.",
            "Playlist", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MediaPlayer_EndReached(object? sender, EventArgs e)
    {
        if (_isClosing)
            return;

        Dispatcher.BeginInvoke(new Action(() =>
        {
            if (_isClosing || _currentPlaylist.Count == 0)
                return;

            _currentPlaylistIndex++;
            PlayCurrentPlaylistItem();
        }));
    }

    private void AllMedia_Click(object sender, RoutedEventArgs e)
    {
        MediaList.ItemsSource = _mediaManager.DisplayAllMedia();
    }

    private void Audio_Click(object sender, RoutedEventArgs e)
    {
        MediaList.ItemsSource = _mediaManager.DisplayAllAudio();
    }

    private void Video_Click(object sender, RoutedEventArgs e)
    {
        MediaList.ItemsSource = _mediaManager.DisplayAllVideos();
    }

    private void Play_Click(object sender, RoutedEventArgs e)
    {
        if (MediaList.SelectedItem is not Media selectedMedia)
        {
            MessageBox.Show("Please select a media file first.");
            return;
        }

        ClearPlaylistPlayback();
        PlayMedia(selectedMedia);
    }

    private void MediaList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (MediaList.SelectedItem is Media selectedMedia)
        {
            ClearPlaylistPlayback();
            PlayMedia(selectedMedia);
        }
    }

    private bool PlayMedia(Media selectedMedia)
    {
        var app = (App)Application.Current;
        string fullPath = app.StorageManager.GetFullPath(selectedMedia.FilePath);

        if (!File.Exists(fullPath))
        {
            MessageBox.Show(
                $"Media file not found:\n{fullPath}",
                "Playback Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return false;
        }

        try
        {
            _mediaPlayer.Stop();
            _currentVlcMedia?.Dispose();

            _currentVlcMedia = new LibVLCSharp.Shared.Media(
                _libVLC,
                fullPath,
                FromType.FromPath);

            bool playbackStarted = _mediaPlayer.Play(_currentVlcMedia);

            if (!playbackStarted)
            {
                _currentVlcMedia.Dispose();
                _currentVlcMedia = null;
                MessageBox.Show("Playback Failed.", "Playback Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            _isPaused = false;
            PauseButton.Content = "Ⅱ";
            NowPlayingText.Text = selectedMedia.Name;
            CurrentTimeText.Text = "00:00";
            DurationText.Text = selectedMedia.Duration.ToString(@"hh\:mm\:ss");
            ProgressSlider.Value = 0;
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not play this file:\n{ex.Message}",
                "Playback Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
    }

    private void ClearPlaylistPlayback()
    {
        _currentPlaylist.Clear();
        _currentPlaylistIndex = -1;
    }

    private void PauseResume_Click(object sender, RoutedEventArgs e)
    {
        if (_currentVlcMedia == null)
            return;

        if (_isPaused)
        {
            _mediaPlayer.SetPause(false);
            _isPaused = false;
            PauseButton.Content = "Ⅱ";
        }
        else if (_mediaPlayer.IsPlaying)
        {
            _mediaPlayer.SetPause(true);
            _isPaused = true;
            PauseButton.Content = "▶";
        }
    }

    private void stop_Click(object sender, RoutedEventArgs e)
    {
        ClearPlaylistPlayback();
        _mediaPlayer.Stop();
        _isPaused = false;
        PauseButton.Content = "Ⅱ";
        ProgressSlider.Value = 0;
        CurrentTimeText.Text = "00:00";
        DurationText.Text = "00:00";
    }

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

    private void ProgressSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        SeekToSliderPosition();
    }

    private void ProgressSlider_KeyUp(object sender, KeyEventArgs e)
    {
        SeekToSliderPosition();
    }

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
            if (MuteButton != null)
                MuteButton.Content = "🔊";        }
        else
        {
            _isMuted = true;
            if (MuteButton != null)
                MuteButton.Content = "🔇";        }
    }

    private void Mute_Click(object sender, RoutedEventArgs e)
    {
        if (_isMuted)
        {
            int volume = Math.Max(1, _previousVolume);

            VolumeSlider.Value = volume;
            _mediaPlayer.Volume = volume;

            _isMuted = false;
            MuteButton.Content = "🔊";
        }
        else
        {
            if (VolumeSlider.Value > 0)
                _previousVolume = (int)VolumeSlider.Value;

            VolumeSlider.Value = 0;
            _mediaPlayer.Volume = 0;

            _isMuted = true;
            MuteButton.Content = "🔇";
        }
    }

    private static string FormatTime(long milliseconds)
    {
        if (milliseconds < 0)
            milliseconds = 0;

        var time = TimeSpan.FromMilliseconds(milliseconds);
        return time.TotalHours >= 1
            ? time.ToString(@"h\:mm\:ss")
            : time.ToString(@"mm\:ss");
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string title = SearchTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(title))
        {
            LoadMedia();
            return;
        }

        MediaList.ItemsSource = _mediaManager.SearchMediaByTitle(title);
    }

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

        if (_currentVlcMedia != null &&
            string.Equals(NowPlayingText.Text, selectedMedia.Name, StringComparison.OrdinalIgnoreCase))
        {
            ClearPlaylistPlayback();
            _mediaPlayer.Stop();
            _currentVlcMedia.Dispose();
            _currentVlcMedia = null;
            _isPaused = false;
            PauseButton.Content = "Ⅱ";
        }

        string message = _mediaManager.RemoveMedia(selectedMedia.Id);
        LoadMedia();
        LoadPlaylists();

        MessageBox.Show(message, "Delete Result",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
