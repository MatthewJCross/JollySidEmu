using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using JollySidEmu.ViewModels;

namespace JollySidEmu.Controls
{
    public partial class OscilloscopeView : UserControl
    {
        private readonly TextBlock _freqText = new()
        {
            Foreground = Brushes.White,
            FontSize = 14,
            FontFamily = new FontFamily("Consolas")
        };
        
        private OscilloscopeViewModel? VM => DataContext as OscilloscopeViewModel;

        // SID signals are small → one clean scale
        private const double AmplitudeScale = 120.0;

        public OscilloscopeView()
        {
            InitializeComponent();

            Loaded += (_, _) =>
            {
                CompositionTarget.Rendering += OnRenderFrame;
            };

            Unloaded += (_, _) =>
            {
                CompositionTarget.Rendering -= OnRenderFrame;
            };

            Canvas.Children.Add(_freqText);

            Canvas.SetLeft(_freqText, 10);
            Canvas.SetTop(_freqText, 80);
        }

        private void OnRenderFrame(object? sender, EventArgs e)
        {
            if (VM == null)
                return;

            _freqText.Text = $"≈ {VM.EstimatedFrequencyHz:0.0} Hz";
            VM.Snapshot();     
            Draw();            
        }

        private void Draw()
        {
            double w = Canvas.ActualWidth;
            double h = Canvas.ActualHeight;
            if (w <= 0 || h <= 0)
                return;

            Canvas.Children.Clear();

            double centerY = h * 0.5;

            // Center line
            Canvas.Children.Add(new Line
            {
                X1 = 0,
                X2 = w,
                Y1 = centerY,
                Y2 = centerY,
                Stroke = Brushes.DimGray,
                StrokeThickness = 1
            });

            DrawTrace(VM.V1, Brushes.Cyan, w, centerY);
            DrawTrace(VM.V2, Brushes.Magenta, w, centerY);
            DrawTrace(VM.V3, Brushes.Yellow, w, centerY);
            DrawTrace(VM.Out, Brushes.Lime, w, centerY, 2);
            DrawLegend();
        }

        private void DrawTrace(float[] data, Brush color, double width, double centerY, double thickness = 1)
        {
            if (data == null || data.Length < 2)
                return;

            double xStep = width / (data.Length - 1);

            Polyline line = new Polyline
            {
                Stroke = color,
                StrokeThickness = thickness
            };

            for (int i = 0; i < data.Length; i++)
            {
                float v = data[i];
                if (float.IsNaN(v) || float.IsInfinity(v))
                    v = 0;

                line.Points.Add(new Point(i * xStep, centerY - v * AmplitudeScale));
            }

            Canvas.Children.Add(line);
        }

        private void DrawLegend()
        {
            DrawLegendItem("OUT", Brushes.Lime, 10);
            DrawLegendItem("V1", Brushes.Cyan, 26);
            DrawLegendItem("V2", Brushes.Magenta, 42);
            DrawLegendItem("V3", Brushes.Yellow, 58);            
        }

        private void DrawLegendItem(string label, Brush color, double top)
        {
            var box = new Rectangle
            {
                Width = 10,
                Height = 10,
                Fill = color
            };

            var text = new TextBlock
            {
                Text = label,
                Foreground = Brushes.White,
                FontSize = 12
            };

            Canvas.SetLeft(box, 10);
            Canvas.SetTop(box, top);

            Canvas.SetLeft(text, 26);
            Canvas.SetTop(text, top - 2);

            Canvas.Children.Add(box);
            Canvas.Children.Add(text);
        }
    }
}

