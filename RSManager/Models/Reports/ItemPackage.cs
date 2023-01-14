using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Models.Reports
{
    internal class ItemPackage
    {     
        public List<Item> Items { get; private set; }

        public ItemPackage(List<Item> items)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
        }
    }
}
