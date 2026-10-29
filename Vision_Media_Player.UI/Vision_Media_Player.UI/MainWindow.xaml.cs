using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Vision_Media_Player.models;
using Vision_Media_Player.Services;

namespace Vision_Media_Player.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MediaManager _mediaManager;

    public MainWindow()
    {
        InitializeComponent();
        
        _mediaManager = ((App)Application.Current).MediaManager;
        
        LoadMedia();
    }

    private void LoadMedia()
    {
        MediaList.ItemsSource = _mediaManager.DisplayAllMedia();
    }

    private void AddMedia_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog()
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
    
    
    
    
    
    
}