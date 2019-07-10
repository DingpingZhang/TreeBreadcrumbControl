using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TreeBreadcrumbControl
{
    [TemplatePart(Name = CollapseOverflowItemsPanelName, Type = typeof(CollapseOverflowItemsPanel))]
    public class CollapseOverflowItemsControl : ItemsControl
    {
        private const string CollapseOverflowItemsPanelName = "PART_CollapseOverflowItemsPanel";

        public static readonly DependencyProperty OverflowItemsProperty = DependencyProperty.Register(
            "OverflowItems", typeof(IReadOnlyList<object>), typeof(CollapseOverflowItemsControl), new PropertyMetadata(null));

        public IReadOnlyList<object> OverflowItems => (IReadOnlyList<object>)GetValue(OverflowItemsProperty);

        public override void OnApplyTemplate()
        {
            var panel = (CollapseOverflowItemsPanel)GetTemplateChild(CollapseOverflowItemsPanelName);
            SetBinding(OverflowItemsProperty, new Binding(nameof(CollapseOverflowItemsPanel.OverflowItems))
            {
                Source = panel,
                Mode = BindingMode.OneWay
            });

            base.OnApplyTemplate();
        }

        static CollapseOverflowItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapseOverflowItemsControl), new FrameworkPropertyMetadata(typeof(CollapseOverflowItemsControl)));
        }
    }
}
