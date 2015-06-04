using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AlexTheAdventurous
{
    public sealed partial class ModerationList : Page
    {
        public ModerationList()
        {
            InitializeComponent();
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void Moderate(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = (FrameworkElement)sender;
            AlexPost post = (AlexPost)button.DataContext;

            Frame.Navigate(typeof(ModerationPostPage), post);
        }
    }
}
