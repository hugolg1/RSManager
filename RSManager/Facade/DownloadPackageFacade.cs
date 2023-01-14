using RSManager.Models.Dto;
using RSManager.Models.Reports;
using RSManager.Models.Security;
using RSManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Facade
{
    internal class DownloadPackageFacade : IDownloadPackageFacade
    {
        private readonly IReportService reportService;
        private readonly Connection connection;
        private readonly List<Item> items;

        public DownloadPackageFacade(IReportService reportService, Connection connection, List<Item> items)
        {
            this.reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.items = items ?? new List<Item>();
        }

        public List<Item> GetCatalogItems()
        {
            return items;
        }

        public async Task<Result<Stream>> GetReportContentAsync(string id)
        {
            return await reportService.GetReportContentAsync(connection, id);
        }
    }
}
