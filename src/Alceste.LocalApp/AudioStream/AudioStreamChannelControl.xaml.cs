using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Alceste.LocalApp.AudioStream
{
    /// <summary>
    /// Interaction logic for AudioStreamChannelControl.xaml
    /// </summary>
    public partial class AudioStreamChannelControl : UserControl
    {
        public AudioStreamChannelControl()
        {
            InitializeComponent();
        }

        private void ImgMediaView_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(imgMediaView);
            var position = pos.X / gridMediaView.ActualWidth;
            AudioStreamChannelContext.SetMediaPlayerPosition(position);
        }

        public AudioStreamChannelDataContext AudioStreamChannelContext
        {
            get { return DataContext as AudioStreamChannelDataContext; }
        }

        private void UpdateActualWidthHeight()
        {
            if (AudioStreamChannelContext != null)
            {
                AudioStreamChannelContext.UpdateActualWidthHeight(ActualWidth, ActualHeight);
            }
        }

        private void AudioStreamChannelControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateActualWidthHeight();
        }
    }
}
