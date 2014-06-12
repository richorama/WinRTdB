using System;
using System.ComponentModel;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IoT.WinRT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public MainPage()
        {
            this.InitializeComponent();
            ProgressBar1.Maximum = ToDB(UInt16.MaxValue);
            ProgressBar1.Minimum = ToDB(UInt16.MaxValue);
        }

        MediaCapture media;

        protected override async void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            media = new MediaCapture();
            var captureInitSettings = new MediaCaptureInitializationSettings();
            captureInitSettings.StreamingCaptureMode = StreamingCaptureMode.Audio;
            await media.InitializeAsync(captureInitSettings);
            media.Failed += (_, ex) => new MessageDialog(ex.Message).ShowAsync();
            media.RecordLimitationExceeded += (_) => new MessageDialog("record limit exceeded").ShowAsync();

            var stream = new AudioAmplitudeStream();
            media.StartRecordToStreamAsync(MediaEncodingProfile.CreateWav(AudioEncodingQuality.Low), stream);
            stream.AmplitudeReading += stream_AmplitudeReading;

            base.OnNavigatedTo(e);
        }


        public double Value { get; set; }

        void stream_AmplitudeReading(object sender, double reading)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var dbs = ToDB(reading);
                ProgressBar1.Value = dbs;
                ProgressBar1.Minimum = Math.Min(ProgressBar1.Minimum, dbs);
                TextBlock1.Text = string.Format("{0:0} dB", dbs);

            }).AsTask().Wait();
        }

        static double ToDB(double value)
        {
            return 20 * Math.Log10(Math.Sqrt(value * 2));
        }


        public event PropertyChangedEventHandler PropertyChanged;

    }
}
