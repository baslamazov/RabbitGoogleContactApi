using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class Settings
    {
        public string ConnectionString { get; set; }
        public string CiTaskQueue { get; set; }
        public RabbitSettings RabbitSettings { get; set; }
    }
}
