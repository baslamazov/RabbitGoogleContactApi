using Contracts;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class GoogleContactsRepository : BaseRepository<GoogleContactsEntity>, IGoogleContactsRepository
    {
        public GoogleContactsRepository(IServiceProvider provider)
            : base(provider)
        {
        }

    }
}
