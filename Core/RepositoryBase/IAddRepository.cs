using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.RepositoryBase
{
    public interface IAddRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity);
    }
}
