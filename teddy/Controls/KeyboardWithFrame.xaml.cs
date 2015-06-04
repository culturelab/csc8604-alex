using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AlexTheAdventurous
{
    public sealed partial class KeyboardWithFrame : UserControl
    {
        public KeyboardWithFrame()
        {
            InitializeComponent();
        }

        public Frame RootFrame
        {
            get { return _rootFrame; }
        }

        public UIElement KeyboardElement
        {
            get { return _keyboard; }
        }

        public bool IsKeyboardVisible
        {
            get { return _keyboard.Visibility == Visibility.Visible; }
            set { _keyboard.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        private void TypeLetter(object sender, RoutedEventArgs e)
        {
            Button key = (Button)sender;

            if (FocusedBox != null)
            {
                if (key == _backspace)
                {
                    if (FocusedBox.Text.Length > 0)
                        FocusedBox.Text = FocusedBox.Text.Substring(0, FocusedBox.Text.Length - 1);
                }
                else if (key == _space)
                    FocusedBox.Text += " ";
                else
                    FocusedBox.Text += (string)key.Content;

                FocusedBox.SelectionStart = FocusedBox.Text.Length;

                if (char.IsUpper((string)key.Content, 0))
                    PressShift(key, e);

                key.Background = Resources["ButtonPressedBackgroundThemeBrush"] as Brush;
                key.Foreground = Resources["ButtonPressedForegroundThemeBrush"] as Brush;

                ThreadPoolTimer.CreateTimer(_ => Unpress(key), TimeSpan.FromMilliseconds(100));
            }
        }

        private void Unpress(Button key)
        {
            Dispatcher.RunIdleAsync(_ =>
            {
                key.ClearValue(BackgroundProperty);
                key.ClearValue(ForegroundProperty);
            });            
        }

        public TextBox FocusedBox { get; set; }

        private void PressShift(object sender, RoutedEventArgs e)
        {
            if ((string)_shift.Content == "ABC")
            {
                foreach (Button key in _keyboard.Children.Take(26))
                    key.Content = ((string)key.Content).ToUpper();

                _shift.Content = "abc";
            }
            else
            {
                foreach (Button key in _keyboard.Children.Take(26))
                    key.Content = ((string)key.Content).ToLower();

                _shift.Content = "ABC";
            }
        }
    }
}
