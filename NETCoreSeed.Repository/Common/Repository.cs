using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.Repository.Common
{
    public class Repository<TEntity> : IRepository<TEntity>, IDisposable
            where TEntity : class
    {
        protected AnyGymDataContext dbContext;
        protected DbSet<TEntity> dbSet;

        public Repository(AnyGymDataContext context)
        {
            dbContext = context;
            dbSet = dbContext.Set<TEntity>();
        }

        public virtual void Add(TEntity entity)
        {
            dbSet.Add(entity);

            try
            {
                dbContext.SaveChanges();
                //dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void Delete(TEntity entity)
        {
            try
            {
                dbSet.Remove(entity);
                dbContext.SaveChanges();
                //dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual TEntity Get(Guid id)
        {
            return dbSet.Find(id);
        }

        public virtual TEntity GetWithoutTrack(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.AsNoTracking().Where(predicate).FirstOrDefault();
        }

        public virtual void Update(TEntity entity)
        {
            try
            {
                dbSet.Update(entity);
                dbContext.SaveChanges();
                //dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual IEnumerable<TEntity> All()
        {
            try
            {
                return dbSet.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.Where(predicate).ToList();
        }

        protected string CreatePagSeguroRequestURI(string endpoint, List<KeyValuePair<string, string>> parameters = null)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Runtime.PagSeguroApiAdress);
            sb.Append("/" + endpoint);

            sb.Append("?");
            sb.AppendFormat("{0}={1}", "email", Runtime.PagSeguroEmail);
            sb.AppendFormat("&{0}={1}", "token", Runtime.PagSeguroToken);

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                    sb.AppendFormat("&{0}={1}", parameter.Key, parameter.Value);
            }

            return sb.ToString();
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (dbContext == null) return;
            dbContext.Dispose();
        }

        public async Task<PaginatedList<TEntity>> FindPaginatedAsync(Expression<Func<TEntity, bool>> predicate, int? pageIndex, int? pageSize)
        {
            var entityList = dbSet.Where(predicate);

            return await PaginatedList<TEntity>.CreateAsync(entityList, !pageIndex.HasValue || pageIndex == default(int) ? 1 : pageIndex.Value, pageSize != default(int) ? pageSize.Value : await entityList.CountAsync());
        }

        #endregion
    }
}
