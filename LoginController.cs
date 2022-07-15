using CHATFREE.DatabaseAccess;
using CHATFREE.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace CHATFREE.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserDatabaseAccess _userDatabaseAccess;
        public LoginController(UserDatabaseAccess userDatabaseAccess)
        {
            _userDatabaseAccess = userDatabaseAccess;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            var checkuser = _userDatabaseAccess.QueryOne(p => p.Email == "calebjohn3112@gmail.com");
            if (checkuser == null)
            {
                var newUser = new UserModel
                {
                    Id = Guid.NewGuid(),

                    Surname = "caleb",
                    OtherNames = "john",
                    Email = "calebjohn3112@gmail.com",
                    TelephoneNumber = "08036000305",
                    RegistrationDate = DateTime.Now,
                    IsActive = true,
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("calebjohn"),
                    
                };
                _userDatabaseAccess.Insert(newUser);
            }

            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(UserModel login)
        {
            var checkUser = _userDatabaseAccess
                .QueryOne(c => c.Email == login.Email);
            if (checkUser == null)
            {
                ViewBag.ErrorMessage = "User does not exist!";
                return View();
            }

            if (!BCrypt.Net.BCrypt.EnhancedVerify(login.Password,
                    checkUser.Password))
            {
                ViewBag.ErrorMessage = "user's Password is wrong!";
                return View();
            }

            List<Claim> claims = new() {
                new Claim(ClaimTypes.Email, checkUser.Email),
                new Claim("FirstName", checkUser.FirstName),
                new Claim("Surname", checkUser.Surname)
            };

            

            ClaimsIdentity claimIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimPrincipal = new(claimIdentity);
            await HttpContext.SignInAsync(claimPrincipal);
            return RedirectToAction("Index", "Environment");
        }

        
        public IActionResult Register()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult Register(UserModel register, string confirmPassword)
        {
            if (register.Password != confirmPassword)
            {
                ViewBag.Message = "Confirm Password and Password must be the same!";
                return View();
            }

            var checkIfUserExist = _userDatabaseAccess
                .QueryOne(p => p.Email == register.Email);
            if (checkIfUserExist == null)
            {
                register.Id = Guid.NewGuid();
                register.IsActive = true;
                register.RegistrationDate = DateTime.Now;
                register.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(register.Password);
                _userDatabaseAccess.Insert(register);
                ViewBag.Message = "User is added successfully";
                return View();
            }

            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
