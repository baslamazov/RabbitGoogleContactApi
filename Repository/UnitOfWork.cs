using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class UnitOfWork
    {
        public IGoogleContactsRepository GoogleContacts { get; }
        public IBankRepository Bank { get; }
        public UnitOfWork
            (
                IGoogleContactsRepository googleContacts,
                IBankRepository bank
            )
        {
            GoogleContacts = googleContacts;
            Bank = bank;
        }

    }
}
