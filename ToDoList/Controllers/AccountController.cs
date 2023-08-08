using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ToDoList.Models;
using System.Threading.Tasks;
using ToDoList.ViewModels;

namespace ToDoList.Controllers;

public class AccountController : Controller
{
  private readonly ToDoListContext _db;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly SignInManager<ApplicationUser> _signInManager;

  public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ToDoListContext db)
  {
    _userManager = userManager;
    _signInManager = signInManager;
    _db = db;
  }

  public ActionResult Index()
  {
    return View();
  }

  public IActionResult Register()
  {
    return View();
  }

  [HttpPost]
  public async Task<ActionResult> Register (RegisterViewModel model)
  {
    // model state is valid?
    if (!ModelState.IsValid)
    {
      // not valid
      return View(model);
    }
    // valid
    else
    {
      // create new user, try to update db
      ApplicationUser user = new ApplicationUser { UserName = model.Email };
      IdentityResult result = await _userManager.CreateAsync(user, model.Password);
      if (result.Succeeded)
      {
        // if success, redirect to mainpage
        return RedirectToAction("Index");
      }
      else
      {
        // otherwise return to register page with errors
        foreach (IdentityError error in result.Errors)
        {
          ModelState.AddModelError("", error.Description);
        }
        return View(model);
      }
    }
  }

  public ActionResult Login()
  {
    return View();
  }

  [HttpPost]
  public async Task<ActionResult> Login(LoginViewModel model)
  {
    // check model validity
    if (!ModelState.IsValid)
    {
      // if invalid
      return View(model);
    }
    // if valid
    else
    {
      // await signin result
      Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);
      if (result.Succeeded)
      {
        // if success, log in to homepage
        return RedirectToAction("Index");
      }
      else
      {
        // if fail, error at login page
        ModelState.AddModelError("", "Username or password not recognized.");
        return View(model);
      }
    }
  }
}
