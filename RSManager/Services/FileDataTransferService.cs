using RSManager.Models.Dto;
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
    internal class FileDataTransferService : IDataTransferService
    {
        public async Task CreateDataSpaceAsync(DataSpaceDto dataSpace, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!Directory.Exists(dataSpace.Path) && !cancellationToken.IsCancellationRequested)
            {
                Directory.CreateDirectory(dataSpace.Path);
            }
        }

        public async Task UploadDataAsync(DataUploadRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            string destinationPath = Path.Combine(request.Destination.Path, request.Name);

            using (FileStream fs = new FileStream(destinationPath, FileMode.Create, FileAccess.ReadWrite))
            {
                await request.Content.CopyToAsync(fs, 81920, cancellationToken);
            }
        }
    }
}
