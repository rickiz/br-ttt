using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.ToteToToe.Web.DataModels;
using BR.ToteToToe.Web.Properties;
using System.Text;
using System.Security.Cryptography;
using System.Web.Routing;

namespace BR.ToteToToe.Web.Helpers
{
    #region Public Enums

    public enum Priority
    {
        Low,
        Medium,
        High
    }

    public enum RefType
    {
        Brand,
        Category,
        Collection,
        Colour,
        ColourDesc,
        Country,
        CustomizeModelSize,
        Day,
        HeelHeight,
        Lifestyle,
        Model,
        ModelColour,
        ModelColourModel,
        ModelSize,
        Month,
        Price,
        Size,
        Status,
        OrderStatusUpdateStatus,
        Trend,
        Year
    }

    public enum CategoryType
    {
        Shoes,
        Bags,
        Shirts
    }

    public enum Status
    {
        Open,
        Pending,
        Processing,
        Closed,
        Delivery
    }

    public enum CustomizeStyleColour
    {
        OpenPeepToe_Butterfly_Red,
        OpenPeepToe_Polkadot,
        OpenPeepToe_Textury,
        PointedToe_Butterfly,
        PointedToe_Polkadot,
        PointedToe_Textury,
        RoundedToe_Butterfly,
        RoundedToe_Polkadot,
        RoundedToe_Textury
    }

    public enum Gender { Male, Female }

    public enum CheckoutAddressType
    {
        None = 0,
        Primary = 1,
        Secondary = 2,
        Others = 3
    }

    #endregion

    public static class Util
    {
        public static tblaccess SessionAccess
        {
            get
            {
                return HttpContext.Current.Session["Access"] as tblaccess;
            }
            set
            {
                HttpContext.Current.Session["Access"] = value;
            }
        }

        public static string SessionErrMsg
        {
            get
            {
                return HttpContext.Current.Session["ErrMsg"] as string;
            }
            set
            {
                HttpContext.Current.Session["ErrMsg"] = value;
            }
        }

        private static TTTEntities context = new TTTEntities();

        public static List<SelectListItem> GetList(RefType type, bool active = true, string defaultValue = "0",
            string defaultText = "default", int modelID = 0, int modelColourDescID = 0, int modelSizeID = 0, bool includeDefault = true)
        {
            List<SelectListItem> resultList = new List<SelectListItem>();
            if (defaultText == "default")
                defaultText = type.ToString().ToUpper();

            switch (type)
            {
                case RefType.Brand:
                    var listBrand = context.refbrands.Where(a => a.Active == active).Select(a => a.Name).Distinct().ToList();
                    if (listBrand != null)
                    {
                        resultList = listBrand.Select(a =>
                            new SelectListItem()
                            {
                                Text = a,
                                Value = a
                            }).ToList();

                    }
                    break;

                case RefType.Category:
                    var listCategory = context.refcategories.Where(a => a.Active == active).ToList();
                    if (listCategory != null)
                    {
                        resultList = listCategory.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.Name,
                                Value = a.ID.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.Collection:
                    var listCollection = context.refcollections.Where(a => a.Active == active).ToList();
                    if (listCollection != null)
                    {
                        resultList = listCollection.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.Name,
                                Value = a.ID.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.Colour:
                    var listColour = context.refcolours.Where(a => a.Active == active).ToList();
                    if (listColour != null)
                    {
                        resultList = listColour.Select(a =>
                             new SelectListItem()
                             {
                                 Text = a.Name,
                                 Value = a.ID.ToString()
                             }).ToList();
                    }
                    break;

                case RefType.ColourDesc:
                    var listColourDesc = context.refcolourdescs.Where(a => a.Active == active).ToList();
                    if (listColourDesc != null)
                    {
                        resultList = listColourDesc.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.Name,
                                Value = a.ID.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.Country:
                    var listCountry = context.refcountries.Where(a => a.Active == active).ToList();
                    if (listCountry != null)
                    {
                        resultList = listCountry.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.Name,
                                Value = a.ID.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.CustomizeModelSize:

                    for (int i = 5; i <= 9; i++)
                    {
                        resultList.Add(new SelectListItem()
                        {
                            Text = i.ToString(),
                            Value = i.ToString()
                        });
                    }

                    break;

                case RefType.Day:
                    var listDay = Enumerable.Range(1, 31).ToList();
                    if (listDay != null)
                    {
                        resultList = listDay.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.ToString(),
                                Value = a.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.HeelHeight:
                    resultList.Add(new SelectListItem()
                    {
                        Text = "0 - 1.5 inch heel",
                        Value = "0,1.5"
                    });
                    resultList.Add(new SelectListItem()
                    {
                        Text = "2 - 2.5 inch heel",
                        Value = "2,2.5"
                    });
                    resultList.Add(new SelectListItem()
                    {
                        Text = "3 - 3.5 inch heel",
                        Value = "3,3.5"
                    });
                    resultList.Add(new SelectListItem()
                    {
                        Text = "4+ inch heel",
                        Value = "4,0"
                    });
                    //var heightDetails = Properties.Settings.Default.HeelsHeight.Split('|');
                    //var minHeight = Convert.ToDecimal(heightDetails[0].Split(',')[0]);
                    //var maxHeight = Convert.ToDecimal(heightDetails[0].Split(',')[1]);
                    //var heightIncrement = Convert.ToDecimal(heightDetails[1]);

                    //for (var i = minHeight; i <= maxHeight; i += heightIncrement)
                    //{
                    //    var text = "";
                    //    if (i == minHeight)
                    //        text = string.Format("0 - {0} inch heel", minHeight);
                    //    else if (i == maxHeight)
                    //        text = string.Format("{0}+ inch heel", i - 1);
                    //    else
                    //        text = string.Format("{0} - {1} inch heel", i - 1, i);

                    //    resultList.Add(new SelectListItem()
                    //    {
                    //        Text = text,
                    //        Value = i.ToString()
                    //    });
                    //}

                    break;

                case RefType.Lifestyle:
                    var listLifestyle = context.reflifestyles.Where(a => a.Active == active).ToList();
                    if (listLifestyle != null)
                    {
                        resultList = listLifestyle.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.Name,
                                Value = a.ID.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.Price:

                    resultList.Add(new SelectListItem()
                    {
                        Text = "Lowest to Highest",
                        Value = "1"
                    });
                    resultList.Add(new SelectListItem()
                    {
                        Text = "Highest to Lowest",
                        Value = "2"
                    });

                    break;

                case RefType.Model:
                    var listModel = context.refmodels.Where(a => a.Active == active).ToList();
                    if (listModel != null)
                    {
                        resultList = listModel.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.Name,
                                Value = a.ID.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.ModelColour:
                    var queryModelColour = from model in context.refmodels
                                           join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                           join colourDesc in context.refcolourdescs on modelColourDesc.ColourDescID equals colourDesc.ID
                                           select new { ColourDesc = colourDesc, ModelID = modelColourDesc.ModelID };

                    var listModelColour = queryModelColour.Where(a => a.ModelID == modelID).ToList();

                    if (listModelColour != null)
                    {
                        resultList = listModelColour.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.ColourDesc.Name,
                                Value = a.ColourDesc.ID.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.ModelColourModel:
                    var queryModelColourModel = from model in context.refmodels
                                                join modelColourDesc in context.lnkmodelcolourdescs on model.ID equals modelColourDesc.ModelID
                                                join colourDesc in context.refcolourdescs on modelColourDesc.ColourDescID equals colourDesc.ID
                                                select new { ColourDesc = colourDesc, ModelID = model.ID };

                    var listModelColourModel = queryModelColourModel.Where(a => a.ModelID == modelID).ToList();

                    if (listModelColourModel != null)
                    {
                        resultList = listModelColourModel.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.ColourDesc.Name,
                                Value = a.ModelID.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.ModelSize:
                    var queryModelSize = from model in context.refmodels
                                         join modelSize in context.lnkmodelsizes on model.ID equals modelSize.ModelID
                                         select new { ModelSize = modelSize };

                    var listModelSize = queryModelSize.Where(a => a.ModelSize.ModelID == modelID &&
                                                                    a.ModelSize.ColourDescID == modelColourDescID)
                                                      .Select(a => a.ModelSize)
                                                      .OrderBy(a => a.Size)
                                                      .ToList();

                    //var modelSizeDetails = Properties.Settings.Default.InitialSize.Split(',');
                    //var modelSizeValues = modelSizeDetails[0].Split('|');
                    //var europeModelSizeIndex = Properties.Settings.Default.SizeReference.Replace(" ","").Split('|').ToList().IndexOf("EU");

                    var modelSizeDetails = Properties.Settings.Default.InitialSize.Split(',');
                    var modelSizeValues = modelSizeDetails[0].Split('|');
                    var modelSizeReferences = Properties.Settings.Default.SizeReference.Replace(" ", "").Split('|').ToList();
                    var europeModelSizeIndex = modelSizeReferences.IndexOf("EU");
                    var modelNbstring = "\u00A0";
                    var modelTextFormat = "{0}{4}|{4}{1}{4}|{4}{2}{4}|{4}{3}";

                    resultList.Add(new SelectListItem()
                    {
                        Text = string.Format(modelTextFormat,
                            modelSizeReferences[0], modelSizeReferences[1], modelSizeReferences[2], modelSizeReferences[3], modelNbstring),
                        Value = "-1"
                    });

                    for (var i = 0; i < listModelSize.Count(); i++)
                    {
                        resultList.Add(new SelectListItem()
                        {
                            Text = string.Format(modelTextFormat,
                                (Convert.ToInt32(modelSizeValues[0]) + i).ToString().PadLeft(2).Replace(" ", modelNbstring),
                                (Convert.ToInt32(modelSizeValues[1]) + i).ToString().PadLeft(2).Replace(" ", modelNbstring),
                                (Convert.ToInt32(modelSizeValues[2]) + i).ToString().PadLeft(2).Replace(" ", modelNbstring),
                                (Convert.ToInt32(modelSizeValues[3]) + i).ToString().PadLeft(2).Replace(" ", modelNbstring),
                                modelNbstring),
                            Value = listModelSize[i].ID.ToString()
                        });
                    }

                    //if (listModelSize != null)
                    //{
                    //    resultList.Add(new SelectListItem()
                    //    {
                    //        Text = Properties.Settings.Default.SizeReference,
                    //        Value = "-1"
                    //    });

                    //    foreach (var modelSize in listModelSize)
                    //    {
                    //        var sizeDifference = Convert.ToInt32(modelSize.Size) - Convert.ToInt32(modelSizeValues[europeModelSizeIndex]);
                    //        var xxx = (Convert.ToInt32(modelSizeValues[1]) + sizeDifference).ToString().PadLeft(2);
                    //        resultList.Add(new SelectListItem()
                    //        {
                    //            Text = string.Format("{0} | {1} | {2} | {3}",
                    //                            (Convert.ToInt32(modelSizeValues[0]) + sizeDifference).ToString().PadLeft(2),
                    //                            (Convert.ToInt32(modelSizeValues[1]) + sizeDifference).ToString().PadLeft(4),
                    //                            (Convert.ToInt32(modelSizeValues[2]) + sizeDifference).ToString().PadLeft(4),
                    //                            (Convert.ToInt32(modelSizeValues[3]) + sizeDifference).ToString().PadLeft(4)),
                    //            Value = modelSize.ID.ToString(),
                    //            Selected = modelSize.ID == modelSizeID
                    //        });
                    //    }
                    //}
                    break;

                case RefType.Month:
                    var listMonth = Enumerable.Range(1, 12).ToList();
                    if (listMonth != null)
                    {
                        resultList = listMonth.Select(a =>
                            new SelectListItem()
                            {
                                //Text = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(a),
                                Text = a.ToString(),
                                Value = a.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.Size:

                    var sizeDetails = Properties.Settings.Default.InitialSize.Split(',');
                    var sizeRange = Convert.ToInt32(sizeDetails[1]);
                    var sizeValues = sizeDetails[0].Split('|');
                    var sizeReferences = Properties.Settings.Default.SizeReference.Replace(" ", "").Split('|').ToList();
                    var europeSizeIndex = sizeReferences.IndexOf("EU");
                    var nbstring = "\u00A0";
                    var textFormat = "{0}{4}|{4}{1}{4}|{4}{2}{4}|{4}{3}";

                    resultList.Add(new SelectListItem()
                    {
                        Text = string.Format(textFormat,
                            sizeReferences[0], sizeReferences[1], sizeReferences[2], sizeReferences[3], nbstring),
                        Value = "-1"
                    });

                    for (var i = 0; i <= sizeRange; i++)
                    {
                        resultList.Add(new SelectListItem()
                        {
                            Text = string.Format(textFormat,
                                (Convert.ToInt32(sizeValues[0]) + i).ToString().PadLeft(2).Replace(" ", nbstring),
                                (Convert.ToInt32(sizeValues[1]) + i).ToString().PadLeft(2).Replace(" ", nbstring),
                                (Convert.ToInt32(sizeValues[2]) + i).ToString().PadLeft(2).Replace(" ", nbstring),
                                (Convert.ToInt32(sizeValues[3]) + i).ToString().PadLeft(2).Replace(" ", nbstring),
                                nbstring),
                            Value = (Convert.ToInt32(sizeValues[europeSizeIndex]) + i).ToString()
                        });
                    }

                    //for (var i = 0; i <= sizeRange; i++)
                    //{
                    //    var yyy = (Convert.ToInt32(sizeValues[1]) + i).ToString().PadLeft(2);
                    //    resultList.Add(new SelectListItem()
                    //    {
                    //        Text = string.Format("{0} | {1} | {2} | {3}",
                    //                            (Convert.ToInt32(sizeValues[0]) + i).ToString(),
                    //                            (Convert.ToInt32(sizeValues[1]) + i).ToString().PadRight(4),
                    //                            (Convert.ToInt32(sizeValues[2]) + i).ToString().PadRight(4),
                    //                            (Convert.ToInt32(sizeValues[3]) + i).ToString().PadRight(4)),
                    //        Value = (Convert.ToInt32(sizeValues[europeSizeIndex]) + i).ToString()
                    //    });
                    //}
                    //var minSize = Convert.ToInt32(sizeDetails[0].Split(',')[0]);
                    //var maxSize = Convert.ToInt32(sizeDetails[0].Split(',')[1]);
                    //var sizeIncrement = Convert.ToInt32(sizeDetails[1]);

                    //for (var i = minSize; i <= maxSize; i += sizeIncrement)
                    //{
                    //    resultList.Add(new SelectListItem()
                    //    {
                    //        Text = string.Format("{0}", i),
                    //        Value = i.ToString()
                    //    });
                    //}

                    break;

                case RefType.Status:
                    var listStatus = context.refstatus.Where(a => a.Active == active).ToList();
                    if (listStatus != null)
                    {
                        resultList = listStatus.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.Name,
                                Value = a.ID.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.OrderStatusUpdateStatus:
                    var excludedStatus = GetExcludeStatusForOrderStatusUpdate();
                    var listOrderStatusUpdateStatus =
                        context.refstatus.Where(a => !excludedStatus.Contains(a.Name) && a.Active == active).ToList();
                    if (listOrderStatusUpdateStatus != null)
                    {
                        resultList = listOrderStatusUpdateStatus.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.Name,
                                Value = a.ID.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.Trend:
                    var listTrend = context.reftrends.Where(a => a.Active == active).ToList();
                    if (listTrend != null)
                    {
                        resultList = listTrend.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.Name,
                                Value = a.ID.ToString()
                            }).ToList();
                    }
                    break;

                case RefType.Year:
                    var listYear = Enumerable.Range(DateTime.Now.Year - 114, 100).ToList();
                    if (listYear != null)
                    {
                        resultList = listYear.Select(a =>
                            new SelectListItem()
                            {
                                Text = a.ToString(),
                                Value = a.ToString()
                            }).ToList();
                    }
                    break;

                default:
                    break;
            }

            if (type != RefType.ModelSize)
                resultList = resultList.OrderBy(a => a.Text).ToList();

            if (includeDefault)
            {
                resultList.Insert(0, new SelectListItem()
                {
                    Text = defaultText,
                    Value = defaultValue,
                    Selected = (type == RefType.ModelSize && modelSizeID > 0) ? false : true
                });
            }

            return resultList;
        }

        public static List<SelectListItem> GetAddressTypeList()
        {
            var resultList = new List<SelectListItem>();

            resultList.Add(
                new SelectListItem
                {
                    Text = "HOME",
                    Value = ((int)CheckoutAddressType.Primary).ToString()
                });

            resultList.Add(
                new SelectListItem
                {
                    Text = "WORK",
                    Value = ((int)CheckoutAddressType.Secondary).ToString()
                });

            resultList.Add(
                new SelectListItem
                {
                    Text = "OTHERS",
                    Value = ((int)CheckoutAddressType.Others).ToString()
                });

            return resultList;
        }

        public static string TruncateDescription(string oriDesc)
        {
            if (string.IsNullOrEmpty(oriDesc))
                return "";

            return oriDesc.Length > 250 ? string.Format("{0}....", oriDesc.Substring(0, 249)) : oriDesc;
        }

        public static string ConcatenateStrings(string delimiter, params string[] list)
        {
            string result = "";

            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item))
                    continue;

                if (!string.IsNullOrEmpty(result))
                    result += delimiter;

                result += item.Trim();
            }

            return result;
        }

        public static string FormatDate(DateTime? date, string format = "dd/MM/yy")
        {
            if (!date.HasValue)
                return "";

            return date.Value.ToString(format);
        }

        public static T ParseEnum<T>(string enumName)
        {
            return (T)Enum.Parse(typeof(T), enumName, true);
        }

        public static List<SelectListItem> GetEnumList<T>(string defaultValue = "", string defaultText = "-- Please select --", bool includeDefault = false)
        {
            Type enumType = typeof(T);

            var values = GetEnumValues<T>();
            var resultList = values.Select(a =>
                new SelectListItem()
                {
                    Text = a.ToString(),
                    //Value = ((int)Enum.Parse(enumType, a.ToString())).ToString()
                    Value = a.ToString()
                }).ToList();

            if (includeDefault)
            {
                resultList.Insert(0, new SelectListItem()
                {
                    Text = defaultText,
                    Value = defaultValue,
                    Selected = true
                });
            }

            return resultList;
        }

        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static string GetMD5Hash(string value)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static string GetShoppingBagCookieName()
        {
            return "ShoppingBag";
        }

        public static string GetRecentViewCookieName()
        {
            return "RecentView";
        }

        public static string GetCustomerEmail()
        {
            return Util.SessionAccess == null ? "" : Util.SessionAccess.Email;
        }

        public static int GetStatusID(string name)
        {
            var id = 0;

            using (var context = new TTTEntities())
            {
                id = context.refstatus.Where(a => a.Name == name).Single().ID;
            }

            return id;
        }

        public static void SetSessionErrMsg(Exception ex)
        {
            string errorMessage = "";

            errorMessage = string.Format("Error occur: {0}", ex.Message);

            Util.SessionErrMsg = errorMessage;
        }

        public static void CheckSessionAccess(RequestContext reqCtx)
        {
            if (Util.SessionAccess != null) return;

            var request = reqCtx.HttpContext.Request;
            var currentContenct = reqCtx.HttpContext;
            if (request.IsAuthenticated)
            {
                SetSessionAccess(currentContenct.User.Identity.Name);
            }
        }

        public static void SetSessionAccess(string loginID)
        {
            using (var context = new TTTEntities())
            {
                var access = context.tblaccesses.Single(a => a.Email == loginID && a.Active);
                Util.SessionAccess = access;
            }
        }

        public static string Signature(string key)
        {
            var objSHA1 = new SHA1CryptoServiceProvider();

            objSHA1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key.ToCharArray()));

            byte[] buffer = objSHA1.Hash;
            string HashValue = System.Convert.ToBase64String(buffer);

            return HashValue;
        }

        public static string ConstructIPay88Signature(string refNo, double amount)
        {
            var amountString = amount.ToString("0.00").Replace(".", "");
            var merchantCode = Settings.Default.iPay88_MerchantCode;
            var merchantKey = Settings.Default.iPay88_MerchantKey;
            var currency = Settings.Default.iPay88_Currency;
            var key = merchantKey + merchantCode + refNo + amountString + currency;
            var signature = Signature(key);

            return signature;
        }

        public static int GetDefaultCountryID()
        {
            var countryCode = Settings.Default.CountryCode;

            using (var context = new TTTEntities())
            {
                var country = context.refcountries.FirstOrDefault(a => a.Active && a.Code == countryCode);

                return country == null ? 0 : country.ID;
            }
        }

        public static string GetSizeText(int size)
        {
            var modelSizeDetails = Properties.Settings.Default.InitialSize.Split(',');
            var modelSizeValues = modelSizeDetails[0].Split('|');
            var europeModelSizeIndex = Properties.Settings.Default.SizeReference.Replace(" ", "").Split('|').ToList().IndexOf("EU");

            var sizeDifference = size - Convert.ToInt32(modelSizeValues[europeModelSizeIndex]);

            return string.Format("{0} | {1} | {2} | {3}",
                            Convert.ToInt32(modelSizeValues[0]) + sizeDifference,
                            Convert.ToInt32(modelSizeValues[1]) + sizeDifference,
                            Convert.ToInt32(modelSizeValues[2]) + sizeDifference,
                            Convert.ToInt32(modelSizeValues[3]) + sizeDifference);
        }

        public static string[] GetExcludeStatusForOrderStatusUpdate()
        {
            var excludeStatus = new string[] { Status.Open.ToString(), Status.Closed.ToString() };

            return excludeStatus;
        }
    }
}