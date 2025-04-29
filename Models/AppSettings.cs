using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPTracker.Models
{
    public class AppSettings
    {
        public FirebaseSettings Firebase { get; set; } = new();
    }

    public class FirebaseSettings
    {
        public string ApiKey { get; set; } = "";
        public string ProjectId { get; set; } = "";
    }
}


