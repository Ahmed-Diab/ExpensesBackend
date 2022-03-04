 using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExpensesApp.DTO
{
    public class ExpenseDTO
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        [DefaultValue(0)]
        public decimal Amount { get; set; }
        //public IFormFile VoiceNote { get; set; }
        //public IFormFile ImageNote { get; set; }
        [MaxLength(5)]
        public string VoiceNoteFormat { get; set; }
        public string VoiceNote { get; set; }

        [MaxLength(5)]
        public string ImageNoteFormat { get; set; }
        public string ImageNote { get; set; }

        [MaxLength(150)]
        public string? TextNote { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
