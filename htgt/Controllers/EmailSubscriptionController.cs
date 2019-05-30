using System;
using System.Net;
using System.Web.Mvc;
using htgt.Business;
using HTGT.Data.Models;
using log4net;

namespace htgt.Controllers
{
    [Authorize]
    public class EmailSubscriptionController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmailSubscriptionController));

        // GET: EmailSubscription
        public ActionResult Index()
        {
            try
            {
                var archanaList = ArchanaSubscriptionHelper.GetArchanaList();
                return View(archanaList);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        // GET: EmailSubscription/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmailSubscription/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,ParentName,DateOfBirth,EmailAddress")] KidsInformationCreateViewModel kidsInformation)
        {
            try
            {
                if (ModelState.IsValid && kidsInformation != null)
                {
                    ArchanaSubscriptionHelper.CreateSubscription(kidsInformation);
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Invalid Data", "Please try again.");
                return View(kidsInformation);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }


        // GET: EmailSubscription/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                KidsInformationEditViewModel kinfo = ArchanaSubscriptionHelper.GetKidsArchanaInformation(id.Value);
                return View(kinfo);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
                throw;
            }
        }

        // POST: EmailSubscription/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SID,KName,ParentsName,DateOfBirth,EmailAddress,IsActive")] KidsInformationEditViewModel kidsInformation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ArchanaSubscriptionHelper.UpdateSubscription(kidsInformation);
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("Invalid Data!!!", "Please try again.");
                return View(kidsInformation);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
                throw;
            }
        }

        //// GET: EmailSubscription/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ArchanaInformation ai = await db.ArchanaInformations.FindAsync(id);
        //    if (ai == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    KidsInformationIndexViewModel kidsinformation = new KidsInformationIndexViewModel { SID = ai.SID, Name = ai.KidName, ParentName = ai.ParentsName, DateOfBirth = ai.DateOfBirth, EmailAddress = ai.Email };
        //    return View(kidsinformation);
        //}

        //// POST: EmailSubscription/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    ArchanaInformation kidsInformation = await db.ArchanaInformations.FindAsync(id);
        //    db.ArchanaInformations.Remove(kidsInformation);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}
    }
}
