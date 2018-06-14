using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace IEP_Projekat
{
    public class AuctionHub : Hub
    {
        public static void PriceUpdate(string id, decimal price, string lastBidder, string prevBidder, decimal tokenAmmount)
        {
            GlobalHost.ConnectionManager.GetHubContext<AuctionHub>().Clients.All.PriceUpdate(id, price, lastBidder, prevBidder, tokenAmmount);
        }
    }
}