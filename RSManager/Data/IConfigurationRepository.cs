using RSManager.Models.Dto;
using RSManager.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Data
{
    internal interface IConfigurationRepository
    {
        ConnectionDto GetConnectionInfo();
        void AddConnectionInfo(ConnectionDto connection);
        void UpdateConnectionInfo(int id, ConnectionDto connection);
        void DeleteConnectionInfo(int id);

    }
}
