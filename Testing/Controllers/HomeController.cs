using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Testing.Models;

namespace Testing.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _context;

        public HomeController(DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dataList = _context.MasterRole.ToList();
            ViewData["MasterRole"] = dataList;

            return View();
        }

        public IActionResult MasterMenu()
        {
            var dataList = _context.MasterMenu.ToList();
            ViewData["MasterMenu"] = dataList;

            return View();
        }

        public IActionResult Users()
        {
            var dataRoleList = _context.MasterRole.ToList();
            ViewData["MasterRole"] = dataRoleList;

            var dataUserList = _context.Users.ToList();

            var userList = new List<UserResponse>();
            foreach (var data in dataUserList)
            {
                var getDataRole = _context.MasterRole.FirstOrDefault(x => x.Id == data.RoleId);
                var setData = new UserResponse
                {
                    Id = data.Id,
                    Name = data.Name,
                    RoleName = getDataRole.Name,
                    UserName = data.UserName
                };
                userList.Add(setData);
            }

            ViewData["Users"] = userList;

            return View();
        }

        public IActionResult AccessMenu()
        {
            var dataMenuList = _context.MasterMenu.ToList();
            ViewData["MasterMenu"] = dataMenuList;

            var dataRoleList = _context.MasterRole.ToList();
            ViewData["MasterRole"] = dataRoleList;

            var dataAccessList = _context.AccessMenu.ToList();

            var scoped = new List<Scoped>();
            foreach(var data in dataAccessList)
            {
                var getDataMenu = _context.MasterMenu.FirstOrDefault(x => x.Id == data.MenuId);
                var getDataRole = _context.MasterRole.FirstOrDefault(x => x.Id == data.RoleId);
                var setData = new Scoped
                {
                    RoleName = getDataRole.Name,
                    MenuName = getDataMenu.Name
                };
                scoped.Add(setData);
            }

            ViewData["AccessMenu"] = scoped;

            return View();
        }

        [HttpPost]
        public IActionResult MasterRole(Models.MasterRole request)
        {
            _context.MasterRole.Add(request);
            _context.SaveChanges();

            ModelState.Clear();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult MasterMenu(Models.MasterMenu request)
        {
            if (ModelState.IsValid)
            {
                _context.MasterMenu.Add(request);
                _context.SaveChanges();

                ModelState.Clear();

                return RedirectToAction("MasterMenu");
            }
            return View();
        }

        [HttpPost]
        public IActionResult AccessMenu(Models.AccessMenu request)
        {
            if (ModelState.IsValid)
            {
                _context.AccessMenu.Add(request);
                _context.SaveChanges();

                ModelState.Clear();

                return RedirectToAction("AccessMenu");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Users(Models.Users request)
        {
            if (ModelState.IsValid)
            {
                //Hasing Password
                byte[] salt = new byte[16];
                new RNGCryptoServiceProvider().GetBytes(salt);

                var pbkdf2 = new Rfc2898DeriveBytes(request.Password, salt, 10000);
                byte[] hash = pbkdf2.GetBytes(20);

                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);

                string savedPasswordHash = Convert.ToBase64String(hashBytes);
                request.Password = savedPasswordHash;

                _context.Users.Add(request);
                _context.SaveChanges();

                ModelState.Clear();

                return RedirectToAction("Users");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
