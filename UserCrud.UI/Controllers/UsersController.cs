using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UserCrud.BLL;
using UserCrud.BLL.DTO;
using UserCrud.DAL.Context;
using UserCrud.Domain.Entities;

namespace UserCrud.UI.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserCrudDatabaseContext _context;
        private readonly GenericRepository<User> _generic;
        public UsersController(UserCrudDatabaseContext context, GenericRepository<User> generic)
        {
            _context = context;
            _generic = generic;          
        }

        #region Edit Pic
        public void EditPicture(EditProfileDto edit)
        {
            if (edit.UserAvatar != null)
            {
                string ImagePath = "";
                ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfilePicture", edit.Avatar);
                if (System.IO.File.Exists(ImagePath))
                {
                    System.IO.File.Delete(ImagePath);
                }

                edit.Avatar = CodeGenerator.GenerateUniqCode() + Path.GetExtension(edit.UserAvatar.FileName);
                ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfilePicture", edit.Avatar);
                using (var stream = new FileStream(ImagePath, FileMode.Create))
                {
                    edit.UserAvatar.CopyTo(stream);
                }
            }
        }
        #endregion

        // GET: Users
        [Authorize]
        public IActionResult Index()
        {
            return View(_generic.Get());
        }

        // GET: Users/Details/5
        [Authorize]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _generic.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }



        // GET: Users/Edit/5
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _generic.GetById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(new EditProfileDto
            {
                MelliCode= user.MelliCode,
                FullName= user.FullName,
                Email = user.Email,
                Avatar= user.Avatar,
                UserId= user.UserId,
            });
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditProfileDto edit)
        {
            if (ModelState.IsValid)
            {
                 EditPicture(edit);
                _generic.Update(edit);
                _generic.Save();
  
             return RedirectToAction(nameof(Index));
            }
            return View(edit);
        }

        // GET: Users/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _generic.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _generic.GetById(id);
            _generic.Delete(user);
            _generic.Save();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        #region Register 
        [Route("Register")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Register")]
        public IActionResult Create(RegisterUserDto user)
        {
            var newuser = new User()
            {
                Email = user.Email,
                MelliCode = user.MelliCode,
                Password = PasswordHelper.EncodePasswordMd5(user.Password),
                FullName = user.FullName,
                Avatar = "Default.png",
            };
            if (ModelState.IsValid)
            {
                _generic.Add(newuser);
                _generic.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
        #endregion

        #region Login
        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginUserDto login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
            string hashpass = PasswordHelper.EncodePasswordMd5(login.Password);
            string email = FixedText.FixedEmail(login.Email);
            var user = _context.Users.SingleOrDefault(x => x.Email == email && x.Password == hashpass);
            if (user != null)
            {
                var claim = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier , user.UserId.ToString()),
                    new Claim(ClaimTypes.Name , user.FullName)
                };
                var Identity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                var principle = new ClaimsPrincipal(Identity);

                HttpContext.SignInAsync(principle);
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("Email", "کاربری با مشخصات فوق یافت نشد");
            return View(login);
        }
        #endregion

        #region Logout
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
        #endregion

        

    }
}
