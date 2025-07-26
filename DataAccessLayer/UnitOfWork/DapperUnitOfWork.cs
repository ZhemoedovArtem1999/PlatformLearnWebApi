using Core.RepositoryBase;
using DataAccessLayer.Repository;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.UnitOfWork
{
    public class DapperUnitOfWork : IUnitOfWork
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

        public IReadAllRepository<TEntity> GetReadAllRepository<TEntity>() where TEntity : class
        {
            return GetRepository<TEntity, FilterBase>();
        }

        public IReadByIdRepository<TEntity> GetReadByIdRepository<TEntity>() where TEntity : class
        {
            return GetRepository<TEntity, FilterBase>();
        }

        public IReadFilterRepository<TEntity, TFilter> GetFilterRepository<TEntity, TFilter>()
            where TEntity : class
            where TFilter : FilterBase
        {
            return GetRepository<TEntity, TFilter>();
        }

        public IAddRepository<TEntity> GetAddRepository<TEntity>() where TEntity : class
        {
            return GetRepository<TEntity, FilterBase>();
        }

        public IUpdateRepository<TEntity> GetUpdateRepository<TEntity>() where TEntity : class
        {
            return GetRepository<TEntity, FilterBase>();
        }

        public IDeleteRepository<TEntity> GetDeleteRepository<TEntity>() where TEntity : class
        {
            return GetRepository<TEntity, FilterBase>();
        }

        private DapperRepository<TEntity, TFilter> GetRepository<TEntity, TFilter>()
            where TEntity : class
            where TFilter : FilterBase
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

        public Task<IEnumerable<TEntity?>> GetAllAsync<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task<TEntity?> GetByIdAsync<TEntity>(long id) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TEntity?>> GetFilterAsync<TEntity, TFilter>(TFilter filter)
            where TEntity : class
            where TFilter : FilterBase
        {
            throw new NotImplementedException();
        }

        public Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<TEntity>(long id) where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}
