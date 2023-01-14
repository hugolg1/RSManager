using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Models.Security
{
    internal class Credential
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public bool RememberPassword { get; set; }
    }
}
