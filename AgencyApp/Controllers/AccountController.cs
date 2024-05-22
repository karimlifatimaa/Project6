using AgencyApp.DTOs;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using System.Runtime.InteropServices;

namespace AgencyApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if(!ModelState.IsValid)
            {
                return View(registerDto);
            }
            User user = new User()
            {
                Name = registerDto.Name,
                Surname = registerDto.Surname,
                UserName = registerDto.Username,
                Email = registerDto.Email,

            };
            var result= await _userManager.CreateAsync(user,registerDto.Password);
            if (!result.Succeeded)
            {
                foreach(var item in  result.Errors)
                {
                    ModelState.AddModelError("", item.Description);

                }
                return View(registerDto);
            }
            await _userManager.AddToRoleAsync(user, "User");
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }
            var user = await _userManager.FindByEmailAsync(loginDto.UsernameOrEmail);
            if(user == null)
            {
                user=await _userManager.FindByNameAsync(loginDto.UsernameOrEmail);
                if( user == null)
                {
                    ModelState.AddModelError("", "Email/Username or Passwod is not vallid");
                    return View();
                }
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password,true);
            if(result.IsLockedOut)
            {
                ModelState.AddModelError("", "Try agin later");
                return View();
            }
            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Email/Username or password is not valid");
                return View();
            }
            await _signInManager.SignInAsync(user, loginDto.IsRemember);
            return RedirectToAction("Index","Home");

        }
        public async Task<IActionResult> CreateRole()
        {
            IdentityRole role = new IdentityRole("Admin");
            IdentityRole role1 = new IdentityRole("User");
            await _roleManager.CreateAsync(role1);
            await _roleManager.CreateAsync(role);
            return Ok();

        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
        
    }
}
