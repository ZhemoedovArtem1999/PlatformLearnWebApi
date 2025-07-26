using Core.RepositoryBase;
using DataAccessLayer.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Data;

namespace DataAccessLayer.UnitOfWork
{
    public class DapperUnitOfWork<TEntity, TFilter> : UnitOfWorkGetRepository, IUnitOfWork<TEntity, TFilter> where TEntity : class where TFilter : FilterBase
    {
        private readonly string _connectionString;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories = new();

        public DapperUnitOfWork(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        protected override IRepository<TEntity, TFilter> GetRepository<TEntity, TFilter>()
        {
            var entityType = typeof(TEntity);
            if (_repositories.TryGetValue(entityType, out var repository))
            {
                return (DapperRepository<TEntity, TFilter>)repository;
            }

            var tableName = GetTableName<TEntity>();
            var newRepository = new DapperRepository<TEntity, TFilter>(_connection, _transaction, tableName);
            _repositories.Add(entityType, newRepository);
            return newRepository;
        }

        private string GetTableName<TEntity>()
        {
            // Можно добавить кастомные атрибуты для имен таблиц
            return typeof(TEntity).Name + "s"; // Простое преобразование (Product → Products)
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _transaction.Commit();
                return 1;
            }
            catch
            {
                Rollback();
                throw;
            }
            finally
            {
                _transaction = _connection.BeginTransaction();
                ResetRepositories();
            }
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = _connection.BeginTransaction();
            ResetRepositories();
        }

        private void ResetRepositories()
        {
            _repositories.Clear();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }

        public Task<IEnumerable<TEntity?>> GetAllAsync() 
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> GetByIdAsync(long id) 
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity?>> GetFilterAsync(TFilter filter)
        {
            throw new NotImplementedException();
        }

        public Task AddAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TEntity entity) 
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(long id) 
        {
            throw new NotImplementedException();
        }
    }
}
