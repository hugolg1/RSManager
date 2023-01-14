using RSManager.Models.Dto;
using RSManager.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Services
{
    internal interface IConfigurationService
    {
        void SaveConnectionInfo(ConnectionDto connection);
        Connection GetConnectionInfo();
    }
}
