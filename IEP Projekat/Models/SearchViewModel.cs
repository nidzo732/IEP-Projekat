using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static IEP_Projekat.Models.Auction;

namespace IEP_Projekat.Models
{
    public class SearchViewModel
    {
        [Key]
        [Display(Name ="Name contains these words")]
        public string SearchQuery { get; set; }


        public AuctionStatus? Status { get; set; }

        [Display(Name ="Min. price")]
        public decimal? MinPrice { get; set; }
        [Display(Name = "Max. price")]
        public decimal? MaxPrice { get; set; }
        public int? Page { get; set; }

        public bool? My { get; set; }

        [Display(Name = "My purchases")]
        public bool? MyPurchases { get; set; }
    }
}