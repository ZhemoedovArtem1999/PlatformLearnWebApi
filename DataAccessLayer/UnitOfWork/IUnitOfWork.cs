using Core.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IReadAllRepository<TEntity> GetReadAllRepository<TEntity>() where TEntity : class;
        IReadByIdRepository<TEntity> GetReadByIdRepository<TEntity>() where TEntity : class;

        IReadFilterRepository<TEntity, TFilter> GetFilterRepository<TEntity, TFilter>()
            where TEntity : class
            where TFilter : FilterBase;
        IAddRepository<TEntity> GetAddRepository<TEntity>() where TEntity : class;
        IUpdateRepository<TEntity> GetUpdateRepository<TEntity>() where TEntity : class;
        IDeleteRepository<TEntity> GetDeleteRepository<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void Rollback();

        Task<IEnumerable<TEntity?>> GetAllAsync<TEntity>() where TEntity : class;
        Task<TEntity?> GetByIdAsync<TEntity>(long id) where TEntity : class;
        Task<IEnumerable<TEntity?>> GetFilterAsync<TEntity, TFilter>(TFilter filter) where TEntity : class where TFilter : FilterBase;

        Task AddAsync<TEntity>(TEntity entity) where TEntity : class;
        Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
        Task DeleteAsync<TEntity>(long id) where TEntity : class;
    }
}
