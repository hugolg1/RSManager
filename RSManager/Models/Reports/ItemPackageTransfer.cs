using RSManager.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSManager.Models.Reports
{
    internal class ItemPackageTransfer : BaseObservable
    {
        internal enum TransferStatus
        {
            NotStarted = 1,
            Started = 2,
            Cancelled = 3,
            Completed = 4,
            Error = 5,
        }

        private long packageSize;
        public long PackageSize
        {
            get { return packageSize; }
            private set
            {
                SetProperty(ref packageSize, value);
                Notify(nameof(TransferPercent));
            }
        }

        private TransferStatus status;
        public TransferStatus Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private long transferSize;
        public long TransferSize
        {
            get { return transferSize; }
            set
            {
                SetProperty(ref transferSize, value);
                Notify(nameof(TransferPercent));
            }
        }

        public long TransferPercent
        {
            get
            {
                if (TransferSize == 0 || PackageSize == 0)
                {
                    return 100;
                }
                return (TransferSize * 100) / PackageSize;
            }
        }

        public DateTime CreatedDate { get; private set; }

        public string DestinationPath { get; private set; }


        public readonly ItemPackage ItemPackage;

        public ItemPackageTransfer(ItemPackage package, string destinationPath)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.ItemPackage = package;
            this.PackageSize = package.Items.Where(x => x.Size > 0).Sum(x => x.Size) ?? 0;
            this.TransferSize = 0;
            this.Status = TransferStatus.NotStarted;
            this.CreatedDate = DateTime.Now;
            this.DestinationPath = destinationPath;
        }

    }
}
