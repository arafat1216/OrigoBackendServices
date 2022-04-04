

namespace SubscriptionManagementServices.Utilities
{
    public class DateValidator
    {
        public static readonly int _maxDaysForAll = 30;
        

        public static bool ValidDateForAction(DateTime transferDate, DateTime today, int limitDays)
        {

            var maxDays = today.AddDays(_maxDaysForAll);

            var buisinessDays = CountBusinessDays(today, transferDate);

            var firstValidDate = today.AddDays(limitDays + buisinessDays);


            if (firstValidDate.Date <= transferDate.Date && maxDays.Date > transferDate.Date) return true;

            return false;

        }
        public static int CountBusinessDays(DateTime startDate, DateTime endDate)
        {
            int weekendDaysCount = 0;
            if (startDate > endDate)
            {
                DateTime temp = startDate;
                startDate = endDate;
                endDate = temp;
            }
            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {
                var testDate = startDate.AddDays(i);
                if (testDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    weekendDaysCount += 1;
                }
                if (testDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekendDaysCount += 1;
                }
            }
            return weekendDaysCount;
        }

    }
}
