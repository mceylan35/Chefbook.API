using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chefbook.API.Services.Interface;
using Chefbook.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Chefbook.API.Repository
{
    public interface IGenericRepository<T>: ITransactionAble where T : class, IEntity,new()
    {
        List<T> GetAll(Expression<Func<T, bool>> filter = null);
        T Get(Expression<Func<T, bool>> filter = null);
        T GetById(Guid id);
        void Add(T entity);
        void Update(T entity);
     
      
        IEnumerable<T> GetInclude(Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null);
        IEnumerable<T> GetInclude(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null, bool withTracking = true);
       
        void Delete(T entity);
       
    }
}