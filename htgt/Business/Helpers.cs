using System;
using System.Linq;
using Newtonsoft.Json;

namespace htgt.Business
{
    public static class Helpers
    {

        internal static string FormatLogMessage(string tableName, string username, string entityName, string message, params string[] addtionaldata)
        {
            var logEntry = new InfoMessagesLog
            {
                Username = username,
                Message = message,
                AdditionData = string.Join(" ", addtionaldata),
                TableName = tableName,
                EntityName = entityName,
                LogDate = GetTodayDateTime()
            };

            var logMessage = JsonConvert.SerializeObject(logEntry);
            return logMessage;
        }

        private static DateTime GetTodayDateTime()
        {
            var offset = GetDateOffset();
            return DateTime.UtcNow.AddMilliseconds(offset);
        }

        private static double GetDateOffset()
        {
            var currentTimezone = TimeZone.CurrentTimeZone.StandardName;
            const string timezone = "Central Standard Time";

            if (currentTimezone.Equals(timezone, StringComparison.OrdinalIgnoreCase))
                return 0;

            var ts = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault(x => x.StandardName.Equals(timezone, StringComparison.OrdinalIgnoreCase));
            return ts?.BaseUtcOffset.TotalMilliseconds ?? 0;
        }
    }

    public class InfoMessagesLog
    {
        public string TableName { get; set; }
        public string Username { get; set; }
        public string EntityName { get; set; }
        public string Message { get; set; }
        public string AdditionData { get; set; }
        public DateTime LogDate { get; set; }
    }

    public static class TableNameConstants
    {
        public const string UsersTableName = "HTGTUsers";
        
    }

    public static class InfoMessages
    {
        public const string UserLoginSuccess = "User logged in successfully.";
        public const string UserLockedOut = "User account is locked out.";
        public const string UserLoginFailure = "User login failure.";
        public const string UserDeleteSucess = "User deleted succesfully.";
        public const string UserCreateSuccess = "User created succesfully.";
        public const string UserExists = "User exists. Please try again.";
        public const string UserCreateFailure = "Error occurred, please try again.";
    }
}