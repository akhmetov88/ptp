using System;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using TradingPlatform.Models;

namespace TradingPlatform
{
       public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    public class PtpRolesManager : RoleManager<ApplicationRole>
    {
        public PtpRolesManager(IRoleStore<ApplicationRole, string> store) : base(store)
        {
        }

        public static PtpRolesManager Create(IdentityFactoryOptions<PtpRolesManager> options, IOwinContext context)
        {
            var manager = new PtpRolesManager(new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
            return manager;
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
                 
        }
      
        public Data.Group GetUserGroup(ApplicationUser user)
        {
            return user.Group;
        }

       public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
           // var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            var manager = new ApplicationUserManager(new ApplicationUserStore(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true,
            };
            
            manager.PasswordValidator = new PassVal();

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
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
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
        //public static bool CanUserJoinToGroup(int groupId)
        //{
        //    return 
        //}
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
        public async Task<SignInStatus> PasswordEmailSignInAsync(string email, string password, bool isPersistent, bool shouldLockout)
        {
            var user = UserManager.FindByEmail(email);
            return await PasswordSignInAsync(user.UserName, password, isPersistent, shouldLockout);
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class PassVal : PasswordValidator
    {
        public PassVal()
        {
            
        }

        public override Task<IdentityResult> ValidateAsync(string item)
        {
            if (String.IsNullOrEmpty(item) || item.Length < 8)
            {
                return Task.FromResult(IdentityResult.Failed(LocalText.Inst.Get("error", "passwordLength8", "Введіть мінімум 8 символів", "Введите минимум 8 символов")));
            }

            string pattern = @"^(?=.*[\d])(?=.*[A-Z])(?=.*[a-z])[\w\d!@#$%_]{8,40}$";

            if (!Regex.IsMatch(item, pattern))
            {
                return Task.FromResult(IdentityResult.Failed(LocalText.Inst.Get("error", "passwordValidator", "Пароль не відповідає вимогам безпеки", "Пароль не соответсвует требованиям безопасности")));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }

}
