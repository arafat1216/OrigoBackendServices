

namespace SubscriptionManagementServices.Utilities
{
    public class DateValidator
    {

        /// <summary>
        /// Max amount of working days allowed for all order types.
        /// </summary>
        public static readonly int _maxDaysForAll = 30;

        /// <summary>
        /// Validating the date that the user is requesting to do a action for Subscription Management.
        /// Different actions will have it's own limitation of days before the order can excpect to be done.
        /// All Subscription actions have a limitation of 30 working days ahead counted from the date today.
        /// Weekend days (lik Saturday and Sunday) is not counted in the max days ahead.
        /// </summary>
        /// <param name="transferDate">The date that is requested to be the transfer date.</param>
        /// <param name="today">Today's date.</param>
        /// <param name="limitDays">The limited days before the first available transfer day. Transfer can not be done before the limited days from todays date.</param>
        /// <returns></returns>
        public static bool ValidDateForAction(DateOnly transferDate, DateOnly today, int limitDays)
        {
            if (transferDate.DayOfWeek == DayOfWeek.Saturday || transferDate.DayOfWeek == DayOfWeek.Sunday) throw new ArgumentException($"Transfer date can not be a {transferDate.DayOfWeek}.");

            var latestPossibleDate = GetDateAfter(_maxDaysForAll, today);
            if (latestPossibleDate < transferDate) throw new ArgumentException($"Invalid date. Needs to be within {_maxDaysForAll} business days.");

            var earliestPossibleDate = GetDateAfter(limitDays, today);
            if (earliestPossibleDate <= transferDate) return true;

            return false;
        }

        /// <summary>
        /// Gets the date after amount of days, only counting the business days. 
        /// </summary>
        /// <param name="days">The intervall of days to go through.</param>
        /// <param name="date">Today date.</param>
        /// <returns>Returns a DateOnly calculated from todays date with the limited days and only counting business days.</returns>
        public static DateOnly GetDateAfter(int days, DateOnly date)
        {
            for (int i = days; i > 0; i--)
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    i += 1;
                }
                date = date.AddDays(1);
            }

            //If the date gets to a weekday then continue til it reach Monday
            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }
            return date;
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
