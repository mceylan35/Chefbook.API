using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chefbook.API.Services.Interface
{
    public interface ITransactionAble
    {
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void DisposeTransaction();
    }
}
