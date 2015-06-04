using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AlexTheAdventurous
{
    public sealed partial class ModerationPostPage : Page
    {
        private AlexPost _post;

        public ModerationPostPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = _post = (AlexPost)e.Parameter;
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void DeclinePost(object sender, RoutedEventArgs e)
        {
            App.Current.DataSource.Decline(_post);
            Frame.GoBack();
        }

        private void AcceptPost(object sender, RoutedEventArgs e)
        {
            App.Current.DataSource.Approve(_post);
            Frame.GoBack();
        }

        private void ScrollDown(object sender, RoutedEventArgs e)
        {
            TextScroller.ChangeView(null, TextScroller.VerticalOffset + TextScroller.ActualHeight, null);
        }

        private void ScrollUp(object sender, RoutedEventArgs e)
        {
            TextScroller.ChangeView(null, TextScroller.VerticalOffset - TextScroller.ActualHeight, null);
        }
    }
}
