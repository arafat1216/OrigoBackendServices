

namespace SubscriptionManagementServices.Utilities
{
    public class DateValidator
    {
        public static readonly int _maxDaysForAll = 30;
        

        public static bool ValidDateForAction(DateTime transferDate, DateTime today, int limitDays)
        {

            var maxDays = today.AddDays(_maxDaysForAll);

            if (maxDays.Date <= transferDate.Date) return false;

            var buisinessDays = CountBusinessDays(today.Date, transferDate.Date);

            var firstValidDate = today.AddDays(limitDays + buisinessDays);

            if (firstValidDate.Date <= transferDate.Date) return true;

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
                
                if (testDate.DayOfWeek == DayOfWeek.Saturday && i != days)
                {
                    weekendDaysCount += 1;
                }
                
                if (testDate.DayOfWeek == DayOfWeek.Sunday && i != days)
                {
                    weekendDaysCount += 1;
                }

                if (i == days)
                {
                    if (testDate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        weekendDaysCount += 2;
                    }
                    if (testDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        weekendDaysCount += 1;
                    }
                }
            }
            return weekendDaysCount;
        }

    }
}
