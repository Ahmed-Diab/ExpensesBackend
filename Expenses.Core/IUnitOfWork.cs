using Expenses.Core.Interfaces;
using Expenses.Core.Moduels;

namespace Expenses.Core
{
    public interface IUnitOfWork : IDisposable
    {


        IBaseRepository<Expense> Expenses { get; }
        IBaseRepository<Category> Categories { get; }
        Task<int> Complete();
    }
}