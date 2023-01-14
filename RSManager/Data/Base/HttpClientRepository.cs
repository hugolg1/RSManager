using RSManager.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace RSManager.Data.Base
{
    internal abstract class HttpClientRepository : IRepository
    {

        protected HttpClient GetHttpClient(Connection connection, string mediaType = "application/json")
        {
            var uri = new Uri(connection.Uri);
            var networkCredential = new NetworkCredential(connection.Credential.User, connection.Credential.Password, connection.Credential.Domain);
            var credentialsCache = new CredentialCache { { uri, "NTLM", networkCredential } };
            var handler = new HttpClientHandler { Credentials = credentialsCache };
            var httpClient = new HttpClient(handler) { BaseAddress = uri };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            return httpClient;
        }

    }
}
