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
    public class BooksController : Controller
    {
        private myBookQuoteDBEntities db = new myBookQuoteDBEntities();

        // GET: Books
        public ActionResult Index(string searchString, string currentFilter)
        {
            //create IQueryable variable
            var books = from b in db.Books
                          select b;

            //filter by search string
            if (!String.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Name.Contains(searchString));

            }

            return View(books);
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Books/Create
        public ActionResult Create(string returnView)
        {
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "Name"); /*ID is the value that will be passed to authorID field, name is text that will be displayed in dropdownList*/

            if (returnView != null)
            {
                TempData["returnView"] = returnView;
                return View();
            }

            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Genre,Synopsis,ImageURL,AuthorId")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                db.SaveChanges();
                if (Convert.ToString(TempData["returnView"]) == "createQuoteView")
                {
                    return RedirectToAction("Create", "Quotes");
                }

                else {
                    return RedirectToAction("Index");
                     }
            }

            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "Name", book.AuthorId);
            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "Name", book.AuthorId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Genre,Synopsis,ImageURL,AuthorId")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Authors, "Id", "Name", book.AuthorId);
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int AssociatedQuotesNumber = 0;
            Book book = db.Books.Find(id);
            AssociatedQuotesNumber = book.Quotes.Count();
            //only allow delete if there are no books or films associated with detective
            if (AssociatedQuotesNumber == 0)
            {
                db.Books.Remove(book);
                db.SaveChanges();
            }
            else
            {
                TempData["MyErrorMessage"] = "You cannot delete this book while there are quotes linked to them.";
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
