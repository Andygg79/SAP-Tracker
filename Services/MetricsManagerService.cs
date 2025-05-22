using SAPTracker.Models;

namespace SAPTracker.Services
{
    public static class MetricsManagerService
    {
        public static List<MetricEntry> GetMetricsForBranch(string branch)
        {
            switch (branch.ToLower())
            {
                case "army":
                    return GetArmyMetrics();
                case "navy":
                    return GetNavyMetrics();
                case "air force":
                    return GetAirForceMetrics();
                case "marines":
                    return GetMarineMetrics();
                default:
                    return GetDefaultMetrics(); // fallback if no branch matched
            }
        }

        private static List<MetricEntry> GetDefaultMetrics()
        {
            return new List<MetricEntry>
            {
                new MetricEntry { MetricName = "Periodic Health Assessment (PHA)" },
                new MetricEntry { MetricName = "Dental Readiness" },
                new MetricEntry { MetricName = "Vision Screening" },
                new MetricEntry { MetricName = "Hearing Test" },
                new MetricEntry { MetricName = "Fitness Test" },
                new MetricEntry { MetricName = "Height and Weight" },
                new MetricEntry { MetricName = "SGLV Form (Life Insurance)" },
                new MetricEntry { MetricName = "DD93 Form (Emergency Contacts)" }
            };
        }

        private static List<MetricEntry> GetArmyMetrics()
        {
            var metrics = GetDefaultMetrics();
            metrics.Add(new MetricEntry { MetricName = "Weapons Qualification" });
            metrics.Add(new MetricEntry { MetricName = "NCOER (Enlisted Evaluation)" });
            return metrics;
        }

        private static List<MetricEntry> GetNavyMetrics()
        {
            var metrics = GetDefaultMetrics();
            metrics.Add(new MetricEntry { MetricName = "EVAL (Navy Evaluation Report)" });
            return metrics;
        }

        private static List<MetricEntry> GetAirForceMetrics()
        {
            var metrics = GetDefaultMetrics();
            metrics.Add(new MetricEntry { MetricName = "EPR (Enlisted Performance Report)" });
            return metrics;
        }

        private static List<MetricEntry> GetMarineMetrics()
        {
            var metrics = GetDefaultMetrics();
            metrics.Add(new MetricEntry { MetricName = "Rifle Qualification" });
            metrics.Add(new MetricEntry { MetricName = "FitRep (Fitness Report)" });
            return metrics;
        }
    }
}
