using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IEP_Projekat.Models;
using Microsoft.AspNet.Identity.Owin;
using IEP_Projekat.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IEP_Projekat.Controllers
{
    [Authorize]
    public class AuctionsController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _db;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationDbContext Db
        {
            get
            {
                return _db ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _db = value;
            }
        }

        // GET: Auctions
        [AllowAnonymous]
        public async Task<ActionResult> Index(SearchViewModel model)
        {
            bool imAdmin = User.IsInRole("admin");
            var myId = User.Identity.GetUserId();
            var now = DateTime.Now;
            var auctions = Db.Auctions.Include(x => x.User).Include(x=>x.LastBidder);
            if(!imAdmin)
            {
                auctions = auctions.Where(x => (x.Status == Auction.AuctionStatus.OPENED && x.EndDate>now) || x.User.Id == myId || (x.LastBidder!=null && x.LastBidder.Id==myId));
            }
            if(!string.IsNullOrEmpty(model.SearchQuery))
            {
                string[] words = model.SearchQuery.Split(' ');
                auctions = auctions.Where(x => words.Any(y => x.Name.Contains(y)));
            }
            if (model.MaxPrice != null) auctions = auctions.Where(x => x.CurrentAmmount <= model.MaxPrice);
            if (model.MinPrice != null) auctions = auctions.Where(x => x.CurrentAmmount >= model.MinPrice);
            if (model.My != null && (bool)(model.My)) auctions = auctions.Where(x => x.User.Id == myId);
            if (model.MyPurchases != null && (bool)(model.MyPurchases)) auctions = auctions.Where(x => x.LastBidder!=null && x.LastBidder.Id==myId && x.EndDate<now);
            auctions = auctions.OrderByDescending(x => x.StartDate);
            int pageSize = int.Parse(Db.Params.Find("N").Value);
            if(model.Page!=null && model.Page>0)
            {
                auctions = auctions.Skip((int)(model.Page-1)* pageSize);
            }
            else
            {
                model.Page = 1;
            }
            var result = await auctions.Take(pageSize).ToListAsync();
            ViewBag.PageSize = pageSize;
            ViewBag.Query = model;
            if (model.Status != null) result = result.Where(x => x.RealStatus == model.Status).ToList();
            return View(result.Select(x=>new AuctionViewModel(x)).ToList());
        }

        // GET: Auctions/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuctionViewModel auction = new AuctionViewModel(await Db.Auctions.Include(x=>x.User).Include(x=>x.LastBidder).FirstOrDefaultAsync(x=>x.Id==id));
            if (auction == null)
            {
                return HttpNotFound();
            }
            var bids = Db.Bids.Where(x => x.AuctionId == id).OrderByDescending(x=>x.TimeStamp).ToList();
            var users = bids.Select(x => Db.Users.Find(x.UserId)).ToList();
            decimal q = auction.CurrentAmmount;
            foreach(var bid in bids)
            {
                decimal g = bid.Increment;
                bid.Increment = q;
                q -= g;
            }
            ViewBag.Bids = bids;
            ViewBag.Users = users;
            return View(auction);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> Picture(string auctId)
        {
            var auction = await Db.Auctions.FirstOrDefaultAsync(x => x.Id == auctId);
            if (auction == null) return HttpNotFound();
            return File(auction.PictureContent, ImageManipulation.OutputImageFormat);
        }

        // GET: Auctions/Create
        public ActionResult Create()
        {
            return View(new AuctionViewModel
            {
                Duration=long.Parse(Db.Params.Find("D").Value),
                StartAmmount=10,                
            });
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Duration,StartAmmount,Picture")] AuctionViewModel auction)
        {
            if (ModelState.IsValid)
            {
                Db.Auctions.Add(new Auction
                {
                    Id = Guid.NewGuid().ToString(),
                    Duration = auction.Duration,
                    EndDate = DateTime.Now,
                    StartDate = DateTime.Now,
                    StartAmmount = auction.StartAmmount,
                    CurrentAmmount = auction.StartAmmount,
                    Name = auction.Name,
                    PictureContent = ImageManipulation.StoreImage(ImageManipulation.ResizeImage(ImageManipulation.LoadImage(auction.Picture.InputStream), 600, 600)),
                    Status = Auction.AuctionStatus.READY,
                    User = await UserManager.FindByIdAsync(User.Identity.GetUserId()),
                    Currency=Db.Params.Find("C").Value,
                    CurrencyPrice=decimal.Parse(Db.Params.Find("T").Value)
                });
                await Db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(auction);
        }
        [HttpPost]
        [Authorize(Roles ="admin")]
        public ActionResult Start(string id)
        {
            var auction = Db.Auctions.Include(x=>x.User).FirstOrDefault(x => x.Id == id);
            if (auction == null || auction.RealStatus != Auction.AuctionStatus.READY) return HttpNotFound();
            auction.Status = Auction.AuctionStatus.OPENED;
            auction.StartDate = DateTime.Now;
            auction.EndDate = DateTime.Now.AddSeconds(auction.Duration);
            Db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Bid(string id, decimal increment, decimal expVal)
        {
            if (increment <= 0 || expVal <= 0) return HttpNotFound();
            var status = "OK";
            using (var trans = Db.Database.BeginTransaction())
            {
                try
                {
                    var auction = await Db.Auctions.Include(x=>x.LastBidder).Include(x=>x.User).FirstOrDefaultAsync(x=>x.Id==id);
                    var currencyPrice = auction.CurrencyPrice;
                    var user =await  UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (auction==null || auction.RealStatus==Auction.AuctionStatus.READY || user.Id == auction.User.Id)
                    {
                        trans.Rollback();
                        return HttpNotFound();
                    }
                    var avlMoney = user.Tokens * auction.CurrencyPrice;
                    if(auction.RealStatus==Auction.AuctionStatus.COMPLETED)
                    {
                        status = "ERR:Auction has ended";
                    }
                    else if(auction.CurrentAmmount!=expVal)
                    {
                        status = "ERR:Somebody overtook you to this bid";
                    }
                    else if(auction.CurrentAmmount+increment>avlMoney && (auction.LastBidder == null || auction.LastBidder.Id != user.Id))
                    {
                        status = "ERR:Not enough tokens";
                    }
                    else if(auction.LastBidder!=null && auction.LastBidder.Id==user.Id && avlMoney<increment)
                    {
                        status = "ERR:Not enough tokens";
                    }
                    else
                    {
                        var newId = Guid.NewGuid().ToString();
                        Db.Bids.Add(new Models.Bid
                        {
                            Id = newId,
                            AuctionId = auction.Id,
                            Increment = increment,
                            TimeStamp = DateTime.Now,
                            UserId = user.Id
                        });
                        if (auction.LastBidder != null)
                        {
                            auction.LastBidder.Tokens += auction.CurrentAmmount/currencyPrice;
                            auction.User.Tokens-= auction.CurrentAmmount / currencyPrice;
                        }
                        auction.CurrentAmmount += increment;
                        var prevBidder = auction.LastBidder!=null?auction.LastBidder.Id:"";
                        auction.LastBidder = user;
                        auction.LastBidder.Tokens -= auction.CurrentAmmount/currencyPrice;
                        auction.User.Tokens+= auction.CurrentAmmount / currencyPrice;
                        AuctionHub.PriceUpdate(id, auction.CurrentAmmount, auction.LastBidder != null ? auction.LastBidder.FirstName + " " + auction.LastBidder.LastName : auction.User.FirstName + " " + auction.User.LastName, prevBidder, auction.CurrentAmmount/currencyPrice);
                    }
                    await Db.SaveChangesAsync();
                    trans.Commit();
                    var response = new {
                        status=status,
                        ammount=auction.CurrentAmmount,
                        lastBidder= auction.LastBidder != null ? auction.LastBidder.FirstName + " " + auction.LastBidder.LastName : auction.User.FirstName + " " + auction.User.LastName,
                        tokens =user.Tokens
                    };
                    return Json(response);
                }
                catch(Exception ex)
                {
                    trans.Rollback();
                    return HttpNotFound();
                }
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> BidsList(string id)
        {
            var auction = await Db.Auctions.FindAsync(id);
            if (auction == null) return HttpNotFound();
            var bids = await Db.Bids.Where(x => x.AuctionId == id).OrderByDescending(x => x.TimeStamp).ToListAsync();
            var users = bids.Select(x => Db.Users.Find(x.UserId)).ToList();
            decimal q = auction.CurrentAmmount;
            foreach (var bid in bids)
            {
                decimal g = bid.Increment;
                bid.Increment = q;
                q -= g;
            }
            ViewBag.Users = users;
            return PartialView(bids);
        }
        // GET: Auctions/Edit/5
        /*public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuctionViewModel auction = new AuctionViewModel(await db.Auctions.FindAsync(id));
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Duration,StartAmmount,Status,StartDate,EndDate,PictureContent")] AuctionViewModel auction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auction).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(auction);
        }

        // GET: Auctions/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuctionViewModel auction = new AuctionViewModel(await db.Auctions.FindAsync(id));
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            AuctionViewModel auction = new AuctionViewModel(await db.Auctions.FindAsync(id));
            db.Auctions.Remove(new Auction(auction));
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
