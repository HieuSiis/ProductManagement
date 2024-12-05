using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ProductManagement.AppData;
using ProductManagement.Models;
using NuGet.Common;

namespace ProductManagement.Controllers
{
	public class UserController : Controller
	{
		private readonly SignInManager<AppUser> _signInManager;
		private readonly UserManager<AppUser> _userManager;
		private readonly IUserStore<AppUser> _userStore;
		// private readonly IUserEmailStore<AppUser> _emailStore;

		private readonly ILogger<LoginEmailViewModel> _logger;

		public UserController(
			UserManager<AppUser> userManager,
			IUserStore<AppUser> userStore,
			SignInManager<AppUser> signInManager,
			//IUserEmailStore<AppUser> emailStore

			ILogger<LoginEmailViewModel> logger
			)
		{
			_userManager = userManager;
			_userStore = userStore;
			_signInManager = signInManager;
			//_emailStore = emailStore;

			_logger = logger;
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Login()
		{
			var model = new LoginEmailViewModel
			{
				// Convert string "true" to boolean
				RememberMe = Request.Cookies["RememberMe_Status"] == "true"
			};

			return View(model);
		}

		public IActionResult Register()
        {
            return View();
        }

		/*public IActionResult ForgotPassword() 
		{ 
            return View();
		}*/

		[HttpPost]
		public async Task<IActionResult> Login(LoginEmailViewModel model, string? returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			// ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{
				var email = await _userManager.FindByEmailAsync(model.Email);

				if (email == null)
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return View();
				}

				var result = await _signInManager.CheckPasswordSignInAsync(email, model.Password, lockoutOnFailure: false);

				// This doesn't count login failures towards account lockout
				// To enable password failures to trigger account lockout, set lockoutOnFailure: true
				// var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
				
				if (result.Succeeded)
				{
					await _signInManager.SignInAsync(email, model.RememberMe);

					if (model.RememberMe)
					{
						// Store email, password, and RememberMe status in cookies
						Response.Cookies.Append("RememberMe_Email", model.Email, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) });
						Response.Cookies.Append("RememberMe_Password", model.Password, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) });
						Response.Cookies.Append("RememberMe_Status", "true", new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) });
					}
					else
					{
						// Clear the cookies if Remember Me is unchecked
						Response.Cookies.Delete("RememberMe_Email");
						Response.Cookies.Delete("RememberMe_Password");
						Response.Cookies.Delete("RememberMe_Status");
					}

					_logger.LogInformation("User logged in.");
					// return LocalRedirect(returnUrl);
					TempData["LoginMessage"] = "You have successfully logged in.";
					return RedirectToAction("Index", "Home");
				}
				if (result.RequiresTwoFactor)
				{
					return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
				}
				if (result.IsLockedOut)
				{
					_logger.LogWarning("User account locked out.");
					return RedirectToPage("./Lockout");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					// ModelState.AddModelError("Login", "Invalid login attempt.");

					return View();
				}
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Logout(string? returnUrl = null)
		{
			await _signInManager.SignOutAsync();
			_logger.LogInformation("User logged out.");

			TempData["LogoutMessage"] = "You have successfully logged out.";

			return RedirectToAction("Index", "Home");

			/*if (returnUrl != null)
			{
				return LocalRedirect(returnUrl);
			}
			else
			{
				// This needs to be a redirect so that the browser performs a new
				// request and the identity for the user gets updated.
				return RedirectToPage();
			}*/
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");
			//	ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			if (ModelState.IsValid)
			{
				var user = CreateUser();

				// await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
				// await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);

				await _userManager.SetUserNameAsync(user, model.UserName);
				await _userManager.SetEmailAsync(user, model.Email);
				await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);

				user.FirstName = model.FirstName;
				user.LastName = model.LastName;
				user.Address = model.Address;

				var result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					/*
					_logger.LogInformation("User created a new account with password.");

					var userId = await _userManager.GetUserIdAsync(user);
					
					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
						protocol: Request.Scheme);

					await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
						$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
					*/
					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						return RedirectToPage("RegisterConfirmation", new { email = model.Email, returnUrl = returnUrl });
					}
					else
					{
						// await _signInManager.SignInAsync(user, isPersistent: false);
						// return LocalRedirect(returnUrl);
						TempData["RegisterMessage"] = "You have successfully registered.";
						return RedirectToAction("Login", "User");
					}
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			return View();
		}

		private AppUser CreateUser()
		{
			try
			{
				return Activator.CreateInstance<AppUser>();
			}
			catch
			{
				throw new InvalidOperationException($"Can't create an instance of '{nameof(AppUser)}'. " +
					$"Ensure that '{nameof(AppUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
					$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}

		[HttpPost]
		public async Task<IActionResult> CartLogin(LoginUsernameViewModel model, string? returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			// ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, lockoutOnFailure: false);

				if (result.Succeeded)
				{
					_logger.LogInformation("User logged in.");
					// return LocalRedirect(returnUrl);
					TempData["LoginCartMessage"] = "You have successfully logged in.";
					return Redirect("/cart");
				}
				if (result.RequiresTwoFactor)
				{
					return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl});
				}
				if (result.IsLockedOut)
				{
					_logger.LogWarning("User account locked out.");
					return RedirectToPage("./Lockout");
				}
				else
				{
					// ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					// ModelState.AddModelError("Login", "Invalid login attempt.");
					TempData["ErrorMessage"] = "Invalid login attempt.";
					return Redirect("/cart");
				}
			}

			return Redirect("/cart");
		}
	}
}
