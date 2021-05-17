using QuizbeePlus.Data;
using QuizbeePlus.Services;
using QuizbeePlus.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using QuizbeePlus.Entities;
using QuizbeePlus.Commons;
using Microsoft.AspNet.Identity.EntityFramework;

namespace QuizbeePlus.Controllers
{
    [CustomAuthorize(Roles = "Administrator")]
    public class ControlPanelController : BaseController
    {
        public ActionResult Index(bool? isPartial = false)
        {
            ControlPanelViewModel model = new ControlPanelViewModel();
            
            model.PageInfo = new PageInfo()
            {
                PageTitle = "Control Panel",
                PageDescription = "Manage Quizbee."
            };

            return View(model);
        }

        public ActionResult Users(string search, int? page, int? items)
        {
            UsersListingViewModel model = new UsersListingViewModel();

            model.PageInfo = new PageInfo()
            {
                PageTitle = "Users",
                PageDescription = "Quizbee Users."
            };

            model.searchTerm = search;
            model.pageNo = page ?? 1;
            model.pageSize = items ?? 10;

            var usersSearch = UsersService.Instance.GetUsersWithRoles(model.searchTerm, model.pageNo, model.pageSize);

            model.Users = usersSearch.Users;
            model.TotalCount = usersSearch.TotalCount;

            model.Pager = new Pager(model.TotalCount, model.pageNo, model.pageSize);

            return PartialView("_Users", model);
        }
        
        public ActionResult UserDetails(string ID)
        {
            UserDetailsViewModel model = new UserDetailsViewModel();

            model.PageInfo = new PageInfo()
            {
                PageTitle = "User Details",
                PageDescription = "User Details"
            };
            
            model.User = UsersService.Instance.GetUserWithRolesByID(ID);
            model.AvailableRoles = ControlPanelService.Instance.GetAllRoles();

            //remove roles from dropdown which are already with the user 
            foreach (var userRole in model.User.Roles)
            {
                var availableRole = model.AvailableRoles.Where(x => x.Id.Equals(userRole.Id)).FirstOrDefault();

                if(availableRole != null)
                {
                    model.AvailableRoles.Remove(availableRole);
                }
            }

            return PartialView("_UserDetails", model);
        }

        public ActionResult Roles(string search, int? page, int? items)
        {
            RolesListingViewModel model = new RolesListingViewModel();

            model.PageInfo = new PageInfo()
            {
                PageTitle = "Roles",
                PageDescription = "Quizbee Roles."
            };

            model.searchTerm = search;
            model.pageNo = page ?? 1;
            model.pageSize = items ?? 10;

            var rolesSearch = ControlPanelService.Instance.GetRoles(model.searchTerm, model.pageNo, model.pageSize);

            model.Roles = rolesSearch.Roles;
            model.TotalCount = rolesSearch.TotalCount;

            model.Pager = new Pager(model.TotalCount, model.pageNo, model.pageSize);

            return PartialView("_Roles", model);
        }

        public ActionResult NewRole(string ID)
        {
            NewRoleViewModel model = new NewRoleViewModel();

            return PartialView("_NewRole", model);
        }

        [HttpPost]
        public async Task<JsonResult> NewRole(NewRoleViewModel model)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (!ModelState.IsValid)
            {
                var Errors = new List<string>();

                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        Errors.Add(error.ErrorMessage.ToString());
                    }
                }

                result.Data = new { Success = false, Errors = Errors };

                return result;
            }
            else
            {
                try
                {
                    result.Data = new { Success = await ControlPanelService.Instance.NewRole(new IdentityRole() { Name = model.Name })};
                }
                catch (Exception ex)
                {
                    result.Data = new { Success = false, Errors = ex.InnerException.Message };
                }
            }

            return result;
        }

        [HttpPost]
        public async Task<JsonResult> AddUserRole(string userID, string roleID)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            try
            {
                result.Data = new { Success = await ControlPanelService.Instance.AddUserRole(userID, roleID) };
            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Errors = ex.InnerException.Message };
            }

            return result;
        }

        [HttpPost]
        public async Task<JsonResult> RemoveUserRole(string userID, string roleID)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            try
            {
                result.Data = new { Success = await ControlPanelService.Instance.RemoveUserRole(userID, roleID) };
            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Errors = ex.InnerException.Message };
            }

            return result;
        }

        public ActionResult RoleDetails(string ID)
        {
            RoleDetailsViewModel model = new RoleDetailsViewModel();

            model.PageInfo = new PageInfo()
            {
                PageTitle = "Role Details",
                PageDescription = "Role Details"
            };
            
            model.Role = ControlPanelService.Instance.GetRoleByID(ID);

            return PartialView("_RoleDetails", model);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateRole(UpdateRoleViewModel model)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (!ModelState.IsValid)
            {
                var Errors = new List<string>();

                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        Errors.Add(error.ErrorMessage.ToString());
                    }
                }

                result.Data = new { Success = false, Errors = Errors };

                return result;
            }
            else
            {
                try
                {
                    await ControlPanelService.Instance.UpdateRole(new IdentityRole() { Id = model.ID, Name = model.Name });

                    result.Data = new { Success = true };
                }
                catch (Exception ex)
                {
                    result.Data = new { Success = false, Errors = ex.InnerException };
                }
            }

            return result;
        }

        [HttpPost]
        public async Task<JsonResult> DeleteRole(string ID)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (string.IsNullOrEmpty(ID))
            {
                result.Data = new { Success = false, Errors = "Role can not be identified." };

                return result;
            }
            else
            {
                try
                {
                    await ControlPanelService.Instance.DeleteRole(ID);

                    result.Data = new { Success = true };
                }
                catch (Exception ex)
                {
                    result.Data = new { Success = false, Errors = ex.Message };
                }
            }

            return result;
        }



        public ActionResult Categories(string search, int? page, int? items)
        {
            CategoriesListingViewModel model = new CategoriesListingViewModel();

            model.PageInfo = new PageInfo()
            {
                PageTitle = "Categories",
                PageDescription = "Question Categories."
            };

            model.searchTerm = search;
            model.pageNo = page ?? 1;
            model.pageSize = items ?? 10;

            var categoriesSearch = CategoryService.Instance.GetCategories(model.searchTerm, model.pageNo, model.pageSize);

            model.Categories = categoriesSearch.Categories;
            model.TotalCount = categoriesSearch.TotalCount;

            model.Pager = new Pager(model.TotalCount, model.pageNo, model.pageSize);

            return PartialView("_Categories", model);
        }

        public ActionResult NewCategory()
        {
            NewCategoryViewModel model = new NewCategoryViewModel();

            return PartialView("_NewCategory", model);
        }

        [HttpPost]
        public async Task<JsonResult> NewCategory(NewCategoryViewModel model)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (!ModelState.IsValid)
            {
                var Errors = new List<string>();

                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        Errors.Add(error.ErrorMessage.ToString());
                    }
                }

                result.Data = new { Success = false, Errors = Errors };

                return result;
            }
            else
            {
                try
                {
                    result.Data = new { Success = await CategoryService.Instance.NewCategory(new Category() { Name = model.Name }) };
                }
                catch (Exception ex)
                {
                    result.Data = new { Success = false, Errors = ex.InnerException.Message };
                }
            }

            return result;
        }

        public ActionResult CategoryDetails(int ID)
        {
            CategoryDetailsViewModel model = new CategoryDetailsViewModel();

            model.PageInfo = new PageInfo()
            {
                PageTitle = "Category Details",
                PageDescription = "Category Details"
            };

            model.Category = CategoryService.Instance.GetCategoryByID(ID);

            return PartialView("_CategoryDetails", model);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCategory(UpdateCategoryViewModel model)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            if (!ModelState.IsValid)
            {
                var Errors = new List<string>();

                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        Errors.Add(error.ErrorMessage.ToString());
                    }
                }

                result.Data = new { Success = false, Errors = Errors };

                return result;
            }
            else
            {
                try
                {
                    await CategoryService.Instance.UpdateCategory(new Category() {  Name = model.Name });

                    result.Data = new { Success = true };
                }
                catch (Exception ex)
                {
                    result.Data = new { Success = false, Errors = ex.InnerException };
                }
            }

            return result;
        }

        [HttpPost]
        public async Task<JsonResult> DeleteCategory(int ID)
        {
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

       
            try
            {
                await CategoryService.Instance.DeleteCategory(ID);

                result.Data = new { Success = true };
            }
            catch (Exception ex)
            {
                result.Data = new { Success = false, Errors = ex.Message };
            }
            

            return result;
        }

        public ActionResult Questions(string search, int? page, int? items)
        {
            QuestionsListingViewModel model = new QuestionsListingViewModel();

            model.PageInfo = new PageInfo()
            {
                PageTitle = "Questions",
                PageDescription = "Question Categories."
            };

            model.searchTerm = search;
            model.pageNo = page ?? 1;
            model.pageSize = items ?? 10;

            var questionsSearch = QuestionService.Instance.GetQuestions(model.searchTerm, model.pageNo, model.pageSize);

            model.Questions = questionsSearch.Questions;
            model.TotalCount = questionsSearch.TotalCount;

            model.Pager = new Pager(model.TotalCount, model.pageNo, model.pageSize);

            return PartialView("_Questions", model);
        }
        public ActionResult NewQuestion()
        {
            NewQuestionViewModel model = new NewQuestionViewModel() {
                Categories = CategoryService.Instance.GetAllCategories()
            };

            return PartialView("_NewQuestion", model);
        }

        //[HttpPost]
        //public async Task<JsonResult> NewQuestion(NewQuestionViewModel model)
        //{
        //    JsonResult result = new JsonResult();
        //    result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

        //    if (!ModelState.IsValid)
        //    {
        //        var Errors = new List<string>();

        //        foreach (ModelState modelState in ViewData.ModelState.Values)
        //        {
        //            foreach (ModelError error in modelState.Errors)
        //            {
        //                Errors.Add(error.ErrorMessage.ToString());
        //            }
        //        }

        //        result.Data = new { Success = false, Errors = Errors };

        //        return result;
        //    }
        //    else

        //        try
        //        {
        //            result.Data = new { Success = await QuestionService.Instance.NewQuestion(new Question() { Title = model.Title,CategoryID=model.CategoryID }) };
        //        }
        //        catch (Exception ex)
        //        {
        //            result.Data = new { Success = false, Errors = ex.InnerException.Message };
        //        }
        //    return result;
        //}

        [HttpPost]
        public async Task<ActionResult> NewQuestion(FormCollection collection)
        {
            NewQuestionViewModel model = new NewQuestionViewModel();

            model = GetNewTextQuestionViewModelFromFormCollection(model, collection);

            model.PageInfo = new PageInfo()
            {
                PageTitle = "Add New Question",
                PageDescription = "Add questions to selected quiz."
            };

            if (string.IsNullOrEmpty(model.Title) || model.CorrectOptions.Count == 0 || model.Options.Count == 0)
            {
                if (string.IsNullOrEmpty(model.Title))
                {
                    ModelState.AddModelError("Title", "Please enter question title.");
                }

                if (model.CorrectOptions.Count == 0)
                {
                    ModelState.AddModelError("CorrectOptions", "Please enter some correct options.");
                }

                if (model.Options.Count == 0)
                {
                    ModelState.AddModelError("Options", "Please enter some other options.");
                }

          
                return View(model);
            }

            var question = new Question();

           
            question.Title = model.Title;
            question.CategoryID = model.CategoryID;
            question.Options = new List<Option>();
            question.Options.AddRange(model.CorrectOptions);
            question.Options.AddRange(model.Options);

            question.ModifiedOn = DateTime.Now;

            if (await QuestionService.Instance.SaveNewQuestion(question))
            {
                return RedirectToAction("NewQuestion");
            }
            else
            {
                return new HttpStatusCodeResult(500);
            }
        }


        private NewQuestionViewModel GetNewTextQuestionViewModelFromFormCollection(NewQuestionViewModel model, FormCollection collection)
        {
            model.Options = new List<Option>();
            model.CorrectOptions = new List<Option>();

            if (collection.AllKeys.Count() > 0)
            {
                foreach (string key in collection)
                {
                    if (key == "Title")
                    {
                        model.Title = collection[key];
                    }
                    else if (key == "CategoryID")
                    {
                        model.CategoryID = Convert.ToInt32(collection[key]);
                    }
                    else if (key.Contains("correctOption")) //this must be Correct Option
                    {
                        if (!string.IsNullOrEmpty(collection[key]))
                        {
                            var correctOption = new Option();
                            correctOption.Answer = collection[key];
                            correctOption.IsCorrect = true;
                            correctOption.ModifiedOn = DateTime.Now;

                            model.CorrectOptions.Add(correctOption);
                        }
                    }
                    else if (key.Contains("option")) //this must be Option
                    {
                        if (!string.IsNullOrEmpty(collection[key]))
                        {
                            var option = new Option();
                            option.Answer = collection[key];
                            option.IsCorrect = false;
                            option.ModifiedOn = DateTime.Now;

                            model.Options.Add(option);
                        }
                    }
                }
            }

            return model;
        }



        public ActionResult AttemptQuiz()
        {
            return PartialView("_AttemptQuiz");
        }


    }



}