using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using BR.ToteToToe.Web.Areas.Admin.ViewModels;
using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Helpers;
using System.IO;
using BR.ToteToToe.Web.Properties;

namespace BR.ToteToToe.Web.Areas.Admin.Controllers
{
    [Authorize]
    public partial class CategoryController : TTTBaseController
    {
        #region Private Methods

        private List<CategoryModel> Search(CategorySearchCriteria criteria)
        {
            var results = new List<CategoryModel>();

            using (var context = new TTTEntities())
            {
                var query = context.refcategories.AsQueryable();

                if (criteria.ID.HasValue && criteria.ID > 0)
                    query = query.Where(a => a.ID == criteria.ID.Value);

                if (!string.IsNullOrEmpty(criteria.Name))
                    query = query.Where(a => a.Name.ToLower().Trim() == criteria.Name.ToLower().Trim());

                if (!string.IsNullOrEmpty(criteria.Type))
                    query = query.Where(a => a.Type == criteria.Type);

                if (!string.IsNullOrEmpty(criteria.Image))
                    query = query.Where(a => a.Name.ToLower().Trim().Contains(criteria.Image.ToLower().Trim()));

                if (criteria.Active.HasValue)
                    query = query.Where(a => a.Active == criteria.Active.Value);

                query = query.OrderBy(a => a.Name);

                results =
                    query.Select(a => new CategoryModel
                    {
                        ID = a.ID,
                        Name = a.Name,
                        Active = a.Active,
                        Type = a.Type,
                        Image = a.Image
                    }).ToList();
            }

            return results;
        }

        private void SaveImage(CategoryModel viewModel, HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
                return;

            var fileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(fileName).ToLower();
            var validImageExtensions = new List<string>() { ".jpg", ".png", ".gif", ".bmp" };

            if (!validImageExtensions.Contains(extension))
                throw new ApplicationException("Invalid Image extension.");

            var savePath = string.Format("~/Images/{0}/", viewModel.Name);
            savePath = Server.MapPath(savePath);

            Directory.CreateDirectory(savePath);

            savePath = Path.Combine(savePath, fileName);

            file.SaveAs(savePath);

            viewModel.Image = fileName;
        }

        #endregion

        #region Index

        public virtual ActionResult Index()
        {
            ValidateIsAdmin();

            var viewModel = new CategoryViewModel();

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Index(CategoryViewModel viewModel)
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

            var viewModel = new CategoryModel { Active = true };

            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult Create(CategoryModel viewModel, HttpPostedFileBase file)
        {
            ValidateIsAdmin();

            SaveImage(viewModel, file);

            using (var context = new TTTEntities())
            {
                var newRecord = new refcategory
                {
                    Active = viewModel.Active,
                    Name = viewModel.Name,
                    CreateDT = DateTime.Now,
                    Type = viewModel.Type,
                    Image = viewModel.Image
                };

                context.refcategories.Add(newRecord);
                context.SaveChanges();
            }

            return RedirectToAction(MVC.Admin.Category.Index());
        }

        #endregion

        #region Edit

        public virtual ActionResult Edit(int id)
        {
            ValidateIsAdmin();

            using (var context = new TTTEntities())
            {
                var record = context.refcategories.Single(a => a.ID == id);
                var imagePath = string.Format("~/Images/{0}/", record.Name);
                var imageUrl =
                    string.IsNullOrEmpty(record.Image) ? "" : Path.Combine(imagePath, record.Image);

                var viewModel = new CategoryModel
                {
                    Active = record.Active,
                    ID = record.ID,
                    Name = record.Name,
                    Type = record.Type,
                    Image = imageUrl
                };

                return View(viewModel);
            }

        }

        [HttpPost]
        public virtual ActionResult Edit(CategoryModel viewModel, HttpPostedFileBase file)
        {
            ValidateIsAdmin();

            SaveImage(viewModel, file);

            using (var context = new TTTEntities())
            {
                var record = context.refcategories.Single(a => a.ID == viewModel.ID);

                record.Name = viewModel.Name;
                record.Active = viewModel.Active;
                record.Type = viewModel.Type;

                if (!string.IsNullOrEmpty(viewModel.Image))
                    record.Image = viewModel.Image;

                context.SaveChanges();
            }

            return RedirectToAction(MVC.Admin.Category.Index());
        }

        #endregion
    }
}
