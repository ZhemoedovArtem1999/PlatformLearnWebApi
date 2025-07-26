using Core.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.UnitOfWork
{
    public interface IUnitOfWork<TEntity, TFilter> : IDisposable where TEntity : class where TFilter : FilterBase
    {
        //IReadAllRepository<TEntity> GetReadAllRepository<TEntity>() where TEntity : class;
        //IReadByIdRepository<TEntity> GetReadByIdRepository<TEntity>() where TEntity : class;

        //IReadFilterRepository<TEntity, TFilter> GetFilterRepository<TEntity, TFilter>()
        //    where TEntity : class
        //    where TFilter : FilterBase;
        //IAddRepository<TEntity> GetAddRepository<TEntity>() where TEntity : class;
        //IUpdateRepository<TEntity> GetUpdateRepository<TEntity>() where TEntity : class;
        //IDeleteRepository<TEntity> GetDeleteRepository<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        void Rollback();

        Task<IEnumerable<TEntity?>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(long id);
        Task<IEnumerable<TEntity?>> GetFilterAsync(TFilter filter);

        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(long id);
    }
}
