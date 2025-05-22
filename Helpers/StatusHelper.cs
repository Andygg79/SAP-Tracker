namespace SAPTracker.Helpers
{
    public static class StatusHelper
    {
        public static (string ColorCode, string StatusName) GetStatus(DateTime lastCompletedDate)
        {
            DateTime nextDueDate = lastCompletedDate.AddYears(1);
            int daysUntilDue = (nextDueDate - DateTime.Today).Days;

            if (daysUntilDue > 90)
                return ("Green", "Green");
            else if (daysUntilDue > 30)
                return ("#FFA500", "Amber"); // ✅ Correct color + label
            else
                return ("Red", "Red");
        }


    }
}

