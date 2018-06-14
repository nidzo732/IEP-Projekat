using IEP_Projekat.Models;
using log4net;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;

namespace IEP_Projekat.Controllers
{
    public class CentiliController : ApiController
    {
        private ApplicationDbContext _db;
        public ApplicationDbContext Db
        {
            get
            {
                return _db ?? Request.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _db = value;
            }
        }
        public class RequestContent
        {
            public string clientid { get; set; }
            public string amount { get; set; }
            public string sign { get; set; }
            public string status { get; set; }
        }
        private ILog logger = LogManager.GetLogger(typeof(ApiController));
        [HttpGet]
        public async Task<string> PurchaseStatus(string clientid, string amount, string sign, string status)
        {
            try
            {
                logger.Info("Got request from Centili");
                logger.Info(Request.RequestUri.ToString());

                var pairs = Request.GetQueryNameValuePairs().ToList();
                pairs = pairs.OrderBy(x => x.Key).ToList();
                var joined = string.Join("&", pairs.Where(x => x.Key != "sign").Select(x => x.Value));
                //TODO: verify HMAC

                var tokenOrder = await Db.TokenOrders.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == clientid);
                if(tokenOrder==null)
                {
                    logger.Warn("Got a nonexistent order from Centili " + clientid);
                    return "OK";
                }
                if(tokenOrder.Status!= "SUBMITTED")
                {
                    logger.Warn("Got a request for an order with status " + tokenOrder.Status);
                    return "OK";
                }
                tokenOrder.TokenCount = decimal.Parse(amount);
                tokenOrder.Price = decimal.Parse(Db.Params.Find("T").Value) * tokenOrder.TokenCount;
                if(status=="success")
                {
                    tokenOrder.User.Tokens += tokenOrder.TokenCount;
                    tokenOrder.Status = "COMPLETED";
                    logger.Info("Transaction successful, user got " + amount + " tokens");
                }
                else
                {
                    logger.Info("Transaction canceled");
                    tokenOrder.Status = "CANCELED";
                }
                await Db.SaveChangesAsync();
                return "OK";                
            }
            catch(Exception ex)
            {
                logger.Error("Exception happened when handling Centili request", ex);
            }
            return "OK";
        }
    }
}
