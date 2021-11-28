using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DayCare.Models;

namespace DayCare.Controllers
{
    public class ChildrenController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Children
        public ActionResult Index()
        {
            var children = db.Children.Include(c => c.Parent).Where(c=>c.Approved==false);
            return View(children.ToList());
        }
        public ActionResult FullScreen(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           Child child = db.Children.Find(id);
            if (child == null)
            {
                return HttpNotFound();
            }
            return View(child);
        }

        public ActionResult Accept(string id)
        {
            Child children = db.Children.Find(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

         
            children.Approved = true;
            db.Entry(children).State = EntityState.Modified;
            db.SaveChanges();
            Email emails = new Email();
            ViewBag.Subject = " Acceptance Confirmation ";
            ViewBag.Body = $"Hi {children.Parent.Parent_Name}" + "<br/>" +
                $"Your application was successfully approved for your child's enrolment with Smart DayCare." + "<br/>" +
                $"You will get further instructions on how to make payment for you once the Regitration Process is Complete." + "<br/>" +
                $"Thank you." + "<br/>" +
                $"Smart DayCare.";
           
            emails.Gmail(ViewBag.Subject, ViewBag.Body, children.Parent.Parent_Email);
       

            return RedirectToAction("Create","ClassRooms", new { id = children.Child_Id });
        }
        public ActionResult Decline(string id)
        {
            Child children = db.Children.Find(id);
            Parent parent = db.Parents.Find(children.Parent_Id);
            db.Parents.Remove(parent);
            db.Children.Remove(children);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // GET: Children/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = db.Children.Find(id);
            if (child == null)
            {
                return HttpNotFound();
            }
            return View(child);
        }

        // GET: Children/Create
        public ActionResult Create(string id)
        {
            var child = new Child();
            child.Parent_Id = id;
            ViewBag.Parent_Id = new SelectList(db.Parents, "Parent_Id", "Parent_Name");
            return View(child);
        }

        // POST: Children/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Child_Id,Child_Name,Child_LastName,Child_Ducuments,Child_Image,Parent_Id")] Child child, HttpPostedFileBase filelist, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (filelist != null && filelist.ContentLength > 0)
                {
                    child.Child_Image = ConvertToBytes(filelist);
                }
                if (upload != null && upload.ContentLength > 0)
                {
                    int filelength = upload.ContentLength;
                    Byte[] array = new Byte[filelength];
                    upload.InputStream.Read(array, 0, filelength);
                    child.Child_Ducuments = array;
                }
                child.Payment_Date = DateTime.Now;
                if (ValidateAge(child.Child_Id) > 5)
                {
                    ViewBag.ErrorDate = "The day care does not accept a child over the age of 5";
                    return View(child);
                }

                db.Children.Add(child);
                db.SaveChanges();
                return RedirectToAction("Successfull");
            }

            ViewBag.Parent_Id = new SelectList(db.Parents, "Parent_Id", "Parent_Name", child.Parent_Id);
            return View(child);
        }
        public byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            return reader.ReadBytes((int)file.ContentLength);
        }
        public int ValidateAge(string id)
        {
            DateTime date;
            string getYear;
            if (int.Parse(id.Substring(0, 1)) > 0)
            {
                getYear = id.Substring(2, 2) + "/" + id.Substring(4, 2) + "/" + "19" + id.Substring(0, 2);
            }
            else
            {
                getYear = id.Substring(2, 2) + "/" + id.Substring(4, 2) + "/" + "20" + id.Substring(0, 2);
            }
            date = DateTime.Parse(getYear);
            return (DateTime.Now.Year - date.Year);
        }

        public ActionResult Successfull()
        {
            return View();
        }
        // GET: Children/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = db.Children.Find(id);
            if (child == null)
            {
                return HttpNotFound();
            }
            ViewBag.Parent_Id = new SelectList(db.Parents, "Parent_Id", "Parent_Name", child.Parent_Id);
            return View(child);
        }
        public ActionResult Testing(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = db.Children.Find(id);
            if (child == null)
            {
                return HttpNotFound();
            }
            ViewBag.Parent_Id = new SelectList(db.Parents, "Parent_Id", "Parent_Name", child.Parent_Id);
            return View(child);
        }

        // POST: Children/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Child_Id,Child_Name,Child_LastName,Child_Ducuments,Child_Image,Parent_Id")] Child child)
        {
            if (ModelState.IsValid)
            {
                db.Entry(child).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Parent_Id = new SelectList(db.Parents, "Parent_Id", "Parent_Name", child.Parent_Id);
            return View(child);
        }

        // GET: Children/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Child child = db.Children.Find(id);
            if (child == null)
            {
                return HttpNotFound();
            }
            return View(child);
        }

        // POST: Children/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Child child = db.Children.Find(id);
            db.Children.Remove(child);
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
