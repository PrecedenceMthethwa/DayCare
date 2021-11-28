using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DayCare.Models;

namespace DayCare.Controllers
{
    public class DeliverystatusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Deliverystatus
        public ActionResult Index()
        {
            return View(db.Deliverystatuses.ToList());
        }

        // GET: Deliverystatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deliverystatus deliverystatus = db.Deliverystatuses.Find(id);
            if (deliverystatus == null)
            {
                return HttpNotFound();
            }
            return View(deliverystatus);
        }

        // GET: Deliverystatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Deliverystatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DeliveryStatus_ID,Status_Name")] Deliverystatus deliverystatus)
        {
            if (ModelState.IsValid)
            {
                db.Deliverystatuses.Add(deliverystatus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(deliverystatus);
        }

        // GET: Deliverystatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deliverystatus deliverystatus = db.Deliverystatuses.Find(id);
            if (deliverystatus == null)
            {
                return HttpNotFound();
            }
            return View(deliverystatus);
        }

        // POST: Deliverystatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DeliveryStatus_ID,Status_Name")] Deliverystatus deliverystatus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deliverystatus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(deliverystatus);
        }

        // GET: Deliverystatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deliverystatus deliverystatus = db.Deliverystatuses.Find(id);
            if (deliverystatus == null)
            {
                return HttpNotFound();
            }
            return View(deliverystatus);
        }

        // POST: Deliverystatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Deliverystatus deliverystatus = db.Deliverystatuses.Find(id);
            db.Deliverystatuses.Remove(deliverystatus);
            db.SaveChanges();
            return RedirectToAction("Index");
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
