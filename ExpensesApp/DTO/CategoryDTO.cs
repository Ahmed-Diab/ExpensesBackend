using System.ComponentModel.DataAnnotations;

namespace ExpensesApp.DTO
{
    public class CategoryDTO
    {
        public int? UserId { get; set; }
        public string Name { get; set; }
    }
}
