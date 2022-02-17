
using System;

namespace api.ViewModels.Account
{
    public class AccountOptions
    {
        public static bool AllowLocalLogin = false;
        public static bool AllowRememberLogin = false;
        public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);

        public static bool ShowLogoutPrompt = true;
        public static bool AutomaticRedirectAfterSignOut = false;

        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}