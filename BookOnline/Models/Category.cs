using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookOnline.Models
{ 
    public class Category
    {
        [Key]
        public int Category_Id { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống.")]
        [Column(TypeName = "nvarchar(100)")]
        public string Category_Name { get; set; }

        [Required(ErrorMessage = "Vui lòng không để trống.")]
        [Column(TypeName = "nvarchar(100)")]
        public string Category_DataName { get; set; }

        [Column(TypeName = "bit")]
        public bool Category_Status { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
