using Microsoft.AspNet.Identity.EntityFramework;
using QuizbeePlus.Entities;
using QuizbeePlus.Entities.CustomEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizbeePlus.ViewModels
{
    public class ControlPanelViewModel : BaseViewModel
    {
        public bool isPartial { get; set; }
    }

    public class UsersListingViewModel : ListingBaseViewModel
    {
        public List<UserWithRoleEntity> Users { get; set; }
    }

    public class UserDetailsViewModel : BaseViewModel
    {
        public UserWithRoleEntity User { get; set; }
        public List<IdentityRole> AvailableRoles { get; set; }
    }



    public class RolesListingViewModel : ListingBaseViewModel
    {
        public List<IdentityRole> Roles { get; set; }
    }

    public class NewRoleViewModel : BaseViewModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class UpdateRoleViewModel : BaseViewModel
    {
        [Required]
        public string ID { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class RoleDetailsViewModel : BaseViewModel
    {
        public RoleWithUsersEntity Role { get; set; }
    }



    public class CategoriesListingViewModel : ListingBaseViewModel
    {
        public List<Category> Categories { get; set; }
    }
    public class NewCategoryViewModel : BaseViewModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class UpdateCategoryViewModel : BaseViewModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class CategoryDetailsViewModel : BaseViewModel
    {
        public CategoryWithQuestionsEntity Category { get; set; }
    }


    public class QuestionsListingViewModel : ListingBaseViewModel
    {
        public List<Question> Questions { get; set; }
    }
    public class NewQuestionViewModel: BaseViewModel
    {
        [Required]
        public string Title { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public int CategoryID { get; set; }

        public List<Option> Options { get; set; }
        public List<Option> CorrectOptions { get; set; }
    }

}