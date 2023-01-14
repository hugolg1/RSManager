using RSManager.Models;
using RSManager.Models.Dto;
using RSManager.Models.Reports;
using RSManager.Models.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Services
{
    internal interface IReportService
    {
        Task<Result<List<Item>>> GetCatalogItemsAsync(Connection connection);

        Task<Result<Stream>> GetReportContentAsync(Connection connection, string id);

    }
}
