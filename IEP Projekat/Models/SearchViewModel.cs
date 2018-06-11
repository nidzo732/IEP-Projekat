using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IEP_Projekat.Models.Auction;

namespace IEP_Projekat.Models
{
    public class SearchViewModel
    {
        public string SearchQuery { get; set; }

        public AuctionStatus? Status { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? Page { get; set; }
        public bool? My { get; set; }
        public bool? MyPurchases { get; set; }
    }
}