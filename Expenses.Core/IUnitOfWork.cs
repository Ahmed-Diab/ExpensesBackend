using Expenses.Core.Interfaces;
using Expenses.Core.Moduels;

namespace Expenses.Core
{
    public interface IUnitOfWork : IDisposable
    {


        IBaseRepository<Expense> Expenses { get; }
        IBaseRepository<Category> Categories { get; }
          //IBaseRepository<User> Users { get; }
        //IBaseRepository<User> Users { get; }
        //IBaseRepository<User> Users { get; }

        Task<int> Complete();
    }
}