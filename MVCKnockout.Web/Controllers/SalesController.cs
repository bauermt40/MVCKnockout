using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCKnockout.DataLayer;
using MVCKnockout.Model;
using MVCKnockout.Web.ViewModels;

namespace MVCKnockout.Web.Controllers
{
    public class SalesController : Controller
    {
        private SalesContext _salesContext;

        public SalesController()
        {
            _salesContext = new SalesContext();
        }

        // GET: Sales
        public ActionResult Index()
        {
            return View(_salesContext.SalesOrders.ToList());
        }

        // GET: Sales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesOrder salesOrder = _salesContext.SalesOrders.Find(id);
            if (salesOrder == null)
            {
                return HttpNotFound();
            }

            SalesOrderViewModel salesOrderViewModel = new SalesOrderViewModel()
            {
                SalesOrderId = salesOrder.SalesOrderId,
                CustomerName = salesOrder.CustomerName,
                PONumber = salesOrder.PONumber,
                MessageToClient = "From viewmodel"
            };
            return View(salesOrderViewModel);
        }

        // GET: Sales/Create
        public ActionResult Create()
        {
            SalesOrderViewModel salesOrderViewModel = new SalesOrderViewModel();
            salesOrderViewModel.ObjectState = ObjectState.Added;
            return View(salesOrderViewModel);
        }

        // POST: Sales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SalesOrderId,CustomerName,PONumber")] SalesOrder salesOrder)
        {
            if (ModelState.IsValid)
            {
                _salesContext.SalesOrders.Add(salesOrder);
                _salesContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(salesOrder);
        }

        // GET: Sales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesOrder salesOrder = _salesContext.SalesOrders.Find(id);
            if (salesOrder == null)
            {
                return HttpNotFound();
            }

            SalesOrderViewModel salesOrderViewModel = new SalesOrderViewModel();
            salesOrderViewModel.SalesOrderId = salesOrder.SalesOrderId;
            salesOrderViewModel.CustomerName = salesOrder.CustomerName;
            salesOrderViewModel.PONumber = salesOrder.PONumber;
            salesOrderViewModel.ObjectState = ObjectState.Unchanged;
            salesOrderViewModel.MessageToClient = string.Format("The original value of Customer Name is {0}.", salesOrderViewModel.CustomerName);

            return View(salesOrderViewModel);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SalesOrderId,CustomerName,PONumber")] SalesOrder salesOrder)
        {
            if (ModelState.IsValid)
            {
                _salesContext.Entry(salesOrder).State = EntityState.Modified;
                _salesContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(salesOrder);
        }

        // GET: Sales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SalesOrder salesOrder = _salesContext.SalesOrders.Find(id);
            if (salesOrder == null)
            {
                return HttpNotFound();
            }
            return View(salesOrder);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SalesOrder salesOrder = _salesContext.SalesOrders.Find(id);
            _salesContext.SalesOrders.Remove(salesOrder);
            _salesContext.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _salesContext.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult Save(SalesOrderViewModel salesOrderViewModel)
        {
            SalesOrder salesOrder = new SalesOrder()
            {
                SalesOrderId = salesOrderViewModel.SalesOrderId,
                CustomerName = salesOrderViewModel.CustomerName,
                PONumber = salesOrderViewModel.PONumber,
                ObjectState = salesOrderViewModel.ObjectState
            };

            _salesContext.SalesOrders.Attach(salesOrder);
            _salesContext.ChangeTracker.Entries<IObjectWithState>().Single().State = Helpers.ConvertState(salesOrder.ObjectState);
            _salesContext.SaveChanges();

            switch(salesOrderViewModel.ObjectState)
            {
                case ObjectState.Added:
                    salesOrderViewModel.MessageToClient = string.Format("{0}'s sales order has been added to the database.", salesOrder.CustomerName);
                    break;
                case ObjectState.Modified:
                    salesOrderViewModel.MessageToClient = string.Format("The customer name for this sales order has updated to {0}.", salesOrder.CustomerName);
                    break;
            }

            salesOrderViewModel.SalesOrderId = salesOrder.SalesOrderId;
            salesOrderViewModel.ObjectState = ObjectState.Unchanged;

            return Json(new { salesOrderViewModel });
        }
    }
}
