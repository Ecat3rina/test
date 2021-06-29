using DocumentProject.Common.DataTables;
using Newtonsoft.Json;
using RepoApp.BLL.Models.AddModels;
using RepoApp.BLL.Models.DetailModels;
using RepoApp.BLL.Models.EditModels;
using RepoApp.BLL.Repositories;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;

namespace RepoApp.Controllers
{
    [Authorize(Roles = "Admin, Operator, User")]

    public class UserController : BaseController
    {
        [Authorize(Roles = "Admin, Operator, User")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
      
        [HttpPost]
        public JsonResult GetUsers(DataTablesParameters parameters)
        {
            using (UserRepository repo = new UserRepository())
            {
                var userList = repo.GetUsers(parameters);
                return CreateDataTablesResult(userList, parameters);

            }

        }

       // [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Add()
        {
            using (UserRepository repo = new UserRepository())
            {
                var roles = repo.GetRoles();
                ViewBag.Roles = new MultiSelectList(roles, "Id", "Name");
            }
            return PartialView("_Add");
        }

        [HttpPost]
        public ActionResult Add(UserAddModel model)
        {
           
            using (UserRepository repo = new UserRepository())
            {

                if (ModelState.IsValid)
                {
                    if (repo.CheckUserName(model.UserName))
                    {
                        ModelState.AddModelError("UserName", "User name already exists");
                        var roles = repo.GetRoles();
                        ViewBag.Roles = new MultiSelectList(roles, "Id", "Name");
                        return JsonViewUnValidResult("~/Views/User/_Add.cshtml", model);
                    }
                    if (repo.CheckEmail(model.Email))
                    {
                        ModelState.AddModelError("Email", "This email already exists");
                        var roles = repo.GetRoles();
                        ViewBag.Roles = new MultiSelectList(roles, "Id", "Name");
                        return JsonViewUnValidResult("~/Views/User/_Add.cshtml", model);
                    }
                    repo.Add(model);
                }
                else
                {
                    var roles = repo.GetRoles();
                    ViewBag.Roles = new MultiSelectList(roles, "Id", "Name");
                    return JsonViewUnValidResult("~/Views/User/_Add.cshtml", model);
                }
            }
            
            return JsonViewValidResult("~/Views/User/Index.cshtml");

        }


        [Authorize(Roles = "Admin, Operator")]
        [HttpGet]
        public ActionResult GetEdit(Guid id)
        {
            using (UserRepository repo = new UserRepository())
            {
                var roles = repo.GetRoles();
                var model = repo.GetEdit(id);
                ViewBag.Roles = new MultiSelectList(roles, "Id", "Name");
                return PartialView("_Edit", model);
            }
        }

        [HttpPost]
        public ActionResult Edit(UserEditModel model)
        {
            

            using (UserRepository repo = new UserRepository())
            {
                //var password = repo.GetPassword(model.Id);
                var roles = repo.GetRoles();
                ViewBag.Roles = new MultiSelectList(roles, "Id", "Name");
                if (model.IsChangePassword)
                {

                    if (string.IsNullOrEmpty(model.Password))
                    {
                        ModelState.AddModelError("Password", "Insert password");
                        return JsonViewUnValidResult("~/Views/User/_Edit.cshtml", model);

                    }


                    if (!model.Password.Equals(model.ConfirmPassword))
                    {
                        ModelState.AddModelError("ConfirmPassword", "Passwords don't match");
                        return JsonViewUnValidResult("~/Views/User/_Edit.cshtml", model);

                    }

                }
                
                if (model.IsChangeRoles)
                {
                    if (model.Roles.Count == 0)
                    {
                        ModelState.AddModelError("IsChangeRoles", "Select at least one role");
                        return JsonViewUnValidResult("~/Views/User/_Edit.cshtml", model);
                    }
                }

                if (ModelState.IsValid)
                {
                    if (repo.CheckUserNameForEdit(model.UserName, model.Id))
                    {
                        ModelState.AddModelError("UserName", "User name already exists");
                        //var roles = repo.GetRoles();
                        //ViewBag.Roles = new MultiSelectList(roles, "Id", "Name");
                        return JsonViewUnValidResult("~/Views/User/_Edit.cshtml", model);
                    }
                    if (repo.CheckUserEmailForEdit(model.Email, model.Id))
                    {
                        ModelState.AddModelError("Email", "This email already exists");
                        ////var roles = repo.GetRoles();
                        ////ViewBag.Roles = new MultiSelectList(roles, "Id", "Name");
                        return JsonViewUnValidResult("~/Views/User/_Edit.cshtml", model);
                    }
                }

                repo.Edit(model);

                return JsonViewValidResult("~/Views/User/Index.cshtml");

            }

        }


        [HttpGet]
        public ActionResult GetDetails(Guid id)
        {
            using (UserRepository repo = new UserRepository())
            {
                var userDetails = repo.GetDetails(id);
                return PartialView("_Details", userDetails);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            using (UserRepository repo = new UserRepository())
            {
                var model = repo.GetDelete(id);
                return PartialView("_Delete", model);
            }
        }
        [HttpPost]
        public ActionResult Delete(UserDetailsModel model)
        {
            using (UserRepository repo = new UserRepository())
            {
                repo.Delete(model);
            }
            return JsonViewValidResult("~/Views/User/Index.cshtml");

        }
        public JsonResult GetSuperiorRole(string name)
        {
            using (UserRepository repo = new UserRepository())
            {
                var role = repo.GetSuperiorRoleName(name);
                return Json(role, JsonRequestBehavior.AllowGet);
            }
        }

        //public JsonResult IsUserNameValidForEdit(string name,Guid id)
        //{
        //    using (UserRepository repo = new UserRepository())
        //    {
        //        var result = repo.CheckUserNameForEdit(name,id);
        //        return Json(result == false, JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public JsonResult IsUserEmailValidForEdit(string name,Guid id)
        //{
        //    using (UserRepository repo = new UserRepository())
        //    {
        //        var result = repo.CheckUserEmailForEdit(name,id);
        //        return Json(result == false, JsonRequestBehavior.AllowGet);
        //    }
        //}

    }
}



