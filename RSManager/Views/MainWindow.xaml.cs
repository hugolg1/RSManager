using RSManager.Data;
using RSManager.Services;
using RSManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RSManager.Views
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool loaded;
        private MainVM vm;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                return;
            }

            //TODO DI
            IConfigurationRepository configurationRepository = new ConfigurationRepository();
            IReportRepository reportRepository = new ReportRepository();

            IProtectionService protectionService = new ProtectionService();
            IConfigurationService configurationService = new ConfigurationService(configurationRepository, protectionService);
            IReportService reportService = new ReportService(reportRepository);
            IDataTransferService dataTransferService = new FileDataTransferService();

            vm = new MainVM(reportService, configurationService, dataTransferService);
            this.DataContext = vm;
            vm.Initialize();
            loaded = true;
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (vm == null)
            {
                return;
            }

            switch (e.ChangedButton)
            {
                case MouseButton.XButton1://Back button
                    if (vm.NavigateToBackCmd.CanExecute(null))
                    {
                        vm.NavigateToBackCmd.Execute(null);
                    }
                    break;
                case MouseButton.XButton2://forward button
                    if (vm.NavigateToAheadCmd.CanExecute(null))
                    {
                        vm.NavigateToAheadCmd.Execute(null);
                    }
                    break;
            }
        }
    }
}
