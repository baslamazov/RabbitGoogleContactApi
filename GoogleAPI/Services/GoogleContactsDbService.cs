using Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleAPI.Services
{
    public class GoogleContactsDbService
    {
        public UnitOfWork UnitOfWork { get; }
        private readonly ILogger<GoogleContactsDbService> _logger;


        public GoogleContactsDbService(IServiceProvider provider)
        {
            _logger = provider.GetService<ILogger<GoogleContactsDbService>>();
            UnitOfWork = provider.GetService<UnitOfWork>();
        }
        public async Task<GoogleContactsEntity> UpdateContact(GoogleContactsEntity googleContacts)
        {
            var bankItem = await UnitOfWork.Bank.GetWhereAsync(c => c.NAME == googleContacts.BANK, 0, 1);
            if (bankItem.Count() != 0)
            {
                googleContacts.BANK_QUEUE = bankItem.First().QUEUE;
            }
            await UnitOfWork.GoogleContacts.UpdateAsync(googleContacts);
            _logger.LogInformation($"Запись в бд обновлена: {googleContacts.Id}");
            return googleContacts;
        }

        public async Task<GoogleContactsEntity> CreateNewContact(GoogleContactsEntity googleContacts)
        {
            var bankItem = await UnitOfWork.Bank.GetWhereAsync(c => c.NAME == googleContacts.BANK, 0, 1);
            if (bankItem.Count() != 0)
            {
                googleContacts.BANK_QUEUE = bankItem.First().QUEUE;
            }
            GoogleContactsEntity contactItem = await UnitOfWork.GoogleContacts.AddAsync(googleContacts);
            _logger.LogInformation($"Создана новая запись в main_table: {contactItem.Id}");

            return contactItem;
        }
    }
}
