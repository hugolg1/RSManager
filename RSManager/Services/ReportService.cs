using RSManager.Data;
using RSManager.Models;
using RSManager.Models.Dto;
using RSManager.Models.Reports;
using RSManager.Models.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RSManager.Services
{
    internal class ReportService : IReportService
    {
        private readonly IReportRepository reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            this.reportRepository = reportRepository;
        }

        public async Task<Result<List<Item>>> GetCatalogItemsAsync(Connection connection)
        {
            var catalogItemsResult = await reportRepository.GetCatalogItemsAsync(connection);
            if (!catalogItemsResult.IsSuccess)
            {
                return Result<List<Item>>.Failure(catalogItemsResult.Error);
            }

            List<Item> items = catalogItemsResult.Item.Where(x => x.Size != 0).Select(x => new Item()
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Path = x.Path,
                ParentId = x.ParentId,
                Size = x.Size,
                CreatedBy = x.CreatedBy,
                CreatedDate = x.CreatedDate,
                ModifiedBy = x.ModifiedBy,
                ModifiedDate = x.ModifiedDate,
            }).ToList();

            foreach (var item in items)
            {
                item.CalculateSize(items);
            }

            return Result<List<Item>>.Success(items);
        }

        public async Task<Result<Stream>> GetReportContentAsync(Connection connection, string id)
        {
            var contentResult = await reportRepository.GetReportContentAsync(connection, id);
            if (!contentResult.IsSuccess)
            {
                return Result<Stream>.Failure(contentResult.Error);
            }

            return Result<Stream>.Success(contentResult.Item);
        }

    }
}
