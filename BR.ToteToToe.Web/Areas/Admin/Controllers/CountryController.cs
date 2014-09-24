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
    public partial class CountryController : TTTBaseController
    {
        #region Private Methods

        private List<BaseRefModel> Search(BaseRefSearchCriteria criteria)
        {
            var results = new List<BaseRefModel>();

            using (var context = new TTTEntities())
            {
                var query = context.refcountries.AsQueryable();

                if (criteria.ID.HasValue && criteria.ID > 0)
                    query = query.Where(a => a.ID == criteria.ID.Value);

                if (!string.IsNullOrEmpty(criteria.Name))
                    query = query.Where(a => a.Name.ToLower().Trim() == criteria.Name.ToLower().Trim());

                if (criteria.Active.HasValue)
                    query = query.Where(a => a.Active == criteria.Active.Value);

                query = query.OrderBy(a => a.Name);

                results =
                    query.Select(a => new BaseRefModel
                    {
                        ID = a.ID,
                        Name = a.Name,
                        Active = a.Active
                    }).ToList();
            }

            return results;
        }

        #endregion

        #region Index

        public virtual ActionResult Index()
        {
            ValidateIsAdmin();

            var viewModel = new BaseRefViewModel { ControllerName = "Country" };

            return View(MVC.Admin.Shared.Views.BaseRefIndex, viewModel);
        }

        [HttpPost]
        public virtual ActionResult Index(BaseRefViewModel viewModel)
        {
            ValidateIsAdmin();

            viewModel.SearchResults = Search(viewModel.Criteria);

            return View(MVC.Admin.Shared.Views.BaseRefIndex, viewModel);
        }

        #endregion

        #region Create

        public virtual ActionResult Create()
        {
            ValidateIsAdmin();

            var viewModel = new BaseRefCreateUpdateViewModel { ControllerName = "Country", Active = true };

            return View(MVC.Admin.Shared.Views.BaseRefCreate, viewModel);
        }

        [HttpPost]
        public virtual ActionResult Create(BaseRefCreateUpdateViewModel viewModel)
        {
            ValidateIsAdmin();

            using (var context = new TTTEntities())
            {
                var newRecord = new refcountry
                {
                    Active = viewModel.Active,
                    Name = viewModel.Name,
                    CreateDT = DateTime.Now
                };

                context.refcountries.Add(newRecord);
                context.SaveChanges();
            }

            return RedirectToAction(MVC.Admin.Country.Index());
        }

        #endregion

        #region Edit

        public virtual ActionResult Edit(int id)
        {
            ValidateIsAdmin();

            using (var context = new TTTEntities())
            {
                var record = context.refcountries.Single(a => a.ID == id);

                var viewModel = new BaseRefCreateUpdateViewModel
                {
                    ControllerName = "Country",
                    Active = record.Active,
                    ID = record.ID,
                    Name = record.Name
                };

                return View(MVC.Admin.Shared.Views.BaseRefEdit, viewModel);
            }

        }

        [HttpPost]
        public virtual ActionResult Edit(BaseRefCreateUpdateViewModel viewModel)
        {
            ValidateIsAdmin();

            using (var context = new TTTEntities())
            {
                var record = context.refcountries.Single(a => a.ID == viewModel.ID);

                record.Name = viewModel.Name;
                record.Active = viewModel.Active;

                context.SaveChanges();
            }

            return RedirectToAction(MVC.Admin.Country.Index());
        }

        #endregion

    }
}
