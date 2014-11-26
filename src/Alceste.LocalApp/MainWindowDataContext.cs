using System.Collections.ObjectModel;
using Alceste.Model;
using Alceste.LocalApp.AudioStream;
using Alceste.LocalApp.AudioStream.Loader;
using Alceste.LocalApp.Notification;

namespace Alceste.LocalApp
{
    public sealed class MainWindowDataContext : ObservableObject
    {
        private readonly string _fileRecordsPath;

        private ObservableCollection<MediaFileServerRecord> _serverRecords;
        private MediaFileServerRecord _selectedServerRecord;
        private readonly IAudioItemsListLoader _audioItemsListLoader;
        private AudioStreamControlDataContext _audioStreamControlDataContext;

        public MainWindowDataContext()
        {
            _audioStreamControlDataContext = new AudioStreamControlDataContext()
            {
                AudioInfoTemplate = "http://localhost:15319/Service.svc/getinfo/{0}",
                AudioSoundPicTemplate = "http://localhost:15319/Service.svc/mediapic/{0}/%channelnum%/{1}/{2}",
                AudioSoundTemplate = "http://localhost:15319/Service.svc/media/{0}/%channelnum%"
            };

            NotifyPropertyChanged(() => AudioStreamControlDataContext);
        }


        public MainWindowDataContext(string fileRecordsPath)
            : this()
        {
            _fileRecordsPath = fileRecordsPath;
            _audioItemsListLoader = new AudioItemsListLoader(_fileRecordsPath);
            ServerRecords = new ObservableCollection<MediaFileServerRecord>();
            var records = _audioItemsListLoader.GetMediaFileServerRecords();
            foreach (var record in records)
                ServerRecords.Add(record);
        }

        public ObservableCollection<MediaFileServerRecord> ServerRecords
        {
            get { return _serverRecords; }
            set
            {
                if (_serverRecords == value)
                    return;
                _serverRecords = value;
                NotifyPropertyChanged(() => ServerRecords);
            }
        }

        public MediaFileServerRecord SelectedServerRecord
        {
            get { return _selectedServerRecord; }
            set
            {
                if (_selectedServerRecord == value)
                    return;

                _selectedServerRecord = value;
                NotifyPropertyChanged(() => SelectedServerRecord);

                _audioStreamControlDataContext.ItemId = value.Title;
            }
        }

        public AudioStreamControlDataContext AudioStreamControlDataContext
        {
            get { return _audioStreamControlDataContext; }
        }
    }
}
