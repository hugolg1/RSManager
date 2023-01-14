using RSManager.Data;
using RSManager.Models.Dto;
using RSManager.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Services
{
    internal class ConfigurationService : IConfigurationService
    {

        private readonly IConfigurationRepository configurationRepository;
        private readonly IProtectionService protectionService;

        public ConfigurationService(IConfigurationRepository configurationRepository, IProtectionService protectionService)
        {
            this.configurationRepository = configurationRepository;
            this.protectionService = protectionService;
        }

        public Connection GetConnectionInfo()
        {
            var result = configurationRepository.GetConnectionInfo();
            return new Connection()
            {
                Uri = result?.Uri,
                Credential = new Credential()
                {
                    User = result?.User,
                    Password = result?.Password != null ? protectionService.Decrypt(result.Password) : null,
                    Domain = result?.Domain,
                    RememberPassword = result?.RememberPassword ?? false,
                }
            };
        }

        public void SaveConnectionInfo(ConnectionDto connection)
        {
            var dbConnection = configurationRepository.GetConnectionInfo();

            if (connection.Password != null)
            {
                connection = new ConnectionDto(
                    connection.Uri,
                    connection.User,
                    protectionService.Encrypt(connection.Password),
                    connection.Domain,
                    connection.RememberPassword);
            }

            if (dbConnection != null)
            {
                configurationRepository.UpdateConnectionInfo(0, connection);
            }
            else
            {
                configurationRepository.AddConnectionInfo(connection);
            }
        }
    }
}
