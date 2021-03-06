﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using WebApp_Test.Models;

namespace WebApp_Test
{/// <summary>
/// 
/// </summary>
    public class EmailService : IIdentityMessageService
    {
        /// Plug in your email service here to send an email.
        public Task SendAsync(IdentityMessage message)
        {
            
            return Task.FromResult(0);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class SmsService : IIdentityMessageService
    {
        /// Plug in your SMS service here to send a text message.
        public Task SendAsync(IdentityMessage message)
        {
        
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.


        /// <summary>
        /// تفعيل وتجاهل بعض الخصائص في قبول اسم المستخدم وكلمة السر
        /// </summary>
    public class ApplicationUserManager : UserManager<MyUsers>
    {/// <summary>
    ///
    /// </summary>
    /// <param name="store"></param>
        public ApplicationUserManager(IUserStore<MyUsers> store)
            : base(store)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<MyUsers>(context.Get<DB>()));
            // Configure validation logic for usernames
             
            manager.UserValidator = new UserValidator<MyUsers>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 3,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<MyUsers>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<MyUsers>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<MyUsers>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    /// Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<MyUsers, string>
    {
        /// <summary>
    /// 
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="authenticationManager"></param>
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public override Task<ClaimsIdentity> CreateUserIdentityAsync(MyUsers user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
