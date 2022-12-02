using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

namespace Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        public Settings Settings { get; }
        protected IServiceProvider Provider { get; }
        protected IDbContextBuilder DbContextOptions { get; }

        public BaseRepository(IServiceProvider provider)
        {
            Provider = provider;
            DbContextOptions = Provider.GetService<IDbContextBuilder>();
        }

        /// <summary>
        /// Получение записи 
        /// </summary>
        /// <param name="id"></param>
        public async Task<T> GetByIdAsync(int id)
        {
            using (new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted
                }, TransactionScopeAsyncFlowOption.Enabled))
            {
                using var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions());
                return await context.Set<T>().FirstOrDefaultAsync(c => c.Id == id);
            }
        }


        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            using (new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
                }, TransactionScopeAsyncFlowOption.Enabled))
            {
                using var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions());
                return await context.Set<T>().FirstOrDefaultAsync(predicate);
            }
        }

        public async Task<T> AddAsync(T entity)
        {
            using var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions());
            var result = await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            using var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions());
            await context.Set<T>().AddRangeAsync(entities);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            using var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions());
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            using var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions());
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, int offset, int limit)
        {
            using (new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
                }, TransactionScopeAsyncFlowOption.Enabled))
            {
                using var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions());
                if (offset == -1 || limit == -1)
                {
                    return await context.Set<T>().Where(predicate).ToListAsync();
                }
                else
                {
                    return await context.Set<T>().Where(predicate).Skip(offset).Take(limit).ToListAsync();
                }
            }
        }

        public async Task<int> CountAllAsync()
        {
            using (new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
                }, TransactionScopeAsyncFlowOption.Enabled))
            {
                using var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions());
                return await context.Set<T>().CountAsync();
            }
        }

        public async Task<int> CountWhereAsync(Expression<Func<T, bool>> predicate)
        {
            using (new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
                }, TransactionScopeAsyncFlowOption.Enabled))
            {
                using var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions());
                return await context.Set<T>().Where(predicate).CountAsync();
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync(int offset, int limit)
        {
            using (new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
                }, TransactionScopeAsyncFlowOption.Enabled))
            {
                using var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions());
                if (limit == -1 || offset == -1)
                {
                    return await context.Set<T>().ToListAsync();
                }
                return await context.Set<T>()
                        .Skip(offset).Take(limit).ToListAsync();
            }
        }

        public IEnumerable<T> GetWhere(Expression<Func<T, bool>> predicate)
        {
            using (new TransactionScope(
                 TransactionScopeOption.Required,
                 new TransactionOptions
                 {
                     IsolationLevel = IsolationLevel.ReadUncommitted,
                 }, TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions()))
                {
                    return context.Set<T>().Where(predicate).ToList();
                }
            }
        }

        public IEnumerable<T> GetAll()
        {
            using (new TransactionScope(
               TransactionScopeOption.Required,
               new TransactionOptions
               {
                   IsolationLevel = IsolationLevel.ReadUncommitted,
               }, TransactionScopeAsyncFlowOption.Enabled))
            {
                using var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions());
                return context.Set<T>().ToList();
            }
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            using (new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadUncommitted,
                }, TransactionScopeAsyncFlowOption.Enabled))
            {
                using var context = new DataGoogleAPIContext(DbContextOptions.GetContextOptions());
                return context.Set<T>().FirstOrDefault(predicate);
            }
        }
    }
}
