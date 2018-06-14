using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IEP_Projekat.Models
{
    public class Bid
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [Display(Name ="Ammount")]
        [Range(0.01, int.MaxValue)]
        public decimal Increment { get; set; }

        [Required]
        [MaxLength(200)]
        public string AuctionId { get; set; }

        [Required]
        [Display(Name ="Time and date")]
        public DateTime TimeStamp { get; set; }

        [Required]
        [MaxLength(200)]
        public string UserId { get; set; }

    }
}