using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.Linq;

namespace Raffle.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "*")]
        public string UserName { get; set; }

        public int Reputation
        {
            get
            {
                var now = DateTime.Now.Date;
                var sixMonths = TimeSpan.FromDays(30 * 6);

                var db = new Context();
                var user = db.UserProfiles.Find(UserId);

                var intervalRaffles = user.Raffles.Count(r => now - r.PurchasedAt < sixMonths);

                var intervalItems = user.Items.Where(r => r.DeliveredSuccess != null && r.ClosedAt != null && (now - r.ClosedAt < sixMonths));
                var successItems = intervalItems.Count(r => r.DeliveredSuccess.GetValueOrDefault());
                var failedItems = intervalItems.Count(r => !r.DeliveredSuccess.GetValueOrDefault());

                return (intervalRaffles + successItems) - failedItems;
            }
        }

        public virtual ICollection<Raffle> Raffles { get; set; }

        public virtual ICollection<Item> Items { get; set; }

        public UserProfile()
        {
            Raffles = new List<Raffle>();
            Items = new List<Item>();
        }
    }

    public class RegisterExternalLoginModel
    {
        [Required(ErrorMessage = "*")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "*")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "*")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "*")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}
