using System.ComponentModel;
using System.Windows;

namespace TreeBreadcrumbControl
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        protected override Freezable CreateInstanceCore() => new BindingProxy();

        protected sealed override bool FreezeCore(bool isChecking)
        {
            // Only derived from Freezable to have DataContext and ElementName binding.
            // So we don't want to be freezable.
            return false;
        }
    }
}
