using Core.RepositoryBase;
using DataAccessLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace DataAccessLayer.UnitOfWork
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public EfUnitOfWork(DbContext context)
        {
            _context = context;
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

        private EfRepository<TEntity, TFilter> GetRepository<TEntity, TFilter>()
            where TEntity : class
            where TFilter : FilterBase
        {
            var entityType = typeof(TEntity);
            if (_repositories.TryGetValue(entityType, out var repository))
            {
                return (EfRepository<TEntity, TFilter>)repository;
            }

            var repositoryType = typeof(EfRepository<,>).MakeGenericType(entityType, typeof(TFilter));
            var newRepository = Activator.CreateInstance(repositoryType, _context);
            _repositories.Add(entityType, newRepository);
            return (EfRepository<TEntity, TFilter>)newRepository;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Rollback()
        {
            _context.ChangeTracker.Entries()
                .ToList()
                .ForEach(entry => entry.State = EntityState.Unchanged);
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<IEnumerable<TEntity?>> GetAllAsync<TEntity>() where TEntity : class
        {
            return await GetReadAllRepository<TEntity>().GetAllAsync();
        }

        public async Task<TEntity?> GetByIdAsync<TEntity>(long id) where TEntity : class
        {
            return await GetReadByIdRepository<TEntity>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<TEntity?>> GetFilterAsync<TEntity, TFilter>(TFilter filter)
            where TEntity : class
            where TFilter : FilterBase
        {
            return await GetFilterRepository<TEntity, TFilter>().GetByFilterAsync(filter);
        }

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await GetAddRepository<TEntity>().AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await GetUpdateRepository<TEntity>().UpdateAsync(entity);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync<TEntity>(long id) where TEntity : class
        {
            await GetDeleteRepository<TEntity>().DeleteAsync(id);
            await SaveChangesAsync();
        }
    }
}
