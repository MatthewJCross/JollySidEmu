using JollySidEmu.Audio;
using JollySidEmu.ViewModels;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Windows;
using System.Windows.Controls;

namespace JollySidEmu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _initialized = false;
        
        private WaveOutEvent _waveOut;
        private SidWaveProvider _sidWaveProvider;
        private SidRegisters _sid;
        private C64Memory _memory;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new SidViewModel();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is SidViewModel vm)
                vm.Dispose();   // optional if you add IDisposable

            base.OnClosed(e);
        }
    }
}