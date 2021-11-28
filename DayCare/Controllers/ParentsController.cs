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
    public class ParentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Parents
        public ActionResult Index()
        {
            return View(db.Parents.ToList());
        }

        // GET: Parents/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parent parent = db.Parents.Find(id);
            if (parent == null)
            {
                return HttpNotFound();
            }
            return View(parent);
        }

        // GET: Parents/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Parents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Parent_Id,Parent_Name,Parent_LastName,Parent_Address,Parent_ContactNumber,Parent_Email,Supplier_Ducuments")] Parent parent, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    int filelength = upload.ContentLength;
                    Byte[] array = new Byte[filelength];
                    upload.InputStream.Read(array, 0, filelength);
                    parent.Parent_Ducuments = array;
                }
                if (ValidateAge(parent.Parent_Id) < 18)
                {
                    ViewBag.ErrorDate = "Only a person over the age of 18 can register as a parent";
                    return View(parent);
                }
                    db.Parents.Add(parent);
                    db.SaveChanges();
                    return RedirectToAction("Create", "Children", new { id = parent.Parent_Id });
            }

            return View(parent);
        }
        public int ValidateAge(string id)
        {
            DateTime date;
            string getYear;
            if (int.Parse(id.Substring(0, 1)) > 0){
                getYear = id.Substring(2, 2) + "/"+ id.Substring(4, 2) + "/" + "19" + id.Substring(0, 2);
            }
            else{
                getYear = id.Substring(2, 2) + "/" + id.Substring(4, 2) + "/" + "20" + id.Substring(0, 2);
            }
            date = DateTime.Parse(getYear);
           return (DateTime.Now.Year - date.Year);
        }
        // GET: Parents/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parent parent = db.Parents.Find(id);
            if (parent == null)
            {
                return HttpNotFound();
            }
            return View(parent);
        }

        // POST: Parents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Parent_Id,Parent_Name,Parent_LastName,Parent_Address,Parent_ContactNumber,Parent_Email,Supplier_Ducuments")] Parent parent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(parent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(parent);
        }

        // GET: Parents/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parent parent = db.Parents.Find(id);
            if (parent == null)
            {
                return HttpNotFound();
            }
            return View(parent);
        }

        // POST: Parents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Parent parent = db.Parents.Find(id);
            db.Parents.Remove(parent);
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
