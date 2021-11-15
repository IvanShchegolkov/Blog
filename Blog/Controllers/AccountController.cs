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

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        private PersonContext db;

        public AccountController(PersonContext context)
        {
            db = context;
        }
            
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                Person person = await db.Person.FirstOrDefaultAsync(u => u.PersonLogin == model.PersonLogin && u.PersonPassword == model.PersonPassword);
                if (person != null)
                {
                    if (person.PersonRoleId != null)
                    {
                        List<Role> roles = await db.Role.ToListAsync();
                        foreach(var role in roles)
                        {
                            if(role.RoleId == person.PersonRoleId)
                            {
                                await Authenticate(model.PersonLogin, role.RoleName);
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
                Person person = await db.Person.FirstOrDefaultAsync(u => u.PersonLogin == model.PersonLogin);
                if (person == null)
                {
                    // добавляем пользователя в бд
                    db.Person.Add(new Person
                    {
                        PersonRoleId = 2,
                        PersonLogin = model.PersonLogin,
                        PersonPassword = model.PersonPassword,
                        PersonFirstName = model.PersonFirstName,
                        PersonLastName = model.PersonLastName,
                        PersonEmail = model.PersonEmail
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
    }
}
