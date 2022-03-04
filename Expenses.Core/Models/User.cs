using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Expenses.Core.Moduels
{
    public class User : IdentityUser<int>
    {
        public byte[]? ProfileImage { get; set; }
        [MaxLength(50)]
        public string? FullName { get; set; }
        [MaxLength(11)]
        public string? Phone { get; set; }
        public virtual ICollection<Expense>? Expenses { get; set; }
        public virtual ICollection<Category>? Categories { get; set; }
    }
}
