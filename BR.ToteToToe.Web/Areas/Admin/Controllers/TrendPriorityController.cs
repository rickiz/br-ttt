using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.ToteToToe.Web.Areas.Admin.ViewModels;
using BR.ToteToToe.Web.DataModels;

namespace BR.ToteToToe.Web.Areas.Admin.Controllers
{
    public partial class TrendPriorityController : Controller
    {
        //
        // GET: /Admin/TrendPriority/

        public virtual ActionResult Index()
        {
            var viewModel = new TrendPriorityViewModel();

            using (var context = new TTTEntities())
            {
                var results = context.reftrends
                                     .GroupJoin(context.lnktrendpriorities, a => a.ID, b => b.TrendID, (a, b) => new { trend = a, priority = b })
                                     .Where(a => a.trend.Active)
                                     .Select(a => new
                                     {
                                         TrendID = a.trend.ID,
                                         TrendName = a.trend.Name,
                                         Priority = a.priority.FirstOrDefault()
                                     }).OrderBy(a => a.Priority.Sequence).ToList();

                foreach (var result in results)
                {
                    viewModel.Priorities.Add(new PriorityDetails()
                    {
                        TrendID = result.TrendID,
                        TrendName = result.TrendName,
                        Priority = result.Priority == null ? "" : result.Priority.Sequence.ToString()
                    });
                }
            }

            viewModel.Priorities = viewModel.Priorities.OrderBy(a=> string.IsNullOrWhiteSpace(a.Priority)).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Index(TrendPriorityViewModel viewModel, FormCollection collection)
        {
            using (var context = new TTTEntities())
            {
                var trends = context.reftrends.Where(a => a.Active).ToList();

                //var trendPriorities = context.lnktrendpriorities.Where(a => a.TrendID).SingleOrDefault();

                foreach (var trend in trends)
                {
                    var trendPriority = context.lnktrendpriorities.Where(a => a.TrendID == trend.ID).SingleOrDefault();

                    if (collection[string.Format("txt_{0}", trend.ID)].ToString() != "")
                    {
                        if (trendPriority != null)
                            trendPriority.Sequence = Convert.ToInt16(collection[string.Format("txt_{0}", trend.ID)]);
                        else
                            context.lnktrendpriorities.Add(new lnktrendpriority()
                            {
                                Active = true,
                                CreateDT = DateTime.Now,
                                TrendID = trend.ID,
                                Sequence = Convert.ToInt16(collection[string.Format("txt_{0}", trend.ID)])
                            });
                    }
                    else
                    {
                        if (trendPriority != null)
                            context.lnktrendpriorities.Remove(trendPriority);
                    }
                }

                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
