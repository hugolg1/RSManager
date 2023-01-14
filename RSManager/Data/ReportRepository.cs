using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RSManager.Models;
using RSManager.Models.Dto;
using RSManager.Models.Reports;
using RSManager.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using RSManager.Data.Base;
using System.IO;

namespace RSManager.Data
{
    internal class ReportRepository : HttpClientRepository, IReportRepository
    {
        public async Task<Result<List<ItemApiResponse>>> GetCatalogItemsAsync(Connection connection)
        {
            try
            {
                using (HttpClient httpClient = GetHttpClient(connection))
                {
                    var response = await httpClient.GetAsync("CatalogItems");
                    if (!response.IsSuccessStatusCode)
                    {
                        return Result<List<ItemApiResponse>>.Failure(new Error(Errors.Internal.Code(), response.ToString()));
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    var resultValue = JsonConvert.DeserializeObject<JObject>(result);
                    var itemsReponse = JsonConvert.DeserializeObject<List<ItemApiResponse>>(resultValue.Last.ToString().Replace(@"""value"":", ""));  
                    return Result<List<ItemApiResponse>>.Success(itemsReponse.ToList());
                }
            }
            catch (Exception ex)
            {
                return Result<List<ItemApiResponse>>.Failure(new Error(Errors.Internal.Code(), ex.Message));
            }
        }

        public async Task<Result<Stream>> GetReportContentAsync(Connection connection, string id)
        {
            try
            {
                using (HttpClient httpClient = GetHttpClient(connection))
                {
                    var response = await httpClient.GetAsync($"Reports({id})/Content/$value/");
                    if (!response.IsSuccessStatusCode)
                    {
                        return Result<Stream>.Failure(new Error(Errors.Internal.Code(), response.ToString()));
                    }

                    var result = await response.Content.ReadAsStreamAsync();
                    return Result<Stream>.Success(result);
                }
            }
            catch (Exception ex)
            {
                return Result<Stream>.Failure(new Error(Errors.Internal.Code(), ex.Message));
            }
        }

    }
}
