using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using HTGT.Data.Models;
using log4net;

namespace HTGT.Data
{
    public class DAArchanaInformation : IDisposable
    {
        private readonly IDbConnection _db;
        private IDbTransaction _transaction;
        private static readonly ILog log = LogManager.GetLogger(typeof(DAHTGTUsers));

        public DAArchanaInformation(bool runAsTransaction = false)
        {
            _db = new SqlConnection(DAHelpers.DBConnectionString);
            if (_db != null && _db.State == ConnectionState.Closed)
                _db.Open();
            if (runAsTransaction)
                InitializeTransaction();
        }

        public List<KidsInformationIndexViewModel> GetArchanaList()
        {
            try
            {
                return _db.Query<KidsInformationIndexViewModel>(SQLQueries.GET_ARCHANAS_LIST).ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public void CreateSubscription(string kidsName, string parentName, string emailAddress, int dayOfBirth, int monthOfBirth)
        {
            try
            {
                if (_transaction != null)
                {
                    _db.Execute(SQLQueries.CREATE_ARCHANA_SUBSCRIPTION, new { kidsName, parentName, emailAddress, dayOfBirth, monthOfBirth }, _transaction);
                }
                else
                {
                    _db.Execute(SQLQueries.CREATE_ARCHANA_SUBSCRIPTION, new { kidsName, parentName, emailAddress, dayOfBirth, monthOfBirth });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                RollbackTransaction();
                throw;
            }
        }

        public KidsInformationEditViewModel GetArchanaInfo(int id)
        {
            try
            {
                return _db.Query<KidsInformationEditViewModel>(SQLQueries.GET_ARCHANA_INFO, new { id }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public void UpdateSubscription(int sid, string kidsName, string parentName, string emailAddress, int dayOfBirth, int monthOfBirth, bool isActive)
        {
            try
            {
                if (_transaction != null)
                {
                    _db.Execute(SQLQueries.UPDATE_ARCHANA_SUBSCRIPTION, new { sid, KName = kidsName, ParentsName = parentName, Email = emailAddress, DayOfBirth = dayOfBirth, MonthOfBirth = monthOfBirth, IsActive = isActive }, _transaction);
                }
                else
                {
                    _db.Execute(SQLQueries.UPDATE_ARCHANA_SUBSCRIPTION, new { sid, KName = kidsName, ParentsName = parentName, Email = emailAddress, DayOfBirth = dayOfBirth, MonthOfBirth = monthOfBirth, IsActive = isActive });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                RollbackTransaction();
                throw;
            }
        }

        public List<KidsInformationEmailModel> GetArchanaEmailReminders()
        {
            try
            {
                return _db.Query<KidsInformationEmailModel>(SQLQueries.GET_ARCHANAS_REMINDERS).ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public void UpdateReminderDate(int sid, string toEmail)
        {
            try
            {
                if (_transaction != null)
                {
                    _db.Execute(SQLQueries.UPDATE_REMINDER_DATE, new { sid, toEmail }, _transaction);
                }
                else
                {
                    _db.Execute(SQLQueries.UPDATE_REMINDER_DATE, new { sid, toEmail });
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                RollbackTransaction();
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
