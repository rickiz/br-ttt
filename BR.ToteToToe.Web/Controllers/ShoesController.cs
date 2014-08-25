using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Helpers;
using BR.ToteToToe.Web.ViewModels;
using System.IO;
using System.Transactions;
using System.Collections.Specialized;
using System.Web.Routing;

namespace BR.ToteToToe.Web.Controllers
{
    public partial class ShoesController : TTTBaseController
    {
        //
        // GET: /Shoes/
        
        private HttpCookie shoppingBagCookie;
        private string cookieID;

        public virtual ActionResult Index()
        {
            var categories = new List<refcategory>();
            var type = CategoryType.Shoes.ToString();

            using (var context = new TTTEntities())
            {
                categories = context.refcategories.Where(a => a.Active && a.Type == type).ToList();
            }

            return View(categories);
        }

        public virtual ActionResult List(int categoryID = 0)
        {
            var modelViewModel = new ModelListViewModel();

            modelViewModel.Models = ModelSearch(categoryID: categoryID);

            if (categoryID != 0)
            {
                if (modelViewModel.Models.Count >= 12)
                {
                    modelViewModel.Models12 = modelViewModel.Models.Take(12).ToList();
                    modelViewModel.Models.RemoveRange(0, 12);
                }
            }

            return View(modelViewModel);
        }

        public virtual ActionResult Pumps()
        {
            return View();
        }

        //public virtual ActionResult Details()
        //{
        //    return View();
        //}

        public virtual ActionResult Details(int modelID, int colourDescID, int modelSizeID = 0)
        {
            var viewModel = ConstructModelDetailsViewModel(modelID, colourDescID, modelSizeID);
            ConstructRecommendedShoes(viewModel);

            return View(viewModel);
        }
        private void ConstructRecommendedShoes(ModelDetailsViewModel viewModel)
        {
            var modelColourDescIDs = new List<int>();

            if (viewModel.CategoryName.ToLower().Replace(" ","").Contains("peeptoes"))
            {
                modelColourDescIDs = GetPeepToeRandom(viewModel.HeelHeight, viewModel.ModelID, viewModel.ColourDescID);
            }
            else if (viewModel.CategoryName.ToLower().Replace(" ", "").Contains("pump"))
            {
                modelColourDescIDs = GetPumpRandom(viewModel.HeelHeight, viewModel.ModelID, viewModel.ColourDescID);
            }
            else if (viewModel.CategoryName.ToLower().Replace(" ", "").Contains("sandal"))
            {
                modelColourDescIDs = GetSandalRandom(viewModel.HeelHeight, viewModel.ModelID, viewModel.ColourDescID);
            }
            else if (viewModel.CategoryName.ToLower().Replace(" ", "").Contains("wedge"))
            {
                modelColourDescIDs = GetWedgeRandom(viewModel.HeelHeight, viewModel.ModelID, viewModel.ColourDescID);
            }
            else if (viewModel.CategoryName.ToLower().Replace(" ", "").Contains("platform"))
            {
                modelColourDescIDs = GetPlatformRandom(viewModel.HeelHeight, viewModel.ModelID, viewModel.ColourDescID);
            }
            else if (viewModel.CategoryName.ToLower().Replace(" ", "").Contains("ballerina"))
            {
                modelColourDescIDs = GetBallerinaRandom(viewModel.ModelID, viewModel.ColourDescID);
            }
            else if (viewModel.CategoryName.ToLower().Replace(" ", "").Contains("flat"))
            {
                modelColourDescIDs = GetFlatRandom(viewModel.ModelID, viewModel.ColourDescID);
            }

            if (modelColourDescIDs.Count > 0)
            {
                using (var context = new TTTEntities())
                {
                    var query = from model in context.refmodels
                                join brand in context.refbrands on model.BrandID equals brand.ID
                                join category in context.refcategories on brand.CategoryID equals category.ID
                                join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                join modelImage in context.lnkmodelimages on modelColourDesc.ID equals modelImage.ModelColourDescID
                                join colourDesc in context.refcolourdescs on modelColourDesc.ColourDescID equals colourDesc.ID into cDesc
                                from colourDesc in cDesc.DefaultIfEmpty()
                                join colour in context.refcolours on colourDesc.ColourID equals colour.ID
                                join colourPriority in context.lnkcolourpriorities on colour.ID equals colourPriority.ColourID into cp
                                from actColourPriority in cp.DefaultIfEmpty()
                                join modelSize in context.lnkmodelsizes on new { modelID = model.ID, colourDescID = colourDesc.ID }
                                    equals new { modelID = modelSize.ModelID, colourDescID = modelSize.ColourDescID } into ms
                                from actModelSize in ms.DefaultIfEmpty()
                                select new
                                {
                                    ModelColourDescID = modelColourDesc.ID,
                                    ModelID = model.ID,
                                    ModelName = model.Name,
                                    ColourDescID = colourDesc.ID,
                                    ColourDescName = colourDesc.Name,
                                    BrandName = brand.Name,
                                    CategoryName = category.Name,
                                    Price = model.Price,
                                    DiscountPrice = model.DiscountPrice,
                                    MainImage = modelImage.Image
                                };

                    var results = query.Where(a => modelColourDescIDs.Contains(a.ModelColourDescID)).ToList();
                    var uniqueModelColourDescID = results.Select(a => a.ModelColourDescID).Distinct().ToList();

                    foreach (var id in uniqueModelColourDescID)
                    {
                        var result = results.Where(a => a.ModelColourDescID == id).First();

                        viewModel.ReommendedModels.Add(new ReommendedModel()
                        {
                            BrandName = result.BrandName,
                            CategoryName = result.CategoryName,
                            ColourDescID = result.ColourDescID,
                            ColourDescName = result.ColourDescName,
                            DiscountPrice = result.DiscountPrice.HasValue ? result.DiscountPrice.Value : 0,
                            ModelID = result.ModelID,
                            ModelName = result.ModelName,
                            Price = result.Price,
                            MainImage = result.MainImage
                        });
                    }

                    //foreach (var result in results)
                    //{
                    //    viewModel.ReommendedModels.Add(new ReommendedModel()
                    //    {
                    //        BrandName = result.BrandName,
                    //        CategoryName = result.CategoryName,
                    //        ColourDescID = result.ColourDescID,
                    //        ColourDescName = result.ColourDescName,
                    //        DiscountPrice = result.DiscountPrice.HasValue ? result.DiscountPrice.Value : 0,
                    //        ModelID = result.ModelID,
                    //        ModelName = result.ModelName,
                    //        Price = result.Price,
                    //        MainImage  =result.MainImage
                    //    });
                    //}
                }

                viewModel.ReommendedModels = viewModel.ReommendedModels.GroupBy(x => new
                {
                    x.BrandName,
                    x.CategoryName,
                    x.ColourDescID,
                    x.ColourDescName,
                    x.ModelID,
                    x.ModelName,
                    x.MainImage,
                    x.Price,
                    x.DiscountPrice
                }).Select(grp => grp.First()).ToList();
            }
        }
        private List<int> GetPeepToeRandom(decimal heelHeight, int modelID, int colourDescID)
        {
            var recommendModelColourDescIDs = new List<int>();
            var rnd = new Random();
            var randomNo = 0;
            var randomList = new List<int>();

            //2 pumps, 3 peep toes, same height as current
            using (var context = new TTTEntities())
            {
                // pumps*********************************************************************************************************
                var pumpsModelColourDescIDs = (from model in context.refmodels
                                               join brand in context.refbrands on model.BrandID equals brand.ID
                                               join category in context.refcategories on brand.CategoryID equals category.ID
                                               join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                               where category.Name.Contains("pump")
                                               && model.Active == true && modelColourDesc.HeelHeight == heelHeight 
                                               && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                               select modelColourDesc.ID).ToList();

                if (pumpsModelColourDescIDs.Count > 0)
                {
                    if (pumpsModelColourDescIDs.Count <= 3)
                    {
                        AssignRecommendedIDs(pumpsModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    randomNo = GetRandomNo(pumpsModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(pumpsModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(pumpsModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(pumpsModelColourDescIDs[randomNo]);

                    //randomNo = GetRandomNo(pumpsModelColourDescIDs.Count(), randomList);
                    //recommendModelColourDescIDs.Add(pumpsModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************

                //peep*********************************************************************************************************
                randomList = new List<int>();
                var peepModelColourDescIDs = (from model in context.refmodels
                                              join brand in context.refbrands on model.BrandID equals brand.ID
                                              join category in context.refcategories on brand.CategoryID equals category.ID
                                              join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                              where category.Name.Contains("peep")
                                                    && (modelColourDesc.ModelID != modelID || modelColourDesc.ColourDescID != colourDescID)
                                              && model.Active == true && modelColourDesc.HeelHeight == heelHeight 
                                              && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                              select modelColourDesc.ID).ToList();

                if (peepModelColourDescIDs.Count > 0)
                {
                    if (peepModelColourDescIDs.Count <= 2)
                    {
                        AssignRecommendedIDs(peepModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    rnd = new Random();
                    randomNo = GetRandomNo(peepModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(peepModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(peepModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(peepModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(peepModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(peepModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************
            }

            return recommendModelColourDescIDs;
        }
        private List<int> GetPumpRandom(decimal heelHeight, int modelID, int colourDescID)
        {
            var recommendModelColourDescIDs = new List<int>();
            var rnd = new Random();
            var randomNo = 0;
            var randomList = new List<int>();

            //3 pumps, 2 peep toes, same height as current
            using (var context = new TTTEntities())
            {
                // pumps*********************************************************************************************************
                var pumpsModelColourDescIDs = (from model in context.refmodels
                                               join brand in context.refbrands on model.BrandID equals brand.ID
                                               join category in context.refcategories on brand.CategoryID equals category.ID
                                               join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                               where category.Name.Contains("pump")
                                                    && (modelColourDesc.ModelID != modelID || modelColourDesc.ColourDescID != colourDescID)
                                               && model.Active == true && modelColourDesc.HeelHeight == heelHeight 
                                               && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                               select modelColourDesc.ID).ToList();

                if (pumpsModelColourDescIDs.Count > 0)
                {
                    if (pumpsModelColourDescIDs.Count <= 2)
                    {
                        AssignRecommendedIDs(pumpsModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    randomNo = GetRandomNo(pumpsModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(pumpsModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(pumpsModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(pumpsModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(pumpsModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(pumpsModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************

                //peep*********************************************************************************************************
                randomList = new List<int>();
                var peepModelColourDescIDs = (from model in context.refmodels
                                              join brand in context.refbrands on model.BrandID equals brand.ID
                                              join category in context.refcategories on brand.CategoryID equals category.ID
                                              join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                              where category.Name.Contains("peep")
                                              && model.Active == true && modelColourDesc.HeelHeight == heelHeight 
                                              && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                              select modelColourDesc.ID).ToList();

                if (peepModelColourDescIDs.Count > 0)
                {
                    if (peepModelColourDescIDs.Count <= 3)
                    {
                        AssignRecommendedIDs(peepModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    rnd = new Random();
                    randomNo = GetRandomNo(peepModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(peepModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(peepModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(peepModelColourDescIDs[randomNo]);

                    //randomNo = GetRandomNo(peepModelColourDescIDs.Count(), randomList);
                    //recommendModelColourDescIDs.Add(peepModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************
            }

            return recommendModelColourDescIDs;
        }
        private List<int> GetSandalRandom(decimal heelHeight, int modelID, int colourDescID)
        {
            var recommendModelColourDescIDs = new List<int>();
            var rnd = new Random();
            var randomNo = 0;
            var randomList = new List<int>();

            // 2 wedges, 2 sandal, 1 platform
            using (var context = new TTTEntities())
            {
                // wedges*********************************************************************************************************
                var wedgesModelColourDescIDs = (from model in context.refmodels
                                               join brand in context.refbrands on model.BrandID equals brand.ID
                                               join category in context.refcategories on brand.CategoryID equals category.ID
                                               join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                                where category.Name.Contains("wedge")
                                               && model.Active == true && modelColourDesc.HeelHeight == heelHeight
                                               && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                               select modelColourDesc.ID).ToList();

                if (wedgesModelColourDescIDs.Count > 0)
                {
                    if (wedgesModelColourDescIDs.Count <= 2)
                    {
                        AssignRecommendedIDs(wedgesModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    randomNo = GetRandomNo(wedgesModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(wedgesModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(wedgesModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(wedgesModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************

                // sandal*********************************************************************************************************
                randomList = new List<int>();
                var sandalsModelColourDescIDs = (from model in context.refmodels
                                               join brand in context.refbrands on model.BrandID equals brand.ID
                                               join category in context.refcategories on brand.CategoryID equals category.ID
                                               join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                                 where category.Name.Contains("sandal")
                                                     && (modelColourDesc.ModelID != modelID || modelColourDesc.ColourDescID != colourDescID)
                                               && model.Active == true && modelColourDesc.HeelHeight == heelHeight
                                               && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                               select modelColourDesc.ID).ToList();

                if (sandalsModelColourDescIDs.Count > 0)
                {
                    if (sandalsModelColourDescIDs.Count <= 2)
                    {
                        AssignRecommendedIDs(sandalsModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    randomNo = GetRandomNo(sandalsModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(sandalsModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(sandalsModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(sandalsModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************

                // platform*********************************************************************************************************
                randomList = new List<int>();
                var platformsModelColourDescIDs = (from model in context.refmodels
                                               join brand in context.refbrands on model.BrandID equals brand.ID
                                               join category in context.refcategories on brand.CategoryID equals category.ID
                                               join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                                   where category.Name.Contains("platform")
                                               && model.Active == true 
                                               && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                               select modelColourDesc.ID).ToList();

                if (platformsModelColourDescIDs.Count > 0)
                {
                    randomNo = GetRandomNo(platformsModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(platformsModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************
            }

            return recommendModelColourDescIDs;
        }
        private List<int> GetWedgeRandom(decimal heelHeight, int modelID, int colourDescID)
        {
            var recommendModelColourDescIDs = new List<int>();
            var rnd = new Random();
            var randomNo = 0;
            var randomList = new List<int>();

            // 2 wedges, 1 sandal, 2 platform
            using (var context = new TTTEntities())
            {
                // wedges*********************************************************************************************************
                var wedgesModelColourDescIDs = (from model in context.refmodels
                                                join brand in context.refbrands on model.BrandID equals brand.ID
                                                join category in context.refcategories on brand.CategoryID equals category.ID
                                                join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                                where category.Name.Contains("wedge")
                                                    && (modelColourDesc.ModelID != modelID || modelColourDesc.ColourDescID != colourDescID)
                                                && model.Active == true && modelColourDesc.HeelHeight == heelHeight
                                                && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                                select modelColourDesc.ID).ToList();

                if (wedgesModelColourDescIDs.Count > 0)
                {
                    if (wedgesModelColourDescIDs.Count <= 2)
                    {
                        AssignRecommendedIDs(wedgesModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    randomNo = GetRandomNo(wedgesModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(wedgesModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(wedgesModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(wedgesModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************

                // sandal*********************************************************************************************************
                randomList = new List<int>();
                var sandalsModelColourDescIDs = (from model in context.refmodels
                                                 join brand in context.refbrands on model.BrandID equals brand.ID
                                                 join category in context.refcategories on brand.CategoryID equals category.ID
                                                 join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                                 where category.Name.Contains("sandal")
                                               && model.Active == true
                                               && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                                 select modelColourDesc.ID).ToList();

                if (sandalsModelColourDescIDs.Count > 0)
                {
                    if (sandalsModelColourDescIDs.Count <= 1)
                    {
                        AssignRecommendedIDs(sandalsModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    randomNo = GetRandomNo(sandalsModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(sandalsModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************

                // platform*********************************************************************************************************
                randomList = new List<int>();
                var platformsModelColourDescIDs = (from model in context.refmodels
                                                   join brand in context.refbrands on model.BrandID equals brand.ID
                                                   join category in context.refcategories on brand.CategoryID equals category.ID
                                                   join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                                   where category.Name.Contains("platform")
                                               && model.Active == true && modelColourDesc.HeelHeight == heelHeight
                                               && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                                   select modelColourDesc.ID).ToList();

                if (platformsModelColourDescIDs.Count > 0)
                {
                    if (platformsModelColourDescIDs.Count <= 2)
                    {
                        AssignRecommendedIDs(platformsModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    randomNo = GetRandomNo(platformsModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(platformsModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(platformsModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(platformsModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************
            }

            return recommendModelColourDescIDs;
        }
        private List<int> GetPlatformRandom(decimal heelHeight, int modelID, int colourDescID)
        {
            var recommendModelColourDescIDs = new List<int>();
            var rnd = new Random();
            var randomNo = 0;
            var randomList = new List<int>();

            // 3 platform, 2 wedges, same height as current
            using (var context = new TTTEntities())
            {
                // platform*********************************************************************************************************
                var platformModelColourDescIDs = (from model in context.refmodels
                                               join brand in context.refbrands on model.BrandID equals brand.ID
                                               join category in context.refcategories on brand.CategoryID equals category.ID
                                               join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                               where category.Name.Contains("platform")
                                               && model.Active == true && modelColourDesc.HeelHeight == heelHeight 
                                               && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                               select modelColourDesc.ID).ToList();

                if (platformModelColourDescIDs.Count > 0)
                {
                    if (platformModelColourDescIDs.Count <= 3)
                    {
                        AssignRecommendedIDs(platformModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    randomNo = GetRandomNo(platformModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(platformModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(platformModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(platformModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(platformModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(platformModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************

                //wedge*********************************************************************************************************
                randomList = new List<int>();
                var wedgeModelColourDescIDs = (from model in context.refmodels
                                              join brand in context.refbrands on model.BrandID equals brand.ID
                                              join category in context.refcategories on brand.CategoryID equals category.ID
                                              join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                              where category.Name.Contains("wedge")
                                              && model.Active == true && modelColourDesc.HeelHeight == heelHeight 
                                              && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                              select modelColourDesc.ID).ToList();

                if (wedgeModelColourDescIDs.Count > 0)
                {
                    if (wedgeModelColourDescIDs.Count <= 2)
                    {
                        AssignRecommendedIDs(wedgeModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    rnd = new Random();
                    randomNo = GetRandomNo(wedgeModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(wedgeModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(wedgeModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(wedgeModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************
            }

            return recommendModelColourDescIDs;
        }
        private List<int> GetBallerinaRandom(int modelID, int colourDescID)
        {
            var recommendModelColourDescIDs = new List<int>();
            var rnd = new Random();
            var randomNo = 0;
            var randomList = new List<int>();

            // 3 ballerina, 2 flat
            using (var context = new TTTEntities())
            {
                // ballerina*********************************************************************************************************
                var ballerinasModelColourDescIDs = (from model in context.refmodels
                                               join brand in context.refbrands on model.BrandID equals brand.ID
                                               join category in context.refcategories on brand.CategoryID equals category.ID
                                               join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                                    where category.Name.Contains("ballerina")
                                                         && (modelColourDesc.ModelID != modelID || modelColourDesc.ColourDescID != colourDescID)
                                               && model.Active == true
                                               && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                               select modelColourDesc.ID).ToList();

                if (ballerinasModelColourDescIDs.Count > 0)
                {
                    if (ballerinasModelColourDescIDs.Count <= 3)
                    {
                        AssignRecommendedIDs(ballerinasModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    randomNo = GetRandomNo(ballerinasModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(ballerinasModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(ballerinasModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(ballerinasModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(ballerinasModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(ballerinasModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************

                //flat*********************************************************************************************************
                randomList = new List<int>();
                var flatModelColourDescIDs = (from model in context.refmodels
                                              join brand in context.refbrands on model.BrandID equals brand.ID
                                              join category in context.refcategories on brand.CategoryID equals category.ID
                                              join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                              where category.Name.Contains("flat")
                                              && model.Active == true
                                              && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                              select modelColourDesc.ID).ToList();

                if (flatModelColourDescIDs.Count > 0)
                {
                    if (flatModelColourDescIDs.Count <= 2)
                    {
                        AssignRecommendedIDs(flatModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    rnd = new Random();
                    randomNo = GetRandomNo(flatModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(flatModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(flatModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(flatModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************
            }

            return recommendModelColourDescIDs;
        }
        private List<int> GetFlatRandom(int modelID, int colourDescID)
        {
            var recommendModelColourDescIDs = new List<int>();
            var rnd = new Random();
            var randomNo = 0;
            var randomList = new List<int>();

            // 3 flat, 2 ballerinas
            using (var context = new TTTEntities())
            {
                // ballerina*********************************************************************************************************
                var ballerinasModelColourDescIDs = (from model in context.refmodels
                                                    join brand in context.refbrands on model.BrandID equals brand.ID
                                                    join category in context.refcategories on brand.CategoryID equals category.ID
                                                    join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                                    where category.Name.Contains("ballerina")
                                                    && model.Active == true 
                                                    && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                                    select modelColourDesc.ID).ToList();

                if (ballerinasModelColourDescIDs.Count > 0)
                {
                    if (ballerinasModelColourDescIDs.Count <= 2)
                    {
                        AssignRecommendedIDs(ballerinasModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    randomNo = GetRandomNo(ballerinasModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(ballerinasModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(ballerinasModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(ballerinasModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************

                //flat*********************************************************************************************************
                randomList = new List<int>();
                var flatModelColourDescIDs = (from model in context.refmodels
                                              join brand in context.refbrands on model.BrandID equals brand.ID
                                              join category in context.refcategories on brand.CategoryID equals category.ID
                                              join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                              where category.Name.Contains("flat")
                                                     && (modelColourDesc.ModelID != modelID || modelColourDesc.ColourDescID != colourDescID)
                                              && model.Active == true
                                              && (model.DiscountPrice == 0 || !model.DiscountPrice.HasValue)
                                              select modelColourDesc.ID).ToList();

                if (flatModelColourDescIDs.Count > 0)
                {
                    if (flatModelColourDescIDs.Count <= 3)
                    {
                        AssignRecommendedIDs(flatModelColourDescIDs, recommendModelColourDescIDs);
                        return recommendModelColourDescIDs;
                    }

                    rnd = new Random();
                    randomNo = GetRandomNo(flatModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(flatModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(flatModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(flatModelColourDescIDs[randomNo]);

                    randomNo = GetRandomNo(flatModelColourDescIDs.Count(), randomList);
                    recommendModelColourDescIDs.Add(flatModelColourDescIDs[randomNo]);
                }
                //*********************************************************************************************************
            }

            return recommendModelColourDescIDs;
        }
        private int GetRandomNo(int range, List<int> randomList)
        {
            //if (randomList.Count() == 0)
            //    return 0;

            var rnd = new Random();
            var randomNo = 0;

            while (randomNo == 0 || randomList.Contains(randomNo))
            {
                randomNo = rnd.Next(1, range);
            }

            randomList.Add(randomNo);

            return randomNo;
        }
        private void AssignRecommendedIDs(List<int> ids, List<int> recommendedIDs)
        {
            foreach (var id in ids)
            {
                recommendedIDs.Add(id);
            }
        }

        private ModelDetailsViewModel ConstructModelDetailsViewModel(int modelID, int colourDescID, int modelSizeID = 0)
        {
            var viewModel = new ModelDetailsViewModel();

            using (var context = new TTTEntities())
            {
                var query = from model in context.refmodels
                            join brand in context.refbrands on model.BrandID equals brand.ID
                            join category in context.refcategories on brand.CategoryID equals category.ID
                            join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                            join modelImages in context.lnkmodelimages on modelColourDesc.ID equals modelImages.ModelColourDescID
                            select new
                            {
                                model,
                                brand,
                                category,
                                modelImages,
                                modelColourDesc
                            };

                var results = query.Where(a => a.modelColourDesc.ModelID == modelID && a.modelColourDesc.ColourDescID == colourDescID).Distinct().ToList();

                if (results.Count > 0)
                {
                    viewModel = new ModelDetailsViewModel()
                    {
                        //ModelID = results.Select(a => a.model.ID).First(),
                        ModelID = modelID,
                        ModelName = results.Select(a => a.model.Name).First(),
                        BrandName = results.Select(a => a.brand.Name).First(),
                        CategoryName = results.Select(a => a.category.Name).First(),
                        Price = results.Select(a => a.model.Price).First(),
                        DiscountPrice = results.Select(a => a.model.DiscountPrice).FirstOrDefault(),
                        Description = results.Select(a => a.modelColourDesc.Description).First(),
                        ModelImages = results.Select(a => a.modelImages).ToList(),
                        HeelHeight = results.Select(a => a.modelColourDesc.HeelHeight).First(),
                        //ModelColourDescID = results.Select(a => a.modelColourDesc.ColourDescID).First()
                        ColourDescID = colourDescID,
                        SKU = results.Select(a => a.modelColourDesc.SKU).First(),
                        ModelSizeID = modelSizeID,
                        ModelMainImage = results.Select(a => a.modelImages.Thumbnail).First(),
                        ShareUrl = string.Format("http://www.tote-to-toe.com/Shoes/Details?modelID={0}&colourDescID={1}", modelID, colourDescID),
                        Sole = results.Select(a=>a.modelColourDesc.Sole).First(),
                        Make = results.Select(a => a.modelColourDesc.Make).First(),
                        UpperMaterial = results.Select(a => a.modelColourDesc.UpperMaterial).First(),
                        LiningSock = results.Select(a => a.modelColourDesc.LiningSock).First(),
                        Style = results.Select(a => a.modelColourDesc.Style).First(),
                        HeelDesc = results.Select(a => a.modelColourDesc.HeelDesc).First()
                    };
                }

                ConstructRecentViewCookie(string.Format("{0}/{1}", viewModel.CategoryName, viewModel.ModelMainImage),
                                        string.Format("/Shoes/Details?modelID={0}&colourDescID={1}", modelID, colourDescID));

            }

            return viewModel;
        }

        [HttpPost]
        public virtual ActionResult Details(ModelDetailsViewModel viewModel, FormCollection collection)
        {
            try
            {
                if (collection["submit"].ToString() == "+ ADD TO BAG")
                {
                    ConstructShoppingBagCookie();

                    #region Add to Cart

                    var salesOrder = new trnsalesorder();

                    using (var context = new TTTEntities())
                    {
                        var query = from modelSize in context.lnkmodelsizes
                                    join model in context.refmodels on modelSize.ModelID equals model.ID
                                    select new { ModelSizeID = modelSize.ID, Model = model };

                        var modelDetails = query.Where(a => a.ModelSizeID == viewModel.ModelSizeID).Single();

                        var discountPrice = modelDetails.Model.DiscountPrice.HasValue ? modelDetails.Model.DiscountPrice.Value : 0;

                        var modelPrice = discountPrice != 0 ? modelDetails.Model.DiscountPrice.Value : modelDetails.Model.Price;

                        var customerEmail = Util.SessionAccess == null ? "" : Util.SessionAccess.Email; ;

                        if (!string.IsNullOrEmpty(customerEmail)) // login customer
                        {
                            var openStatusID = Util.GetStatusID(Status.Open.ToString());

                            salesOrder = context.trnsalesorders.Where(a => a.Email == customerEmail && a.StatusID == openStatusID).SingleOrDefault();

                            using (var trans = new TransactionScope())
                            {
                                if (salesOrder == null)
                                {
                                    salesOrder = context.trnsalesorders.Add(new trnsalesorder()
                                    {
                                        StatusID = openStatusID,
                                        Email = customerEmail,
                                        SubTotal = 0,
                                        GrandTotal = modelPrice,
                                        CreateDT = DateTime.Now
                                    });
                                    context.SaveChanges();

                                    AddSalesOrderItem(salesOrder.ID, viewModel.ModelSizeID, viewModel.SKU, modelPrice, context);
                                }
                                else
                                {
                                    // update sales order grand total
                                    salesOrder.GrandTotal += modelPrice;

                                    var soItem = context.lnksalesorders.Where(a => a.SalesOrderID == salesOrder.ID &&
                                                                                    a.ModelSizeID == viewModel.ModelSizeID).SingleOrDefault();
                                    if (soItem == null)
                                    {
                                        salesOrder.UpdateDT = DateTime.Now;

                                        AddSalesOrderItem(salesOrder.ID, viewModel.ModelSizeID, viewModel.SKU, modelPrice, context);
                                    }
                                    else
                                    {
                                        soItem.Active = true;
                                        soItem.UpdateDT = DateTime.Now;
                                    }
                                }

                                context.SaveChanges();
                                trans.Complete();
                            }
                        }
                        else
                        {
                            ConstructShoppingBagCookie();

                            var soItem = context.lnksalesorders.Where(a => a.SalesOrderID == null &&
                                                                            a.ModelSizeID == viewModel.ModelSizeID &&
                                                                            a.CookieID == cookieID).SingleOrDefault();
                            if (soItem == null)
                            {
                                AddSalesOrderItem(null, viewModel.ModelSizeID, viewModel.SKU, modelPrice, context);
                            }
                            else
                            {
                                soItem.Active = true;
                                soItem.UpdateDT = DateTime.Now;
                            }

                            context.SaveChanges();
                        }
                    }

                    #endregion
                }
                else if (collection["submit"].ToString() == "+ ADD TO WISH LIST")
                {
                    #region Add to Wishlist

                    if (!Request.IsAuthenticated)
                    {
                        return RedirectToAction(MVC.SignIn.Index(Url.Action("Details", new { modelID = viewModel.ModelID, colourDescID = viewModel.ColourDescID })));
                    }

                    AddToWishList(viewModel);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return RedirectToAction("Details", new { modelID = viewModel.ModelID, colourDescID = viewModel.ColourDescID });
        }

        [Authorize]
        private void AddToWishList(ModelDetailsViewModel viewModel)
        {
            var wishlistID = 0;

            using (var context = new TTTEntities())
            {
                using (var trans = new TransactionScope())
                {
                    var customerEmail = Util.SessionAccess.Email;
                    var wishlist = context.trnwishlists.Where(a => a.Email == customerEmail).SingleOrDefault();
                    if (wishlist == null)
                    {
                        wishlistID = context.trnwishlists.Add(new trnwishlist()
                        {
                            Active = true,
                            CreateDT = DateTime.Now,
                            Email = customerEmail
                        }).ID;

                        AddWishlistItem(wishlistID, viewModel.ModelSizeID, viewModel.ColourDescID, viewModel.SKU, context);
                    }
                    else
                    {
                        wishlistID = wishlist.ID;

                        var wishlistItem = context.lnkwishlists.Where(a => a.ColourDescID == viewModel.ColourDescID &&
                                                        a.ModelSizeID == viewModel.ModelSizeID &&
                                                        a.WishlistID == wishlistID).SingleOrDefault();

                        if (wishlistItem == null)
                            AddWishlistItem(wishlistID, viewModel.ModelSizeID, viewModel.ColourDescID, viewModel.SKU, context);
                        else
                        {
                            wishlistItem.Active = true;
                            wishlistItem.UpdateDT = DateTime.Now;
                        }
                    }

                    context.SaveChanges();
                    trans.Complete();
                }
            }
        }
        private void AddWishlistItem(int wishlistID, int modelSizeID, int modelColourDescID, string sku, TTTEntities context)
        {
            context.lnkwishlists.Add(new lnkwishlist()
            {
                WishlistID = wishlistID,
                ModelSizeID = modelSizeID,
                ColourDescID = modelColourDescID,
                SKU = sku,
                Active = true,
                CreateDT = DateTime.Now,
            });
        }
        private void AddSalesOrderItem(int? salesOrderID, int modelSizeID, string sku, decimal unitPrice, TTTEntities context)
        {
            context.lnksalesorders.Add(new lnksalesorder()
            {
                CreateDT = DateTime.Now,
                Active = true,
                ModelSizeID = modelSizeID,
                Quantity = 1,
                SalesOrderID = salesOrderID,
                SKU = sku,
                UnitPrice = unitPrice,
                CookieID = salesOrderID.HasValue ? null : cookieID
            });
        }

        [HttpPost]
        public virtual JsonResult Filter(int categoryID, int trendID, int lifestyleID, string brandName, string size, int colourID, string price,
            string heelHeight)
        {
            var modelViewModel = ConstructModelViewModel(categoryID,
                                    ModelSearch(categoryID, trendID, lifestyleID, brandName, size, colourID, price, heelHeight));
            modelViewModel.CategoryID = categoryID;
            modelViewModel.TrendID = trendID;
            modelViewModel.LifestyleID = lifestyleID;
            modelViewModel.BrandName = brandName;
            modelViewModel.Size = Convert.ToInt32(size);
            modelViewModel.ColourID = colourID;
            modelViewModel.HeelHeight = heelHeight;

            //return PartialView("_ListItemParent", modelViewModel);
            var stringView = RenderRazorViewToString("_ListItemParent", modelViewModel);
            return Json(stringView , JsonRequestBehavior.AllowGet);
        }
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        private List<ModelResult> ModelSearch(int categoryID = 0, int trendID = 0, int lifestyleID = 0, string brandName = "", string size = "0",
            int colourID = 0, string price = "", string heelHeight = "0")
        {
            var modelResults = new List<ModelResult>();
            var hasFilter = false;

            using (var context = new TTTEntities())
            {
                var query = from t in
                                (
                                    from model in context.refmodels
                                    join brand in context.refbrands on model.BrandID equals brand.ID
                                    join category in context.refcategories on brand.CategoryID equals category.ID
                                    join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                    join colourDesc in context.refcolourdescs on modelColourDesc.ColourDescID equals colourDesc.ID into cDesc
                                    from colourDesc in cDesc.DefaultIfEmpty()
                                    join colour in context.refcolours on colourDesc.ColourID equals colour.ID
                                    join colourPriority in context.lnkcolourpriorities on colour.ID equals colourPriority.ColourID into cp
                                        from actColourPriority in cp.DefaultIfEmpty()
                                    join modelSize in context.lnkmodelsizes on new { modelID = model.ID, colourDescID = colourDesc.ID }
                                        equals new { modelID = modelSize.ModelID, colourDescID = modelSize.ColourDescID } into ms
                                        from actModelSize in ms.DefaultIfEmpty()
                                    join modelTrend in context.lnkmodeltrends on new { modelID = modelColourDesc.ModelID, colourDescID = modelColourDesc.ColourDescID }
                                        equals new { modelID = modelTrend.ModelID, colourDescID = modelTrend.ColourDescID } into mt
                                        from actModelTrend in mt.DefaultIfEmpty()
                                    join trendPriority in context.lnktrendpriorities on actModelTrend.TrendID equals trendPriority.TrendID into tp
                                        from actTrendPriority in tp.DefaultIfEmpty()
                                    join modelLifestyle in context.lnkmodellifestyles on new { modelID = modelColourDesc.ModelID, colourDescID = modelColourDesc.ColourDescID }
                                        equals new { modelID = modelLifestyle.ModelID, colourDescID = modelLifestyle.ColourDescID } into ml
                                        from actModelLifeStyle in ml.DefaultIfEmpty()
                                    where (actModelSize.Size == size || size == "0")
                                    select new
                                    {
                                        ModelID = model.ID,
                                        ModelName = model.Name,
                                        ModelMainImage = modelColourDesc == null ? model.MainImage : modelColourDesc.MainImage,
                                        ModelSubImage = modelColourDesc == null ? model.SubImage : modelColourDesc.SubImage,
                                        ModelPrice = model.Price,
                                        ModelDiscountPrice = model.DiscountPrice == null ? 0 : model.DiscountPrice,
                                        BrandID = brand.ID,
                                        BrandName = brand.Name,
                                        CategoryID = category.ID,
                                        CategoryName = category.Name,
                                        CategoryImage = category.Image,
                                        ColourID = colour.ID,
                                        ColourName = colour.Name,
                                        ColourDescID = colourDesc == null ? 0 : colourDesc.ID,
                                        ColourDescName = colourDesc == null ? "" : colourDesc.Name,
                                        Quantity = actModelSize == null ? 0 : actModelSize.Quantity,
                                        HeelHeight = modelColourDesc.HeelHeight,
                                        TrendPriority = actTrendPriority == null ? 9999999 : actTrendPriority.Sequence,
                                        ColourPriority = actColourPriority == null ? 9999999 : actColourPriority.Sequence,
                                        TrendID = actTrendPriority == null ? 0 : actTrendPriority.TrendID,
                                        LifeStyleID = actModelLifeStyle == null ? 0 : actModelLifeStyle.LifeStyleID
                                    }
                                )
                            group t by new
                            {
                                t.ModelID,
                                t.ModelName,
                                t.ModelMainImage,
                                t.ModelSubImage,
                                t.ModelPrice,
                                t.ModelDiscountPrice,
                                t.BrandID,
                                t.BrandName,
                                t.CategoryID,
                                t.CategoryName,
                                t.CategoryImage,
                                t.ColourID,
                                t.ColourName,
                                t.ColourDescID,
                                t.ColourDescName,
                                t.HeelHeight,
                                t.TrendPriority,
                                t.ColourPriority,
                                t.TrendID,
                                t.LifeStyleID
                            } into g
                            select new
                            {
                                g.Key.ModelID,
                                g.Key.ModelName,
                                g.Key.ModelMainImage,
                                g.Key.ModelSubImage,
                                g.Key.ModelPrice,
                                g.Key.ModelDiscountPrice,
                                g.Key.BrandID,
                                g.Key.BrandName,
                                g.Key.CategoryID,
                                g.Key.CategoryName,
                                g.Key.CategoryImage,
                                g.Key.ColourID,
                                g.Key.ColourName,
                                g.Key.ColourDescID,
                                g.Key.ColourDescName,
                                Quantity = g.Sum(k => k.Quantity),
                                g.Key.HeelHeight,
                                g.Key.TrendPriority,
                                g.Key.ColourPriority,
                                g.Key.TrendID,
                                g.Key.LifeStyleID
                            };

                if (categoryID > 0)
                {
                    query = query.Where(a => a.CategoryID == categoryID);
                    hasFilter = true;
                }

                if (trendID > 0)
                {
                    query = query.Where(a => a.TrendID == trendID);
                    hasFilter = true;
                }

                if (lifestyleID > 0){
                    query = query.Where(a => a.LifeStyleID == lifestyleID);
                    hasFilter = true;
                }

                if (!string.IsNullOrEmpty(brandName)){
                    query = query.Where(a => a.BrandName == brandName);
                    hasFilter = true;
                }

                if (colourID > 0)
                {
                    query = query.Where(a => a.ColourID == colourID);
                    hasFilter = true;
                }

                if (heelHeight != "0")
                {
                    var minMax = heelHeight.Split(',');
                    var min = Convert.ToDecimal(minMax[0]);
                    var max = Convert.ToDecimal(minMax[1]);

                    if(max==0)
                        query = query.Where(a => a.HeelHeight >= min);
                    else
                        query = query.Where(a => a.HeelHeight >= min && a.HeelHeight <= max);

                    //if (heelHeight == 4)
                    //    query = query.Where(a => a.HeelHeight >= heelHeight - 1);
                    //else
                    //    query = query.Where(a => a.HeelHeight >= heelHeight - 1 && a.HeelHeight <= heelHeight);

                    hasFilter = true;
                }

                if(price!="")
                    hasFilter = true;

                var results = query.Distinct().ToList();

                if (hasFilter)
                {
                    if(price=="1")
                        results = results.OrderBy(a => a.ModelPrice).ToList();
                    else if (price == "2")
                        results = results.OrderByDescending(a => a.ModelPrice).ToList();
                    else
                        results = results.OrderByDescending(a => a.Quantity).ToList();
                }
                else
                    results = results.OrderBy(a => a.ColourPriority).ThenBy(a => a.TrendPriority).ToList();

                foreach (var result in results)
                {
                    modelResults.Add(new ModelResult
                    {
                         BrandID = result.BrandID,
                         BrandName = result.BrandName,
                         CategoryID = result.CategoryID,
                         CategoryName = result.CategoryName,
                         CategoryImage = result.CategoryImage,
                         ColourID = result.ColourID,
                         ColourName = result.ColourName,
                         ColourDescID = result.ColourDescID,
                         ColourDescName = result.ColourDescName,
                         ModelID = result.ModelID,
                         ModelName = result.ModelName,
                         ModelMainImage = result.ModelMainImage,
                         ModelSubImage = result.ModelSubImage,
                         Price = result.ModelPrice,
                         DiscountPrice = result.ModelDiscountPrice.Value
                    });
                }

                //var finalModels = modelResults.Distinct().ToList();
                return modelResults.GroupBy(x => new
                {
                            x.BrandID,
                            x.BrandName,
                            x.CategoryID,
                            x.CategoryName,
                            x.CategoryImage,
                            x.ColourID,
                            x.ColourName,
                            x.ColourDescID,
                            x.ColourDescName,
                            x.ModelID,
                            x.ModelName,
                            x.ModelMainImage,
                            x.ModelSubImage,
                            x.Price,
                            x.DiscountPrice 
                        }).Select(grp=>grp.First()).ToList();
            }
        }

        private ModelListViewModel ConstructModelViewModel(int categoryID, List<ModelResult> results)
        {
            var modelViewModel = new ModelListViewModel()
            {
                 Models = results,
            };

            if (categoryID > 0)
            {
                if (modelViewModel.Models.Count >= 12)
                {
                    modelViewModel.Models12 = modelViewModel.Models.Take(12).ToList();
                    modelViewModel.Models.RemoveRange(0, 12);
                }
            }

            return modelViewModel;
        }

        [HttpPost]
        public virtual JsonResult GetModelSizeByModel(string modelID, string colourDescID, string modelSizeID)
        {
            var stringView = RenderRazorViewToString("_ModelSizeDDL", 
                                                        new ModelDetailsViewModel { ModelID = Convert.ToInt16(modelID), 
                                                                                    ColourDescID = Convert.ToInt16(colourDescID),
                                                                                    ModelSizeID = Convert.ToInt16(modelSizeID)});

            return Json(stringView, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual JsonResult GetQtyByModelSize(string modelSizeID)
        {
            var result = "";
            var modelSize = new lnkmodelsize();
            var mSizeID = string.IsNullOrEmpty(modelSizeID) ? 0 : Convert.ToInt32(modelSizeID);

            using (var context = new TTTEntities())
            {
                modelSize = context.lnkmodelsizes.Where(a => a.ID == mSizeID).SingleOrDefault();

                if (modelSize == null)
                    modelSize = new lnkmodelsize();

                if (modelSize.Quantity >= 5)
                    result = string.Format("{0} pairs left", modelSize.Quantity);
                else if (modelSize.Quantity == 0)
                    result = string.Format("Out of Stock", modelSize.Quantity);
                else
                    result = string.Format("Only {0} pairs left", modelSize.Quantity);
            }

            return Json(new { message = result, availableQty = modelSize.Quantity });
        }

        [HttpPost]
        public virtual JsonResult GetModelImagesByColour(string modelID, string colourDescID)
        {
            var viewModel = ConstructModelDetailsViewModel(Convert.ToInt16(modelID), Convert.ToInt16(colourDescID));

            var stringView = RenderRazorViewToString("_DetailsImages", viewModel);
            return Json(new { htmlString = stringView, sku = viewModel.SKU }, JsonRequestBehavior.AllowGet);
        }

        public void ConstructRecentViewCookie(string imageUrl, string linkUrl)
        {
            try
            {
                var recentViewCookie = GetRecentViewCookie();

                var cookieID = recentViewCookie.Value == null ? "" : recentViewCookie.Value;

                using (var context = new TTTEntities())
                {
                    if (string.IsNullOrEmpty(cookieID)) // create new cookie
                    {
                        var guid = System.Guid.NewGuid().ToString();

                        context.tblrecentviews.Add(new tblrecentview()
                        {
                            Active = true,
                            CookieID = guid,
                            CreateDT = DateTime.Now,
                            UpdateDT = DateTime.Now,
                            ImageUrl = imageUrl,
                            LinkUrl = linkUrl
                        });

                        recentViewCookie.Value = guid;
                        Response.Cookies.Add(recentViewCookie);
                    }
                    else //cookie create early
                    {
                        // check if item already exist, if yes then update
                        var existingItem = context.tblrecentviews.Where(a => a.CookieID == cookieID &&
                                                                            a.ImageUrl == imageUrl &&
                                                                            a.LinkUrl == linkUrl).SingleOrDefault();
                        if (existingItem != null)
                        {
                            existingItem.UpdateDT = DateTime.Now;
                        }
                        else
                        {
                            var recentItems = context.tblrecentviews.Where(a => a.CookieID == cookieID).ToList();
                            if (recentItems.Count < 5)
                            {
                                context.tblrecentviews.Add(new tblrecentview()
                                {
                                    Active = true,
                                    CookieID = cookieID,
                                    CreateDT = DateTime.Now,
                                    UpdateDT = DateTime.Now,
                                    ImageUrl = imageUrl,
                                    LinkUrl = linkUrl
                                });
                            }
                            else if (recentItems.Count == 5)
                            {
                                var itemToUpdate = context.tblrecentviews.Where(a => a.CookieID == cookieID)
                                                                        .OrderBy(a => a.UpdateDT)
                                                                        .First();

                                itemToUpdate.ImageUrl = imageUrl;
                                itemToUpdate.LinkUrl = linkUrl;
                                itemToUpdate.UpdateDT = DateTime.Now;
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private HttpCookie GetRecentViewCookie()
        {
            var recentViewCookie = Request.Cookies[Util.GetRecentViewCookieName()];
            recentViewCookie = recentViewCookie == null ? new HttpCookie(Util.GetRecentViewCookieName()) : recentViewCookie;

            return recentViewCookie;
        }

        public HttpCookie ConstructShoppingBagCookie()
        {
            shoppingBagCookie = Request.Cookies[Util.GetShoppingBagCookieName()];

            if (shoppingBagCookie == null)
            {
                shoppingBagCookie = new HttpCookie(Util.GetShoppingBagCookieName());
                shoppingBagCookie.Value = System.Guid.NewGuid().ToString();

                Response.Cookies.Add(shoppingBagCookie);
            }

            cookieID = shoppingBagCookie.Value;

            return shoppingBagCookie;
        }

        [HttpGet]
        public virtual PartialViewResult _SizeChart()
        {
            return PartialView();
        }
    }
}
