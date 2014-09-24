using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.ToteToToe.Web.Areas.Admin.ViewModels;
using BR.ToteToToe.Web.DataModels;

namespace BR.ToteToToe.Web.Areas.Admin.Controllers
{
    [Authorize]
    public partial class ColourPriorityController : Controller
    {
        //
        // GET: /Admin/ColourPriority/

        public virtual ActionResult Index()
        {
            var viewModel = new ColourPriorityViewModel();

            using (var context = new TTTEntities())
            {
                var results = context.refcolours
                                     .GroupJoin(context.lnkcolourpriorities, a => a.ID, b => b.ColourID, (a, b) => new { Colour = a, priority = b })
                                     .Where(a => a.Colour.Active)
                                     .Select(a => new
                                     {
                                         ColourID = a.Colour.ID,
                                         ColourName = a.Colour.Name,
                                         Priority = a.priority.FirstOrDefault()
                                     }).OrderBy(a => a.Priority.Sequence).ToList();

                foreach (var result in results)
                {
                    viewModel.Priorities.Add(new ColourPriorityDetails()
                    {
                        ColourID = result.ColourID,
                        ColourName = result.ColourName,
                        Priority = result.Priority == null ? "" : result.Priority.Sequence.ToString()
                    });
                }
            }

            viewModel.Priorities = viewModel.Priorities.OrderBy(a => string.IsNullOrWhiteSpace(a.Priority)).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Index(ColourPriorityViewModel viewModel, FormCollection collection)
        {
            using (var context = new TTTEntities())
            {
                var Colours = context.refcolours.Where(a => a.Active).ToList();

                //var ColourPriorities = context.lnkcolourpriorities.Where(a => a.ColourID).SingleOrDefault();

                foreach (var Colour in Colours)
                {
                    var ColourPriority = context.lnkcolourpriorities.Where(a => a.ColourID == Colour.ID).SingleOrDefault();

                    if (collection[string.Format("txt_{0}", Colour.ID)].ToString() != "")
                    {
                        if (ColourPriority != null)
                            ColourPriority.Sequence = Convert.ToInt16(collection[string.Format("txt_{0}", Colour.ID)]);
                        else
                            context.lnkcolourpriorities.Add(new lnkcolourpriority()
                            {
                                Active = true,
                                CreateDT = DateTime.Now,
                                ColourID = Colour.ID,
                                Sequence = Convert.ToInt16(collection[string.Format("txt_{0}", Colour.ID)])
                            });
                    }
                    else
                    {
                        if (ColourPriority != null)
                            context.lnkcolourpriorities.Remove(ColourPriority);
                    }
                }

                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
