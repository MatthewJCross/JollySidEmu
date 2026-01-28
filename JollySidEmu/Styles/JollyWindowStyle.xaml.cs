using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace JollySidEmu.Styles
{
    /// <summary>
    /// Interaction logic for JollyWindowStyle.xaml
    /// </summary>
    public partial class JollyWindowStyle : ResourceDictionary, INotifyPropertyChanged
    {
        #region DllImports
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        #region Properties
        private string _text = "Hello";
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public JollyWindowStyle()
        {
            InitializeComponent();
        }

        #region WindowDrag
        //Attach this to the MouseDown event of your drag control to move the window in place of the title bar
        private void WindowDrag(object sender, MouseButtonEventArgs e) // MouseDown
        {
            ReleaseCapture();
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            SendMessage(new WindowInteropHelper(window).Handle, 0xA1, (IntPtr)0x2, (IntPtr)0);
        }

        //Attach this to the PreviewMousLeftButtonDown event of the grip control in the lower right corner of the form to resize the window
        private void WindowResize(object sender, MouseButtonEventArgs e) //PreviewMousLeftButtonDown
        {
            HwndSource hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
            SendMessage(hwndSource.Handle, 0x112, (IntPtr)61448, IntPtr.Zero);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Window window = sender as Window;
                window.DragMove();
            }
        }
        #endregion

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.Close();
        }

        private void OnMaximizeRestoreClick(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            if (window.WindowState == WindowState.Normal)
                window.WindowState = WindowState.Maximized;
            else
                window.WindowState = WindowState.Normal;
        }

        private void OnMinimizeClick(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.WindowState = WindowState.Minimized;
        }
    }

    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null && (string)value != string.Empty) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool flag)
                return Visibility.Collapsed;

            bool invert = false;
            bool useHidden = false;

            if (parameter is string p)
            {
                invert = p.Contains("Invert", StringComparison.OrdinalIgnoreCase);
                useHidden = p.Contains("Hidden", StringComparison.OrdinalIgnoreCase);
            }

            if (invert)
                flag = !flag;

            if (flag)
                return Visibility.Visible;

            return useHidden ? Visibility.Hidden : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
                return v == Visibility.Visible;

            return false;
        }
    }

    public sealed class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            if (value is int i)
                return i > 0;

            if (value is long l)
                return l > 0;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
