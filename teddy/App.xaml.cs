using System;
using System.IO;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace AlexTheAdventurous
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // initialize peripherals if available
            try
            {
                await Camera.Initialize();
                await Touch.Initialize();
            }
            catch (ArgumentException) {  /*  on current build for phones, SPI device selector is not valid */ }
            catch (FileNotFoundException) { /* on desktop, IoT core is note available */ }

            // inject on-screen keyboard into layout
            KeyboardWithFrame frame = Window.Current.Content as KeyboardWithFrame;
            if (frame == null)
                Window.Current.Content = frame = new KeyboardWithFrame();

            // navigate to the first page
            if (frame.RootFrame.Content == null)
            {
                if (Touch.RequiresCalibration())
                    frame.RootFrame.Navigate(typeof(TouchCalibrationPage), e.Arguments);
                else
                    frame.RootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            Window.Current.Activate();
        }

        public static new App Current
        {
            get { return (App)Application.Current; }
        }

        internal AlexDataSource DataSource
        {
            get { return (AlexDataSource)Resources["AlexDataSource"]; }
        }
    }
}
