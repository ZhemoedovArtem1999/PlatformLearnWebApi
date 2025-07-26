using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.RepositoryBase
{
    public interface IReadByIdRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(long id);
    }
}
