using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.Xaml;

namespace AlexTheAdventurous
{
    public class AlexDataSource : INotifyPropertyChanged
    {
        private const string ServerUri = "http://openlab.ncl.ac.uk/publicweb/alex";
        private const int UploadRetrySeconds = 60;
        private const int DownloadTasksIntervalSeconds = 300;

        private bool _tasksDownloadStarted;
        private ObservableCollection<AlexTask> _tasks = new ObservableCollection<AlexTask>();
        private ObservableCollection<AlexPost> _forApproval = new ObservableCollection<AlexPost>();
        private ObservableCollection<AlexPost> _forUpload = new ObservableCollection<AlexPost>();
        private ThreadPoolTimer _downloadTasks;

        public AlexDataSource()
        {
            _tasks.Add(AlexTask.CreateNewTask);

            _forApproval.CollectionChanged += delegate { OnPropertyChanged("ApprovalVisibility"); };
            _forUpload.CollectionChanged += delegate { OnPropertyChanged("UploadVisibility"); };

            _downloadTasks = ThreadPoolTimer.CreatePeriodicTimer(DownloadTasks, TimeSpan.FromSeconds(DownloadTasksIntervalSeconds));
        }

        public ReadOnlyObservableCollection<AlexTask> Tasks
        {
            get
            {
                if (!_tasksDownloadStarted)
                {
                    _tasksDownloadStarted = true;
                    DownloadTasks(null);
                }                

                return new ReadOnlyObservableCollection<AlexTask>(_tasks);
            }
        }
        public ReadOnlyObservableCollection<AlexPost> PostsForApproval { get { return new ReadOnlyObservableCollection<AlexPost>(_forApproval); } }
        public ReadOnlyObservableCollection<AlexPost> PostsForUpload { get { return new ReadOnlyObservableCollection<AlexPost>(_forUpload); } }

        internal void AddTask(AlexTask task) { _tasks.Insert(0, task); }
        internal void AddPost(AlexPost post) { _forApproval.Add(post); }
        internal void Decline(AlexPost post) { _forApproval.Remove(post); }
        internal void Approve(AlexPost post)
        {
            _forApproval.Remove(post);
            _forUpload.Add(post);

            Upload(post);
        }

        private static readonly char[] NewLines = new char[] { '\r', '\n', '\t' };
        private async void DownloadTasks(ThreadPoolTimer timer)
        {
            //_tasks.Insert(0, new AlexTask { Title = "Recycling", Question = "How does your family recycle frogs?" });
            //_tasks.Insert(0, new AlexTask { Title = "Dinner", Question = "How hungry were you before dinner today?" });

            try
            {
                HttpClient http = new HttpClient();
                string tasks = await http.GetStringAsync(ServerUri + "/tasks.php");

                _tasks.Clear();
                _tasks.Add(AlexTask.CreateNewTask);
                foreach (string task in tasks.Split(NewLines, StringSplitOptions.RemoveEmptyEntries))
                {
                    string question = await http.GetStringAsync(ServerUri + "/contents/" + WebUtility.UrlEncode(task) + "/.question.txt");
                    _tasks.Insert(0, new AlexTask { Title = WebUtility.UrlDecode(task), Question = question });
                }
            }
            catch { }
        }
        private async void Upload(AlexPost post)
        {
            try
            {
                HttpClient http = new HttpClient();
                MultipartFormDataContent form = new MultipartFormDataContent
                {
                    { new StringContent(post.Task.Title), "task_title" },
                    { new StringContent(post.Task.Question), "task_question" },
                    { new StringContent(post.Text), "text" }
                };

                if (post.PhotoData != null)
                {
                    byte[] data = new byte[post.PhotoData.Size];
                    await post.PhotoData.GetInputStreamAt(0).ReadAsync(data.AsBuffer(), (uint)data.Length, InputStreamOptions.None);
                    form.Add(new ByteArrayContent(data), "photo", "photo.jpg");
                }

                HttpResponseMessage response = await http.PostAsync(ServerUri + "/upload.php", form);
                if (response.IsSuccessStatusCode)
                {
                    _forUpload.Remove(post);
                    return;
                }
            }
            catch { }

            await Task.Delay(UploadRetrySeconds * 1000);
            Upload(post);
        }

        public Visibility ApprovalVisibility { get { return _forApproval.Count > 0 ? Visibility.Visible : Visibility.Collapsed; } }
        public Visibility UploadVisibility { get { return _forUpload.Count > 0 ? Visibility.Visible : Visibility.Collapsed; } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
