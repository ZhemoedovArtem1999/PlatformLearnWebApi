using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.RepositoryBase
{
    public interface IReadFilterRepository<TEntity, TFilter> where TEntity : class where TFilter : FilterBase
    {
        Task<IEnumerable<TEntity>> GetByFilterAsync(TFilter filter);

    }
}
