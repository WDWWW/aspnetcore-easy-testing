using System.Security.Claims;
using System.Threading.Tasks;
using Wd3w.TokenAuthentication;

namespace Wd3w.AspNetCore.EasyTesting.SampleApi.Services
{
    public class CustomTokenAuthService : ITokenAuthService
    {
        public async Task<bool> IsValidateAsync(string token)
        {
            return true;
        }

        public async Task<ClaimsPrincipal> GetPrincipalAsync(string token)
        {
            // Do create your own custom claims pricipal object and return them;
            return new ClaimsPrincipal(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.Email, "test@test.com"),
                new Claim(ClaimTypes.Name, "test-user")
            }));
        }
    }}