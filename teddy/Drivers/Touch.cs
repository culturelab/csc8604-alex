using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace AlexTheAdventurous
{
    // This class is needed to inject SPI touch into the process since we did not have a HID touch display.
    // The display is generic HDMI 5 inch screen with XPT2046 touch controller.
    // http://eud.dx.com/product/5-inch-hdmi-tft-lcd-touch-screen-shield-800-x-480-for-raspberry-pi-2-model-b-b-a-b-844384212#.VVG-zO9FCUk

    internal static class Touch
    {
        private const int TouchSampleInterval = 100;
        private static Timer _touchTimer;
        private static SpiDevice _touch;

        private static readonly byte[] _touchReadX = new byte[] { 0xD0, 0, 0 };
        private static readonly byte[] _touchReadY = new byte[] { 0x90, 0, 0 };
        private static readonly byte[] _touchBufferX = new byte[3];
        private static readonly byte[] _touchBufferY = new byte[3];
        private static bool _touchDown;

        private static double _touchScaleX = 1;
        private static double _touchScaleY = 1;
        private static double _touchOffsetX = 0;
        private static double _touchOffsetY = 0;

        public static bool RequiresCalibration()
        {
            if (_touchTimer == null)
                return false;

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("TouchOffsetY"))
            {
                _touchScaleX = (double)ApplicationData.Current.LocalSettings.Values["TouchScaleX"];
                _touchScaleY = (double)ApplicationData.Current.LocalSettings.Values["TouchScaleY"];
                _touchOffsetX = (double)ApplicationData.Current.LocalSettings.Values["TouchOffsetX"];
                _touchOffsetY = (double)ApplicationData.Current.LocalSettings.Values["TouchOffsetY"];
                return false;
            }

            return true;
        }

        public static async Task Initialize()
        {
            if (_touchTimer == null)
            {
                string spiAqs = SpiDevice.GetDeviceSelector("SPI0");
                DeviceInformationCollection devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);

                if (devicesInfo.Count < 1)
                    return;

                SpiConnectionSettings settings = new SpiConnectionSettings(1);
                settings.Mode = SpiMode.Mode0;
                settings.ClockFrequency = 2000000;

                _touch = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);
                _touchTimer = new Timer(QueryTouch, null, TouchSampleInterval, TouchSampleInterval);
            }
        }

        // ability to suspend touch to not interfere with the camera data transfers

        public static void DisableTouch()
        {
            _touchTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public static void EnableTouch()
        {
            _touchTimer.Change(TouchSampleInterval, TouchSampleInterval);
        }

        private static async void QueryTouch(object state)
        {
            _touch.TransferFullDuplex(_touchReadX, _touchBufferX);
            _touch.TransferFullDuplex(_touchReadY, _touchBufferY);

            int x = (_touchBufferX[1] << 4) | (_touchBufferX[2] >> 4);
            int y = (_touchBufferY[1] << 4) | (_touchBufferY[2] >> 4);

            if (x > 0 && y < 2000) // touch has been detected
            {
                if (_touchDown)
                    return;
                else
                    _touchDown = true;

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    KeyboardWithFrame frame = Window.Current.Content as KeyboardWithFrame;
                    if (frame == null) return;

                    if (frame.RootFrame.Content is TouchCalibrationPage)
                    {
                        if (((TouchCalibrationPage)frame.RootFrame.Content).ReportTouch(new Point(x, y)))
                            RequiresCalibration();

                        return;
                    }

                    Point tap = new Point(x * _touchScaleX + _touchOffsetX, y * _touchScaleY + _touchOffsetY);
                    Debug.WriteLine("Touch injection: " + tap);

                    bool hideKeyboard = false;
                    foreach (UIElement element in VisualTreeHelper.FindElementsInHostCoordinates(tap, Window.Current.Content))
                    {
                        if (element is Button)
                        {
                            AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(element);
                            if (peer == null) continue;

                            IInvokeProvider pattern = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                            if (pattern == null) continue;

                            pattern.Invoke();
                            hideKeyboard = true;
                        }
                        else if (element is TextBox)
                        {
                            ((TextBox)element).Focus(FocusState.Keyboard);
                            frame.FocusedBox = (TextBox)element;
                            frame.IsKeyboardVisible = true;
                            return;
                        }
                        else if (element == frame.KeyboardElement)
                            return;
                    }

                    if (hideKeyboard)
                        frame.IsKeyboardVisible = false;
                });
            }
            else
                _touchDown = false;
        }

        public static void Focus(TextBox box)
        {
            if (_touchTimer == null)
                return;

            KeyboardWithFrame frame = Window.Current.Content as KeyboardWithFrame;
            if (frame == null) return;

            frame.FocusedBox = box;
            frame.IsKeyboardVisible = true;
            box.Focus(FocusState.Keyboard);
        }
    }
}
