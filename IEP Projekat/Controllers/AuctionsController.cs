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
                auctions = auctions.Where(x => x.Status == Auction.AuctionStatus.OPENED || x.User.Id == myId);
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
            if (model.Status != null) auctions = auctions.Where(x => x.Status == model.Status);
            auctions = auctions.OrderByDescending(x => x.StartDate);
            if(model.Page!=null)
            {
                auctions.Skip((int)model.Page * 10);
            }
            return View(auctions.Take(10).ToList());
        }

        // GET: Auctions/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuctionViewModel auction = new AuctionViewModel(await Db.Auctions.FindAsync(id));
            if (auction == null)
            {
                return HttpNotFound();
            }
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
                Duration=86400,
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
            if (auction == null || auction.Status != Auction.AuctionStatus.READY) return HttpNotFound();
            auction.Status = Auction.AuctionStatus.OPENED;
            auction.StartDate = DateTime.Now;
            auction.EndDate = DateTime.Now.AddSeconds(auction.Duration);
            Db.SaveChanges();
            return RedirectToAction("Details", new { id = id });
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
