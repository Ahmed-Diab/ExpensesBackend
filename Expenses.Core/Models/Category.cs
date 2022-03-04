using System.ComponentModel.DataAnnotations;

namespace Expenses.Core.Moduels
{
    public class Category
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        public User User { get; set; }
    }
}
