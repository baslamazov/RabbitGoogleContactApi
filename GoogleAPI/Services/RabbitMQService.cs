using Entities;
using Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Repository;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAPI.Services
{
    public class RabbitMQService
    {
        public GoogleContactsEntity Item { get; set; }
        protected Settings Settings { get; }
        public UnitOfWork UnitOfWork { get; set; }
        public GoogleContactsService GoogleContactsService { get; set; }
        public GoogleContactsDbService GoogleContactsDbService { get; set; }
        private ILogger<RabbitMQService> _logger { get; }
        protected ConcurrentDictionary<string, ConnectionFactory> Factories { get; set; }
        protected ConcurrentDictionary<string, IConnection> Connections { get; set; }
        protected ConcurrentDictionary<IConnection, IModel> Channels { get; set; }

        public RabbitMQService(IServiceProvider provider)
        {
            UnitOfWork = provider.GetService<UnitOfWork>();
            GoogleContactsService = provider.GetService<GoogleContactsService>();
            GoogleContactsDbService = provider.GetService<GoogleContactsDbService>();
            _logger = provider.GetService<ILogger<RabbitMQService>>();
            Settings = provider.GetService<IOptions<Settings>>().Value;
            Channels = new ConcurrentDictionary<IConnection, IModel>();
            Connections = new ConcurrentDictionary<string, IConnection>();
            Factories = new ConcurrentDictionary<string, ConnectionFactory>();
            Item = new GoogleContactsEntity();
        }

        public void ServiceStart()
        {
            string hostProd = Settings.RabbitSettings.Host;
            #region
            try
            {
                var factory = Factories.GetOrAdd(hostProd, new ConnectionFactory()
                {
                    HostName = hostProd,
                    UserName = Settings.RabbitSettings.Login,
                    Password = Settings.RabbitSettings.Password

                });

                var connection = Connections.GetOrAdd(hostProd, (h) => factory.CreateConnection());

                var channel = Channels.GetOrAdd(connection, (h) => connection.CreateModel());
                channel.QueueBind(Settings.RabbitSettings.QueueName, "exchange", "", null);
                channel.BasicQos(0, 1, false);

                _logger.LogInformation(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: Settings.RabbitSettings.QueueName, autoAck: false, consumer: consumer);
                consumer.Received += async (model, ea) =>
                {

                    var body = ea.Body.ToArray();

                    var message = JObject.Parse(Encoding.UTF8.GetString(body));
                    _logger.LogInformation($"Получили {message}");

                    Item = CreateGoogleContactsObject(message);
                    _logger.LogInformation($"Item {Item}");

                    var chekItem = await UnitOfWork.GoogleContacts.GetWhereAsync(c => c.TABNUM == Item.TABNUM, 0, 1);
                    _logger.LogInformation($"chekItem {chekItem}");

                    if (chekItem.Count() == 0 && Item.PHONE != null)
                    {

                        Item.RESOURCE_NAME = await GoogleContactsService.CreateContact(Item);

                        if (Item.RESOURCE_NAME != null)
                            GoogleContactsDbService.CreateNewContact(Item);

                        await Task.Delay(1000);

                    }
                    else if (Item.PHONE != null)
                    {
                        Item.RESOURCE_NAME = chekItem.ToList<GoogleContactsEntity>()[0].RESOURCE_NAME;
                        Item.Id = chekItem.ToList()[0].Id;
                        GoogleContactsDbService.UpdateContact(Item);
                        //await UnitOfWork.GoogleContacts.UpdateAsync(Item);
                        //await GoogleContactsService.UpdateContact(Item);
                    }    

                    channel.BasicAck(ea.DeliveryTag, false);
                    _logger.LogInformation($"Все гуд");

                };
                _logger.LogInformation($"Consume to - {Settings.RabbitSettings.QueueName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Где-то плачет маленький карлик");
            }
            #endregion
            #region
           // var factory = new ConnectionFactory()
           // {
           //     HostName = Settings.RabbitSettings.Host,
           //     Port = Protocols.DefaultProtocol.DefaultPort,
           //     UserName = Settings.RabbitSettings.Login,
           //     Password = Settings.RabbitSettings.Password,
           // };
           // using var connection = factory.CreateConnection();
           // using var channel = connection.CreateModel();
           // channel.QueueDeclare(queue: Settings.RabbitSettings.QueueName,
           //                      durable: false,
           //                      exclusive: false,
           //                      autoDelete: false,
           //                      arguments: null);
           // _logger.LogInformation(" [*] Waiting for messages.");
           // var consumer = new EventingBasicConsumer(channel);
           // consumer.Received += async (model, ea) =>
           //{
           //    try
           //    {
           //        var body = ea.Body.ToArray();
           //        var message = JObject.Parse(Encoding.UTF8.GetString(body));//(JObject)JToken.FromObject(body); //Encoding.UTF8.GetString(body);
           //         _logger.LogInformation($"Получили {message}");

           //        Item = CreateGoogleContactsObject(message);
           //        _logger.LogInformation($"Item {Item}");
           //        var chekItem = await UnitOfWork.GoogleContacts.GetWhereAsync(c => c.TABNUM == Item.TABNUM, 0, 1);
           //        _logger.LogInformation($"chekItem {chekItem}");
           //        if (chekItem.Count() == 0 && Item.PHONE.Length != 0)
           //        {

           //            Item.RESOURCE_NAME = await GoogleContactsService.CreateContact(Item);

           //            await GoogleContactsDbService.CreateNewContact(Item);
           //        }
           //        else if (Item.PHONE.Length != 0)
           //        {
           //            Item.RESOURCE_NAME = chekItem.ToList()[0].RESOURCE_NAME;
           //            Item.Id = chekItem.ToList()[0].Id;
           //            await UnitOfWork.GoogleContacts.UpdateAsync(Item);
           //            await GoogleContactsService.UpdateContact(Item);
           //        }
           //        _logger.LogInformation($"Все гуд");
           //         //schannel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
           //     }
           //    catch (Exception ex)
           //    {
           //        _logger.LogInformation("Где-то плачет маленький карлик");
           //        _logger.LogError(ex.Message);
           //        _logger.LogError(ex.InnerException.Message);
           //    }
           //};

           // //channel.BasicConsume(Settings.RabbitSettings.QueueName, false,consumer);
           // channel.BasicConsume(queue: Settings.RabbitSettings.QueueName,
           //                      autoAck: true,
           //                      consumer: consumer);
           // _logger.LogInformation($"Consume to - ATS_USERS");
            #endregion
        }
        public GoogleContactsEntity CreateGoogleContactsObject(JObject message)
        {
            GoogleContactsEntity CurrentItem = new GoogleContactsEntity()
            {
                FIO = message["FIO"].ToString(),
                DOLJNOST = message["DOLJNOST"].ToString(),
                BANK = message["BANK"].ToString(),
                BLOCKED = Convert.ToBoolean(message["BLOCKED"]),
                EMP_OID = message["EMP_OID"].ToString(),
                PLACE_NAME = message["PLACE_NAME"].ToString(),
                EMAIL_SD = message["EMAIL_SD"].ToString(),
                //TABNUM = message["TABNUM"].Type != JTokenType.Null ? Convert.ToInt32(message["TABNUM"]) : 
            };
            if (message.ContainsKey("INTERNAL_NUMBER"))
                CurrentItem.INTERNAL_NUMBER = Convert.ToInt32(message["INTERNAL_NUMBER"]);
            if (message["TABNUM"].Type != JTokenType.Null)
                CurrentItem.TABNUM = Convert.ToInt32(message["TABNUM"]);
            if (message["PHONE"].Type != JTokenType.Null)
                CurrentItem.PHONE = message["PHONE"].ToString().Replace("+7", "98");
            return CurrentItem;
        }
    }
}