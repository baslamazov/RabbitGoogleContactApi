using Contracts;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class BankRepository : BaseRepository<BankEntity>, IBankRepository
    {
        public BankRepository(IServiceProvider provider)
            : base(provider)
        {
        }

    }
}
