using System.Windows;
using System.Windows.Controls.Primitives;

namespace TreeBreadcrumbControl
{
    public static class ControlHelper
    {
        public static readonly DependencyProperty AutoSelectedAllProperty = DependencyProperty.RegisterAttached(
            "AutoSelectedAll", typeof(bool), typeof(ControlHelper), new PropertyMetadata(false, (o, args) =>
            {
                var @this = (TextBoxBase)o;
                if ((bool)args.NewValue)
                {
                    @this.GotFocus += OnFocus;
                }
                else
                {
                    @this.GotFocus -= OnFocus;
                }

                void OnFocus(object sender, RoutedEventArgs e)
                {
                    var textBox = (TextBoxBase)sender;
                    textBox.SelectAll();
                }
            }));

        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static void SetAutoSelectedAll(DependencyObject element, bool value)
        {
            element.SetValue(AutoSelectedAllProperty, value);
        }
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static bool GetAutoSelectedAll(DependencyObject element)
        {
            return (bool)element.GetValue(AutoSelectedAllProperty);
        }
    }
}
