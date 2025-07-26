using Core.RepositoryBase;
using DataAccessLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.UnitOfWork
{
    public class EfUnitOfWork<TEntity, TFilter> : UnitOfWorkGetRepository, IUnitOfWork<TEntity, TFilter> where TEntity : class where TFilter : FilterBase
    {
        private readonly DbContext _context;

        public EfUnitOfWork(DbContext context)
        {
            _context = context;
        }

        protected override IRepository<TEntity, TFilter> GetRepository<TEntity, TFilter>()
        {
            var entityType = typeof(TEntity);

            var repositoryType = typeof(EfRepository<,>).MakeGenericType(entityType, typeof(TFilter));
            var newRepository = Activator.CreateInstance(repositoryType, _context);
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

        public async Task<IEnumerable<TEntity?>> GetAllAsync()
        {
            return await GetReadAllRepository<TEntity>().GetAllAsync();
        }

        public async Task<TEntity?> GetByIdAsync(long id)
        {
            return await GetReadByIdRepository<TEntity>().GetByIdAsync(id);
        }

        public async Task<IEnumerable<TEntity?>> GetFilterAsync(TFilter filter)
        {
            return await GetFilterRepository<TEntity, TFilter>().GetByFilterAsync(filter);
        }

        public async Task AddAsync(TEntity entity) 
        {
            await GetAddRepository<TEntity>().AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity) 
        {
            await GetUpdateRepository<TEntity>().UpdateAsync(entity);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(long id) 
        {
            await GetDeleteRepository<TEntity>().DeleteAsync(id);
            await SaveChangesAsync();
        }

    }
}
