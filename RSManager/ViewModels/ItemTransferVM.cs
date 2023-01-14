using RSManager.Base;
using RSManager.Facade;
using RSManager.Models.Dto;
using RSManager.Models.Dto.Data;
using RSManager.Models.Reports;
using RSManager.Services;
using RSManager.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RSManager.ViewModels
{
    internal class ItemTransferVM : BaseVM
    {
        #region Properties
        private ObservableCollection<ItemPackageTransfer> transfers;
        public ObservableCollection<ItemPackageTransfer> Transfers
        {
            get { return transfers; }
            set { SetProperty(ref transfers, value); }
        }

        public bool HasTransfers
        {
            get { return Transfers.Any(); }
        }

        public ICommand CancelTransferCmd { get; private set; }
        public ICommand CleanTransfersCmd { get; private set; }
        public ICommand OpenTransferFolderCmd { get; private set; }


        private readonly IDataTransferService dataTransferService;
        private readonly Dictionary<ItemPackageTransfer, CancellationTokenSource> actualTransfersCancellationTokens;
        private readonly object actualTransfersCancellationTokensLocker = new object();
        #endregion

        #region Constructor
        internal ItemTransferVM(IDataTransferService dataTransferService)
        {
            this.dataTransferService = dataTransferService;
            Transfers = new ObservableCollection<ItemPackageTransfer>();
            Transfers.CollectionChanged += Transfers_CollectionChanged;
            actualTransfersCancellationTokens = new Dictionary<ItemPackageTransfer, CancellationTokenSource>();
            RegisterCommands();
        }
        #endregion

        #region Public Methods
        internal async Task DownloadPackageAsync(ItemPackage package, string downloadDirectory, IDownloadPackageFacade downloadFacade)
        {
            CheckCanDownloadPackage(package, downloadDirectory);

            var itemPackageTransfer = new ItemPackageTransfer(package, downloadDirectory);

            try
            {
                Transfers.Add(itemPackageTransfer);
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                AddCancellationTokenTransfer(itemPackageTransfer, cancellationTokenSource);
                itemPackageTransfer.Status = ItemPackageTransfer.TransferStatus.Started;
                await DownloadPackageAsyncInternal(itemPackageTransfer, downloadDirectory, downloadFacade, cancellationTokenSource.Token);
                if (itemPackageTransfer.Status == ItemPackageTransfer.TransferStatus.Started)
                {
                    itemPackageTransfer.Status = ItemPackageTransfer.TransferStatus.Completed;
                }
            }
            catch (OperationCanceledException canceledException)
            {
                itemPackageTransfer.Status = ItemPackageTransfer.TransferStatus.Cancelled;
            }
            catch (Exception)
            {
                RemoveCancellationTokenTransfer(itemPackageTransfer);
                itemPackageTransfer.Status = ItemPackageTransfer.TransferStatus.Error;
                throw;
            }
            finally
            {
                RemoveCancellationTokenTransfer(itemPackageTransfer);
            }
        } 
        #endregion

        #region Private Methods
        private void RegisterCommands()
        {
            CancelTransferCmd = new RelayCommand(x => CancelTransfer(x as ItemPackageTransfer), x => CanCancelTransfer(x as ItemPackageTransfer));
            OpenTransferFolderCmd = new RelayCommand(x => OpenTransferFolder(x as ItemPackageTransfer), x => CanOpenTransferFolder(x as ItemPackageTransfer));
            CleanTransfersCmd = new RelayCommand(x => CleanTransfers(), x => CanCleanTransfers());
        }

        private void Transfers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Notify(nameof(HasTransfers));
        }

        private void CheckCanDownloadPackage(ItemPackage package, string downloadDirectory)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }
            if (downloadDirectory == null)
            {
                throw new ArgumentNullException(nameof(downloadDirectory));
            }
        }

        private async Task DownloadPackageAsyncInternal(ItemPackageTransfer transfer, string downloadDirectory, IDownloadPackageFacade downloadFacade, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await CreateSystemFolders(downloadDirectory, transfer.ItemPackage, downloadFacade, cancellationToken);
            }
            if (!cancellationToken.IsCancellationRequested)
            {
                await CopyReportsToSystemFolders(downloadDirectory, transfer.ItemPackage, downloadFacade, cancellationToken);
            }
        }

        private async Task CreateSystemFolders(string directoryPath, ItemPackage package, IDownloadPackageFacade downloadFacade, CancellationToken cancellationToken)
        {
            foreach (var item in package.Items.Where(x => x.IsFolder))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                string path = Path.Combine(directoryPath, item.Name);
                DataSpaceDto dataSpace = new DataSpaceDto(path, item.Name);

                await dataTransferService.CreateDataSpaceAsync(dataSpace);

                var subDirs = downloadFacade.GetCatalogItems().Where(x => x.Path.StartsWith(item.Path) && x.Level == item.Level + 1).ToList();
                await CreateSystemFolders(dataSpace.Path, new ItemPackage(subDirs), downloadFacade, cancellationToken);
            }
        }

        private async Task CopyReportsToSystemFolders(string directoryPath, ItemPackage package, IDownloadPackageFacade downloadFacade, CancellationToken cancellationToken)
        {
            foreach (var item in package.Items)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (!item.IsFolder)
                {
                    Result<Stream> result = await downloadFacade.GetReportContentAsync(item.Id);
                    if (!result.IsSuccess)
                    {
                        //TODO log
                    }

                    DataSpaceDto dataSpace = new DataSpaceDto(directoryPath, Path.GetDirectoryName(directoryPath));
                    await dataTransferService.UploadDataAsync(new DataUploadRequest($"{item.Name}.rdl", item.Size ?? 0, result.Item, dataSpace));

                    var basePackage = GetItemPackageTransfer(cancellationToken);
                    if (basePackage != null)
                    {
                        basePackage.TransferSize += item.Size ?? 0;
                    }
                }
                else
                {
                    var subDirs = downloadFacade.GetCatalogItems().Where(x => x.Path.StartsWith(item.Path) && x.Level == item.Level + 1).ToList();
                    await CopyReportsToSystemFolders(Path.Combine(directoryPath, item.Name), new ItemPackage(subDirs), downloadFacade, cancellationToken);
                }
            }
        }

        private CancellationTokenSource GetCancellationTokenTransfer(ItemPackageTransfer transfer)
        {
            if (transfer == null)
            {
                return null;
            }

            lock (actualTransfersCancellationTokensLocker)
            {
                if (actualTransfersCancellationTokens.ContainsKey(transfer))
                {
                    return actualTransfersCancellationTokens[transfer];
                }
            }

            return null;
        }

        private ItemPackageTransfer GetItemPackageTransfer(CancellationToken cancellationToken)
        {
            lock (actualTransfersCancellationTokensLocker)
            {
                if (actualTransfersCancellationTokens.Any(x => x.Value.Token == cancellationToken))
                {
                    return actualTransfersCancellationTokens.FirstOrDefault(x => x.Value.Token == cancellationToken).Key;
                }
            }

            return null;
        }

        private void AddCancellationTokenTransfer(ItemPackageTransfer transfer, CancellationTokenSource cancellationTokenSource)
        {
            lock (actualTransfersCancellationTokensLocker)
            {
                actualTransfersCancellationTokens.Add(transfer, cancellationTokenSource);
            }
        }

        private void RemoveCancellationTokenTransfer(ItemPackageTransfer transfer)
        {
            lock (actualTransfersCancellationTokensLocker)
            {
                if (actualTransfersCancellationTokens.ContainsKey(transfer))
                {
                    actualTransfersCancellationTokens.Remove(transfer);
                }
            }
        } 
        #endregion

        #region Command Implementation

        private bool CanCancelTransfer(ItemPackageTransfer transfer)
        {
            return GetCancellationTokenTransfer(transfer) != null;
        }

        private void CancelTransfer(ItemPackageTransfer transfer)
        {
            var cancellationToken = GetCancellationTokenTransfer(transfer);
            if (cancellationToken != null && cancellationToken.Token.CanBeCanceled)
            {
                cancellationToken.Cancel();
                transfer.Status = ItemPackageTransfer.TransferStatus.Cancelled;
            }
        }

        private bool CanOpenTransferFolder(ItemPackageTransfer transfer)
        {
            return transfer != null && !String.IsNullOrEmpty(transfer.DestinationPath);
        }

        private void OpenTransferFolder(ItemPackageTransfer transfer)
        {
            if (Directory.Exists(transfer.DestinationPath))
            {
                Process.Start("explorer.exe", transfer.DestinationPath);
            }
        }

        private bool CanCleanTransfers()
        {
            return Transfers.Any(x => x.Status == ItemPackageTransfer.TransferStatus.Cancelled ||
                                      x.Status == ItemPackageTransfer.TransferStatus.Error ||
                                      x.Status == ItemPackageTransfer.TransferStatus.Completed);
        }

        private void CleanTransfers()
        {
            var transfersRemove = Transfers
                .Where(x => x.Status == ItemPackageTransfer.TransferStatus.Cancelled ||
                            x.Status == ItemPackageTransfer.TransferStatus.Error ||
                            x.Status == ItemPackageTransfer.TransferStatus.Completed).ToArray();

            foreach (var transfer in transfersRemove)
            {
                Transfers.Remove(transfer);
            }
        }

        #endregion

    }
}
