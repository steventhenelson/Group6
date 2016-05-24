using System;
using ChocAn.EfData;
using ChocAn.Models;

namespace ChocAn.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private bool _disposed;
        private readonly ChocAnDb _chocanDb = new ChocAnDb();
        private GenericRepository<Service> _serviceRepository;
        private GenericRepository<UserProfile> _userProfileRepository;
        private GenericRepository<TreatmentRecord> _treatmentRecordRepository;

        public GenericRepository<Service> ServiceRepository => _serviceRepository ?? (_serviceRepository = new GenericRepository<Service>(_chocanDb));
        public GenericRepository<UserProfile> UserProfileRepository => _userProfileRepository ?? (_userProfileRepository = new GenericRepository<UserProfile>(_chocanDb));
        public GenericRepository<TreatmentRecord> TreatmentRecordRepository => _treatmentRecordRepository ?? (_treatmentRecordRepository = new GenericRepository<TreatmentRecord>(_chocanDb));

        public bool SaveChanges()
        {
            try
            {
                _chocanDb.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            _chocanDb.Dispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposal)
        {
            if (!_disposed && disposal)
            {
                _chocanDb.Dispose();
            }
            _disposed = true;
        }
    }
}