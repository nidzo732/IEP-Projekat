using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IEP_Projekat.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace IEP_Projekat.Controllers
{
    [Authorize]
    public class TokenOrdersController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _db;

        public TokenOrdersController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }
        public TokenOrdersController()
        {

        }

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


        // GET: TokenOrders
        public ActionResult Index()
        {
            var id = User.Identity.GetUserId();
            return View(Db.TokenOrders.Where(x=>x.User.Id==id).ToList());
        }

        // GET: TokenOrders/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TokenOrder tokenOrder = Db.TokenOrders.Find(id);
            if (tokenOrder == null)
            {
                return HttpNotFound();
            }
            return View(tokenOrder);
        }

        // GET: TokenOrders/Create
        public async Task<ActionResult> Create()
        {
            Db.TokenOrders.Add(new TokenOrder
            {
                Id = Guid.NewGuid().ToString(),
                Price = -1,
                TokenCount = -1,
                Status = "SUBMITTED",
                User=await UserManager.FindByIdAsync(User.Identity.GetUserId()),
                Timestamp=DateTime.Now
            });
            await Db.SaveChangesAsync();
            return Redirect("https://stage.centili.com/widget/WidgetModule?api=ae813e143aba790ae6729f2897849af4");
        }

        [AllowAnonymous]
        public async Task<ActionResult> CentiliHook(string id, decimal price, decimal count, string status)
        {
            try
            {
                if (status != "CANCELED" && status != "COMPLETED") return Content("OK");
                if (price <= 0 || count <= 0) return Content("OK");
                var order = Db.TokenOrders.Include(x=>x.User).FirstOrDefault(x=>x.Id==id);
                if (order == null) return Content("OK");
                if (order.Status != "SUBMITTED") return Content("OK");
                order.Status = status;
                order.Price = price;
                order.TokenCount = count;
                order.User.Tokens += count;
                await Db.SaveChangesAsync();
                return Content("OK");
            }
            catch(Exception ex)
            {
                return Content("OK");
            }
        }

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
