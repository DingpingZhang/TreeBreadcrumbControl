using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TreeBreadcrumbControl
{
    public class HideOverflowItemsPanel : VirtualizingPanel
    {
        public static readonly DependencyProperty OverflowItemsProperty = DependencyProperty.Register(
            "OverflowItems", typeof(IReadOnlyList<object>), typeof(HideOverflowItemsPanel), new PropertyMetadata(null));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(HideOverflowItemsPanel), new FrameworkPropertyMetadata(
                Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        public static readonly DependencyProperty ReserveProperty = DependencyProperty.Register(
            "Reserve", typeof(bool), typeof(HideOverflowItemsPanel), new FrameworkPropertyMetadata(
                false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        public IReadOnlyList<object> OverflowItems
        {
            get => (IReadOnlyList<object>)GetValue(OverflowItemsProperty);
            private set => SetValue(OverflowItemsProperty, value);
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public bool Reserve
        {
            get => (bool)GetValue(ReserveProperty);
            set => SetValue(ReserveProperty, value);
        }

        public HideOverflowItemsPanel()
        {
            OverflowItems = new List<object>().AsReadOnly();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var generatedChildren = GetGeneratedChildren();
            var visibleCount = GetVisibleCount(generatedChildren, availableSize, out var measuredSize);
            var (visibleChildren, overflowItems) = SeparateItems(generatedChildren, visibleCount);

            var children = InternalChildren;
            if (!EqualsList(visibleChildren, children))
            {
                RemoveInternalChildRange(0, children.Count);
                foreach (var child in visibleChildren)
                {
                    AddInternalChild(child);
                }
            }

            if (!EqualsList(OverflowItems, overflowItems))
            {
                OverflowItems = overflowItems.AsReadOnly();
            }

            return measuredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var isHorizontal = Orientation == Orientation.Horizontal;

            var mainAxisOffset = 0D;
            UIElementCollection children = InternalChildren;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                child.Arrange(
                    new Rect(isHorizontal ? new Point(mainAxisOffset, 0) : new Point(0, mainAxisOffset),
                    child.DesiredSize));
                mainAxisOffset += isHorizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
            }

            return finalSize;
        }

        private int GetVisibleCount(IEnumerable<UIElement> generatedChildren, Size availableSize, out Size measuredSize)
        {
            var isHorizontal = Orientation == Orientation.Horizontal;

            var mainAxis = 0d;
            var crossAxis = 0d;
            var count = 0;
            var availableMainAxis = isHorizontal ? availableSize.Width : availableSize.Height;
            var source = Reserve ? generatedChildren.Reverse() : generatedChildren;
            foreach (var generatedChild in source)
            {
                generatedChild.Measure(availableSize);
                var (childMainAxis, childCrossAxis) = isHorizontal
                    ? (generatedChild.DesiredSize.Width, generatedChild.DesiredSize.Height)
                    : (generatedChild.DesiredSize.Height, generatedChild.DesiredSize.Width);

                var preMainAxis = mainAxis + childMainAxis;

                if (preMainAxis >= availableMainAxis) break;

                mainAxis = preMainAxis;
                crossAxis = Math.Max(crossAxis, childCrossAxis);
                count++;
            }

            measuredSize = isHorizontal ? new Size(mainAxis, crossAxis) : new Size(crossAxis, mainAxis);
            return count;
        }

        private (IReadOnlyList<UIElement> visibleChildren, List<object> overflowItems) SeparateItems(IReadOnlyCollection<UIElement> generatedChildren, int visibleCount)
        {
            var generator = (ItemContainerGenerator)ItemContainerGenerator;
            var overflowCount = generatedChildren.Count - visibleCount;

            if (Reserve)
            {
                var visibleChildren = generatedChildren.Skip(overflowCount).ToArray();
                var overflowItems = Enumerable.Range(0, overflowCount)
                    .Select(index => generator.Items[index])
                    .ToList();
                return (visibleChildren, overflowItems);
            }
            else
            {
                var visibleChildren = generatedChildren.Take(visibleCount).ToArray();
                var overflowItems = Enumerable.Range(visibleCount, overflowCount)
                    .Select(index => generator.Items[index])
                    .ToList();
                return (visibleChildren, overflowItems);
            }
        }

        private IReadOnlyList<UIElement> GetGeneratedChildren()
        {
            // HACK: Read the InternalChildren property before reading the ItemContainerGenerator property,
            // otherwise, the ItemContainerGenerator property will be null.
            // ReSharper disable once UnusedVariable
            var children = InternalChildren;
            var containerGenerator = ItemContainerGenerator;
            var result = new List<UIElement>();
            using (containerGenerator.StartAt(new GeneratorPosition(-1, 0), GeneratorDirection.Forward))
            {
                while (containerGenerator.GenerateNext(out var newlyRealized) is UIElement next)
                {
                    if (newlyRealized)
                    {
                        containerGenerator.PrepareItemContainer(next);
                    }

                    result.Add(next);
                }
            }

            return result.AsReadOnly();
        }

        private static bool EqualsList<T>(IReadOnlyCollection<T> list1, IList list2) =>
            Equals(list1, list2) ||
            list1.Count == list2.Count &&
            !list1.Where((item, i) => !Equals(item, list2[i])).Any();
    }
}
