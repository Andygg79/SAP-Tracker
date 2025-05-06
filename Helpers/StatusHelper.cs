namespace SAPTracker.Helpers
{
    public static class StatusHelper
    {
        public static string GetStatusColor(DateTime selectedDate)
        {
            var daysDifference = (DateTime.Today - selectedDate).TotalDays;

            if (daysDifference > 90)
                return "Red";
            else if (daysDifference > 30)
                return "Amber";
            else
                return "Green";
        }
    }
}
