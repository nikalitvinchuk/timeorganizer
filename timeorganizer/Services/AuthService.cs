
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace timeorganizer.Services
{
    public class AuthService : AuthenticationStateProvider
    {

        
        private AuthenticationState currentUser;

        public AuthService(AuthServiceSetUser service)
        {
            currentUser = new AuthenticationState(service.CurrentUser);

            service.UserChanged += (newUser) =>
            {
                currentUser = new AuthenticationState(newUser);
                NotifyAuthenticationStateChanged(Task.FromResult(currentUser));
            };
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(currentUser);
    }

    public class AuthServiceSetUser
    {
        public event Action<ClaimsPrincipal>? UserChanged;
        private ClaimsPrincipal? currentUser;

        public ClaimsPrincipal CurrentUser
        {
            get { return currentUser ?? new ClaimsPrincipal(new ClaimsIdentity()); }
            set
            {
                currentUser = value;

                if (UserChanged is not null)
                {
                    UserChanged(currentUser);
                }
            }
        }

       
        //	private  Task<ClaimsPrincipal> LoginWithExternalProviderAsync(string Login,string Role)
        //	{

        //           var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
        //			new Claim(ClaimTypes.Role, Role),
        //			new Claim(ClaimTypes.NameIdentifier, Login)}, "Basic"));

        //		return Task.FromResult(authenticatedUser);
        //	}
        //}
    }
}
public class AuthenticatedUser
{
    public ClaimsPrincipal Principal { get; set; } = new();
}