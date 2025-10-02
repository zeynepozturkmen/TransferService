using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransferService.Domain.Enums
{
    public enum TransferStatus
    {
        Pending,
        Completed,
        Cancelled,
        Failed,
        Closed
    }
}
