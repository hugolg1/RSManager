using RSManager.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Models.Reports
{
    internal class Item : BaseObservable
    {
        private string id;
        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string path;
        public string Path
        {
            get { return path; }
            set { SetProperty(ref path, value); }
        }

        private string type;
        public string Type
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }

        public bool IsFolder
        {
            get { return Type == "Folder"; }
        }

        public int Level
        {
            get { return Path?.Count(x => x == '/') ?? 1; }
        }

        private string parentId;
        public string ParentId
        {
            get { return parentId; }
            set { SetProperty(ref parentId, value); }
        }

        private long? size;
        public long? Size
        {
            get { return size; }
            set { SetProperty(ref size, value); }
        }

        private string createdBy;
        public string CreatedBy
        {
            get { return createdBy; }
            set { SetProperty(ref createdBy, value); }
        }

        private DateTime createdDate;
        public DateTime CreatedDate
        {
            get { return createdDate; }
            set { SetProperty(ref createdDate, value); }
        }

        private string modifiedBy;
        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { SetProperty(ref modifiedBy, value); }
        }

        private DateTime? modifiedDate;
        public DateTime? ModifiedDate
        {
            get { return modifiedDate; }
            set { SetProperty(ref modifiedDate, value); }
        }

        public void CalculateSize(List<Item> items)
        {
            if (!IsFolder || Path == null)
            {
                return;
            }

            long? totalSize = items.Where(x => x.Size > 0 && (x.Path?.StartsWith(Path) ?? false) && x.Level > Level).Sum(x => x.Size);
            Size = totalSize == 0 ? null : totalSize;
        }

    }
}
