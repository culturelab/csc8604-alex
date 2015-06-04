using System;
using System.IO;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace AlexTheAdventurous
{
    public sealed partial class MainPage : Page
    {
        private static GpioPin RfidReset;
        private static GpioPin RfidEvent;

        static MainPage()
        {
            try
            {
                GpioController gpio = GpioController.GetDefault();

                if (gpio != null)
                {
                    RfidEvent = gpio.OpenPin(22);
                    RfidEvent.SetDriveMode(GpioPinDriveMode.Input);

                    RfidReset = gpio.OpenPin(27);
                    RfidReset.SetDriveMode(GpioPinDriveMode.Output);
                }
            }
            catch (FileNotFoundException) { /* on desktop, IoT core is note available */ }
        }

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame.BackStack.Clear();

            if (RfidEvent != null)
            {
                // reset the SM130 module, which will start the seek for tag command in I2C mode
                RfidReset.Write(GpioPinValue.High);
                RfidReset.Write(GpioPinValue.Low);

                RfidEvent.ValueChanged += OnRfidEvent;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            // navigate to moderation page only from this page

            if (RfidEvent != null)
                RfidEvent.ValueChanged -= OnRfidEvent;
        }

        private void OnTileClicked(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = (FrameworkElement)sender;
            AlexTask task = (AlexTask)button.DataContext;

            if (task == AlexTask.CreateNewTask)
                Frame.Navigate(typeof(NewTaskPage));
            else
                Frame.Navigate(typeof(CameraPage), new AlexPost { Task = task });
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
#if DEBUG
            if (e.Key == Windows.System.VirtualKey.M)
                Frame.Navigate(typeof(ModerationList)); // for testing moderation without RFID
#endif
        }

        private async void OnRfidEvent(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            // SM130 sends a high pulse when RFID tag was found
            // the navigation has to occur from the UI thread

            if (args.Edge == GpioPinEdge.RisingEdge)
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => Frame.Navigate(typeof(ModerationList)));
        }
    }
}
