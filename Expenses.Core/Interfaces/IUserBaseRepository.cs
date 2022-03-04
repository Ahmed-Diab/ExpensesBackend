
using Expenses.Core.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace Expenses.Core.Interfaces
{
    public interface IUserBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> SingUp(T entity);
        Task<IEnumerable<T>> SingIn(T entity);
         Task<IEnumerable<T>> GetDashboard();
        Task<IEnumerable<T>> GetAllUsers();
 
    }
}
