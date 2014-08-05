using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.ToteToToe.Web.Areas.Admin.ViewModels;
using BR.ToteToToe.Web.DataModels;

namespace BR.ToteToToe.Web.Areas.Admin.Controllers
{
    public partial class VoucherController : Controller
    {
        //
        // GET: /Admin/Voucher/

        public virtual ActionResult Index()
        {
            return View(new VoucherSearchViewModel());
        }

        [HttpPost]
        public virtual ActionResult Index(VoucherSearchViewModel viewModel, FormCollection collection)
        {
            if (collection["submit"].ToString() == "Maintain Voucher")
                return RedirectToAction("Maintain");

            using (var context = new TTTEntities())
            {
                var query = context.tblvouchers
                                     .GroupJoin(context.trnsalesorders, a => a.ID, b => b.VoucheID, (a, b) => new { vouher = a, so = b })
                                     .Select(a => new
                                     {
                                         Voucher = a.vouher,
                                         SO = a.so.FirstOrDefault()
                                     });

                if (!string.IsNullOrEmpty(viewModel.CodeFrom))
                    query = query.Where(a => String.Compare(viewModel.CodeFrom, a.Voucher.Code) <= 0);


                if (!string.IsNullOrEmpty(viewModel.CodeTo))
                    query = query.Where(a => String.Compare(a.Voucher.Code, viewModel.CodeTo) <= 0);

                var results = query.ToList();

                foreach (var result in results)
                {
                    viewModel.Results.Add(new VoucherDetails()
                    {
                        Active = result.Voucher.Active,
                        Code = result.Voucher.Code,
                        OrderID = result.SO == null ? "" : result.SO.ID.ToString(),
                        Value = result.Voucher.Value
                    });
                }
            }

            return View(viewModel);
        }


        public virtual ActionResult Maintain()
        {
            return View(new VoucherMaintainViewModel());
        }

        [HttpPost]
        public virtual ActionResult Maintain(VoucherMaintainViewModel viewModel, FormCollection collection)
        {
            var intCodes = Enumerable.Range(Convert.ToInt16(viewModel.CodeFrom), Convert.ToInt16(viewModel.CodeTo) - Convert.ToInt16(viewModel.CodeFrom) + 1).ToList();
            var stringCodes = intCodes.ConvertAll<string>(delegate(int i) { return i.ToString().PadLeft(Properties.Settings.Default.VoucherLength, '0'); });

            if (collection["submit"].ToString() == "Create/Activate")
                ActivateVoucher(viewModel, stringCodes);
            else
                DeactivateVoucher(viewModel, stringCodes);

            return RedirectToAction("Index");
        }
        private void ActivateVoucher(VoucherMaintainViewModel viewModel, List<string> stringCodes)
        {
            var codeFrom = viewModel.CodeFrom.PadLeft(Properties.Settings.Default.VoucherLength, '0');
            var codeTo = viewModel.CodeTo.PadLeft(Properties.Settings.Default.VoucherLength, '0');

            using (var context = new TTTEntities())
            {
                var results = context.tblvouchers
                                      .GroupJoin(context.trnsalesorders, a => a.ID, b => b.VoucheID, (a, b) => new { voucher = a, so = b })
                                      .Where(a => String.Compare(codeFrom, a.voucher.Code) <= 0 && String.Compare(a.voucher.Code, codeTo) <= 0)
                                      .Select(a => new
                                      {
                                          Voucher = a.voucher,
                                          SO = a.so.FirstOrDefault()
                                      }).ToList();

                var oldCodes = results.Where(a => a.SO == null).Select(a => a.Voucher.Code).ToList();
                var newCodes = stringCodes.Except(oldCodes).ToList();

                var oldVouchers = context.tblvouchers.Where(a => oldCodes.Contains(a.Code)).ToList();
                foreach (var voucher in oldVouchers)
                {
                    voucher.Active = true;
                    voucher.UpdateDT = DateTime.Now;
                }

                foreach (var code in newCodes)
                {
                    context.tblvouchers.Add(new tblvoucher()
                    {
                        Active = true,
                        Code = code.PadLeft(Properties.Settings.Default.VoucherLength, '0'),
                        CreateDT = DateTime.Now,
                        Type = "Cash",
                        Value = viewModel.Value,
                    });
                }

                context.SaveChanges();
            }
        }

        private void DeactivateVoucher(VoucherMaintainViewModel viewModel, List<string> stringCodes)
        {
            var codeFrom = viewModel.CodeFrom.PadLeft(Properties.Settings.Default.VoucherLength, '0');
            var codeTo = viewModel.CodeTo.PadLeft(Properties.Settings.Default.VoucherLength, '0');

            using (var context = new TTTEntities())
            {
                var results = context.tblvouchers
                                          .GroupJoin(context.trnsalesorders, a => a.ID, b => b.VoucheID, (a, b) => new { voucher = a, so = b })
                                          .Where(a => String.Compare(codeFrom, a.voucher.Code) <= 0 && String.Compare(a.voucher.Code,codeTo ) <= 0)
                                          .Select(a => new
                                          {
                                              Voucher = a.voucher,
                                              SO = a.so.FirstOrDefault()
                                          }).ToList();

                var oldCodes = results.Where(a => a.SO == null).Select(a => a.Voucher.Code).ToList();

                var oldVouchers = context.tblvouchers.Where(a => oldCodes.Contains(a.Code)).ToList();
                foreach (var voucher in oldVouchers)
                {
                    voucher.Active = false;
                    voucher.UpdateDT = DateTime.Now;
                }

                context.SaveChanges();
            }
        }
    }
}
