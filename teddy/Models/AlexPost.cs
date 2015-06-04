using System;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace AlexTheAdventurous
{
    public class AlexPost
    {
        public AlexTask Task { get; set; }
        public string Text { get; set; }
        public InMemoryRandomAccessStream PhotoData { get; set; }
        public BitmapImage Photo
        {
            get
            {
                if (PhotoData == null)
                    return null;
                else
                    PhotoData.Seek(0);

                BitmapImage photo = new BitmapImage();
                photo.SetSource(PhotoData);
                return photo;
            }
        }
    }
}
