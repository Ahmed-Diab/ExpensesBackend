using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expenses.Core.Consts
{
    public static class OrderBy
    {
        public const string Ascending = "ASC";
        public const string Descending = "DESC";
    }

    public static class Extensions
    {
        public static List<string> AllowedImageExtanchans => new List<string> { ".png", ".jpeg", "jpg" };
        public static List<string> AllowedVoiceExtanchans => new List<string> { ".mp3" };
        public static long AllowedVoiceMaxSize = 2097152; // 1048576 1 MB
        public static long AllowedImageMaxSize = 2097152;// 2 MB

    }


}