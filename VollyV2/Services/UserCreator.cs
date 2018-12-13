using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VollyV2.Data;
using VollyV2.Data.Volly;
using VollyV2.Models;

namespace VollyV2.Services
{
    public class UserCreator
    {
        private const int DefaultPasswordLength = 8;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public UserCreator(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public async Task<ApplicationUser> CreateUser(string email, Company company)
        {
            return await CreateUser(email, GetRandomPassword(DefaultPasswordLength), company);
        }

        public async Task<ApplicationUser> CreateUser(string email, string password, Company company)
        {
            ApplicationUser user = new ApplicationUser() {Email = email, UserName = email, Company = company};
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _emailSender.SendAccountCreatedConfirm(email, password);

                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return user;
        }

        private string GetRandomPassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
