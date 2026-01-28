using System.Windows.Controls;
using System.Windows;

namespace JollySidEmu.Styles.Controls
{
    public class XamlImage : Control
    {
        static XamlImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XamlImage), new FrameworkPropertyMetadata(typeof(XamlImage)));
            IsTabStopProperty.OverrideMetadata(typeof(XamlImage), new FrameworkPropertyMetadata(false));
        }
    }
}
