using RSManager.Models.Dto;
using RSManager.Models.Reports;
using RSManager.Models.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Data
{
    internal interface IReportRepository
    {
        Task<Result<List<ItemApiResponse>>> GetCatalogItemsAsync(Connection connection);

        Task<Result<Stream>> GetReportContentAsync(Connection connection, string id);
    }
}
