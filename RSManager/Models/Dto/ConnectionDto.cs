using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Models.Dto
{
    internal class ConnectionDto
    {
        public string Uri { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }
        public string Domain { get; private set; }
        public bool RememberPassword { get; private set; }

        public ConnectionDto(string uri, string user, string password, string domain, bool rememberPassword)
        {
            Uri = uri;
            User = user;
            Password = password;
            Domain = domain;
            RememberPassword = rememberPassword;
        }
    }
}
