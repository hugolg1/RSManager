using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Models
{
    internal enum Errors
    {
        Internal = 1,
    }

    internal static class ErrorsExtension
    {
        internal static int Code(this Errors error)
        {
            return (int)error;
        }
    }
}
