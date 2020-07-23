using AutoMapper;
using AffiliateProfile.Models;
using Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.InterFace;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AffiliateProfile.Controllers
{
    [Authorize(Roles = "Affiliate")]
    public class CustomersController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public CustomersController(IMapper mapper, IUnitOfWork uow)
        {
            _mapper = mapper;
            _uow = uow;
        }
        public IActionResult Index()
        {
            return View();
        }
        //// <summary>
        /// show list view in View as Datatable jquery
        /// </summary>
        /// <returns></returns>

        public IActionResult List()
        {
            try
            {
                var dtValues = SetDataTableRequest();
                int recordsTotal = 0;
                var data = _uow.SellRepo.AffiliateCustomers(dtValues.draw,
                    dtValues.length,
                    dtValues.sortColumn,
                    dtValues.sortColumnDirection,
                    dtValues.searchValue,
                    dtValues.pageSize,
                    dtValues.skip,
                    ref recordsTotal,
                    UserExtention.GetUserMail(User));

                List<SellDto> list = new List<SellDto>();

                foreach (var email in data)
                {
                    var model = new SellDto()
                    {
                        Email = email
                    };
                    list.Add(model);
                }

                dtValues.recordsTotal = recordsTotal;

                return Json(new AjaxResult { draw = dtValues.draw, recordsFiltered = dtValues.recordsTotal, recordsTotal = dtValues.recordsTotal, data = list });
            }
            catch (Exception ex)
            {
                return Json("error");
            }
            
        }

    }

}