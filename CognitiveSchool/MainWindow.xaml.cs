using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WebEye.Controls.Wpf;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace CognitiveSchool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WebCameraId _webcam;
        
        private DispatcherTimer _dTimer;
        
        private VideoControl VideoControlObj;
        private EmotionControl EmotionControlObj;

        public MainWindow()
        {
            
            InitializeComponent();
            Utility.InitializeCombobox(ComboBox, Preview);
            Utility.InitializeTimer(_dTimer, DispatcherTimer_Tick);
            EmotionControlObj = new EmotionControl();
            VideoControlObj = new VideoControl();
        }
        
        private void ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Start.IsEnabled = e.AddedItems.Count > 0;
        }
        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            _webcam = (WebCameraId) ComboBox.SelectedItem;
            Preview.StartCapture(_webcam);
        }
        private void OnStopButtonClick(object sender, RoutedEventArgs e)
        {
            Preview.StopCapture();
        }

        private async void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Preview.IsCapturing)
            {
                Bitmap bitmap = Preview.GetCurrentImage();
                bitmap.Save(".\\file.jpg", ImageFormat.Jpeg);
                Stream fileStream = File.OpenRead(".\\file.jpg");

                await EmotionControlObj.StartRecognize(fileStream);
                fileStream.Close();
                if (EmotionControlObj.IsEmotion)
                {
                    Anger.Value = EmotionControlObj.Emotions[0].Scores.Anger;
                    Contempt.Value = EmotionControlObj.Emotions[0].Scores.Contempt;
                    Disgust.Value = EmotionControlObj.Emotions[0].Scores.Disgust;
                    Fear.Value = EmotionControlObj.Emotions[0].Scores.Fear;
                    Happiness.Value = EmotionControlObj.Emotions[0].Scores.Happiness;
                    Neutral.Value = EmotionControlObj.Emotions[0].Scores.Neutral;
                    Sadness.Value = EmotionControlObj.Emotions[0].Scores.Sadness;
                    Surprise.Value = EmotionControlObj.Emotions[0].Scores.Surprise;
                }
            }
        }


        private void Emotion_OnClick(object sender, RoutedEventArgs e)
        {
            CameraMenu.Visibility = Visibility.Visible;
            EmotionTable.Visibility = Visibility.Visible;
            Preview.Visibility = Visibility.Visible;
            VideoStack.Visibility = Visibility.Collapsed;
            VideoControls.Visibility = Visibility.Collapsed;
        }

        private void Video_OnClick(object sender, RoutedEventArgs e)
        {
            CameraMenu.Visibility = Visibility.Collapsed;
            EmotionTable.Visibility = Visibility.Collapsed;
            Preview.Visibility = Visibility.Collapsed;
            VideoStack.Visibility = Visibility.Visible;
            VideoControls.Visibility = Visibility.Visible;
            if (Preview.IsCapturing)
            {
                Preview.StopCapture();
            }
        }
        

        private async void Browse_OnClick(object sender, RoutedEventArgs e)
        {
            VideoControlObj.OpenFile(Utility.PickFile());
            if (VideoControlObj.OriginalFileString == null)
            {
                return;
            }
            else
            {
                StartVideo.IsEnabled = true;
                StopVideo.IsEnabled = true;
                Save.IsEnabled = true;
                Bar.Visibility = Visibility.Visible;
            }

            await VideoControlObj.StabilizeVideo(VidOrig, VidCog);
            
            if (VidCog.IsLoaded)
            {
                Bar.Visibility = Visibility.Collapsed;
            }
        }

        private void StartVideo_OnClick(object sender, RoutedEventArgs e)
        {
            if (VidOrig.IsLoaded)
            {
                VidOrig.Stop();
                VidOrig.Play();
            }

            if (VidCog.IsLoaded)
            {
                VidCog.Stop();

                VidCog.Play();
            }
        }

        private void StopVideo_OnClick(object sender, RoutedEventArgs e)
        {
            if (VidOrig.IsLoaded)
            {
                VidOrig.Stop();
            }
            if (VidCog.IsLoaded)
            {
                VidCog.Stop();
            }
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            if (VidCog.IsLoaded)
            {
                Utility.SaveFile(VideoControlObj.CognitiveFile);
            }
        }
    }
}
