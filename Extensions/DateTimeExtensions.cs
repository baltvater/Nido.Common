using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nido.Common.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Lets you easily figure out ifdateTime holds a date value that is a weekend.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsWeekend(this DateTime value)
        {
            return (value.DayOfWeek == DayOfWeek.Sunday || value.DayOfWeek == DayOfWeek.Saturday);
        }

        /// <summary>
        /// Get the actual age of a person
        /// </summary>
        /// <param name="dateOfBirth"></param>
        /// <example>
        /// DateTime henrybirthdate = new DateTime(1998,10,12);
        /// int age = henrybirthdate.Age(); 
        /// </example>
        /// <returns></returns>
        public static int Age(this DateTime dateOfBirth)
        {
            if (DateTime.Today.Month < dateOfBirth.Month ||
            DateTime.Today.Month == dateOfBirth.Month &&
             DateTime.Today.Day < dateOfBirth.Day)
            {
                return DateTime.Today.Year - dateOfBirth.Year - 1;
            }
            else
                return DateTime.Today.Year - dateOfBirth.Year;
        }

        /// <summary>
        /// Checks if the date is between the two provided dates
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="compareTime"></param>
        /// <returns></returns>
        /// <example>
        /// var today = DateTime.Now;
        /// var start = new DateTime(2012, 1, 1);
        /// var end = new DateTime(2013, 11, 25);
        /// 
        /// Boolean isBetween = today.IsBetween(start, end);
        /// </example>
        public static Boolean IsBetween(this DateTime dt, DateTime startDate, DateTime endDate, Boolean compareTime = false)
        {
            return compareTime ?
               dt >= startDate && dt <= endDate :
               dt.Date >= startDate.Date && dt.Date <= endDate.Date;
        }

        /// <summary>
        /// Convert UTC time received from the database, 
        /// back to the given local time zone value
        /// </summary>
        /// <param name="UtcTime">UTC time</param>
        /// <param name="localTimeZone">The needed local time zone</param>
        /// <returns></returns>
        public static DateTime TimeToDisplayForUser(this DateTime UtcTime, TimeZoneInfo localTimeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(UtcTime, localTimeZone);
        }

        /// <summary>
        /// Convert local time back to UTC time format
        /// </summary>
        /// <param name="LocalTime">Local Time</param>
        /// <param name="localTimeZone">The needed original local time zone</param>
        /// <returns></returns>
        public static DateTime TimeToStoreInDatabase(this DateTime LocalTime, TimeZoneInfo localTimeZone)
        { 
            LocalTime = DateTime.SpecifyKind(LocalTime, DateTimeKind.Unspecified);
            return TimeZoneInfo.ConvertTimeToUtc(LocalTime, localTimeZone);
        }
    }
}
