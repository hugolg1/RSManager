using RSManager.Base;
using RSManager.Utils;
using RSManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RSManager.ViewModels
{
    internal class ConnectionVM : BaseVM
    {
        internal enum StatusType
        {
            NONE = 0,
            CONNECT = 1,
        }

        private string uri;
        public string Uri
        {
            get { return uri; }
            set { SetProperty(ref uri, value); }
        }

        private string user;
        public string User
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        private string domain;
        public string Domain
        {
            get { return domain; }
            set { SetProperty(ref domain, value); }
        }

        private bool rememberPassword;
        public bool RememberPassword
        {
            get { return rememberPassword; }
            set { SetProperty(ref rememberPassword, value); }
        }

        internal StatusType Status { get; private set; }
        internal Window View { get; private set; }

        public ICommand CancelCmd { get; private set; }
        public ICommand ConnectCmd { get; private set; }

        public ConnectionVM()
        {
            Status = StatusType.NONE;
            RegisterCommands();
        }

        internal void Initialize()
        {
            View = new ConnectionWindow();
            View.Owner = App.Current.MainWindow;
            View.DataContext = this;

            if(!RememberPassword && Password != null)
            {
                Password = null;
            }

            View.ShowDialog();
        }

        private void RegisterCommands()
        {
            CancelCmd = new RelayCommand(x => Cancel());
            ConnectCmd = new RelayCommand(x => Connect(), x => CanConnect());
        }

        private void Cancel()
        {
            View?.Close();
        }

        private bool CanConnect()
        {
            return !String.IsNullOrEmpty(Uri);
        }

        private void Connect()
        {
            Status = StatusType.CONNECT;
            View?.Close();
        }

    }
}
