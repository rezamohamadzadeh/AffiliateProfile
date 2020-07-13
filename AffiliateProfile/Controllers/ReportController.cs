using AffiliateProfile.Models;
using AutoMapper;
using Common.Extensions;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.InterFace;
using Stimulsoft.Report;
using Stimulsoft.Report.Engine;
using Stimulsoft.Report.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AffiliateProfile.Controllers
{
    [Authorize(Roles = "Affiliate")]
    public class ReportController : BaseController
    {

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public ReportController(IMapper mapper, IUnitOfWork uow)
        {
            _mapper = mapper;
            _uow = uow;
        }
        public IActionResult AffiliateSellPrintPage(SellReportDto model)
        {
            TempData["StartDate"] = model.StartDate;
            TempData["EndDate"] = model.EndDate;
            TempData["FilterType"] = model.FilterType;
            TempData["SkipDate"] = model.SkipDate;

            return View();
        }
        public IActionResult ConversionRatePrintPage()
        {

            return View();
        }

        public ActionResult AffiliateSell()
        {
            try
            {
                var model = new SellReportDto();

                if (TempData["StartDate"] != null)
                {
                    model.StartDate = DateTime.Parse(TempData["StartDate"].ToString());
                }
                if (TempData["EndDate"] != null)
                {
                    model.EndDate = DateTime.Parse(TempData["EndDate"].ToString());
                }
                if(TempData["FilterType"] != null)
                {
                    model.FilterType = (FilterType)(TempData["FilterType"]);
                }
                if(TempData["SkipDate"] != null)
                {
                    model.SkipDate = (bool)(TempData["SkipDate"]);
                }

                StiReport stiReport = new StiReport();

                stiReport.Load(StiNetCoreHelper.MapPath(this, "wwwroot/Reports/AffiliateSellReport.mrt"));

                var affiliates = _uow.AffiliateRepo.Get(d => d.Email == UserExtention.GetUserMail(User)).FirstOrDefault();

                IEnumerable<Tb_Sell> sellRepo = null;
                switch (model.FilterType)
                {
                    case FilterType.All:
                        sellRepo = !model.SkipDate ? _uow.SellRepo.Get(d => d.AffiliateCode == affiliates.Code && d.CreateAt >= model.StartDate && d.CreateAt <= model.EndDate) : _uow.SellRepo.Get(d => d.AffiliateCode == affiliates.Code);
                        break;
                    case FilterType.OnlyRegister:
                        sellRepo = !model.SkipDate ? _uow.SellRepo.Get(d => d.AffiliateCode == affiliates.Code && d.PayStatus == PayStatus.Registered && d.CreateAt >= model.StartDate && d.CreateAt <= model.EndDate) : _uow.SellRepo.Get(d => d.AffiliateCode == affiliates.Code && d.PayStatus == PayStatus.Registered);
                        break;
                    case FilterType.Sells:
                        sellRepo = !model.SkipDate ? _uow.SellRepo.Get(d => d.AffiliateCode == affiliates.Code && d.PayStatus != PayStatus.Registered && d.CreateAt >= model.StartDate && d.CreateAt <= model.EndDate) : _uow.SellRepo.Get(d => d.AffiliateCode == affiliates.Code && d.PayStatus != PayStatus.Registered);
                        break;
                    default:

                        break;
                }

                //if (model.StartDate != null && model.EndDate != null)
                //{
                //    sellRepo = _uow.SellRepo.Get(d => d.AffiliateCode == affiliates.Code && d.CreateAt >= model.StartDate && d.CreateAt <= model.EndDate);
                //}
                //else if (model.StartDate != null)
                //{
                //    sellRepo = _uow.SellRepo.Get(d => d.AffiliateCode == affiliates.Code && d.CreateAt >= model.StartDate);
                //}
                //else if (model.EndDate != null)
                //{
                //    sellRepo = _uow.SellRepo.Get(d => d.AffiliateCode == affiliates.Code && d.CreateAt <= model.EndDate);
                //}
                //else
                //{
                //    sellRepo = _uow.SellRepo.Get(d => d.AffiliateCode == affiliates.Code);
                //}


                var sumSell = sellRepo.Sum(d => d.Price);

                stiReport["SumSell"] = sumSell.ToString("N0");

                stiReport.RegData("affiliateDT", sellRepo);

                return StiNetCoreViewer.GetReportResult(this, stiReport);
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        public ActionResult ConversionRate()
        {
            StiReport stiReport = new StiReport();

            stiReport.Load(StiNetCoreHelper.MapPath(this, "wwwroot/Reports/ConversionRateReport.mrt"));

            var affiliates = _uow.AffiliateRepo.Get(d => d.Email == UserExtention.GetUserMail(User)).FirstOrDefault();
            var registeredSells = _uow.SellRepo.Get(d => d.AffiliateCode == affiliates.Code && d.PayStatus == PayStatus.Registered);
            stiReport["RegisteredSell"] = registeredSells != null ? registeredSells.Sum(d => d.Price) : 0;
            var AllSells = _uow.SellRepo.Get(d => d.AffiliateCode == affiliates.Code && d.PayStatus != PayStatus.Registered);
            stiReport["AllSellValue"] = AllSells != null ? AllSells.Sum(d => d.Price) : 0;

            return StiNetCoreViewer.GetReportResult(this, stiReport);

        }

        public IActionResult FilterAffiliateSell()
        {
            return View();
        }

        public IActionResult ViewerEvent()
        {
            return StiNetCoreViewer.ViewerEventResult(this);
        }
    }
}