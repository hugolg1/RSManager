using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Models.Dto.Data
{
    internal class DataUploadRequest
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public Stream Content { get; set; }
        public DataSpaceDto Destination { get; private set; }

        public DataUploadRequest(string name, long size, Stream content, DataSpaceDto destination)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Size = size;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Destination = destination ?? throw new ArgumentNullException(nameof(destination));
        }
    }
}
