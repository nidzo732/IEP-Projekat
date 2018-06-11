using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    public class TokenOrder
    {
        [Key]
        [MaxLength(200)]
        public string Id { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        [Display(Name ="Number of tokens")]
        public decimal TokenCount { get; set; }

        [Required]
        [Display(Name = "Actual price")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name ="Date")]
        public DateTime Timestamp { get; set; }
    }
}