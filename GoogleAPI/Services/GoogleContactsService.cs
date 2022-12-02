using Entities.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.PeopleService.v1;
using Google.Apis.PeopleService.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleAPI.Services
{
    public class GoogleContactsService
    {
        public UserCredential credential { get; set; }
        public PeopleServiceService service { get; set; }
        public UnitOfWork UnitOfWork { get; set; }
        private readonly ILogger<GoogleContactsService> _logger;

        public GoogleContactsService(IServiceProvider provider)
        {
            _logger = provider.GetService<ILogger<GoogleContactsService>>();
            UnitOfWork = provider.GetService<UnitOfWork>();
            _ = AutorizationGoogle();

        }

        public Person ConvertToPerson(GoogleContactsEntity googleContact)
        {
            Person newContact = new Person();
            try
            {
                newContact.Names = new List<Name>()
                 { new Name()
                    {
                        FamilyName = googleContact.FIO.Split(" ")[0],
                        GivenName = googleContact.FIO.Split(" ")[1],
                        MiddleName = googleContact.FIO.Split(" ").Length > 2 ? googleContact.FIO.Split(" ")[2] : "",
                        DisplayName = googleContact.FIO
                    }
                 };


                newContact.PhoneNumbers = new List<PhoneNumber>() { new PhoneNumber() { Value = googleContact.PHONE } };
                newContact.EmailAddresses = new List<EmailAddress>() { new EmailAddress() { Value = googleContact.EMAIL_SD } };
                newContact.Organizations = new List<Organization>()
                { new Organization()
                    {
                        Title = googleContact.DOLJNOST,
                        Name = googleContact.BANK
                    }
                };
                newContact.UserDefined = new List<UserDefined>();
                newContact.UserDefined.Add(new UserDefined()
                {
                    Key = "Табельный номер",
                    Value = googleContact.TABNUM.ToString()
                });
                newContact.UserDefined.Add(new UserDefined()
                {
                    Key = "Местоположение",
                    Value = googleContact.PLACE_NAME
                });
                newContact.UserDefined.Add(new UserDefined()
                {
                    Key = "Внутренний номер",
                    Value = googleContact.INTERNAL_NUMBER.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
            service.People.CreateContact(newContact);
            return newContact;
        }
        public async Task<Person> UpdateContact(GoogleContactsEntity googleContact)
        {
            //await AutorizationGoogle();
            var dbItem = await UnitOfWork.GoogleContacts.GetWhereAsync(c => c.TABNUM == googleContact.TABNUM, 0, 1);

            var contact = service.People.Get(dbItem.ToList()[0].RESOURCE_NAME);
            contact.PersonFields = "metadata";

            var newGoogleContact = ConvertToPerson(googleContact);
            var person = await contact.ExecuteAsync();
            var eTag = person.Metadata.Sources;
            newGoogleContact.ETag = person.ETag;

            newGoogleContact.Metadata = new PersonMetadata();
            //newGoogleContact.Metadata.Sources = eTag;

            var resourceName = dbItem.ToList()[0].RESOURCE_NAME;
            var updateContactRequest = new PeopleResource.UpdateContactRequest(service, newGoogleContact, resourceName);
            //var requets = service.People.UpdateContact(newGoogleContact, resourceName);
            updateContactRequest.UpdatePersonFields = "addresses,clientData,emailAddresses,externalIds,imClients,miscKeywords,occupations,organizations,phoneNumbers,relations,sipAddresses,userDefined";
            var answer = await updateContactRequest.ExecuteAsync();
            _logger.LogInformation(answer.ToString());
            return null;
        }

        public async Task<string> CreateContact(GoogleContactsEntity entityContact)
        {
            //await AutorizationGoogle();
            try
            {
                var googlecontact = ConvertToPerson(entityContact);
                PeopleResource.CreateContactRequest createContactRequest = new PeopleResource.CreateContactRequest(service, googlecontact);
                //var requets = service.People.CreateContact(googlecontact);
                var answer = await createContactRequest.ExecuteAsync();
                return answer.ResourceName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Где-то плачет маленький карлик");
                return null;
            }

        }

        public async Task<string> AutorizationGoogle()
        {


            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
               new ClientSecrets
               {
                   ClientId = "333239554968-ts8epm9nenpvm9acohu86kubu7ub4h30.apps.googleusercontent.com",
                   ClientSecret = "iI6e41F-SBU_JSaJ4-nDIE0D",
               },
               new[] { "Contact", "https://www.googleapis.com/auth/contacts" },
               "me",
               CancellationToken.None).Result;
            //HttpClientHandler clientHandler = new HttpClientHandler();
            //clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            //HttpClient client = new HttpClient(clientHandler);
            // Create the service.
            service = new PeopleServiceService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SbsGoogleContactsAPI",
            });
            //service.HttpClientInitializer.Initialize(new ConfigurableHttpClient(new ConfigurableMessageHandler(clientHandler)));
            return service.ApiKey;
        }

        public void GoogleContacts()
        {
            //AutorizationGoogle();

            // Groups list ////////////
            ContactGroupsResource groupsResource = new ContactGroupsResource(service);
            ContactGroupsResource.ListRequest listRequest = groupsResource.List();
            ListContactGroupsResponse response = listRequest.Execute();

            // eg to show name of each group
            List<string> groupNames = new List<string>();
            foreach (ContactGroup group in response.ContactGroups)
            {
                groupNames.Add(group.FormattedName);
            }
            ///////////////

            // Contact list ////////////
            PeopleResource.ConnectionsResource.ListRequest peopleRequest =
                service.People.Connections.List("people/me");
            peopleRequest.PersonFields = "addresses,biographies,birthdays,clientData,emailAddresses,externalIds,genders,imClients,memberships,metadata,miscKeywords,names,occupations,organizations,phoneNumbers,relations,sipAddresses,userDefined";
            peopleRequest.SortOrder = (PeopleResource.ConnectionsResource.ListRequest.SortOrderEnum)1;
            ListConnectionsResponse people = peopleRequest.Execute();
            // eg to show display name of each contact
            List<string> contacts = new List<string>();
            //Console.WriteLine(peopleRequest.Sources.Value);
            foreach (var person in people.Connections)
            {
                contacts.Add(person.Names[0].DisplayName);
                Console.WriteLine(person.Organizations[0].Domain);
                //
            }
            /////////////////
        }
    }

}


