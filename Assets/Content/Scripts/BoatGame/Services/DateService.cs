using System;
using System.Globalization;

namespace Content.Scripts.BoatGame.Services
{
    public static class DateService
    {
        public enum WebTimeType
        {
            None, 
            FromWeb,
            Local
        }
        private static WebTimeType webTimeState;
        private static bool isCanPingWebDate = false;
        private static DateTime date;
        
        
        public static DateTime ActualDate => date;
        public static string ActualDateString => date.ToString(CultureInfo.InvariantCulture);

        public static WebTimeType WebTimeState => webTimeState;
        
        
        public static void AddDateTime() => date = date.AddSeconds(1);
        public static void SetDateTime(DateTime dateTime, WebTimeType state)
        {
            date = dateTime;
            webTimeState = state;
        }
    }
}