using RSManager.Models.Dto;
using RSManager.Models.Reports;
using RSManager.Models.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Facade
{
    internal interface IDownloadPackageFacade
    {
        List<Item> GetCatalogItems();

        Task<Result<Stream>> GetReportContentAsync(string id);

    }
}
