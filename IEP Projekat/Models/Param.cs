using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    public class Param
    {
        [Key]
        [Display(Name ="Parameter")]
        public string Id { get; set; }

        [Required]
        [Display(Name ="Value")]
        public string Value { get; set; }

        [Display(Name ="Description")]
        public string Comment { get; set; }
    }
}