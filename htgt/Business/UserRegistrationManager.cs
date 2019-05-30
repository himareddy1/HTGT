using System;
using HTGT.Data;
using HTGT.Data.Models;

namespace htgt.Business
{
    public class UserRegistrationManager
    {
        public RegistrationStatus RegisterUser(HTGTUsersCreateModel model)
        {
            try
            {
                if(!model.Password.Equals(model.ConfirmPassword))
                    return RegistrationStatus.Failure;

                model.Password = UserSignInManager.HashPasswordString(model.Password);
                using (var da = new DAHTGTUsers(true))
                {
                    da.CreateNewUser(model);
                    return RegistrationStatus.Success;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               return RegistrationStatus.Failure;
            }
            
        }
    }

    public enum RegistrationStatus
    {
        Success = 1,
        InvalidEmailAddress = 2,
        Failure = 0
    }
}