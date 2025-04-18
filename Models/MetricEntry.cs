using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPTracker.Models
{
    public class MetricEntry
    {
        public string MetricName { get; set; } = string.Empty;
        public DateTime LastUpdatedDate { get; set; }
        public string StatusColor { get; set; } = "Gray";
    }
}

