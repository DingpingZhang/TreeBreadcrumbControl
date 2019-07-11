using System.Collections.Generic;
using System.Linq;
using TreeBreadcrumbControl;
using WpfExtensions.Xaml.Converters;

namespace Demo
{
    public class ExtractContentConverter : ValueConverterBase<IEnumerable<ITreeNode<object>>, IEnumerable<object>>
    {
        protected override IEnumerable<object> ConvertNonNullValue(IEnumerable<ITreeNode<object>> value)
        {
            return value.Select(item => item.Content);
        }
    }
}
