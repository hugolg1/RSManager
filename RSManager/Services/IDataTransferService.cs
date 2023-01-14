using RSManager.Models.Dto.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RSManager.Services
{
    internal interface IDataTransferService
    {
        Task CreateDataSpaceAsync(DataSpaceDto dataSpace, CancellationToken cancellationToken = default(CancellationToken));

        Task UploadDataAsync(DataUploadRequest request, CancellationToken cancellationToken = default(CancellationToken));

    }
}
