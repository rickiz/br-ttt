using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.ToteToToe.Web.Areas.Admin.ViewModels;
using BR.ToteToToe.Web.DataModels;

namespace BR.ToteToToe.Web.Areas.Admin.Controllers
{
    public partial class ModelController : Controller
    {
        //
        // GET: /Admin/Model/

        public virtual ActionResult Index()
        {
            var viewModel = new ModelViewModel();

            using (var context = new TTTEntities())
            {
                var models = context.refmodels
                    .Join(context.refbrands, a => a.BrandID, b => b.ID, (a, b) => new { Model = a, Brand = b })
                    .Join(context.refcategories, a => a.Brand.CategoryID, b => b.ID, (a, b) => new { ModelBrand = a, Category = b })
                    .Join(context.lnkmodelcolourdescs, a => a.ModelBrand.Model.ID, b => b.ModelID, (a, b) => new { ModelBrandCategory = a, ModelColourDesc = b })
                    .Join(context.refcolourdescs, a => a.ModelColourDesc.ColourDescID, b => b.ID, (a, b) => new { ModelBrandCategoryColourDesc = a, ColourDesc = b})
                    .Take(50)
                    .OrderByDescending(a => a.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Model.UpdateDT)
                    .ThenByDescending(a => a.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Model.CreateDT)
                    .Select(a => new
                    {
                        Model = a.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Model,
                        Brand = a.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Brand,
                        Category = a.ModelBrandCategoryColourDesc.ModelBrandCategory.Category,
                        ColourDesc = a.ColourDesc,
                        ModelColourDesc = a.ModelBrandCategoryColourDesc.ModelColourDesc
                    }).ToList();

                foreach (var model in models)
                {
                    viewModel.ModelDetails.Add(new ModelDetails()
                    {
                        ModelColourDescID = model.ModelColourDesc.ID,
                        ColourDescID = model.ColourDesc.ID,
                        ModelName = model.Model.Name,
                        ModelID = model.Model.ID,
                        BrandName = model.Brand.Name,
                        CategoryName = model.Category.Name,
                        ColourName = model.ColourDesc.Name,
                        Active = model.Model.Active
                    });
                }
            }

            return View(viewModel);
        }

        public virtual ActionResult Edit(int id)
        {
            var maintainViewModel = new MaintainModelViewModel();

            using (var context = new TTTEntities())
            {
                var results = context.refmodels
                    .Join(context.refbrands, a => a.BrandID, b => b.ID, (a, b) => new { Model = a, Brand = b })
                    .Join(context.refcategories, a => a.Brand.CategoryID, b => b.ID, (a, b) => new { ModelBrand = a, Category = b })
                    .Join(context.lnkmodelcolourdescs, a => a.ModelBrand.Model.ID, b => b.ModelID, (a, b) => new { ModelBrandCategory = a, ModelColourDesc = b })
                    .Join(context.refcolourdescs, a => a.ModelColourDesc.ColourDescID, b => b.ID, (a, b) => new { ModelBrandCategoryColourDesc = a, ColourDesc = b })
                    .Join(context.lnkmodeltrends, a=>a.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Model.ID, b=>b.ModelID,
                            (a, b) => new { ModelBrandCategoryColourDescTrends = a, ModelTrend = b })
                    .Join(context.lnkmodellifestyles, a => a.ModelBrandCategoryColourDescTrends.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Model.ID, b => b.ModelID,
                            (a, b) => new { ModelBrandCategoryColourDescTrendLifestyle = a, ModelLifestyle = b })
                    .Select(a => new
                    {
                        Model = a.ModelBrandCategoryColourDescTrendLifestyle.ModelBrandCategoryColourDescTrends.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Model,
                        Brand = a.ModelBrandCategoryColourDescTrendLifestyle.ModelBrandCategoryColourDescTrends.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Brand,
                        Category = a.ModelBrandCategoryColourDescTrendLifestyle.ModelBrandCategoryColourDescTrends.ModelBrandCategoryColourDesc.ModelBrandCategory.Category,
                        ColourDesc = a.ModelBrandCategoryColourDescTrendLifestyle.ModelBrandCategoryColourDescTrends.ColourDesc,
                        ModelColourDesc = a.ModelBrandCategoryColourDescTrendLifestyle.ModelBrandCategoryColourDescTrends.ModelBrandCategoryColourDesc.ModelColourDesc,
                        ModelTrend = a.ModelBrandCategoryColourDescTrendLifestyle.ModelTrend,
                        ModelLifestyle = a.ModelLifestyle
                    })
                    .Where(a => a.ModelColourDesc.ID == id)
                    .ToList();

                var modelDetails = results.First();

                maintainViewModel = new MaintainModelViewModel()
                {
                    BrandID = modelDetails.Brand.ID,
                    BrandName = modelDetails.Brand.Name,
                    CategoryID = modelDetails.Category.ID,
                    ModelID = modelDetails.Model.ID,
                    ModelName = modelDetails.Model.Name,
                    ColourDescID = modelDetails.ColourDesc.ID,
                    ColourID = modelDetails.ColourDesc.ColourID,
                    Description = modelDetails.ModelColourDesc.Description,
                    DiscountPrice = modelDetails.Model.DiscountPrice == null ? 0 : modelDetails.Model.DiscountPrice.Value,
                    Price = modelDetails.Model.Price,
                    HeelDesc = modelDetails.ModelColourDesc.HeelDesc,
                    Sole = modelDetails.ModelColourDesc.Sole,
                    LiningSock = modelDetails.ModelColourDesc.LiningSock,
                    Make = modelDetails.ModelColourDesc.Make,
                    Style = modelDetails.ModelColourDesc.Style,
                    UpperMaterial = modelDetails.ModelColourDesc.UpperMaterial,
                    HeelHeight = modelDetails.ModelColourDesc.HeelHeight,
                    SKU = modelDetails.ModelColourDesc.SKU,
                    SelectedTrend = results.Select(a=>a.ModelTrend.TrendID).ToList(),
                    SelectedLifestyle = results.Select(a=>a.ModelLifestyle.LifeStyleID).ToList(),
                    AvailableLifestyles = context.reflifestyles.Where(a=>a.Active).ToList(),
                    AvailableTrends = context.reftrends.Where(a => a.Active).ToList()
                };
            }

            return View(maintainViewModel);

        }
    }
}
