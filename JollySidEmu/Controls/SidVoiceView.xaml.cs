using System.Windows;
using System.Windows.Controls;
using JollySidEmu.ViewModels;

namespace JollySidEmu.Controls
{
    /// <summary>
    /// Interaction logic for SidVoiceView.xaml
    /// </summary>
    public partial class SidVoiceView : UserControl
    {
        public SidVoiceView()
        {
            InitializeComponent();
        }

        public VoiceViewModel Voice
        {
            get => (VoiceViewModel)GetValue(VoiceProperty);
            set => SetValue(VoiceProperty, value);
        }

        public static readonly DependencyProperty VoiceProperty = DependencyProperty.Register(nameof(Voice), typeof(VoiceViewModel), typeof(SidVoiceView), new PropertyMetadata(null));
    }
}
