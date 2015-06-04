using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AlexTheAdventurous
{
    public sealed partial class NewTaskPage : Page
    {
        public NewTaskPage()
        {
            InitializeComponent();
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void CreateTask(object sender, RoutedEventArgs e)
        {
            AlexTask task = new AlexTask
            {
                Title = NewTitle.Text,
                Question = NewQuestion.Text
            };

            // do not allow task creation until something has been entered into the text boxes
            if (string.IsNullOrWhiteSpace(task.Title) || string.IsNullOrWhiteSpace(task.Question))
                return;

            App.Current.DataSource.AddTask(task);
            Frame.GoBack();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Touch.Focus(NewTitle); // * focus SPI touch & show on-screen keyboard
        }
    }
}
