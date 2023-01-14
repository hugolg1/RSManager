using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Models.Security
{
    internal class Connection
    {
        public string Uri { get; set; }
        public Credential Credential { get; set; }

        public Connection()
        {
            Credential = new Credential();
        }

    }
}
