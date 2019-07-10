using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TreeBreadcrumbControl
{
    [TemplatePart(Name = HideOverflowItemsPanelName, Type = typeof(HideOverflowItemsPanel))]
    public class HideOverflowItemsControl : ItemsControl
    {
        private const string HideOverflowItemsPanelName = "PART_HideOverflowItemsPanel";

        public static readonly DependencyProperty OverflowItemsProperty = DependencyProperty.Register(
            "OverflowItems", typeof(IReadOnlyList<object>), typeof(HideOverflowItemsControl), new PropertyMetadata(null));

        public IReadOnlyList<object> OverflowItems => (IReadOnlyList<object>)GetValue(OverflowItemsProperty);

        public override void OnApplyTemplate()
        {
            var panel = (HideOverflowItemsPanel)GetTemplateChild(HideOverflowItemsPanelName);
            SetBinding(OverflowItemsProperty, new Binding(nameof(HideOverflowItemsPanel.OverflowItems))
            {
                Source = panel,
                Mode = BindingMode.OneWay
            });

            base.OnApplyTemplate();
        }

        static HideOverflowItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HideOverflowItemsControl), new FrameworkPropertyMetadata(typeof(HideOverflowItemsControl)));
        }
    }
}
