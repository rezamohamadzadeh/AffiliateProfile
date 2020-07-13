using AffiliateProfile.Models;
using AutoMapper;
using Common.Extensions;
using Common.Images;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Repository.InterFace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AffiliateProfile.Controllers
{
    [Authorize(Roles = "Affiliate")]
    public class HomeController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(UserManager<ApplicationUser> userManager,
            IMapper mapper, IUnitOfWork uow,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _uow = uow;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetDashboardActivities(int filterValue = 0)
        {
            string affiliateCode = "";

            var AllAffiliatesells = _mapper.Map<IEnumerable<SellDto>>(_uow.SellRepo.GetSellOnDashboard(5, UserExtention.GetUserId(User), ref affiliateCode, filterValue));

            var registered = filterValue == 0 ? _uow.SellRepo.Get(d => d.AffiliateCode == affiliateCode && d.PayStatus == PayStatus.Registered) : _uow.SellRepo.Get(d => d.AffiliateCode == affiliateCode && d.PayStatus == PayStatus.Registered && d.CreateAt > DateTime.Now.AddDays(-filterValue));

            ViewBag.Registered = registered == null ? 0 : registered.Count();

            var Allsells = filterValue == 0 ? _uow.SellRepo.Get(d => d.AffiliateCode == affiliateCode) : _uow.SellRepo.Get(d => d.AffiliateCode == affiliateCode && d.CreateAt > DateTime.Now.AddDays(-filterValue));

            var AllsellCount = Allsells == null ? 0 : Allsells.Count();

            var sumSell = filterValue == 0 ? _uow.SellRepo.Get(d => d.AffiliateCode == affiliateCode && d.PayStatus != PayStatus.Registered) : _uow.SellRepo.Get(d => d.AffiliateCode == affiliateCode && d.PayStatus != PayStatus.Registered && d.CreateAt > DateTime.Now.AddDays(-filterValue));

            var countSell = sumSell == null ? 0 : sumSell.Count();

            ViewBag.sumSell = sumSell == null ? 0 : sumSell.Sum(d => d.Price);

            var userSells = filterValue == 0 ? _uow.SellRepo.Get(d => d.AffiliateCode == affiliateCode && d.PayStatus != PayStatus.Registered)
                .GroupBy(m => m.ProductName)
                .Select(d => new ProductSellDto { ProductName = d.Key, SellCount = d.Count() })
                .OrderBy(x => x.ProductName).ToList() :

                _uow.SellRepo.Get(d => d.AffiliateCode == affiliateCode && d.PayStatus != PayStatus.Registered && d.CreateAt > DateTime.Now.AddDays(-filterValue))
                .GroupBy(m => m.ProductName)
                .Select(d => new ProductSellDto { ProductName = d.Key, SellCount = d.Count() })
                .OrderBy(x => x.ProductName).ToList();


            ViewBag.Sells = userSells.Where(d => d.ProductName != "").Count();

            var comision = _uow.AffiliateRepo.Get(d => d.Email == UserExtention.GetUserMail(User)).FirstOrDefault();
            ViewBag.Comision = comision == null ? 0 : comision.comision;

            var click = _uow.AffiliateRepo.Get(d => d.Email == UserExtention.GetUserMail(User)).FirstOrDefault();
            ViewBag.Clicks = click == null ? 0 : click.Click;

            ViewBag.CommisionAmount = (ViewBag.sumSell * ViewBag.Comision) / 100;
            if (countSell == 0 && AllsellCount == 0)
                ViewBag.Conversion = 0;
            else
                ViewBag.Conversion = countSell / AllsellCount;

            return Json(new
            {
                AllAffiliatesells = AllAffiliatesells,
                Registered = ViewBag.Registered,
                SumSell = ViewBag.sumSell,
                Sells = ViewBag.Sells,
                Comistion = ViewBag.Comision,
                Clicks = ViewBag.Clicks,
                CommisionAmount = ViewBag.CommisionAmount,
                Conversion = ViewBag.Conversion
            });
        }

        public async Task<IActionResult> Profile()
        {

            try
            {
                var userDto = _mapper.Map<ProfileDto>(await _userManager.FindByIdAsync(UserExtention.GetUserId(User)));
                if (userDto == null)
                {
                    ViewBag.ErrorMessage = ErrorMessageForGetInformation;
                    return View();
                }
                return View(userDto);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ErrorMessageForGetInformation + " \n " + ex.Message;
                return View();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileDto profileDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(profileDto.Id))
                    {
                        ViewBag.ErrorMessage = ErrorMessageForGetInformation;
                        return View(profileDto);
                    }

                    var user = await _userManager.FindByIdAsync(profileDto.Id);

                    if (profileDto.Files != null)
                    {
                        Upload uploader = new Upload();
                        Delete delete = new Delete();
                        if (user.Image != null)
                        {
                            string deletPath = Path.Combine(
                                Directory.GetCurrentDirectory(), "wwwroot/UserProfile", user.Image
                            );
                            delete.DeleteImage(deletPath);
                        }


                        profileDto.Image = Guid.NewGuid().ToString() + Path.GetExtension(profileDto.Files.FileName);
                        string savePath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/UserProfile", profileDto.Image

                        );


                        string DirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserProfile");
                        await uploader.UploadImage(savePath, DirectoryPath, profileDto.Files);
                        await _signInManager.SignOutAsync();

                    }
                    else
                        profileDto.Image = user.Image;



                    user.Name = profileDto.Name;
                    user.Image = profileDto.Image;
                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        ViewBag.SuccessMessage = SuccessEditMessage;
                    }
                    else
                        ViewBag.ErrorMessage = ErrorMessage;

                    await _signInManager.RefreshSignInAsync(user);
                    TempData["Image"] = user.Image == null ? "" : user.Image;
                }
                return View(profileDto);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ErrorMessage + " \n " + ex.Message;
                return View(profileDto);
            }

        }

    }
}