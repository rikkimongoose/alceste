using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Alceste.LocalApp.AudioStream.Loader;

namespace Alceste.LocalApp.AudioStream
{
    /// <summary>
    /// Interaction logic for AudioStreamControl.xaml
    /// </summary>
    public partial class AudioStreamControl : UserControl
    {
        public AudioStreamControl()
        {
            InitializeComponent();

            _mediaPlayer = new MediaPlayer();
            _doPlayAfterLoading = false;

            _startBtnMode = MediaBtnMode.Stopped;
            _timeStepSpan = new TimeSpan(0, 0, 0, 5);
        }

        private MediaPlayer _mediaPlayer;
        private MediaBtnMode _startBtnMode;
        private readonly TimeSpan _timeStepSpan;

        private bool _doPlayAfterLoading;

        private string AudioSoundTemplatePrev = string.Empty;
        private string AudioInfoTemplatePrev = string.Empty;
        private string AudioSoundPicTemplatePrev = string.Empty;

        private string ItemIdPrev = string.Empty;

        private IAudioStreamLoaderFactory _audioStreamLoaderFactory;
        private IAudioStreamLoader _audioStreamLoader;

        private void BtnPlay_OnClick(object sender, RoutedEventArgs e)
        {
            DoPlayCommand(_startBtnMode);
        }

        public void DoPlayCommand(MediaBtnMode mediaBtnMode)
        {
            switch (mediaBtnMode)
            {
                case MediaBtnMode.Play:
                    DoPause();
                    BtnPlay.Content = "Play";
                    _startBtnMode = MediaBtnMode.Paused;
                    break;
                case MediaBtnMode.Paused:
                    DoPlay();
                    BtnPlay.Content = "Pause";
                    _startBtnMode = MediaBtnMode.Play;
                    break;
                case MediaBtnMode.Stopped:
                    if (DoStartPlay())
                    {
                        BtnPlay.Content = "Pause";
                        _startBtnMode = MediaBtnMode.Play;
                    }
                    break;
            }
        }

        private bool LocationsChanged()
        {
            return ItemId != ItemIdPrev ||
                   AudioInfoTemplate != AudioInfoTemplatePrev ||
                   AudioSoundPicTemplate != AudioSoundPicTemplatePrev ||
                   AudioSoundTemplate != AudioSoundTemplatePrev;
        }

        private void LocationsSync()
        {
            ItemIdPrev = ItemId;
            AudioInfoTemplatePrev = AudioInfoTemplate;
            AudioSoundPicTemplatePrev = AudioSoundPicTemplate;
            AudioSoundTemplatePrev = AudioSoundTemplate;
        }

        private bool DoStartPlay()
        {
            if (LocationsChanged())
            {
                _doPlayAfterLoading = true;
                LoadChannels();
            }
            else if (ItemId != null)
            {
                DoPlay();
                return true;
            }
            return false;
        }

        private void LoadChannels()
        {
            LocationsSync();

            if (!string.IsNullOrEmpty(ItemId))
            {
                _audioStreamLoaderFactory = new AudioStreamLoaderFactory(AudioSoundTemplate, AudioInfoTemplate,
                                                                         AudioSoundPicTemplate);
                try
                {
                    _audioStreamLoaderFactory.GetLoaderForId(out _audioStreamLoader, ItemId,
                                                             (o, list) =>
                                                             AudioStreamLoaderOnSoundItemsLoadedCompleted(o, list));
                }
                catch (WebException ex)
                {
                    MessageBox.Show(string.Format("Ошибка подключения к серверу: \"{0}\"", ex.Message));
                }
            }
        }

        private void AudioStreamLoaderOnSoundItemsLoadedCompleted(object sender, List<LoadedSoundItem> loadedSoundItems)
        {
            ControlDataContext.AudioItemChannels = new ObservableCollection<AudioStreamChannelDataContext>();
            loadedSoundItems.ForEach(item =>
            {
                var audioStreamChannelDataContext = new AudioStreamChannelDataContext(_audioStreamLoader,
                                                                                      item.ChannelId, item.Duration);
                audioStreamChannelDataContext.StopPlaying += AudioStreamChannelDataContextOnStopPlaying;
                audioStreamChannelDataContext.LinePositionChanged += AudioStreamChannelDataContextOnLinePositionChanged;
                ControlDataContext.AudioItemChannels.Add(audioStreamChannelDataContext);
            });
            var enumerator = ControlDataContext.AudioItemChannels.GetEnumerator();
            if (enumerator.MoveNext())
            {
                LoadFilesForChannels(enumerator);
            }
        }

        private void AudioStreamChannelDataContextOnStopPlaying(object sender, EventArgs eventArgs)
        {
            var isPlaying = false;
            foreach (var audioItemChannel in ControlDataContext.AudioItemChannels)
            {
                if (!audioItemChannel.IsActive && audioItemChannel.MediaMode == MediaBtnMode.Play)
                {
                    isPlaying = true;
                    break;
                }
            }
            if (!isPlaying)
                NotifyStopped();
        }

        private void AudioStreamChannelDataContextOnLinePositionChanged(object sender, TimeSpan timeSpan)
        {
            ApplyToAllChannels(context => context.SetMediaPlayerPosition(timeSpan), item => item != (sender as AudioStreamChannelDataContext));
        }

        private void LoadFilesForChannels(IEnumerator<AudioStreamChannelDataContext> enumerator)
        {
            OnMediaDataLoaded += (sender, args) => LoadImages();
            var currentChannel = enumerator.Current;
            if (enumerator.MoveNext())
            {
                currentChannel.MediaPlayer.MediaOpened += (sender, args) => LoadFilesForChannels(enumerator);
            }
            else if (!string.IsNullOrEmpty(ItemId))
            {
                currentChannel.MediaPlayer.MediaOpened += (sender, args) =>
                {
                    if (OnMediaDataLoaded != null)
                        OnMediaDataLoaded(this, new EventArgs());
                    if (_doPlayAfterLoading)
                    {
                        DoPlay();
                        _doPlayAfterLoading = false;
                    }
                };
            }
            currentChannel.LoadMedia();
        }

        private void LoadImages()
        {
            ApplyToAllChannels(context => context.LoadImage());
        }

        public event EventHandler OnMediaDataLoaded;

        public void DoPlay()
        {
            PlayAll();
        }

        private void PlayAll()
        {
            ApplyToAllChannels(item => item.Start(), item => item.IsActive);
        }

        public void ApplyToAllChannels(Action<AudioStreamChannelDataContext> action)
        {
            ApplyToAllChannels(action, item => item.IsActive);
        }

        public void ApplyToAllChannels(Action<AudioStreamChannelDataContext> action, Func<AudioStreamChannelDataContext, bool> test)
        {
            if (action == null)
                return;

            foreach (var audioItemChannel in ControlDataContext.AudioItemChannels.Where(audioItemChannel => test == null || test(audioItemChannel)))
            {
                action(audioItemChannel);
            }
        }

        public void DoPause()
        {
            PauseAll();
        }

        private void PauseAll()
        {
            ApplyToAllChannels(item => item.Pause());
        }

        public void DoStop()
        {
            StopAll();
            NotifyStopped();
        }

        private void NotifyStopped()
        {
            BtnPlay.Content = "Play";
            _startBtnMode = MediaBtnMode.Stopped;
        }

        private void StopAll()
        {
            ApplyToAllChannels(item => item.Stop());
        }


        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            DoStop();
        }

        private void BtnNext_OnClick(object sender, RoutedEventArgs e)
        {
            ApplyToAllChannels(item => item.MoveNext());
        }

        private void BtnBefore_OnClick(object sender, RoutedEventArgs e)
        {
            ApplyToAllChannels(item => item.MovePrev());
        }


        static AudioStreamControl()
        {
            ItemIdProperty = DependencyProperty.Register("ItemId", typeof(string), typeof(AudioStreamControl),
                                                         new PropertyMetadata(string.Empty, ItemIdPropertyChanged));
            AudioSoundTemplateProperty = DependencyProperty.Register("AudioSoundTemplate", typeof(string),
                                                                     typeof(AudioStreamControl),
                                                                     new PropertyMetadata(string.Empty,
                                                                                          AudioSoundTemplatePropertyChanged));
            AudioInfoTemplateProperty = DependencyProperty.Register("AudioInfoTemplate", typeof(string),
                                                                    typeof(AudioStreamControl),
                                                                    new PropertyMetadata(string.Empty,
                                                                                         AudioInfoTemplatePropertyChanged));
            AudioSoundPicTemplateProperty = DependencyProperty.Register("AudioSoundPicTemplate", typeof(string),
                                                                        typeof(AudioStreamControl),
                                                                        new PropertyMetadata(string.Empty,
                                                                                             AudioSoundPicTemplatePropertyChanged));
        }

        private static void ItemIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var audioStreamControl = d as AudioStreamControl;
            if (audioStreamControl == null)
                return;

            if (e.NewValue == e.OldValue)
                return;

            audioStreamControl.ItemId = e.NewValue as string;
            audioStreamControl.UpdateBinding();
        }

        private static void AudioSoundPicTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var audioStreamControl = d as AudioStreamControl;
            if (audioStreamControl == null)
                return;

            if (e.NewValue == e.OldValue)
                return;

            audioStreamControl.AudioSoundPicTemplate = e.NewValue as string;
            audioStreamControl.UpdateBinding();
        }

        private static void AudioInfoTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var audioStreamControl = d as AudioStreamControl;
            if (audioStreamControl == null)
                return;

            if (e.NewValue == e.OldValue)
                return;

            audioStreamControl.AudioInfoTemplate = e.NewValue as string;
            audioStreamControl.UpdateBinding();
        }

        private static void AudioSoundTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var audioStreamControl = d as AudioStreamControl;
            if (audioStreamControl == null)
                return;

            if (e.NewValue == e.OldValue)
                return;

            audioStreamControl.AudioSoundTemplate = e.NewValue as string;
            audioStreamControl.UpdateBinding();
        }

        private void UpdateBinding()
        {
            LoadChannels();
        }

        public AudioStreamControlDataContext ControlDataContext
        {
            get { return DataContext as AudioStreamControlDataContext; }
        }

        public static DependencyProperty ItemIdProperty;
        public string ItemId
        {
            get { return GetValue(ItemIdProperty) as string; }
            set { SetValue(ItemIdProperty, value); }
        }

        public static DependencyProperty AudioSoundTemplateProperty;
        public string AudioSoundTemplate
        {
            get { return GetValue(AudioSoundTemplateProperty) as string; }
            set { SetValue(AudioSoundTemplateProperty, value); }
        }

        public static DependencyProperty AudioInfoTemplateProperty;
        public string AudioInfoTemplate
        {
            get { return GetValue(AudioInfoTemplateProperty) as string; }
            set { SetValue(AudioInfoTemplateProperty, value); }
        }

        public static DependencyProperty AudioSoundPicTemplateProperty;
        public string AudioSoundPicTemplate
        {
            get { return GetValue(AudioSoundPicTemplateProperty) as string; }
            set { SetValue(AudioSoundPicTemplateProperty, value); }
        }
    }
}
