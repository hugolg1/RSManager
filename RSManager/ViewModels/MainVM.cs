using Ookii.Dialogs.Wpf;
using RSManager.Base;
using RSManager.Facade;
using RSManager.Models.Dto;
using RSManager.Models.Reports;
using RSManager.Models.Security;
using RSManager.Services;
using RSManager.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace RSManager.ViewModels
{
    internal class MainVM : BaseVM
    {

        #region Properties        

        private ICollectionView itemsPath;
        public ICollectionView ItemsPath
        {
            get { return itemsPath; }
            set { SetProperty(ref itemsPath, value); }
        }

        private string currentPath;
        public string CurrentPath
        {
            get { return currentPath; }
            set
            {
                SetProperty(ref currentPath, value);
                if (value == null)
                {
                    NavigateToHome();
                }
                FilterItemsPath();
                SortItemsPath();
                Notify(nameof(ExplorerPath));
            }
        }

        public string ExplorerPath
        {
            get
            {
                string baseUri = null;
                if (Connection != null && Connection.Uri != null)
                {
                    baseUri = Connection.Uri.Contains("/api/")
                        ? Connection.Uri.Substring(0, Connection.Uri.IndexOf("/api/"))
                        : Connection.Uri;
                }
                return String.Concat(baseUri, CurrentPath);
            }
        }        

        private Connection connection;
        public Connection Connection
        {
            get { return connection; }
            set
            {
                SetProperty(ref connection, value);
                Notify(nameof(ExplorerPath));
            }
        }

        public ItemTransferVM ItemTransferVM { get; private set; }

        private bool isDownloadsPopupVisible;
        public bool IsDownloadsPopupVisible
        {
            get { return isDownloadsPopupVisible; }
            set { SetProperty(ref isDownloadsPopupVisible, value); }
        }

        public ICommand DownloadCmd { get; private set; }
        public ICommand CancelDownloadCmd { get; private set; }
        public ICommand SettingsCmd { get; private set; }
        public ICommand ShowHideDownloadsCmd { get; private set; }
        public ICommand NavigateToBackCmd { get; private set; }
        public ICommand NavigateToAheadCmd { get; private set; }
        public ICommand NavigateToHomeCmd { get; private set; }
        public ICommand NavigateToCmd { get; private set; }
        public ICommand RefreshCmd { get; private set; }


        private List<Item> catalogItems;
        private List<string> historyPaths;
        private const string HomePath = "/";

        private readonly IReportService reportService;
        private readonly IConfigurationService configurationService; 

        #endregion

        public MainVM(IReportService reportService, IConfigurationService configurationService, IDataTransferService dataTransferService)
        {
            ItemTransferVM = new ItemTransferVM(dataTransferService);
            this.reportService = reportService;
            this.configurationService = configurationService;
            this.catalogItems = new List<Item>();
            this.ItemsPath = CollectionViewSource.GetDefaultView(catalogItems);
            this.historyPaths = new List<string>();
            this.CurrentPath = HomePath;
            RegisterCommands();
        }

        public async void Initialize()
        {
            Connection = configurationService.GetConnectionInfo() ?? new Connection();
            await SettingsConnection();
        }

        private void RegisterCommands()
        {
            DownloadCmd = new RelayCommand(x => Download(x as List<Item>), x => CanDownload(x as List<Item>));
            SettingsCmd = new RelayCommand(x => SettingsConnection());
            NavigateToBackCmd = new RelayCommand(x => NavigateToBack(), x => CanNavigateToBack());
            NavigateToAheadCmd = new RelayCommand(x => NavigateToAhead(), x => CanNavigateToAhead());
            NavigateToHomeCmd = new RelayCommand(x => NavigateToHome(), x => CanNavigateToHome());
            NavigateToCmd = new RelayCommand(x => NavigateTo(x as Item), x => CanNavigateTo(x as Item));
            ShowHideDownloadsCmd = new RelayCommand(x => IsDownloadsPopupVisible = !IsDownloadsPopupVisible);
            RefreshCmd = new RelayCommand(x => Refresh(), x => CanRefresh());
        }

        private bool InitializeConnectionVM()
        {
            ConnectionVM connectionVM = new ConnectionVM();
            connectionVM.Uri = Connection?.Uri;
            connectionVM.User = Connection?.Credential?.User;
            connectionVM.Password = Connection?.Credential?.Password;
            connectionVM.Domain = Connection?.Credential?.Domain;
            connectionVM.RememberPassword = Connection?.Credential?.RememberPassword ?? false;
            connectionVM.Initialize();

            if (connectionVM.Status != ConnectionVM.StatusType.CONNECT)
            {
                return false;
            }


            Connection = new Connection()
            {
                Uri = connectionVM.Uri,
                Credential = new Credential()
                {
                    User = connectionVM.User,
                    Password = connectionVM.Password,
                    Domain = connectionVM.Domain,
                    RememberPassword = connectionVM.RememberPassword,
                }
            };

            return true;
        }

        private async Task SettingsConnection()
        {
            if (!InitializeConnectionVM())
            {
                return;
            }

            var catalogItemsResult = await reportService.GetCatalogItemsAsync(connection);

            //TODO if error, dont close ConnectionWindow

            if (catalogItemsResult.IsSuccess)
            {
                configurationService.SaveConnectionInfo(new ConnectionDto(
                    Connection.Uri,
                    Connection.Credential.User,
                    Connection.Credential.Password,
                    Connection.Credential.Domain,
                    Connection.Credential.RememberPassword));

                catalogItems = catalogItemsResult.Item;
                ItemsPath = CollectionViewSource.GetDefaultView(catalogItems);
                NavigateToHome();
            }
            else
            {
                MessageBox.Show(catalogItemsResult.Error.ToString());
            }
        }

        private void FilterItemsPath()
        {
            if (ItemsPath == null)
            {
                return;
            }

            if (ItemsPath is ListCollectionView list)
            {
                if (list.IsAddingNew)
                {
                    list.CommitNew();
                }
                if (list.IsEditingItem)
                {
                    list.CommitEdit();
                }
            }

            ItemsPath.Filter = new Predicate<object>(x => ((Item)x).Path == String.Concat(CurrentPath, ((Item)x).Name));
        }

        private void SortItemsPath()
        {
            if (ItemsPath == null || ItemsPath.SortDescriptions.Any())
            {
                return;
            }

            ItemsPath.SortDescriptions.Add(new SortDescription(nameof(Item.Type), ListSortDirection.Ascending));
            ItemsPath.SortDescriptions.Add(new SortDescription(nameof(Item.Name), ListSortDirection.Ascending));
        }

        #region Downloads Methods
        private bool CanDownload(List<Item> items)
        {
            return items?.Any() ?? false;
        }

        private void Download(List<Item> items)
        {
            string downloadDirectory = GetDownloadDirectoryPath();

            if (downloadDirectory == null)
            {
                return;
            }

            ItemPackage package = new ItemPackage(items);
            ItemTransferVM.DownloadPackageAsync(package, downloadDirectory, new DownloadPackageFacade(reportService, Connection, catalogItems));
        }

        private string GetDownloadDirectoryPath()
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Multiselect = false;
            bool result = dialog.ShowDialog() ?? false;

            if (!result || !Directory.Exists(dialog.SelectedPath))
            {
                return null;
            }

            return dialog.SelectedPath;
        } 
        #endregion

        #region Navigation Methods

        private bool CanNavigateToBack()
        {
            return historyPaths.Count > 1 && historyPaths.IndexOf(CurrentPath) > 0;
        }

        private void NavigateToBack()
        {
            int index = historyPaths.IndexOf(CurrentPath);
            CurrentPath = historyPaths[index - 1];
        }

        private bool CanNavigateToAhead()
        {
            return historyPaths.Count > 1 && historyPaths.IndexOf(CurrentPath) < (historyPaths.Count - 1);
        }

        private void NavigateToAhead()
        {
            int index = historyPaths.IndexOf(CurrentPath);
            CurrentPath = historyPaths[index + 1];
        }

        private bool CanNavigateToHome()
        {
            return CurrentPath != HomePath;
        }

        private void NavigateToHome()
        {
            CurrentPath = HomePath;
            historyPaths = new List<string>();
            historyPaths.Add(CurrentPath);
        }

        private bool CanNavigateTo(Item item)
        {
            return item != null && item.IsFolder;
        }

        private void NavigateTo(Item item)
        {
            int index = historyPaths.IndexOf(CurrentPath);
            if (index != -1 && historyPaths.Count > index + 1)
            {
                historyPaths.RemoveRange(index + 1, historyPaths.Count - (index + 1));
            }

            CurrentPath = String.Concat(item.Path, "/");
            historyPaths.Add(CurrentPath);
        }

        private bool CanRefresh()
        {
            return true;
        }

        private async void Refresh()
        {
            var catalogItemsResult = await reportService.GetCatalogItemsAsync(connection);

            if (catalogItemsResult.IsSuccess)
            {
                catalogItems = catalogItemsResult.Item;
                ItemsPath = CollectionViewSource.GetDefaultView(catalogItems);
                if (CurrentPath == HomePath || !catalogItems.Any(x => x.IsFolder && (x.Path + "/") == CurrentPath))
                {
                    NavigateToHome();
                }
                else
                {
                    CurrentPath = currentPath;
                }
            }
            else
            {
                MessageBox.Show(catalogItemsResult.Error.ToString());
            }
        }

        #endregion

    }
}
