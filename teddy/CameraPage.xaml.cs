using System;
using System.Threading;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace AlexTheAdventurous
{
    public sealed partial class CameraPage : Page
    {
        private MediaCapture _mediaCapture;
        private AlexPost _post;

        public CameraPage()
        {
            InitializeComponent();
            _cameraTimer = new Timer(PreviewFrame, null, Timeout.Infinite, Timeout.Infinite); // * manual preview timer for SPI camera
        }

        private bool IsCameraAvailable
        {
            get { return _mediaCapture != null && _mediaCapture.MediaCaptureSettings.VideoDeviceId != ""; }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _post = (AlexPost)e.Parameter;

            if (!IsCameraAvailable)
            {
                // try to initialize the camera
                _mediaCapture = new MediaCapture();
                try { await _mediaCapture.InitializeAsync(); }
                catch { _mediaCapture = null; }
            }

            if (IsCameraAvailable)
            {
                // start live preview
                PreviewCanvas.Source = _mediaCapture;
                await _mediaCapture.StartPreviewAsync();
            }
            else                                          // *
                _cameraTimer.Change(0, Timeout.Infinite); // * start SPI camera preview 
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (IsCameraAvailable)
                await _mediaCapture.StopPreviewAsync();
            else                                                         // *
                _cameraTimer.Change(Timeout.Infinite, Timeout.Infinite); // * stop SPI camera preview
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private async void TakePicture(object sender, RoutedEventArgs e)
        {
            if (IsCameraAvailable)
            {
                InMemoryRandomAccessStream jpegStream = new InMemoryRandomAccessStream();
                await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), jpegStream);

                _post.PhotoData = jpegStream;
            }
            else                                  // *
                _post.PhotoData = _previewStream; // * use the last received picture from SPI camera

            Frame.Navigate(typeof(CameraConfirmPage), _post);
        }


        #region * SPI camera

        private Timer _cameraTimer;
        private InMemoryRandomAccessStream _previewStream;
        private async void PreviewFrame(object state)
        {
            _previewStream = await Camera.TakePicture();
            if (_previewStream == null)
                return;

            await Dispatcher.RunIdleAsync(_ =>
            {
                BitmapImage preview = new BitmapImage();
                _previewStream.Seek(0);
                preview.SetSource(_previewStream);

                AlternatePreview.Source = preview;
            });
            _cameraTimer.Change(777, Timeout.Infinite);
        }

        #endregion
    }
}
