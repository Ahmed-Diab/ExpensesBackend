using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Expenses.Core.Moduels
{
    public class Expense
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        [DefaultValue(0)]
        public decimal Amount { get; set; }
        [MaxLength(5)]
        public string VoiceNoteFormat { get; set; }
        public byte[]? VoiceNote { get; set; }
        [MaxLength(5)]
        public string ImageNoteFormat { get; set; }
        public byte[]? ImageNote { get; set; }
        [MaxLength(150)]
        public string? TextNote { get; set; }
        public DateTime? CreatedAt { get; set; }
        public virtual User User { get; set; }
        public virtual Category Category { get; set; }



    }
}
