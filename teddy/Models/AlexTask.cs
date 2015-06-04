using Windows.UI.Xaml.Media.Imaging;

namespace AlexTheAdventurous
{
    public class AlexTask
    {
        public BitmapImage Picture { get; set; }
        public string Title { get; set; }
        public string Question { get; set; }

        public static readonly AlexTask CreateNewTask = new AlexTask { Title = "Create new task..." };
    }
}
