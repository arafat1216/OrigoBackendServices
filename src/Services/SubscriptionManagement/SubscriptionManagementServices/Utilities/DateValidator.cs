

namespace SubscriptionManagementServices.Utilities
{
    public class DateValidator
    {
        public static readonly int _maxDaysForAll = 30;
        

        public static bool ValidDateForAction(DateOnly transferDate, DateOnly today, int limitDays)
        {
            var maxDays = today.AddDays(_maxDaysForAll);

            if (maxDays < transferDate) return false;

            if (transferDate.DayOfWeek == DayOfWeek.Saturday || transferDate.DayOfWeek == DayOfWeek.Sunday) return false;

            var buisinessDays = CountBusinessDays(today, transferDate);

            var firstValidDate = today.AddDays(limitDays + buisinessDays);

            if (firstValidDate <= transferDate) return true;

            return false;

        }
        public static int CountBusinessDays(DateOnly startDate, DateOnly endDate)
        {
            int weekendDaysCount = 0;
            if (startDate > endDate)
            {
                DateOnly temp = startDate;
                startDate = endDate;
                endDate = temp;
            }
            var end = endDate.ToDateTime(TimeOnly.Parse("08:00 PM"));
            var start = startDate.ToDateTime(TimeOnly.Parse("08:00 PM"));

            TimeSpan interval = end - start;
            var days = interval.Days;

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
    public interface IDateTimeProvider
    {
        DateTime GetNow();
    }
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetNow() => DateTime.Now;
    }

}
