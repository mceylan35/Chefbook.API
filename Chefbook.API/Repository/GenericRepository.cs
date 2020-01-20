using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Chefbook.API.Context;
using Chefbook.API.Services.Interface;
using Chefbook.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Chefbook.API.Repository
{
    public abstract class GenericRepository<T, ChefContext> : IGenericRepository<T> where T : class, IEntity,new() where ChefContext:DbContext,new()
    {
        //DB bağlantısı sonrası context değişecek
        //private readonly ChefContext _dbContext;
        //private readonly DbSet<T> _dbSet;

        //protected GenericRepository(ChefContext dbContext)
        //{
        //    this._dbContext = dbContext;
        //    this._dbSet = _dbContext.Set<T>();
        //}
       
        public void Add(T entity)
        {
           
            using (var context = new ChefContext())
            {
                var addedEntry = context.Entry(entity);
                addedEntry.State = EntityState.Added;
                context.SaveChanges();
                
            }
 
            //return entity;
        }

        public void BeginTransaction()
        {
            using (var context=new ChefContext())
            {
                context.Database.BeginTransaction();
            }
        }

        public void CommitTransaction()
        {
            using (var context = new ChefContext())
            {
                context.Database.CommitTransaction();
            }
        }

        public void Delete(T entity)
        {
            using (var context = new ChefContext())
            {
                var deletedEntity = context.Entry(entity);
                deletedEntity.State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public void DisposeTransaction()
        {
            using (var context = new ChefContext())
            {
                context.Database.CurrentTransaction.Dispose();
            }
        }

        public T Get(Expression<Func<T, bool>> filter = null)
        {
            using (var contect = new ChefContext())
            {
                return contect.Set<T>().SingleOrDefault();
            }
        }

        public List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            using (var context = new ChefContext())
            {
                return filter == null
                    ? context.Set<T>().ToList()
                    : context.Set<T>().Where(filter).ToList();
            }
        }

        public T GetById(Guid id)
        {
            using (var context = new ChefContext())
            {
                return id != null
                    ? context.Set<T>().Find(id)
                    : null;
            }
        }

        public void RollbackTransaction()
        {
            using (var context = new ChefContext())
            {
        
                 context.Database.RollbackTransaction();
            }
        }


        public void Update(T entity)
        {
            using (var context = new ChefContext())
            {
                var updateEntity = context.Entry(entity);
                updateEntity.State = EntityState.Modified;
                context.SaveChanges();
            }
        }


        public IEnumerable<T> GetInclude(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null, bool withTracking = true)
        {
            using (var context = new ChefContext()) {
             IQueryable<T> query = context.Set<T>();

            if (includes != null)
            {
                query = includes(query);
            }

            query = query.Where(predicate);

            if (withTracking == false)
            {
                query = query.Where(predicate).AsNoTracking();
            }

            return query.AsEnumerable();

            }
        }
       

        public IEnumerable<T> GetInclude(Func<IQueryable<T>, IIncludableQueryable<T, object>> includes = null)
        {
            using (var context=new ChefContext())
            {
                IQueryable<T> query = context.Set<T>();

                if (includes != null)
                {
                    query = includes(query);
                }

                return query.AsEnumerable();
            }
          
        }

    }
}