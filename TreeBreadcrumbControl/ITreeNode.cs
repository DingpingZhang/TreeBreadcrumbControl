using System.Collections.Generic;

namespace TreeBreadcrumbControl
{
    public interface ITreeNode<out T>
    {
        T Content { get; }

        ITreeNode<T> Parent { get; }

        IReadOnlyList<ITreeNode<T>> Children { get; }
    }
}
