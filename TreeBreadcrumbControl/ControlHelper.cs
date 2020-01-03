using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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
        public static readonly DependencyProperty RelayCommandProperty = DependencyProperty.RegisterAttached(
            "RelayCommand", typeof(ICommand), typeof(ControlHelper), new PropertyMetadata(default(ICommand)));
        public static readonly DependencyProperty RelayCommandParameterProperty = DependencyProperty.RegisterAttached(
            "RelayCommandParameter", typeof(object), typeof(ControlHelper), new PropertyMetadata(default(object)));

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

        public static void SetRelayCommand(DependencyObject element, ICommand value)
        {
            element.SetValue(RelayCommandProperty, value);
        }
        public static ICommand GetRelayCommand(DependencyObject element)
        {
            return (ICommand)element.GetValue(RelayCommandProperty);
        }

        public static void SetRelayCommandParameter(DependencyObject element, object value)
        {
            element.SetValue(RelayCommandParameterProperty, value);
        }
        public static object GetRelayCommandParameter(DependencyObject element)
        {
            return (object)element.GetValue(RelayCommandParameterProperty);
        }

        public static void ExecuteAfterLoaded<T>(this T element, Action<T> callback) where T : FrameworkElement
        {
            if (callback == null) return;

            if (element.IsLoaded)
            {
                callback(element);
            }
            else
            {
                element.Loaded += OnLoaded;
            }

            void OnLoaded(object sender, RoutedEventArgs e)
            {
                var @this = (T)sender;
                @this.Loaded -= OnLoaded;
                callback(@this);
            }
        }
    }
}
