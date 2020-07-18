using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AffiliateProfile.Models;
using AutoMapper;
using Common.Extensions;
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
        private readonly IHttpClientFactory _clientFactory;
        public IConfiguration Configuration { get; }

        public AffiliateSellApiController(IUnitOfWork uow,
            IMapper mapper,
            IHttpContextAccessor accessor,
            IConfiguration config,
            IHttpClientFactory clientFactory,
            IConfiguration configuration)
        {
            _uow = uow;
            _mapper = mapper;
            _accessor = accessor;
            _config = config;
            _clientFactory = clientFactory;
            Configuration = configuration;
        }


        public async Task<IActionResult> GetAffiliateSells([FromQuery] string UserId)
        {
            try
            {

                var IP = Request.HttpContext.Connection.RemoteIpAddress;

                var url = Configuration["userRateLimitUrl"];
                var apiAddress = url + "/api/UserRateLimitApi/CheckUserLimit?UserIP=" + IP + "&UserId=" + UserId;
                var client = _clientFactory.CreateClient();

                HttpResponseMessage messages = await client.GetAsync(apiAddress);
                HttpContext.Response.ContentType = "application/json";

                if (!messages.IsSuccessStatusCode)
                {
                    var result = await messages.Content.ReadAsAsync<JsonResultContent>();
                    return BadRequest(result);
                }

                var user = _uow.UserRepo.GetById(UserId);

                if (user == null)
                    return NotFound(ErrorResult("The user not found"));

                var affiliate = _uow.AffiliateRepo.Get(d => d.Email == user.Email).FirstOrDefault();

                if (affiliate == null)
                    return NotFound(ErrorResult("The affiliate not found"));

                var sells = _mapper.Map<IEnumerable<SellDto>>(await _uow.SellRepo.GetAsync(d => d.AffiliateCode == affiliate.Code));

                if (sells == null)
                    return NotFound(ErrorResult("The Affiliate Sells not found"));


                var setBlackListAdsrress = url + "/api/UserRateLimitApi/SetUserInBlackList?UserIP=" + IP + "&UserId=" + UserId;

                HttpResponseMessage message = await client.GetAsync(setBlackListAdsrress);
                HttpContext.Response.ContentType = "application/json";

                if (!message.IsSuccessStatusCode)
                {
                    var result = await message.Content.ReadAsAsync<JsonResultContent>();
                    return BadRequest(result);
                }

                return Ok(SuccessResult(sells));
            }
            catch (Exception ex)
            {
                return BadRequest(ErrorResult(ex.Message));
            }

        }
    }
}
