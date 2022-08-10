using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookOnline.Models
{
    public class Account
    {
        [Key]
        public int Account_Id { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string Account_Name { get; set; }

        //[RegularExpression(@"[A-Za-z0-9!@#$%^&*()/*-+_=]{6,20}")]
        [Column(TypeName = "varchar(MAX)")]
        [DataType(DataType.Password)]
        public string Account_Password { get; set; }

        //[RegularExpression(@"[A-Za-z0-9!@#$%^&*()/*-+_=]{6,20}")]
        [Column(TypeName = "varchar(MAX)")]
        [DataType(DataType.Password)]
        public string Account_ConfirmPassword { get; set; }

        //[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}")]
        [Column(TypeName = "varchar(50)")]
        public string Account_Email { get; set; }

        [Column(TypeName = "int")]
        public int Account_Phone { get; set; }

        [Column(TypeName = "int")]
        public int Account_Type { get; set; }

        [Column(TypeName = "bit")]
        public bool Account_Status { get; set; }
    }
}
