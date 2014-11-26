using System.Collections.ObjectModel;
using Alceste.LocalApp.Notification;

namespace Alceste.LocalApp.AudioStream
{
    public class AudioStreamControlDataContext : ObservableObject
    {
        private ObservableCollection<AudioStreamChannelDataContext> _audioItemChannels;
        private string _itemId;
        private string _audioInfoTemplate;
        private string _audioSoundPicTemplate;
        private string _audioSoundTemplate;

        public ObservableCollection<AudioStreamChannelDataContext> AudioItemChannels
        {
            get { return _audioItemChannels; }
            set
            {
                if (_audioItemChannels == value)
                    return;
                _audioItemChannels = value;
                NotifyPropertyChanged(() => AudioItemChannels);
            }
        }

        public string ItemId
        {
            get { return _itemId; }
            set
            {
                if (_itemId == value)
                    return;
                _itemId = value;
                NotifyPropertyChanged(() => ItemId);
            }
        }

        public string AudioInfoTemplate
        {
            get { return _audioInfoTemplate; }
            set
            {
                if (_audioInfoTemplate == value)
                    return;
                _audioInfoTemplate = value;
                NotifyPropertyChanged(() => AudioInfoTemplate);
            }
        }

        public string AudioSoundPicTemplate
        {
            get { return _audioSoundPicTemplate; }
            set
            {
                if (_audioSoundPicTemplate == value)
                    return;
                _audioSoundPicTemplate = value;
                NotifyPropertyChanged(() => AudioSoundPicTemplate);
            }
        }

        public string AudioSoundTemplate
        {
            get { return _audioSoundTemplate; }
            set
            {
                if (_audioSoundTemplate == value)
                    return;
                _audioSoundTemplate = value;
                NotifyPropertyChanged(() => AudioSoundTemplate);
            }
        }
    }
}
