using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Models.Dto.Data
{
    internal class DataSpaceDto
    {
        public string Path { get; private set; }
        public string Name { get; private set; }

        public DataSpaceDto(string path, string name)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
