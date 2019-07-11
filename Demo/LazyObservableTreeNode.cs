using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeBreadcrumbControl;

namespace Demo
{
    public class LazyObservableTreeNode<T> : BindableBase, ITreeNode<T>, IRefreshable
    {
        private bool _isRefreshing;
        private Func<T, Task<IEnumerable<T>>> _childrenProvider;
        private Func<T, string> _stringFormat;
        private IEnumerable<ITreeNode<T>> _children;

        public LazyObservableTreeNode(T content) => Content = content;

        public virtual Func<T, Task<IEnumerable<T>>> ChildrenProvider
        {
            get => _childrenProvider ?? (_childrenProvider = ((LazyObservableTreeNode<T>)Parent)?.ChildrenProvider);
            set => _childrenProvider = value;
        }

        public Func<T, string> StringFormat
        {
            get => _stringFormat ?? (_stringFormat = ((LazyObservableTreeNode<T>)Parent).StringFormat);
            set => _stringFormat = value;
        }

        public T Content { get; }

        public virtual ITreeNode<T> Parent { get; protected set; }

        public virtual IEnumerable<ITreeNode<T>> Children
        {
            get => _children;
            protected set => SetProperty(ref _children, value);
        }

        public virtual async Task<bool> RefreshAsync()
        {
            if (_isRefreshing) return false;
            _isRefreshing = true;

            if (ChildrenProvider == null) return AbortRefresh();
            var enumerable = await ChildrenProvider(Content);
            if (enumerable == null) return AbortRefresh();

            var collection = enumerable.ToList();
            if (!collection.Any()) return AbortRefresh();

            var children = collection.Select(GenerateLazyTreeNode).ToList();
            children.ForEach(item => item.Parent = this);

            SetChildrenCache(children.AsReadOnly());

            _isRefreshing = false;
            return true;
        }

        protected virtual LazyObservableTreeNode<T> GenerateLazyTreeNode(T content) => new LazyObservableTreeNode<T>(content);

        protected virtual void SetChildrenCache(IReadOnlyList<ITreeNode<T>> childrenCache) => Children = childrenCache;

        private bool AbortRefresh()
        {
            Children = null;
            _isRefreshing = false;
            return false;
        }

        public override string ToString() => StringFormat?.Invoke(Content) ?? Content.ToString();
    }

}
