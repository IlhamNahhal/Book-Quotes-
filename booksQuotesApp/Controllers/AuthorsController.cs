using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using booksQuotesApp.Models;

namespace booksQuotesApp.Controllers
{
    public class AuthorsController : Controller
    {
        private myBookQuoteDBEntities db = new myBookQuoteDBEntities();

        // GET: Authors
        public ActionResult Index(string searchString)
        {
            //create IQueryable variable
            var authors = from a in db.Authors
                          select a;

            //filter by search string
            if (!String.IsNullOrEmpty(searchString))
            {
                authors = authors.Where(a => a.Name.Contains(searchString));
                
            }

            return View(authors);
        }

        // GET: Authors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound(); 
            }
            return View(author);
        }

        // GET: Authors/Create
        public ActionResult Create(string returnView2)
        {
            
            TempData["returnView2"] = returnView2;
           


            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Biography,ImageURL")] Author author)
        {
            //if (author.ImageURL == null)
            //{
            //    author.ImageURL = ;
            //}

            try
            {
              

                if (ModelState.IsValid)
                {
                    db.Authors.Add(author);
                    db.SaveChanges();

                    if (Convert.ToString(TempData["returnView2"]) == "createBookView")
                    {  

                        return RedirectToAction("Create","Books");
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the problem persists, contact your system administrator.");
            }

            return View(author);
        }

        // GET: Authors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Biography,ImageURL")] Author author)
        {

        
            if (ModelState.IsValid)
            {
                db.Entry(author).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(author);
        }

        // GET: Authors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int AssociatedBooksNumber = 0;
            Author author = db.Authors.Find(id);
            AssociatedBooksNumber = author.Books.Count();
            //only allow delete if there are no books or films associated with detective
            if (AssociatedBooksNumber == 0)
            {
                db.Authors.Remove(author);
                db.SaveChanges();
            }
            else
            {
                TempData["MyErrorMessage"] = "You cannot delete this author while there are books linked to them.";
                TempData["DeletionError"] = true;
                return RedirectToAction("Delete", new { id = id });
            }

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
