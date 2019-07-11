using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using TreeBreadcrumbControl.Commands;

namespace TreeBreadcrumbControl
{
    [TemplatePart(Name = PopupName, Type = typeof(Popup))]
    public class TreeBreadcrumbItem : ContentControl
    {
        private const string PopupName = "PART_Popup";

        public static readonly DependencyProperty NodeProperty = DependencyProperty.Register(
            "Node", typeof(ITreeNode<object>), typeof(TreeBreadcrumbItem), new PropertyMetadata(null));
        public static readonly DependencyProperty SetCurrentNodeCommandProperty = DependencyProperty.Register(
            "SetCurrentNodeCommand", typeof(ICommand), typeof(TreeBreadcrumbItem), new PropertyMetadata(null));

        static TreeBreadcrumbItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeBreadcrumbItem), new FrameworkPropertyMetadata(typeof(TreeBreadcrumbItem)));
        }

        public ITreeNode<object> Node
        {
            get => (ITreeNode<object>)GetValue(NodeProperty);
            set => SetValue(NodeProperty, value);
        }

        public ICommand SetCurrentNodeCommand
        {
            get => (ICommand)GetValue(SetCurrentNodeCommandProperty);
            set => SetValue(SetCurrentNodeCommandProperty, value);
        }

        // -------------------------------------------------------------------------------------------------------------------------------

        private Popup _popup;

        public ICommand InternalSetCurrentNodeCommand { get; }

        public TreeBreadcrumbItem()
        {
            InternalSetCurrentNodeCommand = new RelayCommand<object>(parameter =>
            {
                SetCurrentNodeCommand?.Execute(parameter);
                _popup.SetCurrentValue(Popup.IsOpenProperty, false);
            });
        }

        public override void OnApplyTemplate()
        {
            _popup = (Popup)Template.FindName(PopupName, this);
            base.OnApplyTemplate();
        }
    }

    public class TreeBreadcrumbRootItem : TreeBreadcrumbItem
    {
        public static readonly DependencyProperty OverflowItemsProperty = DependencyProperty.Register(
            "OverflowItems", typeof(IEnumerable), typeof(TreeBreadcrumbRootItem), new PropertyMetadata(default(IEnumerable)));
        public static readonly DependencyProperty IsTextModeProperty = DependencyProperty.Register(
            "IsTextMode", typeof(bool), typeof(TreeBreadcrumbRootItem), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty SetTextModeCommandProperty = DependencyProperty.Register(
            "SetTextModeCommand", typeof(ICommand), typeof(TreeBreadcrumbRootItem), new PropertyMetadata(default(ICommand)));

        public IEnumerable OverflowItems
        {
            get => (IEnumerable)GetValue(OverflowItemsProperty);
            set => SetValue(OverflowItemsProperty, value);
        }

        public bool IsTextMode
        {
            get => (bool)GetValue(IsTextModeProperty);
            set => SetValue(IsTextModeProperty, value);
        }

        public ICommand SetTextModeCommand
        {
            get => (ICommand)GetValue(SetTextModeCommandProperty);
            set => SetValue(SetTextModeCommandProperty, value);
        }

        static TreeBreadcrumbRootItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeBreadcrumbRootItem), new FrameworkPropertyMetadata(typeof(TreeBreadcrumbRootItem)));
        }
    }
}
