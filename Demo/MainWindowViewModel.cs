using System;
using System.Collections.Generic;
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
        private ITreeNode<DirectoryInfo> _currentNode;
        private IEnumerable<FileInfo> _fileInfos;
        private string _exceptionMessage;

        public ITreeNode<DirectoryInfo> CurrentNode
        {
            get => _currentNode;
            set => SetProperty(ref _currentNode, value);
        }

        public IEnumerable<FileInfo> FileInfos
        {
            get => _fileInfos;
            set => SetProperty(ref _fileInfos, value);
        }

        public ICommand SetCurrentNodeCommand { get; }

        public ICommand OpenDirectoryCommand { get; }

        public string ExceptionMessage
        {
            get => _exceptionMessage;
            set => SetProperty(ref _exceptionMessage, value);
        }

        public MainWindowViewModel()
        {
            SetCurrentNodeCommand = new RelayCommand<LazyObservableTreeNode<DirectoryInfo>>(
                async node => await SetCurrentNodeAsync(node));

            OpenDirectoryCommand = new RelayCommand<DirectoryInfo>(async info =>
            {
                var node = (LazyObservableTreeNode<DirectoryInfo>)CurrentNode.Children.First(item => item.Content == info);
                await SetCurrentNodeAsync(node);
            });

#pragma warning disable 4014
            InitializeCurrentNodeAsync();
#pragma warning restore 4014
        }

        private async Task InitializeCurrentNodeAsync()
        {
            var rootNode = new LazyObservableTreeNode<DirectoryInfo>(new DirectoryInfo(@"C:\"))
            {
                ChildrenProvider = content => Task.Run(() =>
                {
                    try
                    {
                        if (content is DirectoryInfo directoryInfo)
                        {
                            return Directory.GetDirectories(directoryInfo.FullName)
                                .Select(item => new DirectoryInfo(item));
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionMessage = e.Message;
                    }

                    return null;
                }),
                StringFormat = content => content.Name.Replace("\\", "").Replace("/", "")
            };

            await rootNode.RefreshAsync();

            CurrentNode = rootNode;

            await ((IRefreshable)CurrentNode).RefreshAsync();
        }

        private async Task SetCurrentNodeAsync(LazyObservableTreeNode<DirectoryInfo> node)
        {
            ExceptionMessage = null;
            CurrentNode = node;
            await node.RefreshAsync();

            try
            {
                FileInfos = Directory.GetFiles(node.Content.FullName)
                    .Select(item => new FileInfo(item));
            }
            catch (Exception e)
            {
                ExceptionMessage = e.Message;
                FileInfos = null;
            }
        }
    }
}
