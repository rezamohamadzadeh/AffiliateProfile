using System;
using System.Linq;
using AffiliateProfile.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AffiliateProfile.Controllers
{
    public class BaseController : Controller
    {
        protected const string SuccessMessage = "Your information submited successfully";
        protected const string SuccessAddMessage = "Your information added successfully";
        protected const string SuccessDeleteMessage = "Your information deleted successfully";
        protected const string SuccessEditMessage = "Your information edited successfully";
        protected const string ErrorMessage = "The operation failed \n";
        protected const string ErrorMessageForGetInformation = "Calling information was difficult \n";
        protected const string ErrorMessageCheckEmail = "This email is not exist \n";
        protected const string SuccessMessageCheckEmail = "This email is exists \n";
        protected const string SuccessSendEmailMessage = "Your email sent successfully";
        protected const string SuccessPaymentMessage = "Your payment did successfully";
        protected const string ErrorSendEmailMessage = "Your email failed";
        
        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        private static DataTableModel _dataTableModel;
        public static DataTableModel DataTableModel
        {
            get
            {
                if (_dataTableModel == null)
                    _dataTableModel = new DataTableModel();
                return _dataTableModel;
            }
        }
        /// <summary>
        ///  this method get datatable request values
        /// </summary>
        /// <returns></returns>
        protected DataTableModel SetDataTableRequest()
        {
            DataTableModel.draw = Request.Form["draw"].FirstOrDefault();


            DataTableModel.start = Request.Form["start"].FirstOrDefault();


            DataTableModel.length = Request.Form["length"].FirstOrDefault();


            DataTableModel.sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();


            DataTableModel.sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();


            DataTableModel.searchValue = Request.Form["search[value]"].FirstOrDefault();


            DataTableModel.pageSize = DataTableModel.length != null ? Convert.ToInt32(DataTableModel.length) : 0;

            DataTableModel.skip = DataTableModel.start != null ? Convert.ToInt32(DataTableModel.start) : 0;

            return DataTableModel;
        }
        
    }
    public class AjaxResult
    {
        public string draw { get; set; }
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
        public object data { get; set; }
    }
}