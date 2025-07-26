using Core.RepositoryBase;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Text;

namespace DataAccessLayer.Repository
{

    public class DapperRepository<TEntity, TFilter> :
    IReadAllRepository<TEntity>,
    IReadByIdRepository<TEntity>,
    IReadFilterRepository<TEntity, TFilter>,
    IAddRepository<TEntity>,
    IUpdateRepository<TEntity>,
    IDeleteRepository<TEntity>
    where TEntity : class
    where TFilter : FilterBase
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly string _tableName;

        public DapperRepository(IDbConnection connection, IDbTransaction transaction, string tableName)
        {
            _connection = connection;
            _transaction = transaction;
            _tableName = tableName;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _connection.QueryAsync<TEntity>(
                $"SELECT * FROM {_tableName}",
                transaction: _transaction);
        }

        public async Task<TEntity?> GetByIdAsync(long id)
        {
            return await _connection.QueryFirstOrDefaultAsync<TEntity>(
                $"SELECT * FROM {_tableName} WHERE Id = @Id",
                new { Id = id },
                _transaction);
        }

        public async Task<IEnumerable<TEntity>> GetByFilterAsync(TFilter filter)
        {
            var query = BuildFilterQuery(filter);
            return await _connection.QueryAsync<TEntity>(
                query.Sql,
                query.Parameters,
                _transaction);
        }

        private (string Sql, DynamicParameters Parameters) BuildFilterQuery(TFilter filter)
        {
            var sql = new StringBuilder($"SELECT * FROM {_tableName}");
            var parameters = new DynamicParameters();
            var whereClauses = new List<string>();

            // Рефлексия для обработки специфичных фильтров
            var filterProperties = typeof(TFilter).GetProperties()
                .Where(p => p.DeclaringType != typeof(FilterBase));

            foreach (var prop in filterProperties)
            {
                var value = prop.GetValue(filter);
                if (value == null) continue;

                parameters.Add(prop.Name, value);

                if (prop.PropertyType == typeof(string))
                {
                    whereClauses.Add($"{prop.Name} LIKE @{prop.Name}");
                }
                else
                {
                    whereClauses.Add($"{prop.Name} = @{prop.Name}");
                }
            }

            if (whereClauses.Any())
            {
                sql.Append(" WHERE ").AppendJoin(" AND ", whereClauses);
            }

            // Сортировка
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                sql.Append($" ORDER BY {filter.SortBy} {(filter.SortDescending ? "DESC" : "ASC")}");
            }

            // Пагинация
            if (filter.Skip.HasValue || filter.Take.HasValue)
            {
                sql.Append($" OFFSET {filter.Skip ?? 0} ROWS FETCH NEXT {filter.Take ?? 100} ROWS ONLY");
            }

            return (sql.ToString(), parameters);
        }

        public async Task AddAsync(TEntity entity)
        {
            var properties = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

            await _connection.ExecuteAsync(
                $"INSERT INTO {_tableName} ({columns}) VALUES ({values})",
                entity,
                _transaction);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            var properties = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));

            await _connection.ExecuteAsync(
                $"UPDATE {_tableName} SET {setClause} WHERE Id = @Id",
                entity,
                _transaction);
        }

        public async Task DeleteAsync(long id)
        {
            await _connection.ExecuteAsync(
                $"DELETE FROM {_tableName} WHERE Id = @Id",
                new { Id = id },
                _transaction);
        }
    }
}
