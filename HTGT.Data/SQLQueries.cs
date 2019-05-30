namespace HTGT.Data
{
    internal static class SQLQueries
    {
        internal const string GET_USERINFO_BY_EMAIL = "SELECT * FROM dbo.HTGTUsers AS HU WHERE UPPER(HU.EmailAddress) = UPPER(@email_address)";
        internal const string UPDATE_ACCESS_FAILED_COUNT = "UPDATE dbo.HTGTUsers SET AccessFailedCount = @count, LockoutEnabled = @locked, LockoutEndDateUtc = @lockout_end_date WHERE UPPER(EmailAddress) = UPPER(@email_address)";
        internal const string UPDATE_PASSWORD_RESET_TOKEN = "UPDATE dbo.HTGTUsers SET PasswordResetToken = @token, TokenExpirationDateUtc = @expiration_date WHERE HTGTUserID = @userid";
        internal const string UPDATE_PASSWORD = "UPDATE dbo.HTGTUsers SET Password = @password, LockoutEnabled = 0, LockoutEndDateUtc = NULL, PasswordResetToken = NULL, TokenExpirationDateUtc = NULL, AccessFailedCount = 0 WHERE HTGTUserID = @userid";
        internal const string CREATE_USER = "INSERT INTO dbo.HTGTUsers (EmailAddress, Password, FirstName, LastName, IsEnabled, CreatedDate) VALUES (@emailAddress, @password, @firstName, @lastName, 1, GETDATE());SELECT SCOPE_IDENTITY();";

        internal const string GET_ARCHANAS_LIST = "SELECT * FROM dbo.ArchanaInformation ORDER BY KName";
        internal const string CREATE_ARCHANA_SUBSCRIPTION = "INSERT INTO dbo.ArchanaInformation (KName, ParentsName, DayofBirth, MonthofBirth, Email, IsActive) VALUES  (@kidsName, @parentName, @dayOfBirth, @monthOfBirth, @emailAddress, 1)";
        internal const string GET_ARCHANA_INFO = "SELECT SID, KName, ParentsName, Email AS EmailAddress, IsActive, CONVERT(VARCHAR(2), MonthofBirth) + \'/\' + CONVERT(VARCHAR(2), DayofBirth) + \'/\' + CONVERT(VARCHAR(4), DATEPART(YEAR, GETDATE())) AS DateOfBirth FROM dbo.ArchanaInformation WHERE [SID] = @id";
        internal const string UPDATE_ARCHANA_SUBSCRIPTION = "UPDATE dbo.ArchanaInformation SET KName = @KName, ParentsName = @ParentsName, DayofBirth = @DayofBirth, MonthofBirth = @MonthofBirth, Email = @Email, IsActive = @IsActive WHERE [SID] = @sid";
        internal const string UPDATE_REMINDER_DATE = "UPDATE ArchanaInformation SET ReminderSentOn = GETDATE() WHERE SID = @sid;INSERT INTO dbo.ReminderEmails (SID, ToEmail, SentOn) VALUES  (@sid, @toEmail, GETDATE());";

        internal const string GET_ARCHANAS_REMINDERS = "SELECT AI.SID, AI.KName, AI.ParentsName, AI.DayofBirth, AI.MonthofBirth, AI.Email FROM dbo.ArchanaInformation AS AI " +
                                                       "WHERE DATEPART(MONTH, GETDATE()) = AI.MonthofBirth AND (DATEPART(DAY, GETDATE()) + 7 = AI.DayofBirth OR (DATEPART(DAY, GETDATE()) = AI.DayofBirth)) " +
                                                       "AND AI.IsActive = 1";
    }
}
