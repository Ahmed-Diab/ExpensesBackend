using AutoMapper;
using Expenses.Core.Moduels;
using ExpensesApp.DTO;

namespace ExpensesApp.Helper
{
    public class AutoMaperProfile: Profile
    {
        public AutoMaperProfile()
        {
            CreateMap<UserDTO, User>().ReverseMap();
            CreateMap<ExpenseDTO, Expense>().ReverseMap();
            CreateMap<CategoryDTO, Category>().ReverseMap();
            CreateMap<UserDTO, User>().ReverseMap();
            CreateMap<SignUpDTO, User>();

        }
    }
}
