namespace SAPTracker.Models
{
    public class MetricEntry
    {
        public string MetricName { get; set; } = string.Empty;
        public DateTime LastUpdatedDate { get; set; }
        public string StatusColor { get; set; } = "Gray";

        // ✅ Computed property for formatted date display
        public string DisplayDate => LastUpdatedDate.ToString("yyyy-MM-dd");

        // ✅ Computed property for UI color conversion
        public Color UiColor => StatusColor.ToUpper() switch
        {
            "RED" => Colors.Red,
            "AMBER" => Colors.Orange,
            "GREEN" => Colors.Green,
            _ => Colors.Gray
        };
    }
}
