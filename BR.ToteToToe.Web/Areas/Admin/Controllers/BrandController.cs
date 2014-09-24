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
    public partial class BrandController : TTTBaseController
    {
        #region Private Methods

        private List<BrandModel> Search(BrandSearchCriteria criteria)
        {
            var results = new List<BrandModel>();

            using (var context = new TTTEntities())
            {
                var query = context.refbrands.AsQueryable();

                if (criteria.ID.HasValue && criteria.ID > 0)
                    query = query.Where(a => a.ID == criteria.ID.Value);

                if (criteria.CategoryID.HasValue && criteria.CategoryID > 0)
                    query = query.Where(a => a.CategoryID == criteria.CategoryID.Value);

                if (!string.IsNullOrEmpty(criteria.Name))
                    query = query.Where(a => a.Name.ToLower().Trim() == criteria.Name.ToLower().Trim());

                if (criteria.Active.HasValue)
                    query = query.Where(a => a.Active == criteria.Active.Value);

                query = query.OrderBy(a => a.Name);

                results =
                    query.Select(a => new BrandModel
                    {
                        ID = a.ID,
                        Name = a.Name,
                        Active = a.Active,
                        Category = a.refcategory.Name
                    }).ToList();
            }

            return results;
        }

        #endregion

        #region Index

        public virtual ActionResult Index()
        {
            ValidateIsAdmin();

            var viewModel = new BrandViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Index(BrandViewModel viewModel)
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

            var viewModel = new BrandModel { Active = true };

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Create(BrandModel viewModel)
        {
            ValidateIsAdmin();

            using (var context = new TTTEntities())
            {
                var newRecord = new refbrand
                {
                    Active = viewModel.Active,
                    Name = viewModel.Name,
                    CreateDT = DateTime.Now,
                    CategoryID = viewModel.CategoryID
                };

                context.refbrands.Add(newRecord);
                context.SaveChanges();
            }

            return RedirectToAction(MVC.Admin.Brand.Index());
        }

        #endregion

        #region Edit

        public virtual ActionResult Edit(int id)
        {
            ValidateIsAdmin();

            using (var context = new TTTEntities())
            {
                var record = context.refbrands.Single(a => a.ID == id);

                var viewModel = new BrandModel
                {
                    Active = record.Active,
                    ID = record.ID,
                    Name = record.Name,
                    CategoryID = record.CategoryID
                };

                return View(viewModel);
            }

        }

        [HttpPost]
        public virtual ActionResult Edit(BrandModel viewModel)
        {
            ValidateIsAdmin();

            using (var context = new TTTEntities())
            {
                var record = context.refbrands.Single(a => a.ID == viewModel.ID);

                record.Name = viewModel.Name;
                record.Active = viewModel.Active;
                record.CategoryID = viewModel.CategoryID;

                context.SaveChanges();
            }

            return RedirectToAction(MVC.Admin.Brand.Index());
        }

        #endregion
    }
}
