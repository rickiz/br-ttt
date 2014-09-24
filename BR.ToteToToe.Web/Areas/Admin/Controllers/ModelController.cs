using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using BR.ToteToToe.Web.Areas.Admin.ViewModels;
using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Properties;

namespace BR.ToteToToe.Web.Areas.Admin.Controllers
{
    [Authorize]
    public partial class ModelController : Controller
    {
        //
        // GET: /Admin/Model/

        public virtual ActionResult Index()
        {
            return View(new ModelViewModel());
        }

        [HttpPost]
        public virtual ActionResult Index(ModelViewModel viewModel)
        {
            using (var context = new TTTEntities())
            {
                var query = context.refmodels
                    .Join(context.refbrands, a => a.BrandID, b => b.ID, (a, b) => 
                        new { Model = a, Brand = b })
                    .Join(context.refcategories, a => a.Brand.CategoryID, b => b.ID, (a, b) => 
                        new { a.Model, a.Brand, Category = b })
                    .Join(context.lnkmodelcolourdescs, a => a.Model.ID, b => b.ModelID, (a, b) => 
                        new { a.Model, a.Brand, a.Category, ModelColourDesc = b })
                    .Join(context.refcolourdescs, a => a.ModelColourDesc.ColourDescID, b => b.ID, (a, b) => 
                        new { a.Model,a.Brand,a.Category,a.ModelColourDesc, ColourDesc = b })
                    .Join(context.refcolours, a => a.ColourDesc.ColourID, b => b.ID, (a, b) =>
                        new { a.Model, a.Brand, a.Category, a.ModelColourDesc, a.ColourDesc, Colour = b })
                    .Take(50)
                    .OrderByDescending(a => a.Model.UpdateDT)
                    .ThenByDescending(a => a.Model.CreateDT)
                    .Select(a => new
                    {
                        ModelColourDescID = a.ModelColourDesc.ID,
                        ColourDescID = a.ColourDesc.ID,
                        ColourDescName = a.ColourDesc.Name,
                        ModelName = a.Model.Name,
                        ModelID = a.Model.ID,
                        BrandName = a.Brand.Name,
                        CategoryName = a.Category.Name,
                        CategoryID = a.Category.ID,
                        ColourName = a.Colour.Name,
                        ColourID = a.Colour.ID,
                        Active = a.Model.Active

                        //Model = a.Model,
                        //Brand = a.Brand,
                        //Category = a.Category,
                        //ColourDesc = a.ColourDesc,
                        //ModelColourDesc = a.ModelColourDesc
                    });

                if (viewModel.CategoryID != 0)
                    query = query = query.Where(a => a.CategoryID == viewModel.CategoryID);

                if (viewModel.BrandName!="0" && !string.IsNullOrEmpty(viewModel.BrandName))
                    query = query = query.Where(a => a.BrandName == viewModel.BrandName);

                if (viewModel.ColourID != 0)
                    query = query.Where(a => a.ColourID == viewModel.ColourID);

                if (viewModel.ColourDescID.HasValue)
                    query = query.Where(a => a.ColourDescID == viewModel.ColourDescID.Value || viewModel.ColourDescID.Value == 0);

                if (viewModel.ModelID != 0)
                  query =  query.Where(a => a.ModelID == viewModel.ModelID);

                if (viewModel.Active.HasValue)
                  query =  query.Where(a => a.Active == viewModel.Active.Value);

                var models = query.ToList();

                foreach (var model in models)
                {
                    viewModel.ModelDetails.Add(new ModelDetails()
                    {
                        ModelColourDescID = model.ModelColourDescID,
                        ColourDescID = model.ColourDescID,
                        ModelName = model.ModelName,
                        ModelID = model.ModelID,
                        BrandName = model.BrandName,
                        CategoryName = model.CategoryName,
                        ColourName = model.ColourName,
                        ColourDescName = model.ColourDescName,
                        Active = model.Active
                    });
                }
            }

            return View(viewModel);
        }

        public virtual ActionResult Create()
        {
            var maintainViewModel = new MaintainModelViewModel();

            using (var context = new TTTEntities())
            {
                maintainViewModel = new MaintainModelViewModel()
                {
                    SelectedTrendIDText = "0",
                    SelectedLifestyleIDText = "0",
                    AvailableLifestyles = context.reflifestyles.Where(a=>a.Active).ToList(),
                    AvailableTrends = context.reftrends.Where(a => a.Active).ToList(),
                };
            }

            return View(maintainViewModel);

        }

        [HttpPost]
        public virtual ActionResult Create(MaintainModelViewModel viewModel)
        {
            try
            {
                var modelColourDesc = new lnkmodelcolourdesc();

                using (var trans = new TransactionScope())
                {
                    using (var context = new TTTEntities())
                    {
                        var brandID = context.refbrands.Where(a => a.Name == viewModel.BrandName && a.CategoryID == viewModel.CategoryID).Single().ID;

                        // Model details**********************************************************************************

                        var model = new refmodel();
                        if (!string.IsNullOrEmpty(viewModel.NewModelName))
                        {
                            model = new refmodel()
                            {
                                BrandID = brandID,
                                Price = viewModel.Price,
                                DiscountPrice = viewModel.DiscountPrice,
                                CreateDT = DateTime.Now,
                                Active = true,
                                Gender = "Female",
                                Name = viewModel.NewModelName,
                                Description = "-"
                            };

                            context.refmodels.Add(model);
                            context.SaveChanges();
                        }
                        else
                        {
                            model = context.refmodels.Where(a => a.ID == viewModel.ModelID).Single();
                        }

                        //***************************************************************************************************

                        // Model colour description details******************************************************************

                        modelColourDesc = new lnkmodelcolourdesc()
                        {
                            ModelID = model.ID,
                            ColourDescID = viewModel.ColourDescID,
                            Active = viewModel.Active,
                            SKU = viewModel.SKU,
                            HeelHeight = viewModel.HeelHeight,
                            Style = viewModel.Style,
                            Description = viewModel.Description,
                            LiningSock = viewModel.LiningSock,
                            Sole = viewModel.Sole,
                            Make = viewModel.Make,
                            UpperMaterial = viewModel.UpperMaterial,
                            HeelDesc = viewModel.HeelDesc,
                            CreateDT = DateTime.Now
                        };

                        context.lnkmodelcolourdescs.Add(modelColourDesc);
                        context.SaveChanges();

                        // ****************************************************************************************************

                        // Model lifestyle************************************************************
                        var selectedLifestyleIDs = viewModel.SelectedLifestyleIDText.Replace(",,", ",").Split(',').Select(int.Parse).ToList();
                        var selectedLifeStyles = context.reflifestyles.Where(a => selectedLifestyleIDs.Contains(a.ID)).ToList();

                        foreach (var lifeStyle in selectedLifeStyles)
                        {
                            context.lnkmodellifestyles.Add(new lnkmodellifestyle()
                            {
                                ColourDescID = viewModel.ColourDescID,
                                ModelID = model.ID,
                                Active = true,
                                CreateDT = DateTime.Now,
                                LifeStyleID = lifeStyle.ID,
                            });
                        }
                        //********************************************************************************

                        // Model trend********************************************************************************
                        var selectedTrendIDs = viewModel.SelectedTrendIDText.Replace(",,", ",").Split(',').Select(int.Parse).ToList();
                        var selectedTrends = context.reftrends.Where(a => selectedTrendIDs.Contains(a.ID)).ToList();

                        foreach (var trend in selectedTrends)
                        {
                            context.lnkmodeltrends.Add(new lnkmodeltrend()
                            {
                                ColourDescID = viewModel.ColourDescID,
                                ModelID = model.ID,
                                Active = true,
                                CreateDT = DateTime.Now,
                                TrendID = trend.ID,
                            });
                        }
                        // ************************************************************************************************

                        context.SaveChanges();
                    }

                    trans.Complete();
                }

                return RedirectToAction("Edit", new { id = modelColourDesc.ID });
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                                            b => new { b.ModelID, b.ColourDescID },
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
                    SelectedTrendIDText = string.Format("0,{0}", string.Join(",", trendIDs)),
                    SelectedLifestyle = lifestyleIDs,
                    SelectedLifestyleIDText = string.Format("0,{0}", string.Join(",", lifestyleIDs)),
                    AvailableLifestyles = context.reflifestyles.Where(a => a.Active).ToList(),
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
                                                         .OrderBy(a=>a.ID)
                                                         .ToList();

                var modelColourDesc = context.lnkmodelcolourdescs
                                             .Where(a => a.ModelID == modelID && a.ColourDescID == colourDescID)
                                             .Single();

                modelImageViewModel.CategoryName = context.refcategories.Where(a => a.ID == categoryID).Single().Name;

                modelImageViewModel.ModelImages = context.lnkmodelimages.Where(a => a.ModelColourDescID == modelColourDesc.ID).ToList();

                modelImageViewModel.ModelID = modelColourDesc.ModelID;
                modelImageViewModel.ModelColourDescID = modelColourDesc.ID;
                modelImageViewModel.MainImage = string.Format("~/Images/{0}/{1}", modelImageViewModel.CategoryName, modelColourDesc.MainImage);
                modelImageViewModel.SubImage = string.Format("~/Images/{0}/{1}", modelImageViewModel.CategoryName, modelColourDesc.SubImage);
            }

            return View(modelImageViewModel);
        }

        [HttpPost]
        public virtual ActionResult UploadImage(ModelImageViewModel viewModel, HttpPostedFileBase fileMain, HttpPostedFileBase fileSub,
            HttpPostedFileBase fileThumbnail1, HttpPostedFileBase fileThumbnail2, HttpPostedFileBase fileThumbnail3, HttpPostedFileBase fileThumbnail4,
            HttpPostedFileBase fileImage1, HttpPostedFileBase fileImage2, HttpPostedFileBase fileImage3, HttpPostedFileBase fileImage4)
        {
            try
            {
                using (var context = new TTTEntities())
                {
                    var modelColourDesc = context.lnkmodelcolourdescs.Where(a => a.ID == viewModel.ModelColourDescID).Single();
                    var modelImages = context.lnkmodelimages.Where(a => a.ModelColourDescID == viewModel.ModelColourDescID).ToList();

                    if (fileMain != null)
                    {
                        SaveImage(viewModel, fileMain);
                        modelColourDesc.MainImage = Path.GetFileName(fileMain.FileName);
                    }

                    if (fileSub != null)
                    {
                        SaveImage(viewModel, fileSub);
                        modelColourDesc.SubImage = Path.GetFileName(fileSub.FileName);
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        var modelImage = new lnkmodelimage()
                        {
                            CreateDT = DateTime.Now,
                            Active = true,
                            ModelColourDescID = viewModel.ModelColourDescID,
                        };

                        //existing model images
                        if (modelImages.Count() > i)
                            modelImage = modelImages[i];

                        switch (i)
                        {
                            case 0:
                                UploadModelImage(viewModel, fileThumbnail1, fileImage1, modelImage);
                                break;
                            case 1:
                                UploadModelImage(viewModel, fileThumbnail2, fileImage2, modelImage);
                                break;
                            case 2:
                                UploadModelImage(viewModel, fileThumbnail3, fileImage3, modelImage);
                                break;
                            case 3:
                                UploadModelImage(viewModel, fileThumbnail4, fileImage4, modelImage);
                                break;
                        }

                        // new model images
                        if (modelImage.ID == 0)
                            context.lnkmodelimages.Add(modelImage);

                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Edit", new { id = viewModel.ModelColourDescID }); 
        }

        private void UploadModelImage(ModelImageViewModel viewModel, HttpPostedFileBase thumbnail, HttpPostedFileBase image, lnkmodelimage modelImage)
        {
            if (thumbnail != null)
            {
                SaveImage(viewModel, thumbnail);
                modelImage.Thumbnail = Path.GetFileName(thumbnail.FileName);
            }

            if (image != null)
            {
                SaveImage(viewModel, image);
                modelImage.Image = Path.GetFileName(image.FileName);
            }
        }

        private void SaveImage(ModelImageViewModel viewModel, HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
                return;

            var fileName = Path.GetFileName(file.FileName);
            var extension = Path.GetExtension(fileName).ToLower();
            var validImageExtensions = new List<string>() { ".jpg", ".png", ".gif", ".bmp" };

            if (!validImageExtensions.Contains(extension))
                throw new ApplicationException("Invalid Image extension.");

            var savePath = string.Format("~/Images/{0}/", viewModel.CategoryName);

            if (savePath.StartsWith("~"))
                savePath = Server.MapPath(savePath);
            else
                savePath = savePath;

            savePath = Path.Combine(savePath, fileName);

            file.SaveAs(savePath);
        }

        public virtual ActionResult GetColourDescByColour(int colourID, int colourDescID)
        {
            var selectListItem = new List<SelectListItem>();

            using (var context = new TTTEntities())
            {
                var colourDescs = context.refcolourdescs.Where(a => a.ColourID == colourID).ToList();

                selectListItem = colourDescs.Select(a =>
                new SelectListItem()
                {
                    Text = a.Name,
                    Value = a.ID.ToString(),
                    Selected = colourDescID == a.ID ? true : false
                }).ToList();

                selectListItem.Insert(0, new SelectListItem() { Value = "", Text = "ACTUAL COLOUR" });
            }

            return Json(selectListItem);
        }

        public virtual ActionResult GetBrandByCategory(int categoryID, string brandName)
        {
            var selectListItem = new List<SelectListItem>();

            using (var context = new TTTEntities())
            {
                var brandNames = context.refbrands.Where(a => a.CategoryID == categoryID).Select(a => a.Name).Distinct().ToList();

                selectListItem = brandNames.Select(a =>
                new SelectListItem()
                {
                    Text = a,
                    Value = a,
                    Selected = brandName == a ? true : false
                }).ToList();

                selectListItem.Insert(0, new SelectListItem() { Value = "", Text = "BRAND" });
            }

            return Json(selectListItem);
        }
    }
}
