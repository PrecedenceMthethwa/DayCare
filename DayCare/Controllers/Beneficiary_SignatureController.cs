using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DayCare.Models;
using Microsoft.AspNet.Identity;

namespace DayCare.Controllers
{
    public class Beneficiary_SignatureController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Beneficiary_Signature
        public ActionResult Index()
        {
            var beneficiary_Signature = db.Beneficiary_Signature.Include(b => b.Child);
            return View(beneficiary_Signature.ToList());
        }

        // GET: Beneficiary_Signature/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beneficiary_Signature beneficiary_Signature = db.Beneficiary_Signature.Find(id);
            if (beneficiary_Signature == null)
            {
                return HttpNotFound();
            }
            return View(beneficiary_Signature);
        }

        // GET: Beneficiary_Signature/Create
        public ActionResult Create(string id)
        {
            var benSignature = new Beneficiary_Signature();
            benSignature.Child_Id = id;
            ViewBag.Child_Id = new SelectList(db.Children, "Child_Id", "Child_Name");
            return View(benSignature);
        }

        // POST: Beneficiary_Signature/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize]
        public ActionResult Create([Bind(Include = "Signaturee_ID,Sign_Date,MySignature,SignedBy,Child_Id")] Beneficiary_Signature beneficiary_Signature)
        {
            if (ModelState.IsValid)
            {
                var uid = User.Identity.GetUserId();
                beneficiary_Signature.Sign_Date = DateTime.Now;
                var roomId = (from i in db.ClassRooms
                              where i.Child_Id == beneficiary_Signature.Child_Id
                              select i.Room_Id).FirstOrDefault();
                ClassRoom classRoom = db.ClassRooms.Find(roomId);

                if (classRoom.Deliverystatus.Status_Name == "At home" && classRoom.Driver_ID == uid)
                {
                    classRoom.Deliverystatus.Status_Name = "Out to deliver";
                    db.Entry(classRoom).State = EntityState.Modified;
                    db.Beneficiary_Signature.Add(beneficiary_Signature);
                    db.SaveChanges();
                    return RedirectToAction("WithDriver", "ClassRooms");

                }
               
                if (classRoom.Deliverystatus.Status_Name == "Out to deliver" && classRoom.Employee_Id == uid)
                {
                    classRoom.Deliverystatus.Status_Name = "In class";
                    db.Entry(classRoom).State = EntityState.Modified;
                    db.Beneficiary_Signature.Add(beneficiary_Signature);
                    db.SaveChanges();
                    return RedirectToAction("ClassRoom", "ClassRooms");
                }
              
                if (classRoom.Deliverystatus.Status_Name == "In class" && classRoom.Driver_ID == uid)
                {
                    classRoom.Deliverystatus.Status_Name = "Out to deliver";
                    db.Entry(classRoom).State = EntityState.Modified;
                    db.Beneficiary_Signature.Add(beneficiary_Signature);
                    db.SaveChanges();
                    return RedirectToAction("WithDriver", "ClassRooms");
                }

                if (classRoom.Deliverystatus.Status_Name == "Out to deliver")
                {
                    classRoom.Deliverystatus.Status_Name = "At home";
                    db.Entry(classRoom).State = EntityState.Modified;
                    db.Beneficiary_Signature.Add(beneficiary_Signature);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Child_Id = new SelectList(db.Children, "Child_Id", "Child_Name", beneficiary_Signature.Child_Id);
            return View(beneficiary_Signature);
        }

        // GET: Beneficiary_Signature/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beneficiary_Signature beneficiary_Signature = db.Beneficiary_Signature.Find(id);
            if (beneficiary_Signature == null)
            {
                return HttpNotFound();
            }
            ViewBag.Child_Id = new SelectList(db.Children, "Child_Id", "Child_Name", beneficiary_Signature.Child_Id);
            return View(beneficiary_Signature);
        }

        // POST: Beneficiary_Signature/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Signaturee_ID,Sign_Date,MySignature,SignedBy,Child_Id")] Beneficiary_Signature beneficiary_Signature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beneficiary_Signature).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Child_Id = new SelectList(db.Children, "Child_Id", "Child_Name", beneficiary_Signature.Child_Id);
            return View(beneficiary_Signature);
        }

        // GET: Beneficiary_Signature/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beneficiary_Signature beneficiary_Signature = db.Beneficiary_Signature.Find(id);
            if (beneficiary_Signature == null)
            {
                return HttpNotFound();
            }
            return View(beneficiary_Signature);
        }

        // POST: Beneficiary_Signature/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Beneficiary_Signature beneficiary_Signature = db.Beneficiary_Signature.Find(id);
            db.Beneficiary_Signature.Remove(beneficiary_Signature);
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
