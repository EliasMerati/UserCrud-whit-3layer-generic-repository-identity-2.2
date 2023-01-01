using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserCrud.BLL.DTO;
using UserCrud.DAL.Context;

namespace UserCrud.BLL
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        private readonly UserCrudDatabaseContext _db;
        private readonly DbSet<TEntity> _dbset;

        public GenericRepository(UserCrudDatabaseContext db)
        {
            _db = db;
            _dbset = db.Set<TEntity>();
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity,bool>> where = null , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderby = null , string Includes = "" )
        {
            IQueryable<TEntity> query = _dbset;
            if (where != null)
            {
                query = query.Where(where);
            }

            if (orderby != null)
            {
                query = orderby(query);
            }

            if (Includes != "")
            {
                foreach (string include in Includes.Split(","))
                {
                    query = query.Include(include);
                }
            }

            return query.ToList();
        }

        public virtual TEntity GetById(object id)
        {
            return _dbset.Find(id);
        }

        public virtual void Add(TEntity entity)
        {
            _dbset.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            _dbset.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
        }
        public virtual void Delete(TEntity entity)
        {
            if (_db.Entry(entity).State == EntityState.Detached)
            {
                _dbset.Attach(entity);
            }
            _dbset.Remove(entity);
        }

        public virtual void Delete(object id)
        {
            var entity = GetById(id);
            Delete(entity);

        }

        public virtual void Save()
        {
            _db.SaveChanges();
        }


    }
}
