using System.ComponentModel.DataAnnotations;

namespace ToDoList.ViewModels;

public class RegisterViewModel
{
  [Required]
  [EmailAddress]
  [Display(Name = "Email Address")]
  public string Email { get; set; }

  [Required]
  [DataType(DataType.Password)]
  [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{6,}$",
      ErrorMessage = "Password must contain at least six characters, a lowercase and uppercase letter, a number, and a special character.")]
  public string Password { get; set; }

  [Required]
  [DataType(DataType.Password)]
  [Display(Name = "Confirm password")]
  [Compare("Password", ErrorMessage = "Passwords do not match!")]
  public string ConfirmPassword { get; set; }
}
