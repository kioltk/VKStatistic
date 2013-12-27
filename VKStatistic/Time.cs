using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace application.Models
{
    public class Time
    {
        public static int UNIXFromDateTime(DateTime utc)
        {
            return (int)(utc - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }
        public static int UNIXNow()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }
        public static int Age(int date)
        {
            DateTime today = DateTime.Today;
            DateTime bday = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(date);
            int age = today.Year - bday.Year;
            if (bday > today.AddYears(-age)) age--;
            return age;
        }
        public static int HowManyDaysFromThe(int date)
        {
            var a= (UNIXNow() - date) / 86400;
            return a;
        }
        
        public static DateTime DatetimeFormUNIX(int date)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(date);
        }
    }
}