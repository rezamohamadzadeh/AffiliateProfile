using AutoMapper;
using AffiliateProfile.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.InterFace;
using System;
using System.Collections.Generic;
using Common.Extensions;

namespace AffiliateProfile.Controllers
{
    [Authorize(Roles = "Affiliate")]
    public class AffiliateSellController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public AffiliateSellController(IMapper mapper, IUnitOfWork uow)
        {
            _mapper = mapper;
            _uow = uow;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult List()
        {
            try
            {
                var dtValues = SetDataTableRequest();
                int recordsTotal = 0;
                var data = _mapper.Map<List<SellDto>>(_uow.SellRepo.Filter(dtValues.draw,
                    dtValues.length,
                    dtValues.sortColumn,
                    dtValues.sortColumnDirection,
                    dtValues.searchValue,
                    dtValues.pageSize,
                    dtValues.skip,
                    ref recordsTotal,
                    UserExtention.GetUserMail(User)));

                dtValues.recordsTotal = recordsTotal;

                return Json(new AjaxResult { draw = dtValues.draw, recordsFiltered = dtValues.recordsTotal, recordsTotal = dtValues.recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                return Json("error");
            }
            
        }

    }

}