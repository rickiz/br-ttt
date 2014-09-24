using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using BR.ToteToToe.Web.Areas.Admin.ViewModels;
using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Helpers;

namespace BR.ToteToToe.Web.Areas.Admin.Controllers
{
    [Authorize]
    public partial class ColourDescController : TTTBaseController
    {
        #region Private Methods

        private List<ColourDescModel> Search(ColourDescSearchCriteria criteria)
        {
            var results = new List<ColourDescModel>();

            using (var context = new TTTEntities())
            {
                var query = context.refcolourdescs.AsQueryable();

                if (criteria.ID.HasValue && criteria.ID > 0)
                    query = query.Where(a => a.ID == criteria.ID.Value);

                if (criteria.ColourID.HasValue && criteria.ColourID > 0)
                    query = query.Where(a => a.ColourID == criteria.ColourID.Value);

                if (!string.IsNullOrEmpty(criteria.Name))
                    query = query.Where(a => a.Name.ToLower().Trim() == criteria.Name.ToLower().Trim());

                if (criteria.Active.HasValue)
                    query = query.Where(a => a.Active == criteria.Active.Value);

                query = query.OrderBy(a => a.Name);

                results =
                    query.Select(a => new ColourDescModel
                    {
                        ID = a.ID,
                        Name = a.Name,
                        Active = a.Active,
                        Colour = a.refcolour.Name
                    }).ToList();
            }

            return results;
        }

        #endregion

        #region Index

        public virtual ActionResult Index()
        {
            ValidateIsAdmin();

            var viewModel = new ColourDescViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Index(ColourDescViewModel viewModel)
        {
            ValidateIsAdmin();

            viewModel.SearchResults = Search(viewModel.Criteria);

            return View(viewModel);
        }

        #endregion

        #region Create

        public virtual ActionResult Create()
        {
            ValidateIsAdmin();

            var viewModel = new ColourDescModel { Active = true };

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Create(ColourDescModel viewModel)
        {
            ValidateIsAdmin();

            using (var context = new TTTEntities())
            {
                var newRecord = new refcolourdesc
                {
                    Active = viewModel.Active,
                    Name = viewModel.Name,
                    CreateDT = DateTime.Now,
                    ColourID = viewModel.ColourID
                };

                context.refcolourdescs.Add(newRecord);
                context.SaveChanges();
            }

            return RedirectToAction(MVC.Admin.ColourDesc.Index());
        }

        #endregion

        #region Edit

        public virtual ActionResult Edit(int id)
        {
            ValidateIsAdmin();

            using (var context = new TTTEntities())
            {
                var record = context.refcolourdescs.Single(a => a.ID == id);

                var viewModel = new ColourDescModel
                {
                    Active = record.Active,
                    ID = record.ID,
                    Name = record.Name,
                    ColourID = record.ColourID
                };

                return View(viewModel);
            }

        }

        [HttpPost]
        public virtual ActionResult Edit(ColourDescModel viewModel)
        {
            ValidateIsAdmin();

            using (var context = new TTTEntities())
            {
                var record = context.refcolourdescs.Single(a => a.ID == viewModel.ID);

                record.Name = viewModel.Name;
                record.Active = viewModel.Active;
                record.ColourID = viewModel.ColourID;

                context.SaveChanges();
            }

            return RedirectToAction(MVC.Admin.ColourDesc.Index());
        }

        #endregion
    }
}
