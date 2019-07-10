using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TreeBreadcrumbControl;
using TreeBreadcrumbControl.Commands;

namespace Demo
{
    public class MainWindowViewModel : BindableBase
    {
        private ITreeNode<object> _currentNode;
        private ICommand _setCurrentNodeCommand;
        private string _exceptionMessage;

        public ITreeNode<object> CurrentNode
        {
            get => _currentNode;
            set => SetProperty(ref _currentNode, value);
        }

        public ICommand SetCurrentNodeCommand
        {
            get => _setCurrentNodeCommand;
            set => SetProperty(ref _setCurrentNodeCommand, value);
        }

        public string ExceptionMessage
        {
            get => _exceptionMessage;
            set => SetProperty(ref _exceptionMessage, value);
        }

        public MainWindowViewModel()
        {
            SetCurrentNodeCommand = new RelayCommand<LazyObservableTreeNode<File>>(async node =>
            {
                CurrentNode = node;
                await node.RefreshAsync();
            });

#pragma warning disable 4014
            InitializeCurrentNodeAsync();
#pragma warning restore 4014
        }

        private async Task InitializeCurrentNodeAsync()
        {
            var rootNode = new LazyObservableTreeNode<File>(new File { Path = @"C:\" })
            {
                ChildrenProvider = node => Task.Run(() =>
                {
                    try
                    {
                        var result = Directory.GetDirectories(node.Path).Select(path => new File { Path = path });
                        ExceptionMessage = null;
                        return result;
                    }
                    catch (Exception e)
                    {
                        ExceptionMessage = e.Message;
                        return null;
                    }
                })
            };

            await rootNode.RefreshAsync();

            CurrentNode = rootNode.Children[0];

            await ((IRefreshable)CurrentNode).RefreshAsync();
        }
    }
}
