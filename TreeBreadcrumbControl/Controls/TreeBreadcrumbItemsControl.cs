using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TreeBreadcrumbControl
{
    [TemplatePart(Name = CollapseOverflowItemsPanelName, Type = typeof(CollapseOverflowItemsPanel))]
    public class TreeBreadcrumbItemsControl : ItemsControl
    {
        private const string CollapseOverflowItemsPanelName = "PART_CollapseOverflowItemsPanel";

        public static readonly DependencyProperty OverflowItemsProperty = DependencyProperty.Register(
            "OverflowItems", typeof(IEnumerable), typeof(TreeBreadcrumbItemsControl), new PropertyMetadata(null));

        public IEnumerable OverflowItems => (IEnumerable)GetValue(OverflowItemsProperty);

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

        static TreeBreadcrumbItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeBreadcrumbItemsControl), new FrameworkPropertyMetadata(typeof(TreeBreadcrumbItemsControl)));
        }
    }
}
