using Core.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace DataAccessLayer.Repository
{
    public interface IRepository<TEntity, TFilter> : IReadAllRepository<TEntity>,
      IReadByIdRepository<TEntity>,
      IReadFilterRepository<TEntity, TFilter>,
      IAddRepository<TEntity>,
      IUpdateRepository<TEntity>,
      IDeleteRepository<TEntity>
      where TEntity : class
      where TFilter : FilterBase
    {
    }
}
