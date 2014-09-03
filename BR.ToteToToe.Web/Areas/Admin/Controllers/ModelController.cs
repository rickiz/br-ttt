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
                    .GroupJoin(context.lnkmodeltrends, a=>a.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Model.ID, b=>b.ModelID,
                            (a, b) => new { ModelBrandCategoryColourDescTrends = a, ModelTrend = b })
                    .GroupJoin(context.lnkmodellifestyles, a => a.ModelBrandCategoryColourDescTrends.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Model.ID, b => b.ModelID,
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

                var trendIDs = results.Select(a => a.ModelTrend).Count() > 0 ? 
                                        results.Select(a => a.ModelTrend.Select(b=>b.TrendID)).First().Distinct().ToList() : 
                                        new List<int>();
                var lifestyleIDs = results.Select(a => a.ModelLifestyle).Count() > 0 ?
                                        results.Select(a => a.ModelLifestyle.Select(b => b.LifeStyleID)).First().Distinct().ToList() : 
                                    new List<int>();

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
                    SelectedTrend = trendIDs,
                    SelectedTrendIDText = string.Format("0,{0}",string.Join(",", trendIDs)),
                    SelectedLifestyle = lifestyleIDs,
                    SelectedLifestyleIDText = string.Format("0,{0}", string.Join(",", lifestyleIDs)),
                    AvailableLifestyles = context.reflifestyles.Where(a=>a.Active).ToList(),
                    AvailableTrends = context.reftrends.Where(a => a.Active).ToList(),
                };
            }

            return View(maintainViewModel);

        }

        public virtual ActionResult Size(int modelID, int colourDescID)
        {
            var modelSizeViewModel = new ModelSizeViewModel()
            {
                ModelID = modelID,
                ColourDescID = colourDescID
            };

            using (var context = new TTTEntities())
            {
                modelSizeViewModel.ModelSizes = context.lnkmodelsizes.Where(a => a.ModelID == modelID && a.ColourDescID == colourDescID).ToList();
            }

            //35|5|2|4,8
            var beginSize = Convert.ToInt16((Properties.Settings.Default.InitialSize.Split(',')[0]).Split('|')[0]);
            var sizeRange = Convert.ToInt16((Properties.Settings.Default.InitialSize.Split(',')[1]));

            for (var i = beginSize; i <= beginSize + sizeRange; i++)
                modelSizeViewModel.AvailableSize.Add(i.ToString());

            return View(modelSizeViewModel);
        }

        public virtual ActionResult UploadImage(int modelID, int colourDescID, int categoryID)
        {
            var modelImageViewModel = new ModelImageViewModel()
            {
                ModelID = modelID,
                ColourDescID = colourDescID
            };

            using (var context = new TTTEntities())
            {
                modelImageViewModel.ModelImages = context.lnkmodelimages
                                                         .Join(context.lnkmodelcolourdescs, a => a.ModelColourDescID, b => b.ID, 
                                                                    (a, b) => new { modelColourDesc = b, modelImage = a })
                                                         .Where(a => a.modelColourDesc.ModelID == modelID && a.modelColourDesc.ColourDescID == colourDescID)
                                                         .Select(a => a.modelImage)
                                                         .ToList();

                var modelColourDesc = context.lnkmodelcolourdescs
                                             .Where(a => a.ModelID == modelID && a.ColourDescID == colourDescID)
                                             .Single();

                modelImageViewModel.CategoryName = context.refcategories.Where(a => a.ID == categoryID).Single().Name;

                modelImageViewModel.ModelImages = context.lnkmodelimages.Where(a => a.ModelColourDescID == modelColourDesc.ID).ToList();

                modelImageViewModel.MainImage = string.Format("~/Images/{0}/{1}", modelImageViewModel.CategoryName, modelColourDesc.MainImage);
                modelImageViewModel.SubImage = string.Format("~/Images/{0}/{1}", modelImageViewModel.CategoryName, modelColourDesc.SubImage);
            }

            return View(modelImageViewModel);
        }
    }
}
