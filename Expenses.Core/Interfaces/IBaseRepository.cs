
using Expenses.Core.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Expenses.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(string[] includes = null);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> criteria);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<bool> AnyAsync();
        Task<bool> AnyAsync(Expression<Func<T, bool>> criteria);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int skip, int take);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? skip, int? take,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);
        Task<T> AddAsync(T entity);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        Task<T> Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        int Count();
        int Count(Expression<Func<T, bool>> criteria);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> criteria);
        Task<T> GetLastRecored();
        //void Attach(T entity);
        //void AttachRange(IEnumerable<T> entities);
        // IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null);
        // IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int take, int skip);
        //  IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? take, int? skip, 
        //     Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);
        //  IEnumerable<T> AddRange(IEnumerable<T> entities);
        //   T Update(T entity);
        // T Add(T entity);
        //  T Find(Expression<Func<T, bool>> criteria, string[] includes = null);
        //  IEnumerable<T> GetAll();
        //  T GetById(int id);

    }
}