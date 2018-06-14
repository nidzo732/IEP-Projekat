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

namespace IEP_Projekat.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public async Task<ActionResult> Index()
        {
            return View(await db.Params.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Param param = await db.Params.FindAsync(id);
            if (param == null)
            {
                return HttpNotFound();
            }
            return View(param);
        }

        // GET: Admin/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Param param = await db.Params.FindAsync(id);
            if (param == null)
            {
                return HttpNotFound();
            }
            return View(param);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Value")] Param param)
        {
            try
            {
                decimal dummyD;
                int dummyI;
                var par = await db.Params.FindAsync(param.Id);
                if (par == null) return HttpNotFound();
                List<string> errors = new List<string>();
                if (ModelState.IsValid)
                {
                    switch (param.Id)
                    {
                        case "C":
                            break;
                        case "D":
                            if (!int.TryParse(param.Value, out dummyI)) errors.Add("Invalid value for this parameter");
                            if (dummyI <= 0) errors.Add("D must be positive and non zero");
                            break;
                        case "G":
                            if (!int.TryParse(param.Value, out dummyI)) errors.Add("Invalid value for this parameter");
                            if (dummyI <= 0) errors.Add("G must be positive and non zero");
                            break;
                        case "N":
                            if (!int.TryParse(param.Value, out dummyI)) errors.Add("Invalid value for this parameter");
                            if (dummyI <= 0) errors.Add("N must be positive and non zero");
                            break;
                        case "P":
                            if (!int.TryParse(param.Value, out dummyI)) errors.Add("Invalid value for this parameter");
                            if (dummyI <= 0) errors.Add("P must be positive and non zero");
                            break;
                        case "S":
                            if (!int.TryParse(param.Value, out dummyI)) errors.Add("Invalid value for this parameter");
                            if (dummyI <= 0) errors.Add("S must be positive and non zero");
                            break;
                        case "T":
                            if (!decimal.TryParse(param.Value, out dummyD)) errors.Add("Invalid value for this parameter");
                            if (dummyD <= 0) errors.Add("T must be positive and non zero");
                            break;
                    }
                    if (errors.Count == 0)
                    {
                        par.Value = param.Value;
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        errors.ForEach(x => ModelState.AddModelError("", x));
                    }
                }
                return View(par);
            }
            catch(Exception ex)
            {
                return HttpNotFound();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
