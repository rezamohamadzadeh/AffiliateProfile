using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AffiliateProfile.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repository.InterFace;

namespace AffiliateProfile.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AffiliateSellApiController : JsonActions
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private IHttpContextAccessor _accessor;
        private IConfiguration _config;

        public AffiliateSellApiController(IUnitOfWork uow,
            IMapper mapper,
            IHttpContextAccessor accessor,
            IConfiguration config)
        {
            _uow = uow;
            _mapper = mapper;
            _accessor = accessor;
            _config = config;
        }


        public async Task<IActionResult> GetAffiliateSells([FromQuery] string UserId)
        {
            try
            {
                var user = _uow.UserRepo.GetById(UserId);

                if (user == null)
                    return NotFound(ErrorResult("The user not found"));

                var affiliate = _uow.AffiliateRepo.Get(d => d.Email == user.Email).FirstOrDefault();

                if (affiliate == null)
                    return NotFound(ErrorResult("The affiliate not found"));

                var sells = _mapper.Map<IEnumerable<SellDto>>(await _uow.SellRepo.GetAsync(d => d.AffiliateCode == affiliate.Code));

                if (sells == null)
                    return NotFound(ErrorResult("The Affiliate Sells not found"));                
                
                return Ok(SuccessResult(sells));
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorResult(ex.Message));
            }

        }
    }
}
