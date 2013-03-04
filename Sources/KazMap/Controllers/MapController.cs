using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KazMap.DataAccess;

namespace KazMap.Controllers
{ 
    public class MapController : Controller
    {
        private KazMapConnection db = new KazMapConnection();

        //
        // GET: /Map/

        public ViewResult Index()
        {
            return View(db.Nodes.Include("Tags").ToList());
        }

        public ViewResult Surface()
        {
            return View();
        }

        //
        // GET: /Map/Details/5

        public ViewResult Details(long id)
        {
            Nodes nodes = db.Nodes.Single(n => n.Id == id);
            return View(nodes);
        }

        //
        // GET: /Map/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Map/Create

        [HttpPost]
        public ActionResult Create(Nodes nodes)
        {
            if (ModelState.IsValid)
            {
                db.Nodes.AddObject(nodes);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(nodes);
        }
        
        //
        // GET: /Map/Edit/5
 
        public ActionResult Edit(long id)
        {
            Nodes nodes = db.Nodes.Single(n => n.Id == id);
            return View(nodes);
        }

        //
        // POST: /Map/Edit/5

        [HttpPost]
        public ActionResult Edit(Nodes nodes)
        {
            if (ModelState.IsValid)
            {
                db.Nodes.Attach(nodes);
                db.ObjectStateManager.ChangeObjectState(nodes, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nodes);
        }

        //
        // GET: /Map/Delete/5
 
        public ActionResult Delete(long id)
        {
            Nodes nodes = db.Nodes.Single(n => n.Id == id);
            return View(nodes);
        }

        //
        // POST: /Map/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            Nodes nodes = db.Nodes.Single(n => n.Id == id);
            db.Nodes.DeleteObject(nodes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}