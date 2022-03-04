using Expenses.Core;
using Expenses.Core.Interfaces;
using Expenses.Core.Moduels;
using Expenses.EF.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expenses.EF
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        //public IBaseRepository<Category> Users { get; private set; }
        public IBaseRepository<Category> Categories { get; private set; }
        public IBaseRepository<Expense> Expenses { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Categories = new BaseRepository<Category>(_context);
             Expenses = new BaseRepository<Expense>(_context);
            
        }

        public Task<int> Complete()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}