using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TreeBreadcrumbControl.Commands;

namespace TreeBreadcrumbControl
{
    [TemplatePart(Name = TextBoxName, Type = typeof(TextBox))]
    public class TreeBreadcrumb : Control, INotifyPropertyChanged
    {
        public const string TextBoxName = "PART_TextBox";

        public static readonly DependencyProperty CurrentNodeProperty = DependencyProperty.Register(
            "CurrentNode", typeof(ITreeNode<object>), typeof(TreeBreadcrumb), new PropertyMetadata(null,
                (o, args) =>
                {
                    if (o is TreeBreadcrumb @this && args.NewValue is ITreeNode<object> node)
                    {
                        (@this.Root, @this.Breadcrumb) = GetAncestors(node);
                    }
                }));
        public static readonly DependencyProperty SetCurrentNodeCommandProperty = DependencyProperty.Register(
            "SetCurrentNodeCommand", typeof(ICommand), typeof(TreeBreadcrumb), new PropertyMetadata(default(ICommand)));
        public static readonly DependencyProperty PathSeparatorProperty = DependencyProperty.Register(
            "PathSeparator", typeof(string), typeof(TreeBreadcrumb), new PropertyMetadata("/"));

        static TreeBreadcrumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeBreadcrumb), new FrameworkPropertyMetadata(typeof(TreeBreadcrumb)));
        }

        public ITreeNode<object> CurrentNode
        {
            get => (ITreeNode<object>)GetValue(CurrentNodeProperty);
            set => SetValue(CurrentNodeProperty, value);
        }

        public ICommand SetCurrentNodeCommand
        {
            get => (ICommand)GetValue(SetCurrentNodeCommandProperty);
            set => SetValue(SetCurrentNodeCommandProperty, value);
        }

        public string PathSeparator
        {
            get => (string)GetValue(PathSeparatorProperty);
            set => SetValue(PathSeparatorProperty, value);
        }

        // ----------------------------------------------------------------------------------------------------------------

        private IEnumerable<ITreeNode<object>> _breadCrumb;
        private ITreeNode<object> _root;
        private bool _isTextMode;
        private TextBox _textBox;

        public IEnumerable<ITreeNode<object>> Breadcrumb
        {
            get => _breadCrumb;
            private set => SetProperty(ref _breadCrumb, value);
        }

        public ITreeNode<object> Root
        {
            get => _root;
            private set => SetProperty(ref _root, value);
        }

        public bool IsTextMode
        {
            get => _isTextMode;
            private set => SetProperty(ref _isTextMode, value);
        }

        public ICommand SetTextModeCommand { get; }

        public TreeBreadcrumb()
        {
            SetTextModeCommand = new RelayCommand(() =>
            {
                IsTextMode = true;

                _textBox.ExecuteAfterLoaded(@this =>
                {
                    @this.Text = string.Join(PathSeparator, new[] { Root }.Concat(Breadcrumb).Select(item => item.ToString()));
                    if (!@this.Focus())
                    {
                        throw new InvalidOperationException(
                            "The focus of the TextBox setting operation should not fail, please check the custom template.");
                    }
                    @this.LostKeyboardFocus += TextBoxOnLostKeyboardFocus;
                });
            });
        }

        public override void OnApplyTemplate()
        {
            _textBox = (TextBox)Template.FindName(TextBoxName, this);

            base.OnApplyTemplate();
        }

        private void TextBoxOnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _textBox.LostKeyboardFocus -= TextBoxOnLostKeyboardFocus;
            IsTextMode = false;
        }

        private static (ITreeNode<T> Root, IEnumerable<ITreeNode<T>> Breadcrumb) GetAncestors<T>(ITreeNode<T> node)
        {
            var ancestors = new List<ITreeNode<T>>();

            var parent = node;
            while (parent != null)
            {
                ancestors.Add(parent);
                parent = parent.Parent;
            }
            ancestors.Reverse();

            return (ancestors.FirstOrDefault(), ancestors.Skip(1));
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
