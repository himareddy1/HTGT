using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using htgt.Models;
using HTGT.Data;
using HTGT.Data.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace htgt.Business
{
    public class UserSignInManager
    {
        internal SignInResult ValidateUserCredentials(string emailAddress, string password)
        {
            var result = new SignInResult();
            if (string.IsNullOrWhiteSpace(emailAddress) || string.IsNullOrWhiteSpace(password))
            {
                result.Status = SignInStatus.Failure;
                return result;
            }

            var user = GetUserInfo(emailAddress);
            if (user == null)
            {
                result.Status = SignInStatus.Failure;
            }
            else
            {
                if (!user.IsEnabled)
                {
                    result.Status = SignInStatus.Failure;
                }
                else if (user.LockoutEnabled && user.LockoutEndDateUtc.CompareTo(DateTime.UtcNow) <= 0)
                {
                    result.Status = SignInStatus.LockedOut;
                }
                else
                {
                    PasswordVerificationResult pwdResult = ValidatePassword(user.Password, password);
                    if (pwdResult == PasswordVerificationResult.Success)
                    {
                        result.Status = SignInStatus.Success;
                        result.UserInfo = GetUserViewModel(user);
                        SetUserAccessSuccess(user);
                    }
                    else
                    {
                        result.Status = SetUserAccessFailed(user);
                    }
                }

            }
            return result;
        }

        private HTGTUsersViewModel GetUserViewModel(HTGTUserValidationModel user)
        {
            var viewModel = new HTGTUsersViewModel
            {
                EmailAddress = user.EmailAddress,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AccessFailedCount = user.AccessFailedCount,
                HTGTUserID = user.HTGTUserID,
                IsEnabled = user.IsEnabled
            };

            return viewModel;
        }

        private static void SetUserAccessSuccess(HTGTUserValidationModel user)
        {
            using (var da = new DAHTGTUsers(true))
            {
                da.SetUserAccessStatus(user.EmailAddress, false, null, 0);
            }
        }

        private static SignInStatus SetUserAccessFailed(HTGTUserValidationModel user)
        {
            const int maxLoginFailures = 5;
            int accessFailedCount = user.AccessFailedCount ?? 0;
            bool locked = false;
            DateTime? lockoutEndDate = null;
            if (accessFailedCount >= maxLoginFailures)
            {
                locked = true;
                lockoutEndDate = DateTime.UtcNow;
            }
            accessFailedCount++;

            using (var da = new DAHTGTUsers(true))
            {
                da.SetUserAccessStatus(user.EmailAddress, locked, lockoutEndDate, accessFailedCount);
            }
            return locked ? SignInStatus.LockedOut : SignInStatus.Failure;
        }

        private static PasswordVerificationResult ValidatePassword(string hashedPassword, string password)
        {
            var hasher = new PasswordHasher();
            return hasher.VerifyHashedPassword(hashedPassword, password);
        }

        private static HTGTUserValidationModel GetUserInfo(string emailAddress)
        {
            using (var da = new DAHTGTUsers())
            {
                return da.GetUserInfo(emailAddress);
            }
        }

        public static string FormatName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                firstName = string.Empty;

            if (string.IsNullOrWhiteSpace(lastName))
                lastName = string.Empty;

            return $"{firstName.Trim()} {lastName.Trim()}";
        }

        internal PasswordResetTokenResult GeneratePasswordResetToken(string email)
        {
            PasswordResetTokenResult result = new PasswordResetTokenResult();
            var userinfo = GetUserInfo(email);
            if (userinfo == null)
            {
                result.Status = PasswordResetStatus.InvalidEmailAddress;
            }
            else
            {
                result.ResetToken = GetPasswordResetToken();
                result.EmailAddress = userinfo.EmailAddress;

                if (result.ResetToken != null)
                {
                    result.Status = PasswordResetStatus.Success;
                    var hashedToken = HashPasswordString(result.ResetToken);
                    SavePasswordToken(userinfo.HTGTUserID, hashedToken);
                }
                else
                {
                    result.Status = PasswordResetStatus.Failure;
                }
            }
            return result;
        }

        private string GetPasswordResetToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[16];
                rng.GetBytes(bytes);

                return Convert.ToBase64String(bytes);
            }
        }

        internal static string HashPasswordString(string passcode)
        {
            var hasher = new PasswordHasher();
            return hasher.HashPassword(passcode);
        }

        private void SavePasswordToken(int userid, string resetcode)
        {
            using (var da = new DAHTGTUsers(true))
            {
                DateTime expirationDate = DateTime.UtcNow.AddHours(1);
                da.SavePasswordToken(userid, resetcode, expirationDate);
            }
        }

        internal PasswordResetResult ResetUserPassword(ResetPasswordViewModel model)
        {
            PasswordResetResult result = new PasswordResetResult { Errors = new List<string>() };
            var userInfo = GetUserInfo(model.Email);
            if (userInfo == null)
            {
                result.Status = PasswordResetStatus.InvalidEmailAddress;
                result.Errors.Add("Invalid email address provided!!!");
            }
            else
            {
                if (ValidateToken(userInfo, model.Code))
                {
                    if (model.Password == model.ConfirmPassword)
                    {
                        var validationResult = ValidatePasswordStrength(model.Password);
                        if (validationResult.Succeeded)
                        {
                            result.Status = PasswordResetStatus.Success;
                            var hashedPassword = HashPasswordString(model.Password);
                            UpdateUserPassword(userInfo.HTGTUserID, hashedPassword);
                        }
                        else
                        {
                            result.Status = PasswordResetStatus.Failure;
                            result.Errors = validationResult.Errors.ToList();
                        }
                    }
                    else
                    {
                        result.Status = PasswordResetStatus.Failure;
                    }
                }
                else
                {
                    result.Status = PasswordResetStatus.Failure;
                    result.Errors.Add("Invalid token. Please use forgot password page to get a new password reset link.");
                }
            }
            return result;
        }

        private void UpdateUserPassword(int userid, string password)
        {
            using (var da = new DAHTGTUsers(true))
            {
                da.UpdateUserPassword(userid, password);
            }
        }

        private IdentityResult ValidatePasswordStrength(string password)
        {
            var passwordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            return passwordValidator.ValidateAsync(password).Result;
        }

        private bool ValidateToken(HTGTUserValidationModel userInfo, string token)
        {
            bool result = false;
            if (!string.IsNullOrWhiteSpace(userInfo.PasswordResetToken) && userInfo.TokenExpirationDateUtc.HasValue)
            {
                var validationResult = ValidatePassword(userInfo.PasswordResetToken, token);
                if (validationResult == PasswordVerificationResult.Success)
                {
                    if (userInfo.TokenExpirationDateUtc.Value.CompareTo(DateTime.UtcNow) >= 0)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
    }

    public class SignInResult
    {
        public SignInStatus Status { get; set; }

        public HTGTUsersViewModel UserInfo { get; set; }
    }

    public class PasswordResetResult
    {
        public PasswordResetStatus Status { get; set; }

        public List<string> Errors { get; set; }
    }

    public class PasswordResetTokenResult
    {
        public string ResetToken { get; set; }

        public PasswordResetStatus Status { get; set; }

        public string EmailAddress { get; set; }
    }

    public enum PasswordResetStatus
    {
        Success = 1,
        InvalidEmailAddress = 2,
        Failure = 0
    }
}