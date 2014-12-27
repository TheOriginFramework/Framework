using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.Utils
{
    public static class DateTimeRangeChecker
    {
        public static bool Check(DateTime CheckDateStart, DateTime CheckDateEnd)
        {
            return Check(DateTime.Now, CheckDateStart, CheckDateEnd);
        }

        public static bool Check(DateTime DateToCheck, DateTime CheckDateStart, DateTime CheckDateEnd)
        {
            return (CheckDateStart <= DateToCheck && CheckDateEnd >= DateToCheck);
        }
    }

    public static class DateTimeRangeCheckExtension
    {
        public static bool Check(this DateTime DateToCheck, DateTime CheckStart, DateTime CheckEnd)
        {
            return (CheckStart <= DateToCheck && CheckEnd >= DateToCheck);
        }

        public static bool CheckByDayLevel(this DateTime DateToCheck, DateTime CheckStart, DateTime CheckEnd)
        {
            DateTime dateStart = new DateTime(CheckStart.Year, CheckStart.Month, CheckStart.Day, 0, 0, 0);
            DateTime dateEnd = new DateTime(CheckEnd.Year, CheckEnd.Month, CheckEnd.Day, 23, 59, 59);

            return (dateStart <= DateToCheck && dateEnd >= DateToCheck);
        }
    }
}
