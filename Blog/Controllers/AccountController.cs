using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Blog.ViewModels;
using Blog.Models;
using System.Security.Cryptography;
using System.Text;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        private BlogContext db;

        public AccountController(BlogContext context)
        {
            db = context;
        }
            
        [HttpGet]
        public IActionResult Login()
        {
            int i = 0;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                Person person = await db.Person.FirstOrDefaultAsync(u => 
                    u.Login == model.PersonLogin && 
                    u.Password == ComputeHash(model.PersonPassword, new SHA256CryptoServiceProvider()));
                if (person != null)
                {
                    if (person.RoleId != null)
                    {
                        List<Role> roles = await db.Role.ToListAsync();
                        foreach(var role in roles)
                        {
                            if(role.Id == person.RoleId)
                            {
                                await Authenticate(model.PersonLogin, role.Name);
                            }
                        }
                    }
                    //await Authenticate(model.PersonLogin, "admin");

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                Person person = await db.Person.FirstOrDefaultAsync(u => u.Login == model.PersonLogin);
                if (person == null)
                {
                    // добавляем пользователя в бд
                    db.Person.Add(new Person
                    {
                        RoleId = 2,
                        Login = model.PersonLogin,
                        Password = ComputeHash(model.PersonPassword, new SHA256CryptoServiceProvider()),
                        FirstName = model.PersonFirstName,
                        LastName = model.PersonLastName,
                        Email = model.PersonEmail
                    });
                    await db.SaveChangesAsync();

                    await Authenticate(model.PersonLogin, "user"); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Users()
        {
            return View(await db.Person.ToListAsync());
        }

        private async Task Authenticate(string userName, string userRole)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, userRole)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public string ComputeHash(string input, HashAlgorithm algorithm)
        {
            //string hPassword = ComputeHash(password, new SHA256CryptoServiceProvider());
            //string hPassword = ComputeHash(password, new MD5CryptoServiceProvider());

            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

            return BitConverter.ToString(hashedBytes);
        }
    }
}
