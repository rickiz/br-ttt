using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
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
                //var results = context.refmodels
                //    .Join(context.refbrands, a => a.BrandID, b => b.ID, (a, b) => new { Model = a, Brand = b })
                //    .Join(context.refcategories, a => a.Brand.CategoryID, b => b.ID, (a, b) => new { ModelBrand = a, Category = b })
                //    .Join(context.lnkmodelcolourdescs, a => a.ModelBrand.Model.ID, b => b.ModelID, (a, b) => new { ModelBrandCategory = a, ModelColourDesc = b })
                //    .Join(context.refcolourdescs, a => a.ModelColourDesc.ColourDescID, b => b.ID, (a, b) => new { ModelBrandCategoryColourDesc = a, ColourDesc = b })
                //    .GroupJoin(context.lnkmodeltrends, a=>a.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Model.ID, b=>b.ModelID,
                //            (a, b) => new { ModelBrandCategoryColourDescTrends = a, ModelTrend = b })
                //    .GroupJoin(context.lnkmodellifestyles, a => a.ModelBrandCategoryColourDescTrends.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Model.ID, b => b.ModelID,
                //            (a, b) => new { ModelBrandCategoryColourDescTrendLifestyle = a, ModelLifestyle = b })
                //    .Select(a => new
                //    {
                //        Model = a.ModelBrandCategoryColourDescTrendLifestyle.ModelBrandCategoryColourDescTrends.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Model,
                //        Brand = a.ModelBrandCategoryColourDescTrendLifestyle.ModelBrandCategoryColourDescTrends.ModelBrandCategoryColourDesc.ModelBrandCategory.ModelBrand.Brand,
                //        Category = a.ModelBrandCategoryColourDescTrendLifestyle.ModelBrandCategoryColourDescTrends.ModelBrandCategoryColourDesc.ModelBrandCategory.Category,
                //        ColourDesc = a.ModelBrandCategoryColourDescTrendLifestyle.ModelBrandCategoryColourDescTrends.ColourDesc,
                //        ModelColourDesc = a.ModelBrandCategoryColourDescTrendLifestyle.ModelBrandCategoryColourDescTrends.ModelBrandCategoryColourDesc.ModelColourDesc,
                //        ModelTrend = a.ModelBrandCategoryColourDescTrendLifestyle.ModelTrend,
                //        ModelLifestyle = a.ModelLifestyle
                //    })
                //    .Where(a => a.ModelColourDesc.ID == id)
                //    .ToList();

                var results = context.lnkmodelcolourdescs
                                     .Join(context.refmodels, a => a.ModelID, b => b.ID, (a, b) => new { ModelColourDesc = a, Model = b })
                                     .Join(context.refcolourdescs, a => a.ModelColourDesc.ColourDescID, b => b.ID,
                                            (a, b) => new { a.ModelColourDesc, a.Model, ColourDesc = b })
                                     .Join(context.refbrands, a => a.Model.BrandID, b => b.ID,
                                            (a, b) => new { a.ModelColourDesc, a.Model, a.ColourDesc, Brand = b })
                                     .Join(context.refcategories, a => a.Brand.CategoryID, b => b.ID,
                                            (a, b) => new { a.ModelColourDesc, a.Model, a.ColourDesc, a.Brand, Category = b })
                                     .GroupJoin(context.lnkmodeltrends, a => new { a.ModelColourDesc.ModelID, a.ModelColourDesc.ColourDescID }, 
                                            b => new {b.ModelID,b.ColourDescID },
                                            (a, b) => new { a.ModelColourDesc, a.Model, a.ColourDesc, a.Brand, a.Category, ModelTrends = b })
                                     .GroupJoin(context.lnkmodellifestyles, a => new { a.ModelColourDesc.ModelID, a.ModelColourDesc.ColourDescID },
                                            b => new { b.ModelID, b.ColourDescID },
                                            (a, b) => new { a.ModelColourDesc, a.Model, a.ColourDesc, a.Brand, a.Category, a.ModelTrends, ModelLifestyles = b })
                                     .Where(a => a.ModelColourDesc.ID == id)
                                     .ToList();

                var modelDetails = results.First();

                var trendIDs = results.Select(a => a.ModelTrends).First().Count() > 0 ?
                                        results.Select(a => a.ModelTrends).First().Where(b => b.Active).Select(c => c.TrendID).Distinct().ToList() :
                                        new List<int>();
                var lifestyleIDs = results.Select(a => a.ModelLifestyles).First().Count() > 0 ?
                                        results.Select(a => a.ModelLifestyles).First().Where(b => b.Active).Select(c => c.LifeStyleID).Distinct().ToList() :
                                    new List<int>();

                maintainViewModel = new MaintainModelViewModel()
                {
                    ModelColourDescID = id,
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
                    Active = modelDetails.Model.Active
                };
            }

            return View(maintainViewModel);

        }

        [HttpPost]
        public virtual ActionResult Edit(MaintainModelViewModel viewModel)
        {
            using (var trans = new TransactionScope())
            {
                using (var context = new TTTEntities())
                {
                    var model = context.refmodels.Where(a => a.ID == viewModel.ModelID).Single();
                    var modelColourDesc = context.lnkmodelcolourdescs.Where(a => a.ID == viewModel.ModelColourDescID).Single();
                    var brandID = context.refbrands.Where(a => a.Name == viewModel.BrandName && a.CategoryID == viewModel.CategoryID).Single().ID;

                    // Model details****************************************************
                    model.BrandID = brandID;
                    model.Price = viewModel.Price;
                    model.DiscountPrice = viewModel.DiscountPrice;
                    // *****************************************************************

                    // Model colour description details*****************************************
                    modelColourDesc.Active = viewModel.Active;
                    modelColourDesc.SKU = viewModel.SKU;
                    modelColourDesc.HeelHeight = viewModel.HeelHeight;
                    modelColourDesc.Style = viewModel.Style;
                    modelColourDesc.Description = viewModel.Description;
                    modelColourDesc.LiningSock = viewModel.LiningSock;
                    modelColourDesc.Sole = viewModel.Sole;
                    modelColourDesc.Make = viewModel.Make;
                    modelColourDesc.UpperMaterial = viewModel.UpperMaterial;
                    modelColourDesc.HeelDesc = viewModel.HeelDesc;
                    // ***************************************************************************

                    // Model lifestyle************************************************************
                    var selectedLifestyleIDs = viewModel.SelectedLifestyleIDText.Replace(",,",",").Split(',').Select(int.Parse).ToList();
                    var selectedLifeStyles = context.reflifestyles.Where(a => selectedLifestyleIDs.Contains(a.ID)).ToList();
                    var modelLifestyles = context.lnkmodellifestyles.Where(a => a.ModelID == viewModel.ModelID &&
                                                                           a.ColourDescID == viewModel.ColourDescID).ToList();
                    modelLifestyles.ForEach(a => { a.Active = false; a.UpdateDT = DateTime.Now; });

                    foreach (var lifeStyle in selectedLifeStyles)
                    {
                        var modelLifestyle = modelLifestyles.Where(a => a.LifeStyleID == lifeStyle.ID).SingleOrDefault();

                        if (modelLifestyle != null)
                        {
                            modelLifestyle.Active = true;
                            modelLifestyle.UpdateDT = DateTime.Now;
                        }
                        else
                        {
                            context.lnkmodellifestyles.Add(new lnkmodellifestyle()
                            {
                                 ColourDescID = viewModel.ColourDescID,
                                 ModelID = viewModel.ModelID,
                                 Active=true,
                                 CreateDT=DateTime.Now,
                                 LifeStyleID=lifeStyle.ID,
                            }); 
                        }
                    }
                    //********************************************************************************

                    // Model trend********************************************************************************
                    var selectedTrendIDs = viewModel.SelectedTrendIDText.Replace(",,", ",").Split(',').Select(int.Parse).ToList();
                    var selectedTrends = context.reftrends.Where(a => selectedTrendIDs.Contains(a.ID)).ToList();
                    var modelTrends = context.lnkmodeltrends.Where(a => a.ModelID == viewModel.ModelID &&
                                                                        a.ColourDescID == viewModel.ColourDescID).ToList();
                    modelTrends.ForEach(a => { a.Active = false; a.UpdateDT = DateTime.Now; });

                    foreach (var trend in selectedTrends)
                    {
                        var modelTrend = modelTrends.Where(a => a.TrendID == trend.ID).SingleOrDefault();

                        if (modelTrend != null)
                        {
                            modelTrend.Active = true;
                            modelTrend.UpdateDT = DateTime.Now;
                        }
                        else
                        {
                            context.lnkmodeltrends.Add(new lnkmodeltrend()
                            {
                                ColourDescID = viewModel.ColourDescID,
                                ModelID = viewModel.ModelID,
                                Active = true,
                                CreateDT = DateTime.Now,
                                TrendID = trend.ID,
                            });
                        }
                    }
                    // ************************************************************************************************

                    context.SaveChanges();
                }

                trans.Complete();
            }

            return RedirectToAction("Edit", new { id = viewModel.ModelColourDescID });
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
                modelSizeViewModel.ModelColourDescID = context.lnkmodelcolourdescs.Where(a => a.ModelID == modelID && a.ColourDescID == colourDescID).First().ID;
            }

            //35|5|2|4,8
            var beginSize = Convert.ToInt16((Properties.Settings.Default.InitialSize.Split(',')[0]).Split('|')[0]);
            var sizeRange = Convert.ToInt16((Properties.Settings.Default.InitialSize.Split(',')[1]));

            for (var i = beginSize; i <= beginSize + sizeRange; i++)
                modelSizeViewModel.AvailableSize.Add(i.ToString());

            return View(modelSizeViewModel);
        }

        [HttpPost]
        public virtual ActionResult Size(ModelSizeViewModel viewModel, FormCollection collection)
        {
            using (var trans = new TransactionScope())
            {
                using (var context = new TTTEntities())
                {
                    var modelSizes = context.lnkmodelsizes
                                           .Where(a => a.ModelID == viewModel.ModelID && a.ColourDescID == viewModel.ColourDescID)
                                           .ToList();

                    var beginSize = Convert.ToInt16((Properties.Settings.Default.InitialSize.Split(',')[0]).Split('|')[0]);
                    var sizeRange = Convert.ToInt16((Properties.Settings.Default.InitialSize.Split(',')[1]));

                    for (var i = beginSize; i <= beginSize + sizeRange; i++)
                    {
                        var modelSize = modelSizes.Where(a => a.Size == i.ToString()).SingleOrDefault();

                        if (modelSize == null)
                        {
                            context.lnkmodelsizes.Add(new lnkmodelsize()
                            {
                                ModelID = viewModel.ModelID,
                                ColourDescID = viewModel.ColourDescID,
                                Active = true,
                                CreateDT = DateTime.Now,
                                Size = i.ToString(),
                                Quantity = Convert.ToInt32(collection[string.Format("txt_{0}", i.ToString())])
                            });
                        }
                        else
                        {
                            modelSize.Quantity = Convert.ToInt32(collection[string.Format("txt_{0}", modelSize.Size)]);
                            modelSize.UpdateDT = DateTime.Now;
                        }
                    }

                    context.SaveChanges();
                }

                trans.Complete();
            }

            return RedirectToAction("Edit", new { id = viewModel.ModelColourDescID });
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

        public virtual ActionResult GetColourDescByColour(int colourID)
        {
            var selectListItem = new List<SelectListItem>();

            using (var context = new TTTEntities())
            {
                var colourDescs = context.refcolourdescs.Where(a => a.ColourID == colourID).ToList();

                selectListItem = colourDescs.Select(a =>
                new SelectListItem()
                {
                    Text = a.Name,
                    Value = a.ID.ToString()
                }).ToList();

                selectListItem.Insert(0, new SelectListItem() { Value = "", Text = "ACTUAL COLOUR", Selected = true });
            }

            return Json(selectListItem);
        }
    }
}
