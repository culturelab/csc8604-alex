using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AlexTheAdventurous
{
    public sealed partial class NewPostPage : Page
    {
        private AlexPost _post;

        public NewPostPage()
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

        private void SharePost(object sender, RoutedEventArgs e)
        {
            // do not allow sharing until something has been entered into the text box
            if (!string.IsNullOrWhiteSpace(_post.Text))
            {
                App.Current.DataSource.AddPost(_post);
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Touch.Focus(PostText);  // * focus SPI touch & show on-screen keyboard
        }
    }
}
