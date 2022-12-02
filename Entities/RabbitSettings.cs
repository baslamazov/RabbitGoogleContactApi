using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities
{
    public class RabbitSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int ConnectionTimeout { get; set; }
        public string AuthMechanism { get; set; }
        public string Vhost { get; set; }
        public bool NoDelay { get; set; }
        public string QueueName { get; set; }
    }
}
