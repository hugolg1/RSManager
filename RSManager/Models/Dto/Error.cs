using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Models.Dto
{
    internal class Error
    {
        public int Code { get; private set; }
        public string Description { get; private set; }

        public Error(int code)
        {
            this.Code = code;
        }

        public Error(int code, string description)
        {
            this.Code = code;
            this.Description = description;
        }

        public override string ToString()
        {
            return $"Error ({Code}) {Description}".TrimEnd();
        }

    }
}
