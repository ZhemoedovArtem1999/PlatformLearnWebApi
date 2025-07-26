using Core.RepositoryBase;
using DataAccessLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.UnitOfWork
{
    public abstract class UnitOfWorkGetRepository
    {
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

        protected abstract IRepository<TEntity, TFilter> GetRepository<TEntity, TFilter>()
            where TEntity : class
            where TFilter : FilterBase;
    }
}
