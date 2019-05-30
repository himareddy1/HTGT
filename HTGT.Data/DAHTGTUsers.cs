using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using HTGT.Data.Models;
using log4net;

namespace HTGT.Data
{
    public class DAHTGTUsers : IDisposable
    {
        private readonly IDbConnection _db;
        private IDbTransaction _transaction;
        private static readonly ILog log = LogManager.GetLogger(typeof(DAHTGTUsers));

        public DAHTGTUsers(bool runAsTransaction = false)
        {
            _db = new SqlConnection(DAHelpers.DBConnectionString);
            if (_db != null && _db.State == ConnectionState.Closed)
                _db.Open();
            if (runAsTransaction)
                InitializeTransaction();
        }

        public HTGTUserValidationModel GetUserInfo(string emailAddress)
        {
            try
            {
                return _db.Query<HTGTUserValidationModel>(SQLQueries.GET_USERINFO_BY_EMAIL, new { email_address = emailAddress }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public void SetUserAccessStatus(string emailAddress, bool locked, DateTime? lockoutEndDate, int accessFailedCount)
        {
            try
            {
                if (_transaction == null)
                    _db.Execute(SQLQueries.UPDATE_ACCESS_FAILED_COUNT, new { count = accessFailedCount, locked, lockout_end_date = lockoutEndDate, email_address = emailAddress });
                else
                    _db.Execute(SQLQueries.UPDATE_ACCESS_FAILED_COUNT, new { count = accessFailedCount, locked, lockout_end_date = lockoutEndDate, email_address = emailAddress }, transaction: _transaction);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                RollbackTransaction();
                throw;
            }
        }

        public void SavePasswordToken(int userid, string resetcode, DateTime expirationDate)
        {
            try
            {
                if (_transaction == null)
                    _db.Execute(SQLQueries.UPDATE_PASSWORD_RESET_TOKEN, new { userid, token = resetcode, expiration_date = expirationDate });
                else
                    _db.Execute(SQLQueries.UPDATE_PASSWORD_RESET_TOKEN, new { userid, token = resetcode, expiration_date = expirationDate }, _transaction);
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public void UpdateUserPassword(int userid, string password)
        {
            try
            {
                if (_transaction == null)
                    _db.Execute(SQLQueries.UPDATE_PASSWORD, new {userid, password });
                else
                    _db.Execute(SQLQueries.UPDATE_PASSWORD, new {userid, password }, transaction: _transaction);
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public int CreateNewUser(HTGTUsersCreateModel user)
        {
            try
            {
                if (_transaction == null)
                    return _db.Query<int>(SQLQueries.CREATE_USER, new { emailAddress = user.EmailAddress, password = user.Password, firstName = user.FirstName, lastName = user.LastName }).Single();

                return _db.Query<int>(SQLQueries.CREATE_USER, new { emailAddress = user.EmailAddress, password = user.Password, firstName = user.FirstName, lastName = user.LastName }, transaction: _transaction).Single();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public void Dispose()
        {
            CommitTransaction();
            if (_db != null && _db.State == ConnectionState.Open)
                _db.Close();
        }

        private void InitializeTransaction()
        {
            _transaction = _db.BeginTransaction();
        }

        private void CommitTransaction()
        {
            _transaction?.Commit();
        }

        private void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction = null;
            }
        } 
    }
}
