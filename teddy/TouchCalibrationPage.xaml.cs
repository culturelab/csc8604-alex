using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace AlexTheAdventurous
{
    public sealed partial class TouchCalibrationPage : Page
    {
        private Point _topLeft;
        private Point _bottomRight;
        private bool _reportingBR;

        public TouchCalibrationPage()
        {
            InitializeComponent();
        }

        public bool ReportTouch(Point p)
        {
            // this page will be shown only once per application instalation
            // we receive top left and bottom right touch inputs and compute the appropriate scale & offset
            if (_reportingBR)
            {
                _bottomRight = p;
                Calibrate();
                return true;
            }
            else
            {
                _topLeft = p;
                Canvas.SetTop(Cross, ActualHeight - 100);
                Canvas.SetLeft(Cross, ActualWidth - 100);
                _reportingBR = true;
                return false;
            }
        }

        private void Calibrate()
        {
            double screenDiffX = ActualWidth - 200;
            double screenDiffY = ActualHeight - 200;

            double reportedDiffX = _bottomRight.X - _topLeft.X;
            double reportedDiffY = _bottomRight.Y - _topLeft.Y;

            double scaleX = screenDiffX / reportedDiffX;
            double scaleY = screenDiffY / reportedDiffY;

            double offsetX = (102 + Frame.Margin.Left) - _topLeft.X * scaleX;
            double offsetY = (102 + Frame.Margin.Top) - _topLeft.Y * scaleY;

            ApplicationData.Current.LocalSettings.Values["TouchScaleX"] = scaleX;
            ApplicationData.Current.LocalSettings.Values["TouchScaleY"] = scaleY;
            ApplicationData.Current.LocalSettings.Values["TouchOffsetX"] = offsetX;
            ApplicationData.Current.LocalSettings.Values["TouchOffsetY"] = offsetY;

            System.Diagnostics.Debug.WriteLine("Touch TL calibration:" + new Point(_topLeft.X * scaleX + offsetX, _topLeft.Y * scaleY + offsetY));
            System.Diagnostics.Debug.WriteLine("Touch BR calibration:" + new Point(_bottomRight.X * scaleX + offsetX, _bottomRight.Y * scaleY + offsetY));
            Frame.Navigate(typeof(MainPage));
        }
    }
}
