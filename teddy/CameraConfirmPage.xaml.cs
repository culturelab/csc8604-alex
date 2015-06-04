using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AlexTheAdventurous
{
    public sealed partial class CameraConfirmPage : Page
    {
        private AlexPost _post;

        public CameraConfirmPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = _post = (AlexPost)e.Parameter;
        }

        private void Continue(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewPostPage), _post);
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
