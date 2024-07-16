using System;

namespace Odk.BluePrism.Skat.Utils
{
    public static class Dates
    {
        public static string GetBasisMonthFromDate(DateTime date)
        {
            return date.Year.ToString("0000") + date.Month.ToString("00");
        }
    }
}
