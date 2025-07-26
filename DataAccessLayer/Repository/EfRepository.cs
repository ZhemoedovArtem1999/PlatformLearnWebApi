using Core.RepositoryBase;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace DataAccessLayer.Repository
{
    public class EfRepository<TEntity, TFilter> :
      IReadAllRepository<TEntity>,
      IReadByIdRepository<TEntity>,
      IReadFilterRepository<TEntity, TFilter>,
      IAddRepository<TEntity>,
      IUpdateRepository<TEntity>,
      IDeleteRepository<TEntity>
      where TEntity : class
      where TFilter : FilterBase

    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public EfRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }
        
        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<TEntity?> GetByIdAsync(long id)
            => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<TEntity>> GetByFilterAsync(TFilter filter)
        {
            var query = _dbSet.AsQueryable();

            // Применяем условия фильтрации через рефлексию
            query = ApplyFilterConditions(query, filter);

            // Сортировка
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var property = Expression.Property(parameter, filter.SortBy);
                var lambda = Expression.Lambda<Func<TEntity, object>>(Expression.Convert(property, typeof(object)), parameter);

                query = filter.SortDescending
                    ? query.OrderByDescending(lambda)
                    : query.OrderBy(lambda);
            }

            // Пагинация
            if (filter.Skip.HasValue) query = query.Skip(filter.Skip.Value);
            if (filter.Take.HasValue) query = query.Take(filter.Take.Value);

            return await query.ToListAsync();
        }

        private IQueryable<TEntity> ApplyFilterConditions(IQueryable<TEntity> query, TFilter filter)
        {
            var filterType = filter.GetType();
            var properties = filterType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var prop in properties)
            {
                // Пропускаем свойства базового класса
                if (prop.DeclaringType == typeof(FilterBase)) continue;

                var value = prop.GetValue(filter);
                if (value == null) continue;

                // Создаем условие фильтрации
                var parameter = Expression.Parameter(typeof(TEntity), "x");
                var property = Expression.Property(parameter, prop.Name);
                var constant = Expression.Constant(value);
                var equals = Expression.Equal(property, constant);
                var lambda = Expression.Lambda<Func<TEntity, bool>>(equals, parameter);

                query = query.Where(lambda);
            }

            return query;
        }

        public async Task AddAsync(TEntity entity)
            => await _dbSet.AddAsync(entity);

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
                _dbSet.Remove(entity);
        }
    }

}
