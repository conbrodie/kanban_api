using IdentityModel;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace api.Controllers
{
    public class ExternalController : Controller
    {    
        public UserManager<User> _userManager { get; }
        public SignInManager<User> _signInManager { get; }
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly ILogger<ExternalController> _logger;
        private readonly IEventService _events;
        private readonly IConfiguration _configuration;

        public ExternalController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            IConfiguration configuration,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events,
            ILogger<ExternalController> logger)
        {
            _userManager = userManager;
             _interaction = interaction;
            _clientStore = clientStore;
            _logger = logger;
            _events = events;
            _signInManager = signInManager;
            _configuration = configuration;
        }

         /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [AllowAnonymous]
        //[Route("signin-external")]
        public IActionResult Challenge([FromQuery] string scheme, [FromQuery] string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }
            
            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)), 
                Items =
                {
                    { "returnUrl", returnUrl }, 
                    { "scheme", scheme },
                }
            };
            return Challenge(props, scheme);
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        //[Route("response-external")]
        public async Task<IActionResult> Callback()
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogDebug("External claims: {@claims}", externalClaims);
            }

            // lookup our user and external provider info
            var (user, provider, providerUserId, claims, email, firstName, lastName) = await FindUserFromExternalProvider(result);
            if (user == null)
            {
                // this might be where you might initiate a custom workflow for user registration
                // in this sample we don't show how that would be done, as our sample implementation
                // simply auto-provisions new external user
                user = await AutoProvisionUser(provider, providerUserId, claims, email, firstName, lastName);
            }

            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);
           
            // issue authentication cookie for user
            // we must issue the cookie maually, and can't use the SignInManager because
            // it doesn't expose an API to issue additional claims from the login workflow
            additionalLocalClaims.Add(new Claim("firstname", user.FirstName ?? string.Empty));
            additionalLocalClaims.Add(new Claim("lastname", user.LastName ?? string.Empty));
            additionalLocalClaims.Add(new Claim("email", user.Email ?? string.Empty));


            // add persisted claims to additionalLocalClaims 
            // var persistedUserClaims = await _userManager.GetClaimsAsync(user);

            // foreach (var claim in persistedUserClaims){
            //     additionalLocalClaims.Add(new Claim(claim.Type, claim.Value));
            // }

            // await _userManager.AddClaimsAsync(user, additionalLocalClaims);
            
            // issue authentication cookie for user
            var isuser = new IdentityServerUser(user.Id.ToString())
            {
                DisplayName = user.FirstName + user.LastName,
                IdentityProvider = provider,
                AdditionalClaims = additionalLocalClaims,
            };


            await HttpContext.SignInAsync(isuser, localSignInProps);

            //delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            // validate return URL and redirect back to authorization endpoint or a local page
            if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        private async Task<(User user, string provider, string providerUserId, IEnumerable<Claim> claims, string email, string firstName, string lastName)> FindUserFromExternalProvider(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            var email = externalUser.FindFirst(ClaimTypes.Email).Value;
            var firstName = externalUser.FindFirst(ClaimTypes.GivenName).Value;
            var lastName = externalUser.FindFirst(ClaimTypes.Surname).Value;

            // find external user
            var user = await _userManager.FindByLoginAsync(provider, providerUserId);

            return (user, provider, providerUserId, claims, email, firstName, lastName);
        }

        private async Task<User> AutoProvisionUser(string provider, string providerUserId, IEnumerable<Claim> claims, string email, string firstName, string lastName)
        {
            // create dummy internal account (you can do something more complex)
            User user = new User()
            {
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                // add external user ID to new account
                await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
                return user;
            }

            return user;
        }

        // if the external login is OIDC-based, there are certain things we need to preserve to make logout work
        // this will be different for WS-Fed, SAML2p or other protocols
        private void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
            }
        }
    }
}

