using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookOnline.Models
{
    public class Book
    {
        [Key]
        public int Book_Id { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống.")]
        [Column(TypeName = "nvarchar(100)")]
        public string Book_Name { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống.")]
        [Column(TypeName = "nvarchar(100)")]
        public string Book_Author { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống.")]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Book_Content { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống.")]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Book_ShortContent { get; set; }

        public int Book_Category { get; set; }
        public Category Category { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống.")]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Book_Img { get; set; }

        [DataType(DataType.Date)]
        public string Book_Date { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Book_NXB { get; set; }

        [Column(TypeName = "int")]
        public int Book_View { get; set; }

        [Column(TypeName = "bit")]
        public bool Book_Status { get; set; }
    }
}
