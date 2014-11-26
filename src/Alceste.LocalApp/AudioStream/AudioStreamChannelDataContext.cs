using System;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Alceste.LocalApp.AudioStream.Loader;
using Alceste.LocalApp.Notification;

namespace Alceste.LocalApp.AudioStream
{
    public class AudioStreamChannelDataContext : ObservableObject
    {
        private readonly TimeSpan _timeStepSpan;
        private readonly TimeSpan _timeMediaSpan;
        private readonly MediaPlayer _mediaPlayer;
        private readonly string _channelId;

        private double _actualWidth;
        private double _actualHeight;

        private BitmapImage _imageSource;
        private Brush _lineBrush;
        private IAudioStreamLoader _audioStreamLoader;
        private Thickness _margin;
        private TimeSpan _duration;
        private bool _isActive;
        private bool _isLoaded;
        private DispatcherTimer _mediaTimer;
        private MediaBtnMode _mediaMode;

        public AudioStreamChannelDataContext(IAudioStreamLoader audioStreamLoader, string channelId, TimeSpan duration)
        {
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.MediaEnded += (sender, args) => Stop();
            _mediaTimer = new DispatcherTimer();
            _timeStepSpan = new TimeSpan(0, 0, 0, 5);
            _timeMediaSpan = new TimeSpan(0, 0, 0, 0, 1);
            _mediaTimer.Interval = _timeMediaSpan;
            _mediaTimer.Tick += MediaTimerOnTick;
            _margin = new Thickness();
            _mediaMode = MediaBtnMode.Stopped;

            _actualWidth = 600;
            _actualHeight = 200;

            _isActive = true;
            _isLoaded = false;
            LineBrush = new SolidColorBrush(Colors.Red);

            _audioStreamLoader = audioStreamLoader;
            _channelId = channelId;
            _duration = duration;
        }

        public void LoadMedia()
        {
            try
            {
                _mediaPlayer.Open(new Uri(_audioStreamLoader.LoadSoundUrl(_channelId)));
            }
            catch (WebException ex)
            {
                MessageBox.Show(string.Format("Ошибка подключения к серверу: \"{0}\"", ex.Message));
            }
        }

        public void Start()
        {
            _mediaPlayer.Play();
            _mediaTimer.Start();
            MediaMode = MediaBtnMode.Play;
            OnStartPlaying();
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
            _mediaTimer.Stop();
            SetMediaPlayerPosition(TimeSpan.Zero);
            SetLinePosition(0);
            MediaMode = MediaBtnMode.Stopped;
            OnStopPlaying();
        }

        public void Pause()
        {
            _mediaPlayer.Pause();
            _mediaTimer.Stop();
            MediaMode = MediaBtnMode.Paused;
            OnPausePlaying(_mediaPlayer.Position);
        }

        public void LoadImage()
        {
            try
            {
                ImageSource = _audioStreamLoader.LoadImage((int)ActualWidth, (int)ActualHeight, ChannelId);
            }
            catch (WebException ex)
            {
                MessageBox.Show(string.Format("Ошибка подключения к серверу: \"{0}\"", ex.Message));
            }
        }

        public void MoveNext()
        {
            SetMediaPlayerPosition(_mediaPlayer.Position.Add(_timeStepSpan));
        }

        public void MovePrev()
        {
            SetMediaPlayerPosition(_mediaPlayer.Position.Add(_timeStepSpan.Negate()));
        }

        public void SetMediaPlayerPosition(TimeSpan timeSpan)
        {
            _mediaPlayer.Position = timeSpan;
        }

        public void SetMediaPlayerPosition(double position)
        {
            var newTimeSpan = DurationByPosition(position);
            _mediaPlayer.Position = newTimeSpan;
            LinePositionChanged(this, newTimeSpan);
        }

        public TimeSpan DurationByPosition(double percents)
        {
            if (percents >= 1)
                return Duration;
            return new TimeSpan(0, 0, 0, 0, (int)Math.Ceiling(Duration.TotalMilliseconds * percents));
        }

        public double PositionByDuration(TimeSpan timeSpan)
        {
            if (timeSpan > Duration)
                return 1;
            return timeSpan.TotalMilliseconds / Duration.TotalMilliseconds;
        }

        public void UpdateActualWidthHeight(double actualWidth, double actualHeight)
        {
            ActualWidth = actualWidth;
            ActualHeight = actualHeight;
        }

        private void MediaTimerOnTick(object sender, EventArgs eventArgs)
        {
            SetLinePosition(PositionByDuration(_mediaPlayer.Position));
        }

        private void SetLinePosition(double currentPosition)
        {
            Margin = new Thickness(currentPosition * ActualWidth, 0, 0, 0);
        }

        public event EventHandler<TimeSpan> LinePositionChanged;

        protected virtual void OnOnLinePositionChanged(TimeSpan e)
        {
            EventHandler<TimeSpan> handler = LinePositionChanged;
            if (handler != null) handler(this, e);
        }

        public event EventHandler StartPlaying;

        protected virtual void OnStartPlaying()
        {
            EventHandler handler = StartPlaying;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public event EventHandler<TimeSpan> PausePlaying;

        protected virtual void OnPausePlaying(TimeSpan e)
        {
            EventHandler<TimeSpan> handler = PausePlaying;
            if (handler != null) handler(this, e);
        }

        public event EventHandler StopPlaying;

        protected virtual void OnStopPlaying()
        {
            EventHandler handler = StopPlaying;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public BitmapImage ImageSource
        {
            get { return _imageSource; }
            set
            {
                if (_imageSource == value)
                    return;
                _imageSource = value;
                NotifyPropertyChanged(() => ImageSource);
            }
        }

        public Brush LineBrush
        {
            get { return _lineBrush; }
            set
            {
                if (_lineBrush == value)
                    return;
                _lineBrush = value;
                NotifyPropertyChanged(() => LineBrush);
            }
        }

        public double LinePosition
        {
            get { return _margin.Left; }
            set
            {
                if (_margin.Left == value)
                    return;
                _margin.Left = value;
                NotifyPropertyChanged(() => Margin);
                NotifyPropertyChanged(() => LinePosition);
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive == value)
                    return;
                _isActive = value;
                NotifyPropertyChanged(() => IsActive);
            }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
        }


        public Thickness Margin
        {
            get { return _margin; }
            set
            {
                if (_margin == value)
                    return;
                _margin = value;
                NotifyPropertyChanged(() => Margin);
            }
        }

        public double ActualWidth
        {
            get { return _actualWidth; }
            set
            {
                if (_actualWidth == value)
                    return;
                _actualWidth = value;
                NotifyPropertyChanged(() => ActualWidth);
            }
        }

        public double ActualHeight
        {
            get { return _actualHeight; }
            set
            {
                if (_actualHeight == value)
                    return;
                _actualHeight = value;
                NotifyPropertyChanged(() => ActualHeight);
            }
        }

        public MediaBtnMode MediaMode
        {
            get { return _mediaMode; }
            protected set
            {
                if (_mediaMode == value)
                    return;
                _mediaMode = value;
                NotifyPropertyChanged(() => MediaMode);
            }
        }

        public MediaPlayer MediaPlayer
        { get { return _mediaPlayer; } }

        public string ChannelId
        { get { return _channelId; } }

        public TimeSpan Duration
        { get { return _duration; } }
    }
}
