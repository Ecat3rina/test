using RepoApp.BLL.Models;
using RepoApp.BLL.Models.AddModels;
using RepoApp.BLL.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentProject.Common.DataTables;
using RepoApp.BLL.Models.DetailModels;
using RepoApp.BLL.Models.EditModels;

namespace RepoApp.Controllers
{
    [Authorize(Roles = "Admin, Operator, User")]
    public class ProjectController : BaseController
    {
       [Authorize(Roles = "Admin, Operator, User")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
      

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Add()
        {
            using (ProjectRepository repo = new ProjectRepository())
            {
                var departments = repo.GetDepartments();
                ViewBag.Departments = new SelectList(departments, "Id", "Name");
                var users = repo.GetUsers();
                ViewBag.Users = new SelectList(users, "Id", "UserName");

            }
            return View("Add");
        }


        [HttpPost]
        public JsonResult GetProjects(DataTablesParameters parameters)
        {
            using (ProjectRepository repo = new ProjectRepository())
            {
                var projectList = repo.GetProjects(parameters);
                return CreateDataTablesResult(projectList, parameters);

            }

        }


        [HttpGet]
        public ActionResult GetDetails(Guid id)
        {
            using (ProjectRepository repo = new ProjectRepository())
            {
                var projectDetails = repo.GetDetails(id);
                return PartialView("_Details", projectDetails);
            }
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            using (ProjectRepository repo = new ProjectRepository())
            {
                var model = repo.GetDelete(id);
                return PartialView("_Delete", model);
            }
        }
        [HttpPost]
        public ActionResult Delete(ProjectDetailsModel model)
        {
            using (ProjectRepository repo = new ProjectRepository())
            {
                repo.Delete(model);
            }

            return JsonViewValidResult("~/Views/Project/Index.cshtml");

        }

        [Authorize(Roles = "Admin, Operator")]

        [HttpGet]
        public ActionResult GetEdit(Guid id)
        {
            using (ProjectRepository repo = new ProjectRepository())
            {
                var departments = repo.GetDepartments();
                ViewBag.Departments = new SelectList(departments, "Id", "Name");
                var users = repo.GetUsers();
                ViewBag.Users = new SelectList(users, "Id", "UserName");
                var model = repo.GetEdit(id);
                return View("_Edit", model);
            }
        }
        [HttpPost]
        public ActionResult Edit(List<string> projectData)
        {
            Guid projectId = Guid.Parse(projectData[0]);
            string projectName = projectData[1];
            Guid department = Guid.Parse(projectData[2]);
            Guid user = Guid.Parse(projectData[3]);
            string userName = projectData[4];

            var project = new ProjectEditModel
            {
                Name = projectName,
                Department = department,
                User = user,
                Username = userName
            };

            var result = new ArrayList();

            foreach (var item in projectData)
            {
                if (item.Contains('^'))
                {
                    string[] repoData = item.Split('^');
                    Guid id = Guid.Parse(repoData[0]);
                    string url = repoData[1];
                    int repoNr = Int16.Parse(repoData[2]);
                    var repo = new RepositoryEditModel
                    {
                        URL = url,
                        TypeId = id,
                        Index = repoNr
                    };

                    result.Add(repo);

                }
            }

            result.Add(project);
            using (ProjectRepository repo = new ProjectRepository())
            {
                repo.Edit(result, projectId);
            }

            return View("~/Views/Project/Index.cshtml");
        }

        [HttpPost]
        public ActionResult AddRepository()
        {
            using (ProjectRepository repo = new ProjectRepository())
            {
                string res = repo.GetRepository();
                return Json(res);

            }
        }

        [HttpPost]
        public ActionResult AddRepositoryForEdit(string id)
        {
            using (ProjectRepository repo = new ProjectRepository())
            {
                string res = repo.GetRepository(id);
                return Json(res);

            }
        }

        [HttpPost]
        public ActionResult GetAllUserRepositories(string Id)
        {
            using (ProjectRepository repo = new ProjectRepository())
            {
                string res = repo.GetUserRepositories(Id);
                return Json(res);

            }
        }

        [HttpPost]
        public void DeleteRepository(List<string> info)
        {
            using (ProjectRepository repo = new ProjectRepository())
            {
               repo.DeleteRepo(info);
                //return Json(res);

            }
        }

        [HttpPost]
        public JsonResult SubmitProject(List<string> projectData)
        {

            string projectName = projectData[0];
            Guid department = Guid.Parse(projectData[1]);
            Guid user = Guid.Parse(projectData[2]);
            string userName = projectData[3];

            var project = new ProjectAddModel
            {
                Name = projectName,
                Department = department,
                User = user,
                Username = userName
            };

            var result = new ArrayList();

            foreach (var item in projectData)
            {
                if (item.Contains('^'))
                {
                    string[] repoData = item.Split('^');
                    Guid id = Guid.Parse(repoData[0]);
                    string url = repoData[1];
                    var repo = new RepositoryAddModel
                    {
                        URL = url,
                        TypeId = id

                    };

                    result.Add(repo);

                }
            }

            result.Add(project);

            using (ProjectRepository repo = new ProjectRepository())
            {
                repo.AddProject(result);
            }

            return Json(new { flag = "success" }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult IsProjectNameValid(string name)
        {
            using (ProjectRepository repo = new ProjectRepository())
            {
                var res=repo.CheckProjectName(name);
                 return Json(res==false, JsonRequestBehavior.AllowGet);

            }
        }
        public JsonResult IsProjectNameValidForEdit(string name, Guid id)
        {
            using (ProjectRepository repo = new ProjectRepository())
            {
                var res = repo.CheckProjectNameForEdit(name,id);
                return Json(res == false, JsonRequestBehavior.AllowGet);

            }
        }

        //public JsonResult GetRoleName(Guid id)
        //{
        //    using (ProjectRepository repo = new ProjectRepository())
        //    {
        //        var res = repo.GetSuperiorRoleName(id);
        //        return Json(res, JsonRequestBehavior.AllowGet);

        //    }
        //}


        //public JsonResult IsRepoAlready(List<string> info)
        //{
        //    using (ProjectRepository repo = new ProjectRepository())
        //    {
        //        var res = repo.CheckIfRepoExists(info);
        //        return Json(res == false, JsonRequestBehavior.AllowGet);

        //    }
        //}
    }
}
